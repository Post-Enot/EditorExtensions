using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal abstract class BasePropertyDrawer<TAttribute> : PropertyDrawer where TAttribute : Attribute
    {
#pragma warning disable IDE1006 // Стили именования
        private new PropertyAttribute attribute => base.attribute;
#pragma warning restore IDE1006 // Стили именования

        private protected EditorAttributesSettingsAsset Settings => EditorAttributesSettingsAsset.GetSettings();

        public sealed override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            TAttribute attribute = this.attribute as TAttribute;
            return CreateProperty(property, fieldInfo, attribute);
        }

        private protected virtual VisualElement CreateProperty(SerializedProperty property, FieldInfo fieldInfo, TAttribute attribute) => null;

        [Obsolete]
        public sealed override bool CanCacheInspectorGUI(SerializedProperty property) => base.CanCacheInspectorGUI(property);

        public sealed override float GetPropertyHeight(SerializedProperty property, GUIContent label) => base.GetPropertyHeight(property, label);

        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label) => base.OnGUI(position, property, label);
    }
}
