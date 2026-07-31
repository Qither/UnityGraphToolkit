using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NPBehave
{
    public sealed class NPBehaveJsonSerializer
    {
        private readonly JsonSerializerSettings m_Settings;

        public NPBehaveJsonSerializer(INPBehaveTypeProvider typeProvider)
            : this(typeProvider?.GetKnownTypes())
        {
        }

        public NPBehaveJsonSerializer(IEnumerable<Type> knownTypes)
        {
            HashSet<Type> allowedTypes = new HashSet<Type>(knownTypes ?? Array.Empty<Type>())
            {
                typeof(NPBehaveData),
                typeof(NPBehaveData.NodeDictionary)
            };

            this.m_Settings = new JsonSerializerSettings
            {
                ContractResolver = new FieldOnlyContractResolver(),
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                NullValueHandling = NullValueHandling.Ignore,
                SerializationBinder = new NPBehaveKnownTypesBinder(allowedTypes)
            };
        }

        public string Serialize(NPBehaveData data, Formatting formatting = Formatting.None)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            this.m_Settings.Formatting = formatting;
            return JsonConvert.SerializeObject(data, this.m_Settings);
        }

        public NPBehaveData Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("NPBehave JSON cannot be empty.", nameof(json));
            }

            NPBehaveData data = JsonConvert.DeserializeObject<NPBehaveData>(json, this.m_Settings);
            return data ?? throw new JsonSerializationException("NPBehave JSON did not contain a graph.");
        }

        private sealed class FieldOnlyContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                return member.MemberType == MemberTypes.Field
                    ? base.CreateProperty(member, memberSerialization)
                    : null;
            }
        }

        private sealed class NPBehaveKnownTypesBinder : ISerializationBinder
        {
            private readonly Dictionary<string, Type> m_TypesByName;

            public NPBehaveKnownTypesBinder(IEnumerable<Type> knownTypes)
            {
                this.m_TypesByName = knownTypes
                    .Where(type => type != null)
                    .Distinct()
                    .ToDictionary(type => type.FullName, type => type, StringComparer.Ordinal);
            }

            public Type BindToType(string assemblyName, string typeName)
            {
                if (!string.IsNullOrEmpty(typeName) && this.m_TypesByName.TryGetValue(typeName, out Type type))
                {
                    return type;
                }

                throw new JsonSerializationException($"NPBehave type '{typeName}' is not in the whitelist.");
            }

            public void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                if (serializedType?.FullName == null || !this.m_TypesByName.ContainsKey(serializedType.FullName))
                {
                    throw new JsonSerializationException($"NPBehave type '{serializedType}' is not in the whitelist.");
                }

                assemblyName = null;
                typeName = serializedType.FullName;
            }
        }
    }
}
