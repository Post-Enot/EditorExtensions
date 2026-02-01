using PostEnot.Toolkits;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PostEnot.EditorExtensions.Editor
{
    internal static class RefUtilityInitializer
    {
        [InitializeOnLoadMethod]
        internal static void SetInitReferencesImplementation()
            => RefUtility.InitReferencesImplementation ??= InitReferencesEditorImplementation;

        internal static void InitReferencesEditorImplementation(MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour == null)
            {
                throw new ArgumentNullException(nameof(monoBehaviour));
            }
            SerializedObject serializedObject = new(monoBehaviour);
            SerializedProperty property = serializedObject.GetIterator();
            List<Component> buffer = new();
            while (property.NextVisible(true))
            {
                if (property.propertyPath == SerializationUtility.mScriptField)
                {
                    continue;
                }
                if (property.propertyType is not SerializedPropertyType.ObjectReference
                    && !SerializationUtility.IsArrayNotString(property))
                {
                    continue;
                }
                FieldInfo fieldInfo = SerializationUtility.GetFieldInfo(property);
                Type fieldType = fieldInfo.FieldType;
                if (property.isArray)
                {
                    Type elementType = SerializationUtility.GetElementType(fieldType);
                    if ((elementType == null) || !typeof(Component).IsAssignableFrom(elementType))
                    {
                        continue;
                    }
                    fieldType = elementType;
                }
                else if (!typeof(Component).IsAssignableFrom(fieldType))
                {
                    continue;
                }
                if (TryAssignSelf(monoBehaviour.gameObject, property, fieldInfo, fieldType, buffer)
                    || TryAssignChildren(monoBehaviour.gameObject, property, fieldInfo, fieldType)
                    || TryAssignParent(monoBehaviour.gameObject, property, fieldInfo, fieldType)
                    || TryAssignHierarchy(monoBehaviour.gameObject, property, fieldInfo, fieldType)
                    || TryAssignChildrenOnly(monoBehaviour.gameObject, property, fieldInfo, fieldType)
                    || TryAssignParentOnly(monoBehaviour.gameObject, property, fieldInfo, fieldType)
                    || TryAssignHierarchyOnly(monoBehaviour.gameObject, property, fieldInfo, fieldType))
                {
                    continue;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        private static bool TryAssignHierarchyOnly(
            GameObject gameObject,
            SerializedProperty property,
            FieldInfo fieldInfo,
            Type fieldType)
        {
            if (!fieldInfo.TryGetCustomAttribute(out GetInHierarchyOnlyAttribute attribute))
            {
                return false;
            }
            if (property.isArray)
            {
                Component[] components = gameObject.GetComponentsInChildren(fieldType, attribute.IncludeInactive);
                InsertAllInsteadSelf(gameObject, property, components);
                components = gameObject.GetComponentsInParent(fieldType, attribute.IncludeInactive);
                InsertAllInsteadSelf(gameObject, property, components);
            }
            else
            {
                Component[] components = gameObject.GetComponentsInChildren(fieldType, attribute.IncludeInactive);
                if (AssignInsteadSelf(gameObject, property, components))
                {
                    return true;
                }
                components = gameObject.GetComponentsInParent(fieldType, attribute.IncludeInactive);
                _ = AssignInsteadSelf(gameObject, property, components);
            }
            return true;
        }

        private static bool TryAssignParentOnly(
            GameObject gameObject,
            SerializedProperty property,
            FieldInfo fieldInfo,
            Type fieldType)
        {
            if (!fieldInfo.TryGetCustomAttribute(out GetInParentOnlyAttribute attribute))
            {
                return false;
            }
            if (property.isArray)
            {
                Component[] components = gameObject.GetComponentsInParent(fieldType, attribute.IncludeInactive);
                InsertAllInsteadSelf(gameObject, property, components);
            }
            else
            {
                Component[] components = gameObject.GetComponentsInParent(fieldType, attribute.IncludeInactive);
                _ = AssignInsteadSelf(gameObject, property, components);
            }
            return true;
        }

        private static bool TryAssignChildrenOnly(
            GameObject gameObject,
            SerializedProperty property,
            FieldInfo fieldInfo,
            Type fieldType)
        {
            if (!fieldInfo.TryGetCustomAttribute(out GetInChildrenOnlyAttribute attribute))
            {
                return false;
            }
            if (property.isArray)
            {
                Component[] components = gameObject.GetComponentsInChildren(fieldType, attribute.IncludeInactive);
                InsertAllInsteadSelf(gameObject, property, components);
            }
            else
            {
                Component[] components = gameObject.GetComponentsInChildren(fieldType, attribute.IncludeInactive);
                _ = AssignInsteadSelf(gameObject, property, components);
            }
            return true;
        }

        private static bool TryAssignHierarchy(
            GameObject gameObject,
            SerializedProperty property,
            FieldInfo fieldInfo,
            Type fieldType)
        {
            if (!fieldInfo.TryGetCustomAttribute(out GetInHierarchyAttribute attribute))
            {
                return false;
            }
            if (property.isArray)
            {
                Component[] components = gameObject.GetComponentsInChildren(fieldType, attribute.IncludeInactive);
                SerializationUtility.AddArrayElements(property, components);
                components = gameObject.GetComponentsInParent(fieldType, attribute.IncludeInactive);
                InsertAllInsteadSelf(gameObject, property, components);
            }
            else
            {
                Component component = gameObject.GetComponentInChildren(fieldType, attribute.IncludeInactive);
                if (component == null)
                {
                    component = gameObject.GetComponentInParent(fieldType, attribute.IncludeInactive);
                }
                property.objectReferenceValue = component;
            }
            return true;
        }

        private static bool TryAssignParent(GameObject gameObject, SerializedProperty property, FieldInfo fieldInfo, Type fieldType)
        {
            if (!fieldInfo.TryGetCustomAttribute(out GetInParentAttribute attribute))
            {
                return false;
            }
            if (property.isArray)
            {
                Component[] components = gameObject.GetComponentsInParent(fieldType, attribute.IncludeInactive);
                SerializationUtility.AddArrayElements(property, components);
            }
            else
            {
                property.objectReferenceValue = gameObject.GetComponentInParent(fieldType, attribute.IncludeInactive);
            }
            return true;
        }

        private static bool TryAssignChildren(GameObject gameObject, SerializedProperty property, FieldInfo fieldInfo, Type fieldType)
        {
            if (!fieldInfo.TryGetCustomAttribute(out GetInChildrenAttribute attribute))
            {
                return false;
            }
            if (property.isArray)
            {
                Component[] components = gameObject.GetComponentsInChildren(fieldType, attribute.IncludeInactive);
                SerializationUtility.AddArrayElements(property, components);
            }
            else
            {
                property.objectReferenceValue = gameObject.GetComponentInChildren(fieldType, attribute.IncludeInactive);
            }
            return true;
        }

        private static bool TryAssignSelf(
            GameObject gameObject,
            SerializedProperty property,
            FieldInfo fieldInfo,
            Type fieldType,
            List<Component> buffer)
        {
            if (!fieldInfo.HasCustomAttribute<GetSelfAttribute>())
            {
                return false;
            }
            if (property.isArray)
            {
                gameObject.GetComponents(fieldType, buffer);
                SerializationUtility.AddArrayElements(property, buffer);
            }
            else
            {
                property.objectReferenceValue = gameObject.GetComponent(fieldType);
            }
            return true;
        }

        private static bool AssignInsteadSelf(GameObject gameObject, SerializedProperty property, IEnumerable<Component> components)
        {
            foreach (Component component in components)
            {
                if (component.gameObject != gameObject)
                {
                    property.objectReferenceValue = component;
                    return true;
                }
            }
            return false;
        }

        private static void InsertAllInsteadSelf(GameObject gameObject, SerializedProperty property, IEnumerable<Component> components)
        {
            foreach (Component component in components)
            {
                if (component.gameObject != gameObject)
                {
                    SerializationUtility.AddArrayElement(property, component);
                }
            }
        }
    }
}
