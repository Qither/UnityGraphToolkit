#if UNITY_EDITOR
using PiRhoSoft.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NPBehave
{
    public class SharedValuePropertyReferenceDrawer : IReferenceDrawer
    {
        private readonly SerializedProperty m_Property;
        private readonly PropertyDrawer     m_Drawer;

        private VisualElement m_Element;

        public SharedValuePropertyReferenceDrawer(SerializedProperty property, PropertyDrawer drawer)
        {
            this.m_Property = property;
            this.m_Drawer   = drawer;
        }

        public VisualElement CreateElement(object value)
        {
            ElementUnbind(this.m_Element);
            this.m_Property.serializedObject.Update();
            this.m_Element = this.m_Drawer.CreatePropertyGUI(this.m_Property);
            return this.m_Element;
        }

        private void ElementUnbind(VisualElement element)
        {
            if (element == null) return;
                element.Unbind();
            
            if (element.Children().Any())
            {
                foreach (VisualElement childElement in element.Children())
                {
                    ElementUnbind(childElement);
                }
            }
        }
    }
}
#endif