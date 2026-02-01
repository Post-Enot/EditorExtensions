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
            VisualElement root = new();
            Type targetType = serializedObject.targetObject.GetType();
            if (targetType.HasCustomAttribute<HideInspectorAttribute>())
            {
                return new VisualElement();
            }
            if (targetType.HasCustomAttribute<DisableInspectorAttribute>())
            {
                root.SetEnabled(false);
            }
            string[] propertiesToExclude =
                targetType.HasCustomAttribute<HideClassFieldAttribute>()
                ? new string[1]
                {
                    SerializationUtility.mScriptField
                }
                : Array.Empty<string>();
            InspectorElement.FillDefaultInspector(root, serializedObject, this, propertiesToExclude);
            return root;
        }

        protected override bool ShouldHideOpenButton() => target.GetType().HasCustomAttribute<HideClassFieldAttribute>();
    }
}
