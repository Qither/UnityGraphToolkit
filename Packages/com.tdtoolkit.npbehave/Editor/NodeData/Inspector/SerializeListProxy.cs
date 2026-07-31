#if UNITY_EDITOR
using PiRhoSoft.Utilities.Editor;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NPBehave
{
    public class SerializeListProxy : IListProxy
    {
        public Func<object>     OnAddItem;
        public Func<bool>       CanAddCallback;
        public Func<Type, bool> CanAddTypeCallback;
        public Func<int, bool>  CanRemoveCallback;

        private readonly SerializedProperty m_Property;
        private readonly PropertyDrawer m_Drawer;

        public SerializeListProxy(SerializedProperty property, PropertyDrawer drawer)
        {
            this.m_Property = property;
            this.m_Drawer   = drawer;
        }

        public VisualElement CreateElement(int index)
        {
            var property = this.m_Property.GetArrayElementAtIndex(index);
            var field    = this.m_Drawer?.CreatePropertyGUI(property) ?? property.CreateField();
            field.Bind(this.m_Property.serializedObject);

            if (!(field is Foldout))
                field.SetFieldLabel(null); // TODO: for references this should be the type name

            return field;
        }

        public int Count => this.m_Property.arraySize;

        public bool CanAdd()
        {
            return this.CanAddCallback?.Invoke() ?? true;
        }

        public bool CanAdd(Type type)
        {
            return type == null || this.CanAddTypeCallback == null || this.CanAddTypeCallback.Invoke(type);
        }

        public bool AddItem(Type type)
        {
            try
            {
                var newSize = this.m_Property.arraySize + 1;
                this.m_Property.ResizeArray(newSize);
                if (type != null)
                {
                    object newValue = this.OnAddItem != null ? this.OnAddItem.Invoke() : Activator.CreateInstance(type);
                    var valueProperty = this.m_Property.GetArrayElementAtIndex(newSize - 1);

                    if (!valueProperty.TrySetValue(newValue))
                    {
                        this.m_Property.arraySize = newSize - 1;
                        return false;
                    }
                }
                this.m_Property.serializedObject.ApplyModifiedProperties(); // TODO: not applying new reference values for some reason
                return true;
            }
            catch
            {
                // Technically a user could do something really wierd like set the item type on the DictionaryField
                // to Float when the property is actually a string

                // TODO: this also happens if the type is not Serializable (_property will be null)
                return false;
            }
        }

        public bool CanRemove(int index)
        {
            return this.CanRemoveCallback?.Invoke(index) ?? true;
        }
        
        public void RemoveItem(int index)
        {
            this.m_Property.RemoveFromArray(index);
            this.m_Property.serializedObject.ApplyModifiedProperties();
        }

        public void ReorderItem(int from, int to)
        {
            this.m_Property.MoveArrayElement(from, to);
            this.m_Property.serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif