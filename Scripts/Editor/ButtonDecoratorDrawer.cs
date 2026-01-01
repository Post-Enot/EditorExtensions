using PostEnot.Toolkits;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    internal sealed class ButtonDecoratorDrawer : DecoratorDrawer
    {
        public override VisualElement CreatePropertyGUI()
        {
            ButtonAttribute buttonAttribute = attribute as ButtonAttribute;
            Button button = new()
            {
                userData = buttonAttribute.MethodName,
                text = buttonAttribute.Text
            };
            button.RegisterCallbackOnce<AttachToPanelEvent>(OnAttachToPanel);
            return button;
        }

        private void OnAttachToPanel(AttachToPanelEvent context)
        {
            Button button = context.target as Button;
            PropertyField propertyField = button.GetFirstAncestorOfType<PropertyField>();
            SerializedProperty serializedProperty = SerializationUtility.GetSerializedProperty(propertyField);
            SerializedProperty parentSerializedProperty = SerializationUtility.GetParentProperty(serializedProperty);
            string methodName = button.userData as string;

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
