using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(PreviewAttribute))]
    internal sealed class PreviewAttributeDrawer : BasePropertyDrawer<PreviewAttribute>
    {
        private protected override VisualElement CreateProperty(SerializedProperty property, FieldInfo fieldInfo, PreviewAttribute attribute)
        {
            if (property.propertyType is not SerializedPropertyType.ExposedReference and not SerializedPropertyType.ObjectReference)
            {
                return new Label($"Use Preview with UnityEngine.Object.");
            }
            ObjectField objectField = new()
            {
                objectType = fieldInfo.FieldType,
                label = preferredLabel
            };
            VisualElement input = objectField.Q<VisualElement>(className: "unity-object-field__input");
            Length length = new(attribute.SizeInPixels, LengthUnit.Pixel);
            input.style.minWidth = length;
            input.style.width = length;
            input.style.maxWidth = length;
            objectField.RegisterValueChangedCallback(OnValueChanged);
            objectField.AddToClassList("pe-preview-object-field");
            if (Settings.PreviewStyleSheet != null)
            {
                objectField.styleSheets.Add(Settings.PreviewStyleSheet);
            }
            objectField.AddToClassList(BaseField<Object>.alignedFieldUssClassName);
            objectField.BindProperty(property);
            return objectField;
        }

        private void OnValueChanged(ChangeEvent<Object> context)
        {
            ObjectField objectField = context.target as ObjectField;
            UpdatePreview(objectField);
        }

        private void UpdatePreview(ObjectField objectField)
        {
            VisualElement objectFieldDisplay = objectField.Q<VisualElement>(className: "unity-object-field-display");
            if (objectField.value == null)
            {
                objectFieldDisplay.style.backgroundImage = null;
                return;
            }
            Texture2D preview = AssetPreview.GetAssetPreview(objectField.value);
            if (preview == null)
            {
                EntityId entityId = objectField.value.GetEntityId();
                if (AssetPreview.IsLoadingAssetPreview(entityId))
                {
                    objectField.schedule.Execute(() => UpdatePreview(objectField)).StartingIn(200);
                }
                else
                {
                    objectFieldDisplay.style.backgroundImage = AssetPreview.GetMiniThumbnail(objectField.value);
                }
            }
            else
            {
                objectFieldDisplay.style.backgroundImage = preview;
            }
        }
    }
}
