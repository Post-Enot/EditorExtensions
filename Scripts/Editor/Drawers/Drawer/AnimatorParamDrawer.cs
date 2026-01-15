using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(AnimatorParamAttribute))]
    internal sealed class AnimatorParamDrawer : BasePropertyDrawer<AnimatorParamAttribute>
    {
        private protected override VisualElement CreateProperty(
            SerializedProperty property,
            FieldInfo fieldInfo,
            AnimatorParamAttribute attribute)
        {
            if (property.propertyType is not SerializedPropertyType.Integer)
            {
                return new Label($"Use AnimatorParam with int.");
            }
            string parentPath = SerializationUtility.GetParentPropertyPath(property.propertyPath);
            string animatorPropertyPath = string.IsNullOrEmpty(parentPath)
                ? attribute.AnimatorPropertyPath
                : $"{parentPath}.{attribute.AnimatorPropertyPath}";
            SerializedProperty animatorProperty = property.serializedObject.FindProperty(animatorPropertyPath);
            if ((animatorProperty == null)
                || (animatorProperty.propertyType is not SerializedPropertyType.ObjectReference)
                || (animatorProperty.objectReferenceValue is not Animator animator))
            {
                return AnimatorNotFoundLabel();
            }
            AnimatorParamField animatorParamField = new(preferredLabel, animator);
            animatorParamField.AddToClassList(BaseField<int>.alignedFieldUssClassName);
            animatorParamField.BindProperty(property);
            return animatorParamField;
        }

        private static Label AnimatorNotFoundLabel() => new("Animator property not found.");
    }
}
