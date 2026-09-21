#nullable enable

using PostEnot.Toolkits;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Pool;
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
            Type? targetType = GetTargetType(serializedObject);
            if (targetType == null)
            {
                return container;
            }
            if (targetType.HasCustomAttribute<HideInspectorAttribute>())
            {
                return container;
            }
            string[] propertiesToExclude = GetPropertiesToExclude(targetType);
            InspectorElement.FillDefaultInspector(container, serializedObject, this, propertiesToExclude);
            if (targetType.HasCustomAttribute<ReadOnlyInspectorAttribute>())
            {
                container.SetEnabled(false);
            }
            else
            {
                if (targetType.TryGetCustomAttribute(out ReadOnlyInspectorInAttribute? readOnlyInspectorInAttribute))
                {
                    bool isEnabled = readOnlyInspectorInAttribute.IsEnabledInEditor ^ EditorApplication.isPlayingOrWillChangePlaymode;
                    container.SetEnabled(isEnabled);
                }
            }
            List<ButtonAttributeMethodData> buttonsData = ListPool<ButtonAttributeMethodData>.Get();
            try
            {
                ButtonMethodsFinder.FindButtons(targetType, buttonsData);
                foreach (ButtonAttributeMethodData buttonData in buttonsData)
                {
                    Action clickEvent = () =>
                    {
                        foreach (UnityEngine.Object target in targets)
                        {
                            if (target == null)
                            {
                                continue;
                            }
                            Action? methodAction = SerializationUtility.MethodInfoToDelegate(buttonData.Method, target);
                            methodAction?.Invoke();
                        }
                    };
                    Button button = new(clickEvent)
                    {
                        text = string.IsNullOrWhiteSpace(buttonData.Attribute.Text)
                            ? buttonData.Method.Name
                            : buttonData.Attribute.Text
                    };
                    container.Add(button);
                }
            }
            finally
            {
                ListPool<ButtonAttributeMethodData>.Release(buttonsData);
            }
            return container;
        }

        protected override bool ShouldHideOpenButton()
        {
            Type? targetType = GetTargetType(serializedObject);
            if (targetType == null)
            {
                return false;
            }
            return targetType.HasCustomAttribute<HideOpenButtonAttribute>();
        }

        private static string[] GetPropertiesToExclude(Type targetType)
        {
            if (targetType.HasCustomAttribute<HideClassFieldAttribute>())
            {
                return new string[1]
                {
                    SerializationUtility.mScriptField
                };
            }
            return Array.Empty<string>();
        }

        private static Type? GetTargetType(SerializedObject? serializedObject)
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
