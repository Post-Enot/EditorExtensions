using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace PostEnot.EditorExtensions.Editor
{
    internal sealed class AnimatorParamField : AdvancedPopupField<int>
    {
        public AnimatorParamField(string label, SerializedProperty animatorProperty) : base(label)
        {
            _animatorProperty = animatorProperty;
            formatListItemCallback = FormatListItem;
            formatSelectedValueCallback = FormatSelectedValue;
            this.TrackPropertyValue(_animatorProperty, OnAnimatorPropertyValueChanged);
            UpdateEnabling();
        }

        private readonly SerializedProperty _animatorProperty;

        private protected override void OnCreateMenu() => UpdateChoices();

        private bool TryGetAnimator(out Animator result)
        {
            if (_animatorProperty.objectReferenceValue is Animator animator)
            {
                result = animator;
                return true;
            }
            result = null;
            return false;
        }

        private void UpdateChoices()
        {
            if (TryGetAnimator(out Animator animator))
            {
                List<int> choices = new();
                for (int i = 0; i < animator.parameterCount; i += 1)
                {
                    choices.Add(animator.parameters[i].nameHash);
                }
                this.choices = choices;
            }
            else
            {
                choices = new List<int>();
            }
        }

        private string FormatSelectedValue(int value)
        {
            if (TryGetAnimator(out Animator animator))
            {
                foreach (AnimatorControllerParameter param in animator.parameters)
                {
                    if (param.nameHash == value)
                    {
                        return param.name;
                    }
                }
            }
            return string.Empty;
        }

        private string FormatListItem(int value)
        {
            if (TryGetAnimator(out Animator animator))
            {
                foreach (AnimatorControllerParameter param in animator.parameters)
                {
                    if (param.nameHash == value)
                    {
                        return param.name;
                    }
                }
            }
            return string.Empty;
        }

        private void UpdateEnabling() => enabledSelf = TryGetAnimator(out _);

        private void OnAnimatorPropertyValueChanged(SerializedProperty property)
        {
            MarkDirtyRepaint();
            UpdateEnabling();
        }
    }
}
