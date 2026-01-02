using PostEnot.Toolkits;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    internal sealed class SceneAttributePropertyDrawer : PropertyDrawer
    {
        private sealed class DropdownData
        {
            public DropdownData(DropdownField dropdownField, SerializedProperty serializedProperty)
            {
                DropdownField = dropdownField;
                SerializedProperty = serializedProperty;
                dropdownField.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
                dropdownField.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
                dropdownField.RegisterValueChangedCallback(OnValueChanged);
                _helpBoxInvalidIndex = new HelpBox("", HelpBoxMessageType.Error);
            }

            public DropdownField DropdownField { get; }
            public SerializedProperty SerializedProperty { get; }

            private readonly HelpBox _helpBoxInvalidIndex;

            private void OnValueChanged(ChangeEvent<string> context)
            {
                if (DropdownField.index != -1)
                {
                    SerializedProperty.intValue = DropdownField.index;
                    SerializedProperty.serializedObject.ApplyModifiedProperties();
                    if (_helpBoxInvalidIndex.parent != null)
                    {
                        _helpBoxInvalidIndex.RemoveFromHierarchy();
                    }
                }
            }

            private void OnDetachFromPanel(DetachFromPanelEvent context) => EditorBuildSettings.sceneListChanged -= UpdateView;

            private void OnAttachToPanel(AttachToPanelEvent context)
            {
                EditorBuildSettings.sceneListChanged += UpdateView;
                UpdateView();
            }

            private void UpdateView()
            {
                List<string> choices = new();
                for (int i = 0; i < EditorBuildSettings.scenes.Length; i += 1)
                {
                    EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
                    string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                    choices.Add($"{sceneName} ({i})");
                }
                DropdownField.choices = choices;
                DropdownField.index = SerializedProperty.intValue;
                if (DropdownField.index == -1)
                {
                    DropdownField.SetValueWithoutNotify($"Missing Scene: ({SerializedProperty.intValue})");
                    _helpBoxInvalidIndex.text = $"Invalid index: scene with {SerializedProperty.intValue} index was removed.";
                    if (_helpBoxInvalidIndex.parent == null)
                    {
                        DropdownField.parent.parent.Add(_helpBoxInvalidIndex);
                    }
                }
                else if (_helpBoxInvalidIndex.parent != null)
                {
                    _helpBoxInvalidIndex.RemoveFromHierarchy();
                }
            }
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                return new Label($"Implement {nameof(SceneAttribute)} to int field.");
            }
            DropdownField dropdownField = new(preferredLabel);
            dropdownField.userData = new DropdownData(dropdownField, property);
            Button button = new(OnButtonClick)
            {
                name = "button-open-build-settings",
                text = "...",
                tooltip = "Open Builds Profiles."
            };
            button.style.width = new Length(24, LengthUnit.Pixel);
            VisualElement container = new()
            {
                name = "container"
            };
            VisualElement sceneField = new()
            {
                name = "scene-field"
            };
            container.Add(dropdownField);
            container.Add(button);
            sceneField.Add(container);
            dropdownField.style.flexGrow = 1;
            dropdownField.AddToClassList(BaseField<string>.alignedFieldUssClassName);
            container.style.flexDirection = FlexDirection.Row;
            return sceneField;
        }

        private static void OnButtonClick()
        {
            BuildPlayerWindow window = EditorWindow.GetWindow<BuildPlayerWindow>();
            window.Show();
        }
    }
}
