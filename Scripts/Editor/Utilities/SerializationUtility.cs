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
        internal const string mScriptField = "m_Script";

        internal static bool IsArrayNotString(SerializedProperty property)
            => property.isArray && property.propertyType is not SerializedPropertyType.String;

        internal static void AddArrayElements(SerializedProperty property, IEnumerable<Component> components)
        {
            int index = property.arraySize;
            foreach (Component component in components)
            {
                property.InsertArrayElementAtIndex(index);
                SerializedProperty elementProperty = property.GetArrayElementAtIndex(index);
                elementProperty.objectReferenceValue = component;
                index += 1;
            }
        }

        internal static void AddArrayElement(SerializedProperty property, Component component)
        {
            int index = property.arraySize;
            property.InsertArrayElementAtIndex(index);
            SerializedProperty elementProperty = property.GetArrayElementAtIndex(index);
            elementProperty.objectReferenceValue = component;
        }

        internal static int GetArrayIndexFromPath(ReadOnlySpan<char> propertyPath)
        {
            if (propertyPath.IsEmpty)
            {
                return -1;
            }
            int lastIndex = propertyPath.Length - 1;
            char lastChar = propertyPath[lastIndex];
            if (lastChar != ']')
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

        internal static Action MethodInfoToDelegate(MethodInfo methodInfo, object instance = null)
            => instance == null
                ? methodInfo.CreateDelegate(typeof(Action)) as Action
                : methodInfo.CreateDelegate(typeof(Action), instance) as Action;

        internal static MethodInfo FindMethod(Type type, string methodName) => type.GetMethod(
                methodName,
                BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.Static
                | BindingFlags.Instance
                | BindingFlags.FlattenHierarchy,
                null,
                Type.EmptyTypes,
                null);

        internal static ReadOnlySpan<char> GetParentPropertyPath(ReadOnlySpan<char> propertyPath)
        {
            int lastDotIndex = propertyPath.LastIndexOf('.');
            if (lastDotIndex == -1)
            {
                return string.Empty;
            }
            ReadOnlySpan<char> lastPart = propertyPath[(lastDotIndex + 1)..];
            if (lastPart.StartsWith("Array.data[", StringComparison.Ordinal))
            {
                int prevDotIndex = propertyPath.LastIndexOf('.', lastDotIndex - 1);
                if (prevDotIndex == -1)
                {
                    return propertyPath[..lastDotIndex];
                }
                ReadOnlySpan<char> prevPart = propertyPath.Slice(prevDotIndex + 1, lastDotIndex - prevDotIndex - 1);
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
            FieldInfo typeFieldInfo = propertyFieldType.GetField("m_SerializedProperty", BindingFlags.NonPublic | BindingFlags.Instance);
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

        internal static Type GetElementType(Type type)
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

        internal static FieldInfo GetFieldInfo(SerializedProperty property)
        {
            Type targetType = property.serializedObject.targetObject.GetType();
            string[] pathParts = property.propertyPath.Split('.');
            FieldInfo fieldInfo = null;
            Type currentType = targetType;
            foreach (string part in pathParts)
            {
                if (part == "Array")
                {
                    continue;
                }
                if (part.StartsWith("data["))
                {
                    currentType = GetElementType(currentType);
                    continue;
                }
                fieldInfo = currentType.GetField(
                    part,
                    BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.Instance
                    | BindingFlags.FlattenHierarchy);
                if (fieldInfo == null)
                {
                    Type baseType = currentType.BaseType;
                    while ((baseType != null) && (fieldInfo == null))
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
                currentType = fieldInfo.FieldType;
            }
            return fieldInfo;
        }
    }
}
