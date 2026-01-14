using System;

namespace PostEnot.Toolkits
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false)]
    public abstract class ClassAttribute : Attribute {}
}
