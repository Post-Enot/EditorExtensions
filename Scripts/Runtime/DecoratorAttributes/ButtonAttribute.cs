using System;
using UnityEngine;

namespace PostEnot.Toolkits
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class ButtonAttribute : PropertyAttribute
    {
        public ButtonAttribute(string text, string methodName, ButtonPosition position = ButtonPosition.Up) : base(true)
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
