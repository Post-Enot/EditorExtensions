using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(SortingLayerAttribute))]
    internal sealed class SortingLayerAttributeDrawer : BasePropertyDrawer<SortingLayerAttribute>
    {
        private protected override VisualElement CreateProperty(
            SerializedProperty property,
            FieldInfo fieldInfo,
            SortingLayerAttribute attribute)
        {
            if (property.propertyType is not SerializedPropertyType.Integer)
            {
                return new Label($"Use SortingLayer with int.");
            }
            SortingLayerField sortingLayerField = new(preferredLabel);
            sortingLayerField.AddToClassList(BaseField<int>.alignedFieldUssClassName);
            sortingLayerField.BindProperty(property);
            if (attribute.ValidateInvalidIndex)
            {
                ValidatorContainer<SortingLayerField, int> validatorContainer = new(sortingLayerField, UpdateValidationHelpBox);
                sortingLayerField.ChoicedUpdated += validatorContainer.InvokeValidationRequest;
                validatorContainer.Add(sortingLayerField);
                return validatorContainer;
            }
            return sortingLayerField;
        }

        private static bool UpdateValidationHelpBox(SortingLayerField sortingLayerField, HelpBox helpBox)
        {
            if ((sortingLayerField.value < 0) || (sortingLayerField.value >= SortingLayer.layers.Length))
            {
                helpBox.messageType = HelpBoxMessageType.Error;
                helpBox.text = $"Invalid index: sorting layer with {sortingLayerField.value} index was removed.";
                return true;
            }
            return false;
        }
    }
}
