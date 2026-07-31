using UnityEditor;

namespace GraphProcessor
{
    [InitializeOnLoad]
    internal static class JsonSerializerEditorBridge
    {
        static JsonSerializerEditorBridge()
        {
            JsonSerializer.SerializeOverride = EditorJsonUtility.ToJson;
            JsonSerializer.DeserializeOverride = EditorJsonUtility.FromJsonOverwrite;
        }
    }
}
