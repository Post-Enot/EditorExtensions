using System;
using System.Reflection;

namespace PostEnot.EditorExtensions.Editor
{
    internal static class FieldInfoExtension
    {
        internal static bool HasCustomAttribute<T>(this FieldInfo self) where T : Attribute
        {
            T attribute = self.GetCustomAttribute<T>();
            return attribute != null;
        }

        internal static bool TryGetCustomAttribute<T>(this FieldInfo self, out T attribute) where T : Attribute
        {
            attribute = self.GetCustomAttribute<T>();
            return attribute != null;
        }
    }
}
