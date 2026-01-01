using UnityEngine;

namespace PostEnot.Toolkits
{
    public sealed class ButtonAttribute : PropertyAttribute
    {
        public ButtonAttribute(string text, string methodName, ButtonPosition position = ButtonPosition.Down) : base(true)
        {
            Text = text;
            MethodName = methodName;
            Position = position;
        }

        public string Text { get; }
        public string MethodName { get; }
        public ButtonPosition Position { get; }
    }
}
