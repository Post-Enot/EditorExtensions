using UnityEngine;

namespace PostEnot.Toolkits
{
    public sealed class WithoutFoldoutAttribute : PropertyAttribute
    {
        public WithoutFoldoutAttribute() : base(true) { }
    }
}
