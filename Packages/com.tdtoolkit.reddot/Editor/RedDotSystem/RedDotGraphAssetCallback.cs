using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RedDotSystem.Editor
{
    public static class RedDotGraphAssetCallback
    {
        private static RedDotGraphWindow s_RedDotGraphWindow;
        
        private static RedDotGraph s_RedDotGraph;
        
        public static RedDotGraph RedDotGraph
        {
            get
            {
                if (s_RedDotGraphWindow != null)
                {
                    return s_RedDotGraphWindow.RedDotGraph;
                }

                if (s_RedDotGraph)
                {
                    return s_RedDotGraph;
                }
                
                string[] graphSettingsGuid = AssetDatabase.FindAssets("t:RedDotGraph");
                if (!graphSettingsGuid.Any()) return null;

                string graphSettingsPath = AssetDatabase.GUIDToAssetPath(graphSettingsGuid.First());
                s_RedDotGraph = AssetDatabase.LoadAssetAtPath<RedDotGraph>(graphSettingsPath);
                return s_RedDotGraph;

            }
        }

        [MenuItem("Assets/Create/RedDotGraph", false, 10)]
        public static void CreateRedDotGraphAsset()
        {
            string[] graphSettingsGuid = AssetDatabase.FindAssets("t:RedDotGraph");
            if (graphSettingsGuid.Length > 0)
            {
                Debug.LogError("RedDotGraphAssetCallback.CreateRedDotGraphAsset: RedDotGraph asset already exists!");
                return;
            }
            
            RedDotGraph graph = ScriptableObject.CreateInstance<RedDotGraph>();
            ProjectWindowUtil.CreateAsset(graph, "RedDotGraph.asset");
        }
        
        [OnOpenAsset(0)]
        public static bool OnRedDotGraphOpened(int instanceID, int line)
        {
            RedDotGraph asset = EditorUtility.InstanceIDToObject(instanceID) as RedDotGraph;
            if (asset == null)
            {
                return false;
            }
            
            if (s_RedDotGraphWindow != null)
            {
                s_RedDotGraphWindow.Focus();
            }
            else
            {
                s_RedDotGraphWindow = EditorWindow.CreateWindow<RedDotGraphWindow>();
                s_RedDotGraphWindow.InitializeGraph(asset);
            }
            
            /*
                string assetPath = AssetDatabase.GetAssetPath(instanceID);
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(assetPath, line);
            */
            return true;
        }
    }
}