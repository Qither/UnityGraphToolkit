#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NPBehave
{
    [CustomPropertyDrawer(typeof(ASharedValue), true)]
    public class SharedValueDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement element = new VisualElement()
            {
                name = "SharedValue",
                style =
                {
                    flexGrow = 1,
                }
            };

            SerializedProperty childValue = property?.FindPropertyRelative("Value");
            /*
            string             sharedValueNamespace = property?.managedReferenceFullTypename.Split(' ')[0];
            string             sharedValue          = property?.managedReferenceFullTypename.Split(' ')[1];
            Type               sharedValueType      = Type.GetType($"{sharedValue}, {sharedValueNamespace}");

            if (sharedValueType != null)
            {
                var drawerType = PropertyDrawerExtensions.GetDrawerTypeForType(sharedValueType);

                if (drawerType != null)
                {
                    var drawer = drawerType.CreateInstance<PropertyDrawer>();
                    return drawer.CreatePropertyGUI(property);
                }
            }
            */

            if (childValue == null)
            {
                element.Add(new Label("Null"));
                return element;
            }

            if (childValue.propertyType == SerializedPropertyType.Generic)
            {
                //var drawer = this.GetNextDrawer();
                var proxy  = new PropertyListProxy(childValue, null);
                
                var field = new ListField
                {
                    IsCollapsable = true,
                    bindingPath   = childValue.propertyPath,
                    Label         = property.managedReferenceFullTypename.Split('.')[1].Substring(6)
                };
                field.SetProxy(proxy, null, false);
                element.Add(field);
                return element;
            }
            
            VisualElement childField = childValue.CreateField();
            childField.name = "SharedValueField";
            if (childField[0] is Label label)
            {
                label.text           = property.managedReferenceFullTypename.Split('.')[1].Substring(6);
                label.style.minWidth = 45;
            }
            
            element.Add(childField);
            element.Bind(property.serializedObject);
            return element;
        }
    }
}
#endif