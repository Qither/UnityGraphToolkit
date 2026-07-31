#if UNITY_EDITOR
using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NPBehave
{
    [CustomPropertyDrawer(typeof(MatchInfo), true)]
    public class MatchInfoDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement()
            {
                name = "NPMatchInfo",
                style =
                {
                    minWidth = 220,
                }
            };
            SerializedProperty op = property?.FindPropertyRelative("op");
            if (op != null)
            {
                root.Add(op.CreateField());
            }
            
            SerializedProperty blackBoardRelationData = property?.FindPropertyRelative("blackBoardRelationData");
            if (blackBoardRelationData == null)
            {
                return root;
            }
            
            FieldInfo[] fields = typeof(NPBlackBoardHandleData).GetFields();
            foreach (FieldInfo field in fields)
            {
                SerializedProperty  fieldValue         = blackBoardRelationData.FindPropertyRelative(field.Name);
                PropertyAttribute[] propertyAttributes = field.GetCustomAttributes<PropertyAttribute>().ToArray();
                if (!propertyAttributes.Any())
                {
                    continue;
                }
                foreach (PropertyAttribute propertyAttribute in propertyAttributes)
                {
                    var drawerType = PropertyDrawerExtensions.GetDrawerTypeForType(propertyAttribute?.GetType() ?? field.GetFieldType());

                    if (drawerType != null)
                    {
                        var drawer = drawerType.CreateInstance<PropertyDrawer>();
                        drawer.SetFieldInfo(field);
                        drawer.SetAttribute(propertyAttribute);
                        root.Add(drawer.CreatePropertyGUI(fieldValue));
                    }
                    if (propertyAttribute is ChangeTriggerAttribute)
                    {
                        break;
                    }
                }
            }
            
            return root;
        }
    }
}
#endif