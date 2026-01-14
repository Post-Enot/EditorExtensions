using PostEnot.Toolkits;
using System;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal static class UIUtility
    {
        private const string _footerDecoratorDrawerContainerName = "footer-decorator-drawers-container";
        private const string _decoratorDrawerContainerUss = "unity-decorator-drawers-container";

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
    }
}
