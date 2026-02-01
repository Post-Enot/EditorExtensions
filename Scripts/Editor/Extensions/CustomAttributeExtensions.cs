using System;
using System.Reflection;

namespace PostEnot.EditorExtensions.Editor
{
    internal static class CustomAttributeExtensions
    {
        internal static bool HasCustomAttribute<T>(this MemberInfo self) where T : Attribute
        {
            T attribute = self.GetCustomAttribute<T>();
            return attribute != null;
        }

        internal static bool TryGetCustomAttribute<T>(this MemberInfo self, out T attribute) where T : Attribute
        {
            attribute = self.GetCustomAttribute<T>();
            return attribute != null;
        }
    }
}
