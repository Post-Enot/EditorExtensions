using System;
using System.Collections.Generic;
using Unity.Hierarchy;
using Unity.Hierarchy.Editor;
using Unity.Scripting.LifecycleManagement;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    public class ComponentsColumn
    {
        private const string _columnId = "GameObject/Components";
        private const string _iconRowUss = "pe-component-icon-row";
        private const string _componentIconUss = "pe-component-icon";

        [NoAutoStaticsCleanup] private static readonly ObjectPool<Image> s_ImagePool = new(() => new Image());

        [InitializeOnLoadMethod]
        private static void Initialize() => HierarchyWindow.BindView += OnBindView;

        static void OnBindView(HierarchyWindow window, HierarchyView view)
        {
            EditorAttributesSettingsAsset settings = EditorAttributesSettingsAsset.GetSettings();
            view.styleSheets.Add(settings.ComponentsColumnStyleSheet);
        }

        [HierarchyViewColumnDescriptor(_columnId)]
        internal static void CreateColumnDesc(HierarchyViewColumnDescriptor desc)
        {
            desc.Title = "Components";
            desc.Tooltip = "Show all Components of GameObject";
            desc.DefaultPriority = 1000;
            desc.DefaultWidth = 100;
        }

        [HierarchyViewCellDescriptor(_columnId, typeof(HierarchyGameObjectHandler))]
        internal static void CreateGameObjectCellDesc(HierarchyViewCellDescriptor desc)
        {
            desc.BindCell = cell =>
            {
                // It's safe to cast cell.handler to HierarchyGameObjectHandler here : this method has the
                // HierarchyViewCellDescriptor attribute on it with the HierarchyGameObjectHandler specified to it.
                // Meaning that this method will only be called for nodes created by the HierarchyGameObjectHandler
                HierarchyGameObjectHandler handler = (HierarchyGameObjectHandler)cell.Handler;
                GameObject gameObject = handler.GetGameObject(cell.Node);
                Component[] components = gameObject.GetComponents<Component>();
                VisualElement iconRow = cell.Q(className: _iconRowUss);
                if (iconRow == null)
                {
                    iconRow = new VisualElement();
                    iconRow.AddToClassList(_iconRowUss);
                    iconRow.style.flexDirection = FlexDirection.Row;
                    cell.Add(iconRow);
                }
                else
                {
                    iconRow.Query<Image>(className: _componentIconUss).ForEach(
                        element =>
                        {
                            element.RemoveFromHierarchy();
                            s_ImagePool.Release(element);
                        });
                }
                HashSet<Texture> icons = new();
                foreach (Component component in components)
                {
                    if (component == null)
                    {
                        continue;
                    }
                    Type componentType = component.GetType();
                    Texture icon = FindTextureForComponentType(componentType);
                    if (icon == null)
                    {
                        continue;
                    }
                    bool isUnique = icons.Add(icon);
                    if (!isUnique)
                    {
                        continue;
                    }
                    Image iconElement = s_ImagePool.Get();
                    iconElement.AddToClassList(_componentIconUss);
                    iconElement.image = icon;
                    iconRow.Insert(0, iconElement);
                }
                cell.IsDefaultValue = false;
            };
        }

        private static Texture FindTextureForComponentType(Type componentType)
        {
            GUIContent guiContent = EditorGUIUtility.ObjectContent(null, componentType);
            return guiContent.image;
        }
    }
}
