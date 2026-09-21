using PostEnot.Toolkits;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal static class UIUtility
    {
        private const string _footerDecoratorDrawerContainerName = "footer-decorator-drawers-container";
        private const string _decoratorDrawerContainerUss = "unity-decorator-drawers-container";

        internal static Texture2D LoadIcon(string iconPath)
        {
            if (string.IsNullOrWhiteSpace(iconPath))
            {
                return null;
            }
            if (iconPath.StartsWith("builtin:"))
            {
                string iconName = iconPath[(iconPath.IndexOf(':') + 1)..];
                return EditorGUIUtility.IconContent(iconName).image as Texture2D;
            }
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            return texture;
        }

        internal static string NicifyButtonName(string buttonText, MethodInfo methodInfo)
        {
            if (buttonText != null)
            {
                return buttonText;
            }
            if (methodInfo == null)
            {
                return string.Empty;
            }
            return ObjectNames.NicifyVariableName(methodInfo.Name);
        }

        internal static Action CreateActionFromMethodName(
            SerializedProperty serializedProperty,
            SerializedProperty parentSerializedProperty,
            FieldInfo fieldInfo,
            string methodName,
            out MethodInfo methodInfo)
        {
            methodInfo = null;

            if (string.IsNullOrWhiteSpace(methodName))
            {
                return CreateInvalidMethodNameAction();
            }

            SerializedObject root = serializedProperty.serializedObject;
            UnityEngine.Object[] targets = root.targetObjects;
            string parentPath = parentSerializedProperty?.propertyPath;
            Type declaringType = fieldInfo.DeclaringType;

            // Режим одинаков для всех target'ов: он определяется схемой класса,
            // а не конкретным значением поля.
            ResolutionMode mode = ClassifyMode(parentSerializedProperty);

            // Один проход по всем target'ам: у каждого свой instance и свой MethodInfo.
            // (Для разных runtime-типов получателя метод может отличаться —
            // например, override в производном типе.)
            var entries = new List<Entry>(targets.Length);
            foreach (UnityEngine.Object target in targets)
            {
                if (target == null) continue;

                object instance = ResolveInstance(target, parentPath, mode);
                if (instance == null) continue;

                if (!MethodUtility.TryFindMethod(
                        instance.GetType(), declaringType, methodName, out MethodInfo mi))
                {
                    continue;
                }

                entries.Add(new Entry(target, instance, mi));
            }

            // Ни один target не дал вызываемого метода — заглушка.
            if (entries.Count == 0)
                return CreateInvalidMethodNameAction();

            // Для подписи кнопки — первый успешный MethodInfo.
            // У всех target'ов обычно тот же тип, поэтому подпись будет корректной.
            methodInfo = entries[0].MethodInfo;

            return mode == ResolutionMode.Reference
                ? BuildReferenceAction(root, entries)
                : BuildGenericAction(entries, parentPath);
        }

        // ============================================================================
        //  Вспомогательное: режим разрешения instance
        // ============================================================================

        private enum ResolutionMode { Reference, Generic }

        private static ResolutionMode ClassifyMode(SerializedProperty parent)
        {
            if (parent == null) return ResolutionMode.Reference;
            return parent.propertyType == SerializedPropertyType.Generic
                ? ResolutionMode.Generic
                : ResolutionMode.Reference;
        }

        private static object ResolveInstance(
            UnityEngine.Object target,
            string parentPath,
            ResolutionMode mode)
        {
            // parentSerializedProperty == null: кнопка на самом объекте,
            // получателем выступает сам target.
            if (parentPath == null)
                return target;

            // Для конкретного target'а — свой SerializedObject и своя копия свойства.
            var so = new SerializedObject(target);
            SerializedProperty parent = so.FindProperty(parentPath);
            if (parent == null) return null;

            if (mode == ResolutionMode.Reference)
            {
                return parent.propertyType switch
                {
                    SerializedPropertyType.ObjectReference
                        or SerializedPropertyType.ExposedReference => parent.objectReferenceValue,
                    SerializedPropertyType.ManagedReference => parent.managedReferenceValue,
                    _ => null
                };
            }

            return parent.boxedValue;
        }

        // ============================================================================
        //  Сборка Action по режимам
        // ============================================================================

        private static Action BuildReferenceAction(SerializedObject root, List<Entry> entries)
        {
            // Reference-инстансы стабильны (ссылки на объекты), поэтому делегаты
            // создаём один раз и переиспользуем.
            var bound = new List<Action>(entries.Count);
            foreach (Entry e in entries)
            {
                Action a = SerializationUtility.MethodInfoToDelegate(e.MethodInfo, e.Instance);
                if (a != null) bound.Add(a);
            }

            if (bound.Count == 0)
                return CreateInvalidMethodNameAction();

            return () =>
            {
                foreach (Action action in bound)
                {
                    action.Invoke();
                }
                // Один общий ApplyModifiedProperties для корневого SerializedObject — отметит грязными все target'ы.
                root.ApplyModifiedProperties();
            };
        }

        private static Action BuildGenericAction(List<Entry> entries, string parentPath)
        {
            // boxedValue для значимых типов возвращает копию на каждое чтение.
            // Поэтому на каждый вызов: перечитать → вызвать метод → записать обратно,
            // и всё это для каждого target'а через свой SerializedObject.
            return () =>
            {
                foreach (Entry e in entries)
                {
                    var so = new SerializedObject(e.Target);
                    SerializedProperty parent = so.FindProperty(parentPath);
                    if (parent == null) continue;

                    object current = parent.boxedValue;
                    if (current == null) continue;

                    Action a = SerializationUtility.MethodInfoToDelegate(e.MethodInfo, current);
                    if (a == null) continue;

                    a.Invoke();

                    // Записываем мутированную копию обратно в сериализованное
                    // состояние именно этого target'а.
                    parent.boxedValue = current;
                    so.ApplyModifiedProperties();
                }
            };
        }

        // ============================================================================

        private readonly struct Entry
        {
            public readonly UnityEngine.Object Target;
            public readonly object Instance;
            public readonly MethodInfo MethodInfo;

            public Entry(UnityEngine.Object target, object instance, MethodInfo methodInfo)
            {
                Target = target;
                Instance = instance;
                MethodInfo = methodInfo;
            }
        }

        internal static void WrapGenericMenuCreation<T>(
            PopupField<T> popupField,
            Action onCreateMenuCallback,
            Action<AbstractGenericMenu> beforeDropDownCallback)
        {
            Type baseType = typeof(BasePopupField<,>).MakeGenericType(typeof(T), typeof(T));
            FieldInfo createMenuCallbackFieldInfo = baseType.GetField(
                "createMenuCallback",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Func<AbstractGenericMenu> createMenuCallback = () =>
            {
                GenericMenuWrapper wrapper = new(popupField, beforeDropDownCallback);
                onCreateMenuCallback?.Invoke();
                return wrapper;
            };
            createMenuCallbackFieldInfo.SetValue(popupField, createMenuCallback);
        }

        internal static void ApplyDrawMode(AttributeDrawMode drawMode, VisualElement temp, VisualElement decorator)
        {
            if (drawMode is AttributeDrawMode.After)
            {
                AddToFooterDecoratorContainer(temp, decorator);
            }
            else if (drawMode is AttributeDrawMode.Before)
            {
                int index = temp.parent.IndexOf(temp);
                temp.parent.Insert(index, decorator);
            }
        }

        internal static VisualElement GetDecoratorContainer(PropertyField propertyField)
            => propertyField.Q<VisualElement>(className: _decoratorDrawerContainerUss);

        internal static void AddToFooterDecoratorContainer(VisualElement field, VisualElement element)
        {
            PropertyField propertyField = field.GetFirstAncestorOfType<PropertyField>();
            VisualElement footerDecoratorContainer = propertyField.Q<VisualElement>(
                _footerDecoratorDrawerContainerName,
                _decoratorDrawerContainerUss);
            if (footerDecoratorContainer == null)
            {
                footerDecoratorContainer = new VisualElement()
                {
                    name = _footerDecoratorDrawerContainerName
                };
                footerDecoratorContainer.AddToClassList(_decoratorDrawerContainerUss);
                propertyField.Add(footerDecoratorContainer);
            }
            footerDecoratorContainer.Insert(0, element);
        }

        private static Action CreateInvalidMethodNameAction() => () => Debug.LogError("Invalid method name.");
    }
}
