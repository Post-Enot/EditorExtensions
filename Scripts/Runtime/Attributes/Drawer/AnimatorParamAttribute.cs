using UnityEngine;

namespace PostEnot.Toolkits
{
    public sealed class AnimatorParamAttribute : PropertyAttribute
    {
        public AnimatorParamAttribute(string animatorPropertyPath) => AnimatorPropertyPath = animatorPropertyPath;

        public string AnimatorPropertyPath { get; }
    }
}
