using System.Collections.Generic;
using UnityEngine;

namespace PostEnot.EditorExtensions.Editor
{
    internal sealed class AnimatorParamField : AdvancedPopupField<int>
    {
        public AnimatorParamField(string label, Animator animator) : base(label)
        {
            _animator = animator;
            formatListItemCallback = FormatListItem;
            formatSelectedValueCallback = FormatSelectedValue;
        }

        private readonly Animator _animator;

        private protected override void OnCreateMenu() => UpdateChoices();

        private void UpdateChoices()
        {
            List<int> choices = new();
            for (int i = 0; i < _animator.parameterCount; i += 1)
            {
                choices.Add(_animator.parameters[i].nameHash);
            }
            this.choices = choices;
        }

        private string FormatSelectedValue(int value)
        {
            foreach (AnimatorControllerParameter param in _animator.parameters)
            {
                if (param.nameHash == value)
                {
                    return param.name;
                }
            }
            return string.Empty;
        }

        private string FormatListItem(int value)
        {
            foreach (AnimatorControllerParameter param in _animator.parameters)
            {
                if (param.nameHash == value)
                {
                    return param.name;
                }
            }
            return string.Empty;
        }
    }
}
