using UnityEngine;

namespace PostEnot.Toolkits
{
    public sealed class SceneAttribute : PropertyAttribute
    {
        public SceneAttribute(bool validateInvalidIndex = true) => ValidateInvalidIndex = validateInvalidIndex;

        public bool ValidateInvalidIndex { get; }
    }
}
