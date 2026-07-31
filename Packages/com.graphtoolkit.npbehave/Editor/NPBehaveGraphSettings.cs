using GraphToolkit.Inspector;
using System;
using UnityEditor;
using UnityEngine;

namespace NPBehaveEditor
{
    [Serializable]
    public class BehaveCRCDictionary : SerializedDictionary<string, string>
    {
    }

    [CreateAssetMenu(fileName = "NPBehaveGraphSettings", menuName = "Unity Graph Toolkit/NPBehave Settings")]
    public class NPBehaveGraphSettings : ScriptableObject
    {
        public string actionNodePath = "Assets/ToolkitDemo/NPBehave/Actions";

        public string actionPath = "Assets/ToolkitDemo/NPBehave/Actions";

        public TextAsset actionNodeTemplate;

        public string graphPath = "Assets/ToolkitDemo/NPBehave/Graphs";

        public string graphExportPath = "Assets/ToolkitDemo/NPBehave/Exports";

        public string assetBundleName = string.Empty;

        public BehaveCRCDictionary graphCRCDictionary = new BehaveCRCDictionary();

        [NonSerialized]
        public bool IsShowAction;

        [NonSerialized]
        public bool IsShowBehaveGraph;

        public MonoScript exportScript;

        [NonSerialized]
        public IBehaveGraphExport Exporter;

        public void SetExporter()
        {
            if (this.exportScript != null && typeof(IBehaveGraphExport).IsAssignableFrom(this.exportScript.GetClass()))
            {
                this.Exporter = (IBehaveGraphExport)Activator.CreateInstance(this.exportScript.GetClass());
            }
            else
            {
                this.Exporter = new DefaultBehaveGraphExport();
            }
        }
    }
}
