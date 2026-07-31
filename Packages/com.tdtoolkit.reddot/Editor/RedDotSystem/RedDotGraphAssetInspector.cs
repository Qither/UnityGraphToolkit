using GraphProcessor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace RedDotSystem.Editor
{
    [CustomEditor(typeof(RedDotGraph), true)]
    public class RedDotGraphAssetInspector : GraphInspector
    {
        protected override void CreateInspector()
        {
            base.CreateInspector();
            if (this.graph is not RedDotGraph redDotGraph)
            {
                return;
            }

            SerializedProperty settings = this.serializedObject.FindProperty(nameof(RedDotGraph.Settings));
            ObjectField settingsField = new ObjectField("Settings")
            {
                objectType = typeof(RedDotGraphSettings),
                bindingPath = settings.propertyPath
            };
            this.root.Add(settingsField);

            Button discoverButton = new Button(redDotGraph.DiscoverTypes)
            {
                text = "Refresh Types"
            };
            this.root.Add(discoverButton);

            Button exportButton = new Button(() =>
            {
                redDotGraph.RedDotExecuteNodeGenerate ??= new RedDotExecuteNodeGenerate(redDotGraph);
                redDotGraph.RedDotExecuteNodeGenerate.Generate();
                redDotGraph.ExportRedDotNode();
            })
            {
                text = "Generate Code and Export JSON"
            };
            this.root.Add(exportButton);
        }
    }
}
