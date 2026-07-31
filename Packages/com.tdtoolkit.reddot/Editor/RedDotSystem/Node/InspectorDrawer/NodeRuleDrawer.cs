using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace RedDotSystem.Editor.Node
{
    [CustomPropertyDrawer(typeof(NodeRule))]
    public class NodeRuleDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty rules  = property.FindPropertyRelative("Rules");
            PropertyField      ruleField = new PropertyField(rules);
            return ruleField;
        }
    }
}