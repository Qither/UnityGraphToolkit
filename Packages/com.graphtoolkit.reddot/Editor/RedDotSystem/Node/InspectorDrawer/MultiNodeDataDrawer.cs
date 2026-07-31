using GraphToolkit.Inspector.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace RedDotSystem.Editor.Node
{
    [CustomPropertyDrawer(typeof(RedDotMultiNode.RedDotMultiNodeData))]
    public class MultiNodeDataDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement      root                 = new VisualElement();
            SerializedProperty nodeDataNameProperty = property.FindPropertyRelative("MultiNodeData");
            PropertyField      nodeDataNameField    = new PropertyField(nodeDataNameProperty);
            root.Add(nodeDataNameField);

            SerializedProperty nodeDataSystemProperty = property.FindPropertyRelative("System");
            PropertyField nodeDataSystemField = new PropertyField(nodeDataSystemProperty);
            root.Add(nodeDataSystemField);
            return root;
        }
    }
}