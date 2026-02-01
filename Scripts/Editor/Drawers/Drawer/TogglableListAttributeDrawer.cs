using PostEnot.Toolkits;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(TogglableListAttribute))]
    internal sealed class TogglableListAttributeDrawer : BasePropertyDrawer<TogglableListAttribute>
    {
        private protected override VisualElement CreateProperty(
            SerializedProperty property,
            FieldInfo fieldInfo,
            TogglableListAttribute attribute)
        {
            if (!property.isArray)
            {
                return new Label("Use TogglableList with collections.");
            }
            VisualElement container = null;
            if (attribute.Foldout == null)
            {
                container = new VisualElement();
            }
            else
            {
                container = new Foldout()
                {
                    text = attribute.Foldout,
                    value = false
                };
            }
            ToggleButtonGroup toggleButtonGroup = new()
            {
                allowEmptySelection = false,
                isMultipleSelection = false
            };
            PropertyField propertyField = new();
            toggleButtonGroup.style.alignSelf = Align.Center;
            string[] enumNames = attribute.EnumType.GetEnumNames();
            foreach (string enumName in enumNames)
            {
                Button button = new()
                {
                    text = enumName
                };
                toggleButtonGroup.Add(button);
            }
            if (property.arraySize == 0)
            {
                property.InsertArrayElementAtIndex(0);
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
            SerializedProperty elementProperty = property.GetArrayElementAtIndex(0);
            propertyField.BindProperty(elementProperty);
            container.Add(toggleButtonGroup);
            container.Add(propertyField);
            toggleButtonGroup.RegisterValueChangedCallback(context =>
            {
                int[] activeOptionsSource = new int[context.newValue.length];
                Span<int> activeOptions = context.newValue.GetActiveOptions(activeOptionsSource);
                if (activeOptions.IsEmpty)
                {
                    propertyField.style.display = DisplayStyle.None;
                    return;
                }
                int activeOption = activeOptions[0];
                if (activeOption >= property.arraySize)
                {
                    int diff = activeOption - property.arraySize;
                    for (int i = 0; i <= diff; i += 1)
                    {
                        property.GetArrayElementAtIndex(property.arraySize - 1).DuplicateCommand();
                    }
                    property.serializedObject.ApplyModifiedProperties();
                }
                SerializedProperty elementProperty = property.GetArrayElementAtIndex(activeOption);
                propertyField.Unbind();
                propertyField.BindProperty(elementProperty);
                propertyField.style.display = DisplayStyle.Flex;
            });
            return container;
        }
    }
}
