using UnityEditor;

namespace GraphToolkit.Inspector.Editor
{
    [InitializeOnLoad]
    internal static class ResourceEditorBridge
    {
        static ResourceEditorBridge()
        {
            Resource.AssetPathResolver = AssetDatabase.GetAssetPath;
        }
    }
}
