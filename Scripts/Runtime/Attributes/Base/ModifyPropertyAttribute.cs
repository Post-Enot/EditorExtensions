using UnityEngine;

namespace PostEnot.Toolkits
{
    public abstract class ModifyPropertyAttribute : PropertyAttribute
    {
        public ModifyPropertyAttribute(bool applyToCollection = true) : base(applyToCollection) {}
    }
}
