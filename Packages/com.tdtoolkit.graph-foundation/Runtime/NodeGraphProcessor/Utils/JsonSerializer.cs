using System;
using UnityEngine;

namespace GraphProcessor
{
    [Serializable]
    public struct JsonElement
    {
        public string type;

        public string jsonDatas;

        public override string ToString()
        {
            return "type: " + this.type + " | JSON: " + this.jsonDatas;
        }
    }

    public static class JsonSerializer
    {
        public static Func<object, string> SerializeOverride { private get; set; }

        public static Action<string, object> DeserializeOverride { private get; set; }

        public static JsonElement Serialize(object obj)
        {
            return new JsonElement
            {
                type = obj.GetType().AssemblyQualifiedName,
                jsonDatas = SerializeOverride?.Invoke(obj) ?? JsonUtility.ToJson(obj)
            };
        }

        public static T Deserialize<T>(JsonElement element)
        {
            if (typeof(T) != Type.GetType(element.type))
            {
                throw new ArgumentException("Deserializing type is not the same as the JSON element type.");
            }

            T obj = Activator.CreateInstance<T>();
            DeserializeInto(element.jsonDatas, obj);
            return obj;
        }

        public static JsonElement SerializeNode(BaseNode node)
        {
            return Serialize(node);
        }

        public static BaseNode DeserializeNode(JsonElement element)
        {
            try
            {
                Type baseNodeType = Type.GetType(element.type);
                if (element.jsonDatas == null || baseNodeType == null)
                {
                    return null;
                }

                BaseNode node = Activator.CreateInstance(baseNodeType) as BaseNode;
                DeserializeInto(element.jsonDatas, node);
                return node;
            }
            catch
            {
                return null;
            }
        }

        private static void DeserializeInto(string json, object target)
        {
            if (DeserializeOverride != null)
            {
                DeserializeOverride(json, target);
            }
            else
            {
                JsonUtility.FromJsonOverwrite(json, target);
            }
        }
    }
}
