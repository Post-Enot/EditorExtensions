using UnityEngine;

namespace PostEnot.Toolkits
{
    public abstract class DecoratorPropertyAttribute : PropertyAttribute
    {
        public DecoratorPropertyAttribute(bool applyToCollection = true) : base(applyToCollection) {}
    }
}
