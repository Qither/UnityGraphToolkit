using UnityEditor;

namespace PiRhoSoft.Utilities.Editor
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
