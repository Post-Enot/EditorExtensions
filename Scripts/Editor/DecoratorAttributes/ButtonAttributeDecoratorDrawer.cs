using PostEnot.Toolkits;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    internal sealed class ButtonAttributeDecoratorDrawer : DecoratorDrawer
    {
        public override VisualElement CreatePropertyGUI()
        {
            ButtonAttribute buttonAttribute = attribute as ButtonAttribute;
            VisualElement temp = new()
            {
                name = "TEMP",
                userData = buttonAttribute
            };
            temp.RegisterCallbackOnce<AttachToPanelEvent>(OnAttachToPanel);
            return temp;
        }

        private void OnAttachToPanel(AttachToPanelEvent context)
        {
            VisualElement temp = context.target as VisualElement;
            PropertyField propertyField = temp.GetFirstAncestorOfType<PropertyField>();
            SerializedProperty serializedProperty = SerializationUtility.GetSerializedProperty(propertyField);
            SerializedProperty parentSerializedProperty = SerializationUtility.GetParentProperty(serializedProperty);
            ButtonAttribute buttonAttribute = temp.userData as ButtonAttribute;
            string methodName = buttonAttribute.MethodName;

            Button button = new()
            {
                text = buttonAttribute.Text,
            };
            button.style.marginLeft = 0;
            button.style.marginRight = 0;

            if (buttonAttribute.Position is ButtonPosition.Down)
            {
                propertyField.Add(button);
            }
            else if (buttonAttribute.Position is ButtonPosition.Up)
            {
                int index = temp.parent.IndexOf(temp);
                temp.parent.Insert(index, button);
            }

            object instance = parentSerializedProperty switch
            {
                null => serializedProperty.serializedObject.targetObject,
                { propertyType: SerializedPropertyType.ExposedReference or SerializedPropertyType.ObjectReference } => parentSerializedProperty.objectReferenceValue,
                { propertyType: SerializedPropertyType.ManagedReference } => parentSerializedProperty.managedReferenceValue,
                _ => null
            };
            if (instance != null)
            {
                Type instanceType = instance.GetType();
                MethodInfo methodInfo = SerializationUtility.FindMethod(instanceType, methodName);
                button.clickable = new Clickable(() =>
                {
                    methodInfo.Invoke(instance, null);
                    serializedProperty.serializedObject.ApplyModifiedProperties();
                });
            }
            else if (parentSerializedProperty.propertyType is SerializedPropertyType.Generic)
            {
                instance = parentSerializedProperty.boxedValue;
                Type instanceType = instance.GetType();
                MethodInfo methodInfo = SerializationUtility.FindMethod(instanceType, methodName);
                button.clickable = new Clickable(() =>
                {
                    object instance = parentSerializedProperty.boxedValue;
                    methodInfo.Invoke(instance, null);
                    parentSerializedProperty.boxedValue = instance;
                    parentSerializedProperty.serializedObject.ApplyModifiedProperties();
                });
            }
        }
    }
}
