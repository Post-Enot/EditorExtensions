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
    }
}
