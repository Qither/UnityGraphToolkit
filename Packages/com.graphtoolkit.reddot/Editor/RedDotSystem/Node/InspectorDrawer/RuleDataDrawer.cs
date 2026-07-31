using PiRhoSoft.Utilities.Editor;
using System;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace RedDotSystem.Editor.Node
{
    [CustomPropertyDrawer(typeof(RuleData), true)]
    public class RuleDataDrawer: PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement element = new VisualElement()
            {
                name = "Rule",
                style =
                {
                    flexGrow = 1,
                }
            };

            if (property == null)
            {
                return element;
            }

            foreach (var child in property.Children())
            {
                var field = new PropertyField(child);

                element.Add(field);
            }


            return element;
        }
    }
}