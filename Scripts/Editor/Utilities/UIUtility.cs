using PostEnot.Toolkits;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal static class UIUtility
    {
        private const string _footerDecoratorDrawerContainerName = "footer-decorator-drawers-container";
        private const string _decoratorDrawerContainerUss = "unity-decorator-drawers-container";

        internal static Texture2D LoadIcon(string iconPath)
        {
            if (string.IsNullOrWhiteSpace(iconPath))
            {
                return null;
            }
            if (iconPath.StartsWith("builtin:"))
            {
                string iconName = iconPath[(iconPath.IndexOf(':') + 1)..];
                return EditorGUIUtility.IconContent(iconName).image as Texture2D;
            }
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            return texture;
        }

        internal static string NicifyButtonName(string buttonText, MethodInfo methodInfo)
        {
            if (buttonText != null)
            {
                return buttonText;
            }
            if (methodInfo == null)
            {
                return string.Empty;
            }
            return ObjectNames.NicifyVariableName(methodInfo.Name);
        }

        internal static Action CreateActionFromMethodName(
            SerializedProperty serializedProperty,
            SerializedProperty parentSerializedProperty,
            FieldInfo fieldInfo,
            string methodName,
            out MethodInfo methodInfo)
        {
            methodInfo = null;
            if (string.IsNullOrWhiteSpace(methodName))
            {
                return CreateInvalidMethodNameAction();
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
                methodInfo = SerializationUtility.FindMethod(fieldInfo.DeclaringType, methodName);
                Action action = SerializationUtility.MethodInfoToDelegate(methodInfo, instance);
                return () =>
                {
                    action?.Invoke();
                    serializedProperty.serializedObject.ApplyModifiedProperties();
                };
            }
            else if (parentSerializedProperty.propertyType is SerializedPropertyType.Generic)
            {
                instance = parentSerializedProperty.boxedValue;
                Type instanceType = instance.GetType();
                methodInfo = SerializationUtility.FindMethod(instanceType, methodName);
                if (methodInfo == null)
                {
                    return CreateInvalidMethodNameAction();
                }
                MethodInfo methodInfoCopy = methodInfo;
                return () =>
                {
                    object instance = parentSerializedProperty.boxedValue;
                    Action action = SerializationUtility.MethodInfoToDelegate(methodInfoCopy, instance);
                    action.Invoke();
                    parentSerializedProperty.boxedValue = instance;
                    parentSerializedProperty.serializedObject.ApplyModifiedProperties();
                };
            }
            return CreateInvalidMethodNameAction();
        }

        internal static void WrapGenericMenuCreation<T>(
            PopupField<T> popupField,
            Action onCreateMenuCallback,
            Action<AbstractGenericMenu> beforeDropDownCallback)
        {
            Type baseType = typeof(BasePopupField<,>).MakeGenericType(typeof(T), typeof(T));
            FieldInfo createMenuCallbackFieldInfo = baseType.GetField(
                "createMenuCallback",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Func<AbstractGenericMenu> createMenuCallback = () =>
            {
                GenericMenuWrapper wrapper = new(popupField, beforeDropDownCallback);
                onCreateMenuCallback?.Invoke();
                return wrapper;
            };
            createMenuCallbackFieldInfo.SetValue(popupField, createMenuCallback);
        }

        internal static void ApplyDrawMode(AttributeDrawMode drawMode, VisualElement temp, VisualElement decorator)
        {
            if (drawMode is AttributeDrawMode.After)
            {
                AddToFooterDecoratorContainer(temp, decorator);
            }
            else if (drawMode is AttributeDrawMode.Before)
            {
                int index = temp.parent.IndexOf(temp);
                temp.parent.Insert(index, decorator);
            }
        }

        internal static VisualElement GetDecoratorContainer(PropertyField propertyField)
            => propertyField.Q<VisualElement>(className: _decoratorDrawerContainerUss);

        internal static void AddToFooterDecoratorContainer(VisualElement field, VisualElement element)
        {
            PropertyField propertyField = field.GetFirstAncestorOfType<PropertyField>();
            VisualElement footerDecoratorContainer = propertyField.Q<VisualElement>(
                _footerDecoratorDrawerContainerName,
                _decoratorDrawerContainerUss);
            if (footerDecoratorContainer == null)
            {
                footerDecoratorContainer = new VisualElement()
                {
                    name = _footerDecoratorDrawerContainerName
                };
                footerDecoratorContainer.AddToClassList(_decoratorDrawerContainerUss);
                propertyField.Add(footerDecoratorContainer);
            }
            footerDecoratorContainer.Insert(0, element);
        }

        private static Action CreateInvalidMethodNameAction() => () => Debug.LogError("Invalid method name.");
    }
}
