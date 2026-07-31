#if UNITY_EDITOR
using System;
using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NPBehave
{
    [CustomPropertyDrawer(typeof(NPBlackBoardHandleData), true)]
    public class BlackBoardHandleDataDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Box root = new Box()
            {
                name = "NPBlackBoardHandleData",
                style =
                {
                    minWidth                = 220,
                    borderBottomColor       = Color.black,
                    borderBottomWidth       = 1,
                    borderLeftColor         = Color.black,
                    borderLeftWidth         = 1,
                    borderRightColor        = Color.black,
                    borderRightWidth        = 1,
                    borderTopColor          = Color.black,
                    borderTopWidth          = 1,
                    borderTopLeftRadius     = 5,
                    borderTopRightRadius    = 5,
                    borderBottomLeftRadius  = 5,
                    borderBottomRightRadius = 5,
                    marginBottom            = 5,
                    marginLeft              = 5,
                    marginRight             = 5,
                    marginTop               = 5,
                }
            };
            string fieldName = $"{this.fieldInfo.Name[0].ToString().ToUpper()}{this.fieldInfo.Name.Remove(0, 1)}";
            Label label = new Label(fieldName)
            {
                style =
                {
                    fontSize                = 15,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    borderBottomWidth       = 1,
                    borderBottomColor       = Color.black,
                    borderTopLeftRadius     = 5,
                    borderTopRightRadius    = 5,
                    paddingBottom           = 2,
                    paddingLeft             = 5,
                    paddingRight            = 5,
                    paddingTop              = 2,
                    marginBottom            = 5,
                    backgroundColor         = new Color(0.345098f, 0.345098f, 0.345098f)
                }
            };
            root.Add(label);
            
            VisualElement container = new VisualElement()
            {
                style =
                {
                    paddingBottom = 5,
                    paddingLeft   = 5,
                    paddingRight  = 5,
                    paddingTop    = 5,
                }
            };
            root.Add(container);
            FieldInfo[] fields = this.fieldInfo.FieldType.GetFields();
            foreach (FieldInfo field in fields)
            {
                SerializedProperty  fieldValue         = property?.FindPropertyRelative(field.Name);
                PropertyAttribute[] propertyAttributes = field.GetCustomAttributes<PropertyAttribute>().ToArray();
                if (!propertyAttributes.Any())
                {
                    if (fieldValue != null)
                    {
                        container.Add(fieldValue.CreateField());
                    }
                    continue;
                }
                foreach (PropertyAttribute propertyAttribute in propertyAttributes)
                {
                    Type drawerType = PropertyDrawerExtensions.GetDrawerTypeForType(propertyAttribute?.GetType() ?? field.GetFieldType());

                    // TODO: This is a temporary plan
                    if (drawerType != null && drawerType != typeof(SharedValueReferenceDrawer))
                    {
                        PropertyDrawer drawer = drawerType.CreateInstance<PropertyDrawer>();
                        drawer.SetFieldInfo(field);
                        drawer.SetAttribute(propertyAttribute);
                        container.Add(drawer.CreatePropertyGUI(fieldValue));
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