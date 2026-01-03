using System;
using UnityEngine;

namespace PostEnot.Toolkits
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class ButtonAttribute : PropertyAttribute
    {
        public ButtonAttribute(string text, string methodName, AttributeDrawMode drawMode = AttributeDrawMode.Before) : base(true)
        {
            Text = text;
            MethodName = methodName;
            DrawMode = drawMode;
        }

        public string Text { get; }
        public string MethodName { get; }
        public AttributeDrawMode DrawMode { get; }
    }
}
