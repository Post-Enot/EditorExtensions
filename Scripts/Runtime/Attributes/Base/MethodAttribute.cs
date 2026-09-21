using System;

namespace PostEnot
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public abstract class MethodAttribute : Attribute { }
}
