using UnityEngine;

namespace PostEnot.Toolkits
{
    public sealed class ButtonAttribute : PropertyAttribute
    {
        public ButtonAttribute(string text, string methodName)
        {
            Text = text;
            MethodName = methodName;
        }

        public string Text { get; }
        public string MethodName { get; }
    }
}
