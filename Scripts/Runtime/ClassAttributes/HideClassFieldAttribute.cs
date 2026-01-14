using System;

namespace PostEnot.Toolkits
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false)]
    public sealed class HideClassFieldAttribute : Attribute {}
}
