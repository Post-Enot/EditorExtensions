using UnityEngine;

namespace PostEnot.Toolkits
{
    public sealed class HideColumnAttribute : PropertyAttribute
    {
        public HideColumnAttribute(string bindingPath) => BindingPath = bindingPath;

        public string BindingPath { get; }
    }
}
