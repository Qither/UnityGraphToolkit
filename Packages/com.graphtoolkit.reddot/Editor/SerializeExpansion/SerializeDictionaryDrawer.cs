#if UNITY_EDITOR
using GraphToolkit.Inspector.Editor;
using SerializeExpansion.Runtime;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SerializeExpansion.Editor
{
    [CustomPropertyDrawer(typeof(SerializeDictionaryAttribute))]
    public class SerializeDictionaryDrawer : PropertyDrawer
    {
        private const string INVALID_TYPE_WARNING = "(NPBehave) invalid type for SerializeDictionaryAttribute on field '{0}': Dictionary can only be applied to SerializedDictionary fields";
        private const string INVALID_ADD_CALLBACK_WARNING = "(NPBehave) invalid add callback for SerializeDictionaryAttribute on field '{0}': The method must accept a string or have no parameters";
        private const string INVALID_ADD_REFERENCE_CALLBACK_WARNING = "(NPBehave) invalid add callback for SerializeDictionaryAttribute on field '{0}': The method must accept a string or have no parameters";
        private const string INVALID_REMOVE_CALLBACK_WARNING = "(NPBehave) invalid remove callback for SerializeDictionaryAttribute on field '{0}': The method must accept an string or have no parameters";
        private const string INVALID_REORDER_CALLBACK_WARNING = "(NPBehave) invalid reorder callback for SerializeDictionaryAttribute on field '{0}': The method must accept two ints or have no parameters";
        private const string INVALID_CHANGE_CALLBACK_WARNING = "(NPBehave) invalid change callback for SerializeDictionaryAttribute on field '{0}': The method must have no parameters";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var keys   = property.FindPropertyRelative(SerializeReferenceDictionary<string, int>.KEY_PROPERTY);
            var values = property.FindPropertyRelative(SerializeReferenceDictionary<string, int>.VALUE_PROPERTY);

            if (keys != null && keys.isArray && values != null && values.isArray && keys.arrayElementType == "string")
            {
                Type fieldTypeBaseType   = this.fieldInfo.FieldType.BaseType;
                var  isReference         = fieldTypeBaseType != null && fieldTypeBaseType.GetGenericTypeDefinition() == typeof(SerializeReferenceDictionary<,>);
                var  referenceType       = isReference ? fieldInfo.GetFieldType() : null;
                var  declaringType       = fieldInfo.DeclaringType;
                var  dictionaryAttribute = attribute as SerializeDictionaryAttribute;
                var  drawer              = this.GetNextDrawer();
                var  proxy               = new PropertyDictionaryProxy(property, keys, values, drawer);

                var field = new DictionaryField
                {
                    IsCollapsable = dictionaryAttribute.IsCollapsable,
                    bindingPath = keys.propertyPath,
                    Label = property.displayName
                };

                // TODO: other stuff from ConfigureField

                if (!string.IsNullOrEmpty(dictionaryAttribute.AddPlaceholder))
                    field.AddPlaceholder = dictionaryAttribute.AddPlaceholder;

                if (!string.IsNullOrEmpty(dictionaryAttribute.EmptyLabel))
                    field.EmptyLabel = dictionaryAttribute.EmptyLabel;

                field.AllowAdd     = dictionaryAttribute.AllowAdd != SerializeDictionaryAttribute.NEVER;
                field.AllowRemove  = dictionaryAttribute.AllowRemove != SerializeDictionaryAttribute.NEVER;
                field.AllowReorder = dictionaryAttribute.AllowReorder;

                SetupAdd(dictionaryAttribute, proxy, field, property, declaringType, isReference);
                SetupRemove(dictionaryAttribute, proxy, field, property, declaringType);
                SetupReorder(dictionaryAttribute, field, property, declaringType);
                SetupChange(dictionaryAttribute, field, property, declaringType);

                field.SetProxy(proxy, referenceType, true);

                return field;
            }
            else
            {
                Debug.LogWarningFormat(INVALID_TYPE_WARNING, property.propertyPath);
                return new FieldContainer(property.displayName);
            }
        }

        private void SetupAdd(SerializeDictionaryAttribute dictionaryAttribute, PropertyDictionaryProxy proxy, DictionaryField field, SerializedProperty property, Type declaringType, bool isReference)
        {
            if (field.AllowAdd)
            {
                if (!string.IsNullOrEmpty(dictionaryAttribute.AllowAdd))
                {
                    proxy.CanAddKeyCallback = ReflectionHelper.CreateFunctionCallback<string, bool>(dictionaryAttribute.AllowAdd, declaringType, property);
                    if (proxy.CanAddKeyCallback == null)
                    {
                        var canAdd = ReflectionHelper.CreateValueSourceFunction(dictionaryAttribute.AllowAdd, property, field, declaringType, true);
                        proxy.CanAddKeyCallback = index => canAdd();
                    }
                }

                if (!string.IsNullOrEmpty(dictionaryAttribute.AddCallback))
                {
                    if (!isReference)
                    {
                        var addCallback = ReflectionHelper.CreateActionCallback(dictionaryAttribute.AddCallback, declaringType, property);
                        if (addCallback != null)
                        {
                            field.RegisterCallback<DictionaryField.ItemAddedEvent>(evt => addCallback.Invoke());
                        }
                        else
                        {
                            var addCallbackKey = ReflectionHelper.CreateActionCallback<string>(dictionaryAttribute.AddCallback, declaringType, property);
                            if (addCallbackKey != null)
                                field.RegisterCallback<DictionaryField.ItemAddedEvent>(evt => addCallbackKey.Invoke(evt.Key));
                            else
                                Debug.LogWarningFormat(INVALID_ADD_CALLBACK_WARNING, property.propertyPath);
                        }
                    }
                    else
                    {
                        var addCallback = ReflectionHelper.CreateActionCallback(dictionaryAttribute.AddCallback, declaringType, property);
                        if (addCallback != null)
                        {
                            field.RegisterCallback<DictionaryField.ItemAddedEvent>(evt => addCallback.Invoke());
                        }
                        else
                        {
                            var addCallbackKey = ReflectionHelper.CreateActionCallback<string>(dictionaryAttribute.AddCallback, declaringType, property);
                            if (addCallbackKey != null)
                                field.RegisterCallback<DictionaryField.ItemAddedEvent>(evt => addCallbackKey.Invoke(evt.Key));
                            else
                                Debug.LogWarningFormat(INVALID_ADD_REFERENCE_CALLBACK_WARNING, property.propertyPath);
                        }
                    }
                }
            }
        }

        private void SetupRemove(SerializeDictionaryAttribute dictionaryAttribute, PropertyDictionaryProxy proxy, DictionaryField field, SerializedProperty property, Type declaringType)
        {
            if (field.AllowRemove)
            {
                if (!string.IsNullOrEmpty(dictionaryAttribute.AllowRemove))
                {
                    proxy.CanRemoveCallback = ReflectionHelper.CreateFunctionCallback<string, bool>(dictionaryAttribute.AllowRemove, declaringType, property);
                    if (proxy.CanRemoveCallback == null)
                    {
                        var canRemove = ReflectionHelper.CreateValueSourceFunction(dictionaryAttribute.AllowRemove, property, field, declaringType, true);
                        proxy.CanRemoveCallback = index => canRemove();
                    }
                }

                if (!string.IsNullOrEmpty(dictionaryAttribute.RemoveCallback))
                {
                    var removeCallback = ReflectionHelper.CreateActionCallback(dictionaryAttribute.RemoveCallback, declaringType, property);
                    if (removeCallback != null)
                    {
                        field.RegisterCallback<DictionaryField.ItemRemovedEvent>(evt => removeCallback.Invoke());
                    }
                    else
                    {
                        var removeCallbackKey = ReflectionHelper.CreateActionCallback<string>(dictionaryAttribute.RemoveCallback, declaringType, property);
                        if (removeCallbackKey != null)
                            field.RegisterCallback<DictionaryField.ItemRemovedEvent>(evt => removeCallbackKey.Invoke(evt.Key));
                        else
                            Debug.LogWarningFormat(INVALID_REMOVE_CALLBACK_WARNING, property.propertyPath);
                    }
                }
            }
        }

        private void SetupReorder(SerializeDictionaryAttribute dictionaryAttribute, DictionaryField field, SerializedProperty property, Type declaringType)
        {
            if (field.AllowReorder)
            {
                if (!string.IsNullOrEmpty(dictionaryAttribute.ReorderCallback))
                {
                    var reorderCallback = ReflectionHelper.CreateActionCallback(dictionaryAttribute.ReorderCallback, declaringType, property);
                    if (reorderCallback != null)
                    {
                        field.RegisterCallback<DictionaryField.ItemReorderedEvent>(evt => reorderCallback.Invoke());
                    }
                    else
                    {
                        var reorderCallbackFromTo = ReflectionHelper.CreateActionCallback<int, int>(dictionaryAttribute.ReorderCallback, declaringType, property);
                        if (reorderCallbackFromTo != null)
                            field.RegisterCallback<DictionaryField.ItemReorderedEvent>(evt => reorderCallbackFromTo.Invoke(evt.FromIndex, evt.ToIndex));
                        else
                            Debug.LogWarningFormat(INVALID_REORDER_CALLBACK_WARNING, property.propertyPath);
                    }
                }
            }
        }

        private void SetupChange(SerializeDictionaryAttribute dictionaryAttribute, DictionaryField field, SerializedProperty property, Type declaringType)
        {
            if (!string.IsNullOrEmpty(dictionaryAttribute.ChangeCallback))
            {
                var changeCallback = ReflectionHelper.CreateActionCallback(dictionaryAttribute.ChangeCallback, declaringType, property);
                if (changeCallback != null)
                    field.RegisterCallback<DictionaryField.ItemsChangedEvent>(evt => changeCallback.Invoke());
                else
                    Debug.LogWarningFormat(INVALID_CHANGE_CALLBACK_WARNING, property.propertyPath);
            }
        }
    }
}
#endif