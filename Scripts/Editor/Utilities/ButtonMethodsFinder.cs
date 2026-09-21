#nullable enable

using PostEnot.EditorExtensions.Editor;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Pool;

namespace PostEnot.Toolkits
{
    internal static class ButtonMethodsFinder
    {
        /// <summary>
        /// Находит все методы <paramref name="targetType"/> (включая приватные и унаследованные),
        /// помеченные <see cref="ButtonAttribute"/>, с учётом переопределений.
        /// </summary>
        internal static void FindButtons(Type? targetType, List<ButtonAttributeMethodData> dataCollection)
        {
            if (targetType == null)
            {
                return;
            }
            if (dataCollection == null)
            {
                throw new ArgumentNullException(nameof(dataCollection));
            }
            List<Type> hierarchy = CollectionPool<List<Type>, Type>.Get();
            GetHierarchy(hierarchy, targetType);
            foreach (Type type in hierarchy)
            {
                const BindingFlags DeclaredMembers
                    = BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.Instance
                    | BindingFlags.Static
                    | BindingFlags.DeclaredOnly;
                MethodInfo[] methods = type.GetMethods(DeclaredMembers);
                foreach (MethodInfo method in methods)
                {
                    // Пропускаем аксессоры свойств/событий: get_/set_/add_/remove_.
                    if (method.IsSpecialName)
                    {
                        continue;
                    }
                    if (method.IsAbstract)
                    {
                        continue;
                    }
                    if (method.TryGetCustomAttribute(out ButtonAttribute? attribute, inherit: false))
                    {
                        ButtonAttributeMethodData dataElement = new(method, attribute);
                        dataCollection.Add(dataElement);
                    }
                }
            }
        }

        private static void GetHierarchy(List<Type> hierarchy, Type targetType)
        {
            Type? type = targetType;
            while ((type != null) && (type != typeof(object)))
            {
                hierarchy.Add(type);
                type = type.BaseType;
            }
            hierarchy.Reverse();
        }
    }

    /// <summary>
    /// Метод, помеченный <see cref="ButtonAttribute"/>, вместе с экземпляром атрибута.
    /// </summary>
    public readonly struct ButtonAttributeMethodData
    {
        public ButtonAttributeMethodData(MethodInfo method, ButtonAttribute attribute)
        {
            Method = method;
            Attribute = attribute;
        }

        public readonly MethodInfo Method { get; }
        public readonly ButtonAttribute Attribute { get; }
    }
}
