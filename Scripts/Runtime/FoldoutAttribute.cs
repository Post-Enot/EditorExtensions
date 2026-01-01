using UnityEngine;

namespace PostEnot.Toolkits
{
    public sealed class FoldoutAttribute : PropertyAttribute
    {
        public FoldoutAttribute(string text) : base(true) => Text = text;

        public string Text { get; }
    }
}
