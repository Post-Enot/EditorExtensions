using PostEnot.Toolkits;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomEditor(typeof(UnityEngine.Object), true)]
    [CanEditMultipleObjects]
    public class AdvancedEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement container = new();
            Type targetType = GetTargetType();
            if (targetType == null)
            {
                return container;
            }
            if (targetType.HasCustomAttribute<HideInspectorAttribute>())
            {
                return container;
            }
            string[] propertiesToExclude = targetType.HasCustomAttribute<HideClassFieldAttribute>()
                ? new string[1]
                {
                    SerializationUtility.mScriptField
                }
                : Array.Empty<string>();
            InspectorElement.FillDefaultInspector(container, serializedObject, this, propertiesToExclude);
            if (targetType.HasCustomAttribute<ReadOnlyInspectorAttribute>())
            {
                container.SetEnabled(false);
            }
            else
            {
                if (targetType.TryGetCustomAttribute(out ReadOnlyInspectorInAttribute readOnlyInspectorInAttribute))
                {
                    bool isEnabled = readOnlyInspectorInAttribute.IsEnabledInEditor ^ EditorApplication.isPlayingOrWillChangePlaymode;
                    container.SetEnabled(isEnabled);
                }
            }
            return container;
        }

        protected override bool ShouldHideOpenButton()
        {
            Type targetType = GetTargetType();
            if (targetType == null)
            {
                return false;
            }
            return targetType.HasCustomAttribute<HideOpenButtonAttribute>();
        }

        private Type GetTargetType()
        {
            if (serializedObject == null)
            {
                return null;
            }
            if (serializedObject.targetObject == null)
            {
                return null;
            }
            return serializedObject.targetObject.GetType();
        }
    }
}
