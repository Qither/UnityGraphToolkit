using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RedDotSystem.Editor.Node
{
    [CustomPropertyDrawer(typeof(RedDotNode.RedDotNodeData))]
    public class NodeDataDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root        = new VisualElement();
            RedDotGraph   redDotGraph = property.serializedObject.targetObject as RedDotGraph;
            if (redDotGraph == null) return root;

            string     path      = property.propertyPath;
            string[]   pathArray = path.Split('.');
            int        index     = int.Parse(pathArray[2].Substring(5, pathArray[2].Length - 6));
            RedDotNode node      = redDotGraph.nodes[index] as RedDotNode;

            if (node == null) return root;
                
            SerializedProperty nodeProperty = property.FindPropertyRelative("Node");
            PropertyField      nodeField    = new PropertyField(nodeProperty);
            nodeField.RegisterValueChangeCallback(node.Data.NodeChanged);
            SerializedProperty linkProperty = property.FindPropertyRelative("Link");
            PropertyField      linkField    = new PropertyField(linkProperty);
            linkField.RegisterValueChangeCallback(node.Data.LinkChanged);
            SerializedProperty descProperty = property.FindPropertyRelative("Desc");
            PropertyField      descField    = new PropertyField(descProperty);
            descField.SetFieldLabel(null);
            SerializedProperty systemProperty = property.FindPropertyRelative("System");
            PropertyField systemField = new PropertyField(systemProperty);
            descField.SetFieldLabel(null);
            SerializedProperty orderProperty = property.FindPropertyRelative("Order");
            PropertyField      orderField    = new PropertyField(orderProperty);
            orderField.SetEnabled(false);
            SerializedProperty keyProperty = property.FindPropertyRelative("Key");
            PropertyField      keyField    = new PropertyField(keyProperty);
            keyField.SetFieldLabel(null);
            keyField.SetEnabled(false);
            SerializedProperty numberProperty = property.FindPropertyRelative("Number");
            PropertyField      numberField    = new PropertyField(numberProperty);
            numberField.SetEnabled(false);
            numberField.RegisterValueChangeCallback(node.Data.NumberChanged);

            root.Add(nodeField);
            root.Add(linkField);
            root.Add(descField);
            root.Add(systemField);
            root.Add(orderField);
            root.Add(keyField);
            root.Add(numberField);
            return root;
        }
    }
}