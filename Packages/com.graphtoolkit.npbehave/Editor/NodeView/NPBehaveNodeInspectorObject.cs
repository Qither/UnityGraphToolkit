using GraphProcessor;
using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NPBehaveEditor
{
    [CustomEditor(typeof(NPBehaveNodeInspectorObject))]
    public class NPBehaveNodeInspectorObjectEditor : NodeInspectorObjectEditor
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            this.selectedNodeList.styleSheets.Remove(Resources.Load<StyleSheet>("GraphProcessorStyles/InspectorView"));
            this.selectedNodeList.styleSheets.Add(Resources.Load<StyleSheet>("NPBehaveGraphStyles/InspectorView"));
        }

        protected override void UpdateNodeInspectorList()
        {
            // Pre-build: expand all serialized properties so Frame binding reads isExpanded=true
            var inspectorObj = target as NodeInspectorObject;
            foreach (var nodeView in inspectorObj.selectedNodes)
            {
                if (nodeView.owner == null || nodeView.owner.serializedGraph == null)
                    continue;

                int nodeIndex = nodeView.owner.graph.nodes.FindIndex(n => n == nodeView.nodeTarget);
                if (nodeIndex < 0)
                    continue;

                var nodesProp = nodeView.owner.serializedGraph.FindProperty("nodes");
                if (nodesProp == null || nodeIndex >= nodesProp.arraySize)
                    continue;

                var nodeProp = nodesProp.GetArrayElementAtIndex(nodeIndex);
                ExpandPropertyRecursive(nodeProp);
            }

            base.UpdateNodeInspectorList();

            // Post-build: force-expand Frame elements after bindings are established
            // ExecuteLater ensures bindings have completed before we override the state
            selectedNodeList.schedule.Execute(() =>
            {
                selectedNodeList.Query<Frame>().ForEach(frame =>
                {
                    frame.IsCollapsed = false;
                });
            }).ExecuteLater(100);
        }

        /// <summary>
        /// Recursively set isExpanded=true on the property and all its descendants
        /// </summary>
        private void ExpandPropertyRecursive(SerializedProperty property)
        {
            if (property == null)
                return;

            property.isExpanded = true;

            var iterator = property.Copy();
            var endProperty = property.GetEndProperty();

            while (iterator.Next(true))
            {
                if (SerializedProperty.EqualContents(iterator, endProperty))
                    break;
                iterator.isExpanded = true;
            }
        }
    }
    public class NPBehaveNodeInspectorObject : NodeInspectorObject
    {
    }
}