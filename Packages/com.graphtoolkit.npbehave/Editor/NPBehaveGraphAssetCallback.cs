using GraphProcessor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace NPBehaveEditor
{
    public class NPBehaveGraphAssetCallback
    {
        [MenuItem("Assets/Create/NPBehaveGraph", false, 10)]
        public static void CreateNPBehaveGraphAsset()
        {
            NPBehaveGraph graph = ScriptableObject.CreateInstance<NPBehaveGraph>();
            ProjectWindowUtil.CreateAsset(graph, "NPBehaveGraph.asset");
        }
        
        [OnOpenAsset(0)]
        public static bool OnNPBehaveGraphOpened(int instanceID, int line)
        {
            NPBehaveGraph asset = EditorUtility.InstanceIDToObject(instanceID) as NPBehaveGraph;
            if (asset == null)
            {
                return false;
            }

            GraphWindowHelper.GetAndShowGraphWindow<NPBehaveGraphWindow>(asset).InitializeGraph(asset);
            return true;
        }
    }
}