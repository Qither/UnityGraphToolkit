#if UNITY_EDITOR
using PiRhoSoft.Utilities.Editor;
using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace NPBehave
{
    [CustomPropertyDrawer(typeof(SharedValueReferenceAttribute))]
    class SharedValueReferenceDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SharedValueReferenceAttribute      referenceAttribute = this.attribute as SharedValueReferenceAttribute;
            Type                               type               = this.GetFieldType();
            PropertyDrawer                     next               = this.GetNextDrawer();
            SharedValuePropertyReferenceDrawer drawer             = new SharedValuePropertyReferenceDrawer(property, next);
            ReferenceField field = new ReferenceField(type, drawer)
            {
                IsCollapsable = true,
                bindingPath   = property.propertyPath // TODO: other stuff from ConfigureField
            };
            field.Label                                                                         = referenceAttribute?.CustomLabel;
            field.Q<IconButton>(className: "pirho-reference-field__clear-button").style.display = DisplayStyle.None;

            return field;
        }
    }
}
#endif