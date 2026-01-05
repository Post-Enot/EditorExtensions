using System;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal sealed class ValidatorContainer<TField, TValue> : VisualElement where TField : BaseField<TValue>
    {
        public ValidatorContainer(
            TField baseField,
            Func<TField, HelpBox, bool> shouldShowCallback)
        {
            _shouldShowCallback = shouldShowCallback ?? throw new ArgumentNullException(nameof(shouldShowCallback));
            _baseField = baseField ?? throw new ArgumentNullException(nameof(baseField));
            Add(baseField);
            baseField.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            baseField.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private readonly TField _baseField;
        private readonly Func<TField, HelpBox, bool> _shouldShowCallback;

        private HelpBox _helpBox;

        public void InvokeValidationRequest()
        {
            _helpBox ??= new();
            bool shouldShow = _shouldShowCallback(_baseField, _helpBox);
            if (shouldShow && (_helpBox.parent == null))
            {
                Add(_helpBox);
            }
            else if (!shouldShow && (_helpBox.parent != null))
            {
                _helpBox.RemoveFromHierarchy();
            }
        }

        private void OnValueChanged(ChangeEvent<TValue> context) => InvokeValidationRequest();

        private void OnDetachFromPanel(DetachFromPanelEvent context) => _baseField.UnregisterValueChangedCallback(OnValueChanged);

        private void OnAttachToPanel(AttachToPanelEvent context)
        {
            _baseField.RegisterValueChangedCallback(OnValueChanged);
            InvokeValidationRequest();
        }

    }
}
