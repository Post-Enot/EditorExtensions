using PostEnot.Toolkits;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    internal sealed class ButtonAttributeDrawer : DecoratorDrawer
    {
        public override VisualElement CreatePropertyGUI()
        {
            VisualElement temp = new()
            {
                name = "TEMP",
                userData = attribute
            };
            temp.RegisterCallbackOnce<AttachToPanelEvent>(OnAttachToPanel);
            return temp;
        }

        private void OnAttachToPanel(AttachToPanelEvent context)
        {
            VisualElement temp = context.target as VisualElement;
            ButtonAttribute attribute = temp.userData as ButtonAttribute;
            temp.schedule.Execute(() => OnAfterAttach(attribute, temp));
        }

        private void OnAfterAttach(ButtonAttribute attribute, VisualElement temp)
        {
            PropertyField propertyField = temp.GetFirstAncestorOfType<PropertyField>();
            SerializedProperty serializedProperty = SerializationUtility.GetSerializedProperty(propertyField);
            FieldInfo fieldInfo = SerializationUtility.GetFieldInfo(serializedProperty);
            SerializedProperty parentSerializedProperty = SerializationUtility.GetParentProperty(serializedProperty);
            string methodName = attribute.MethodName;

            Button button = new()
            {
                text = attribute.Text ?? ObjectNames.NicifyVariableName(attribute.MethodName),
            };
            button.style.marginLeft = 0;
            button.style.marginRight = 0;
            object instance = parentSerializedProperty switch
            {
                null => serializedProperty.serializedObject.targetObject,
                { propertyType: SerializedPropertyType.ExposedReference or SerializedPropertyType.ObjectReference }
                    => parentSerializedProperty.objectReferenceValue,
                { propertyType: SerializedPropertyType.ManagedReference } => parentSerializedProperty.managedReferenceValue,
                _ => null
            };
            if (instance != null)
            {
                MethodInfo methodInfo = SerializationUtility.FindMethod(fieldInfo.DeclaringType, methodName);
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
            UIUtility.ApplyDrawMode(attribute.DrawMode, temp, button);
            temp.RegisterCallbackOnce<DetachFromPanelEvent>(_ => button.RemoveFromHierarchy());
        }
    }
}
