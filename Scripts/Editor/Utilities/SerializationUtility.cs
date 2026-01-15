using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace PostEnot.EditorExtensions.Editor
{
    internal static class SerializationUtility
    {
        internal static int GetArrayIndexFromPath(ReadOnlySpan<char> propertyPath)
        {
            if (propertyPath.IsEmpty)
            {
                return -1;
            }
            int lastIndex = propertyPath.Length - 1;
            if (propertyPath[lastIndex] != ']')
            {
                return -1;
            }
            int result = 0;
            int multiplier = 1;
            for (int i = lastIndex - 1; i >= 0; i -= 1)
            {
                char ch = propertyPath[i];
                if (ch == '[')
                {
                    return multiplier == 1 ? -1 : result;
                }
                if (!char.IsDigit(ch))
                {
                    return -1;
                }
                int digit = ch - '0';
                checked
                {
                    result += digit * multiplier;
                }
                multiplier *= 10;
            }
            return -1;
        }

        internal static Type GetElementTypeOfSerializedCollection(Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }
            Type genericTypeDefinition = type.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(List<>))
            {
                Type[] genericTypes = type.GetGenericArguments();
                return genericTypes[0];
            }
            return null;
        }

        internal static MethodInfo FindMethod(Type type, string methodName)
            => type.GetMethod(
                methodName,
                BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.Static
                | BindingFlags.Instance
                | BindingFlags.FlattenHierarchy,
                null,
                Type.EmptyTypes,
                null);

        internal static string GetParentPropertyPath(string propertyPath)
        {
            if (string.IsNullOrEmpty(propertyPath))
            {
                return string.Empty;
            }
            int lastDotIndex = propertyPath.LastIndexOf('.');
            if (lastDotIndex == -1)
            {
                return string.Empty;
            }
            string lastPart = propertyPath[(lastDotIndex + 1)..];
            if (lastPart.StartsWith("Array.data[", StringComparison.Ordinal))
            {
                int prevDotIndex = propertyPath.LastIndexOf('.', lastDotIndex - 1);
                if (prevDotIndex == -1)
                {
                    return propertyPath[..lastDotIndex];
                }

                string prevPart = propertyPath.Substring(prevDotIndex + 1, lastDotIndex - prevDotIndex - 1);
                if (prevPart == "Array")
                {
                    return propertyPath[..prevDotIndex];
                }
            }
            return propertyPath[..lastDotIndex];
        }

        internal static SerializedProperty GetSerializedProperty(PropertyField propertyField)
        {
            Type propertyFieldType = typeof(PropertyField);
            FieldInfo typeFieldInfo = propertyFieldType.GetField(
                "m_SerializedProperty",
                BindingFlags.NonPublic | BindingFlags.Instance);
            object serializedPropertyObject = typeFieldInfo.GetValue(propertyField);
            return serializedPropertyObject as SerializedProperty;
        }

        internal static SerializedProperty GetParentProperty(SerializedProperty property)
        {
            string path = property.propertyPath;
            int lastDot = path.LastIndexOf('.');
            if (lastDot == -1)
            {
                return null;
            }
            string parentPath = path[..lastDot];
            return property.serializedObject.FindProperty(parentPath);
        }

        internal static FieldInfo GetFieldInfo(SerializedProperty property)
        {
            const BindingFlags bindingFlags = BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.Instance
                | BindingFlags.FlattenHierarchy;
            Type targetType = property.serializedObject.targetObject.GetType();
            // Разбираем путь свойства (учитываем массивы и вложенные объекты)
            string[] pathParts = property.propertyPath.Split('.');
            FieldInfo fieldInfo = null;
            Type currentType = targetType;
            foreach (string part in pathParts)
            {
                if (part == "Array")
                {
                    // Пропускаем части пути, связанные с массивами
                    continue;
                }
                if (part.StartsWith("data["))
                {
                    // Если это элемент массива, получаем тип элемента
                    if (currentType.IsArray)
                    {
                        currentType = currentType.GetElementType();
                    }
                    else if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        currentType = currentType.GetGenericArguments()[0];
                    }
                    continue;
                }

                // Ищем поле в текущем типе
                fieldInfo = currentType.GetField(part, bindingFlags);
                if (fieldInfo == null)
                {
                    // Если не нашли, проверяем базовые классы
                    Type baseType = currentType.BaseType;
                    while (baseType != null && fieldInfo == null)
                    {
                        fieldInfo = baseType.GetField(part, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        baseType = baseType.BaseType;
                    }

                    if (fieldInfo == null)
                    {
                        Debug.LogError($"Field '{part}' not found in type '{currentType}'");
                        return null;
                    }
                }

                // Обновляем текущий тип для следующей итерации
                currentType = fieldInfo.FieldType;

                // Обработка Nullable типов
                if (currentType.IsGenericType &&
                    currentType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    currentType = currentType.GetGenericArguments()[0];
                }
            }

            return fieldInfo;
        }

        internal static void GetFieldAttributes<TAttribute>(FieldInfo field, List<TAttribute> list) where TAttribute : PropertyAttribute
        {
            list.Clear();
            IEnumerable<TAttribute> customAttributes = field.GetCustomAttributes<TAttribute>(inherit: true);
            Comparer<TAttribute> comparer = null;
            foreach (TAttribute item in customAttributes)
            {
                comparer ??= Comparer<TAttribute>.Create((p1, p2) => p1.order.CompareTo(p2.order));
                list.Add(item);
            }
            list?.Sort(comparer);
        }
    }
}
