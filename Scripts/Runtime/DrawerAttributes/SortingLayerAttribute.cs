using UnityEngine;

namespace PostEnot.Toolkits
{
    public sealed class SortingLayerAttribute : PropertyAttribute
    {
        public SortingLayerAttribute(bool validateInvalidIndex = true) => ValidateInvalidIndex = validateInvalidIndex;

        public bool ValidateInvalidIndex { get; }
    }
}
