using PostEnot.Toolkits;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal abstract class BaseDecoratorDrawer<TAttribute> : DecoratorDrawer where TAttribute : DecoratorPropertyAttribute
    {
#pragma warning disable IDE1006 // Стили именования
        private new PropertyAttribute attribute => base.attribute;
#pragma warning restore IDE1006 // Стили именования

        private protected static EditorAttributesSettingsAsset Settings => EditorAttributesSettingsAsset.GetSettings();

        public sealed override VisualElement CreatePropertyGUI()
        {
            VisualElement temp = new()
            {
                name = "TEMP",
                userData = attribute
            };
            temp.RegisterCallbackOnce<AttachToPanelEvent>(HandleAttachToPanel);
            return temp;
        }

        private void HandleAttachToPanel(AttachToPanelEvent context)
        {
            VisualElement temp = context.target as VisualElement;
            PropertyField propertyField = temp.GetFirstAncestorOfType<PropertyField>();
            SerializedProperty property = SerializationUtility.GetSerializedProperty(propertyField);
            FieldInfo fieldInfo = SerializationUtility.GetFieldInfo(property);
            TAttribute attribute = temp.userData as TAttribute;
            OnAttach(property, propertyField, fieldInfo, attribute);
            temp.schedule.Execute(() => InvokeAfterAttach(temp, property, propertyField, fieldInfo, attribute));
        }

        private void InvokeAfterAttach(
            VisualElement temp,
            SerializedProperty property,
            PropertyField propertyField,
            FieldInfo fieldInfo,
            TAttribute attribute)
        {
            temp.RemoveFromHierarchy();
            AfterAttach(property, propertyField, fieldInfo, attribute);
        }

        private protected virtual void OnAttach(
            SerializedProperty property,
            PropertyField propertyField,
            FieldInfo fieldInfo,
            TAttribute attribute) {}

        private protected virtual void AfterAttach(
            SerializedProperty property,
            PropertyField propertyField,
            FieldInfo fieldInfo,
            TAttribute attribute) {}

        [Obsolete]
        public sealed override bool CanCacheInspectorGUI() => base.CanCacheInspectorGUI();

        public sealed override float GetHeight() => base.GetHeight();

        public sealed override void OnGUI(Rect position) => base.OnGUI(position);
    }
}
