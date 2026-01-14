using System;
using UnityEngine;

namespace PostEnot.Toolkits
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public abstract class DecoratorPropertyAttribute : PropertyAttribute
    {
        public DecoratorPropertyAttribute(AttributeDrawMode drawMode = AttributeDrawMode.Before) : base(true) => DrawMode = drawMode;

        public AttributeDrawMode DrawMode { get; }
    }
}
