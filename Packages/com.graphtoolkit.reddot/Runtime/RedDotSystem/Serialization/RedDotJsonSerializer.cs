using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RedDotSystem.Runtime
{
    public sealed class RedDotJsonSerializer
    {
        private readonly JsonSerializerSettings m_Settings;

        public RedDotJsonSerializer(IEnumerable<Type> knownTypes = null)
        {
            HashSet<Type> allowedTypes = new HashSet<Type>(knownTypes ?? Array.Empty<Type>())
            {
                typeof(RedDotConfigDocumentV1),
                typeof(RedDotData),
                typeof(RedDotSingeData),
                typeof(RedDotSingeData.LinkDataNames),
                typeof(RedDotLinkRule)
            };

            this.m_Settings = new JsonSerializerSettings
            {
                ContractResolver = new FieldOnlyContractResolver(),
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                NullValueHandling = NullValueHandling.Ignore,
                SerializationBinder = new RedDotKnownTypesBinder(allowedTypes)
            };
        }

        public string Serialize(RedDotData root, Formatting formatting = Formatting.Indented)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            this.m_Settings.Formatting = formatting;
            return JsonConvert.SerializeObject(new RedDotConfigDocumentV1(root), this.m_Settings);
        }

        public RedDotConfigDocumentV1 DeserializeDocument(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("Red-dot JSON cannot be empty.", nameof(json));
            }

            RedDotConfigDocumentV1 document =
                JsonConvert.DeserializeObject<RedDotConfigDocumentV1>(json, this.m_Settings);
            if (document == null || document.root == null)
            {
                throw new JsonSerializationException("Red-dot JSON did not contain a root node.");
            }

            if (document.formatVersion != 1)
            {
                throw new JsonSerializationException(
                    $"Unsupported red-dot formatVersion '{document.formatVersion}'. Expected 1.");
            }

            return document;
        }

        public RedDotData Deserialize(string json)
        {
            return this.DeserializeDocument(json).root;
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

        private sealed class RedDotKnownTypesBinder : ISerializationBinder
        {
            private readonly Dictionary<string, Type> m_TypesByName;

            public RedDotKnownTypesBinder(IEnumerable<Type> knownTypes)
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

                throw new JsonSerializationException($"Red-dot type '{typeName}' is not in the whitelist.");
            }

            public void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                if (serializedType?.FullName == null || !this.m_TypesByName.ContainsKey(serializedType.FullName))
                {
                    throw new JsonSerializationException($"Red-dot type '{serializedType}' is not in the whitelist.");
                }

                assemblyName = null;
                typeName = serializedType.FullName;
            }
        }
    }
}
