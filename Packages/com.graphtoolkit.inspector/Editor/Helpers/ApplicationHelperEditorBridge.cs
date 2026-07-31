using UnityEditor;

namespace GraphToolkit.Inspector.Editor
{
    [InitializeOnLoad]
    internal static class ApplicationHelperEditorBridge
    {
        static ApplicationHelperEditorBridge()
        {
            ApplicationHelper.IsPlayingOverride = () => EditorApplication.isPlayingOrWillChangePlaymode;
        }
    }
}
