using PostEnot.Toolkits;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(PreviewAttribute))]
    public sealed class PreviewAttributePropertyDrawer : PropertyDrawer
    {
        [SerializeField] private StyleSheet styleSheet;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            PreviewAttribute previewAttribute = attribute as PreviewAttribute;
            PropertyField propertyField = new(property, preferredLabel)
            {
                userData = previewAttribute
            };
            propertyField.BindProperty(property);
            propertyField.RegisterCallbackOnce<AttachToPanelEvent>(OnAttachToPanel);
            return propertyField;
        }

        private void OnAttachToPanel(AttachToPanelEvent context)
        {
            PropertyField propertyField = context.target as PropertyField;
            PreviewAttribute previewAttribute = propertyField.userData as PreviewAttribute;
            ObjectField objectField = propertyField.Q<ObjectField>();
            VisualElement input = objectField.Q<VisualElement>(className: "unity-object-field__input");
            Length length = new(previewAttribute.SizeInPixels, LengthUnit.Pixel);
            input.style.minWidth = length;
            input.style.width = length;
            input.style.maxWidth = length;
            objectField.RegisterValueChangedCallback(OnValueChanged);
            objectField.AddToClassList("pe-preview-object-field");
            objectField.styleSheets.Add(styleSheet);
            objectField.AddToClassList(BaseField<Object>.alignedFieldUssClassName);
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
