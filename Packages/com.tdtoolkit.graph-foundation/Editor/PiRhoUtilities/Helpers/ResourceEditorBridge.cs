using UnityEditor;

namespace PiRhoSoft.Utilities.Editor
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
