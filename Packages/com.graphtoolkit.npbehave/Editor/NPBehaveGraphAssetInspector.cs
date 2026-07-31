using GraphProcessor;
using NPBehaveEditor;
using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NPBehaveEditor
{
    [CustomEditor(typeof(NPBehaveGraph), true)]
    public class NPBehaveGraphAssetInspector : GraphInspector
    {
        protected override void CreateInspector()
        {
            base.CreateInspector();

            if (this.graph is NPBehaveGraph behaveGraph)
            {
                SerializedProperty config = this.serializedObject.FindProperty("Config");
                PropertyField propertyField = new PropertyField(config)
                {
                    name = "Config",
                    label = "Configuration"
                };
                // var drawer = new PropertyReferenceDrawer(config, null);
                // var field = new ReferenceField(typeof(IBehaveGraphConfig), drawer)
                // {
                //     IsCollapsable = false,
                //     bindingPath = config.propertyPath
                // };
                // this.root.Add(field);
                this.root.Add(propertyField);

                /*
                this.root.Add(new Button(() =>
                {
                    behaveGraph.Export();
                })
                {
                    text = "Export"
                });
                */
            }
        }
    }
}