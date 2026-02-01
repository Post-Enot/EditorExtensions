using System;

namespace PostEnot.Toolkits
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public abstract class BaseGetAttribute : Attribute { }
}
