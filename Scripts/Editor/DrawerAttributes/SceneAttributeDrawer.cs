using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    internal sealed class SceneAttributeDrawer : BasePropertyDrawer<SceneAttribute>
    {
        private protected override VisualElement CreateProperty(
            SerializedProperty property,
            FieldInfo fieldInfo,
            SceneAttribute attribute)
        {
            if (property.propertyType is not SerializedPropertyType.Integer)
            {
                return new Label($"Use Scene with int.");
            }
            SceneField sceneField = new(preferredLabel);
            sceneField.AddToClassList(BaseField<int>.alignedFieldUssClassName);
            sceneField.BindProperty(property);
            if (attribute.ValidateInvalidIndex)
            {
                ValidatorContainer<SceneField, int> validatorContainer = new(sceneField, UpdateValidationHelpBox);
                sceneField.ChoicedUpdated += validatorContainer.InvokeValidationRequest;
                validatorContainer.Add(sceneField);
                return validatorContainer;
            }
            return sceneField;
        }

        private static bool UpdateValidationHelpBox(SceneField sceneField, HelpBox helpBox)
        {
            if ((sceneField.value < 0) || (sceneField.value >= EditorBuildSettings.scenes.Length))
            {
                helpBox.messageType = HelpBoxMessageType.Error;
                helpBox.text = $"Invalid index: scene with {sceneField.value} index was removed.";
                return true;
            }
            return false;
        }
    }
}
