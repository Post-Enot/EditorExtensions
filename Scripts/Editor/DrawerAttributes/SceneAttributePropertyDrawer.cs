using PostEnot.Toolkits;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    internal sealed class SceneAttributePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.propertyType is not SerializedPropertyType.Integer)
            {
                return new Label($"Use Scene with int.");
            }
            SceneField sceneField = new(preferredLabel);
            sceneField.AddToClassList(BaseField<int>.alignedFieldUssClassName);
            sceneField.BindProperty(property);
            ValidatorContainer<SceneField, int> validatorContainer = new(sceneField, UpdateValidationHelpBox);
            sceneField.ChoicedUpdated += validatorContainer.InvokeValidationRequest;
            validatorContainer.Add(sceneField);
            return validatorContainer;
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
