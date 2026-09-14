using Unity.Hierarchy;
using Unity.Hierarchy.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [InitializeOnLoad]
    internal static class AdvancedHierarchyInitializer
    {
        public static string UssBoldName => $"pe-hierarchy-item-view__name--bold";

        static AdvancedHierarchyInitializer() => Initialize();

        public static void Initialize()
        {
            HierarchyWindow.BindView -= OnBindView;
            HierarchyWindow.BindView += OnBindView;

            EditorAttributesSettingsAsset.SpecialNamesChanged -= OnSpecialNamesChanged;
            EditorAttributesSettingsAsset.SpecialNamesChanged += OnSpecialNamesChanged;

            HierarchyWindow.UnbindView -= OnUnbindView;
            HierarchyWindow.UnbindView += OnUnbindView;
        }

        private static void OnSpecialNamesChanged()
        {
            if (!EditorWindow.HasOpenInstances<HierarchyWindow>())
            {
                return;
            }
            HierarchyWindow[] windows = Resources.FindObjectsOfTypeAll<HierarchyWindow>();
            foreach (HierarchyWindow window in windows)
            {
                if (window == null)
                {
                    continue;
                }
                if (window.View == null)
                {
                    continue;
                }
                UQueryState<HierarchyViewItem> items = window.View.Query<HierarchyViewItem>().Build();
                foreach (HierarchyViewItem item in items)
                {
                    if (item.NodeType == HierarchyNodeType.Null)
                    {
                        continue;
                    }
                    EntityId entityId = item.View.Source.GetEntityIdFromNode(item.Node);
                    Object obj = EditorUtility.EntityIdToObject(entityId);
                    if (obj is not GameObject gameObject)
                    {
                        continue;
                    }
                    ApplyFolderMarking(item, gameObject);
                }
            }
        }

        private static void OnBindView(HierarchyWindow window, HierarchyView view)
        {
            view.BindViewItem += OnBindViewItem;
            EditorAttributesSettingsAsset settings = EditorAttributesSettingsAsset.GetSettings();
            if (settings.AdvancedHierarchyStyleSheet != null)
            {
                view.styleSheets.Add(settings.AdvancedHierarchyStyleSheet);
            }
        }

        private static void OnUnbindView(HierarchyWindow window, HierarchyView view)
        {
            if (view == null)
            {
                return;
            }
            view.BindViewItem -= OnBindViewItem;
        }

        private static void OnBindViewItem(HierarchyView view, HierarchyViewItem item)
        {
            if (item == null)
            {
                return;
            }
            if (item.NodeType == HierarchyNodeType.Null)
            {
                return;
            }
            if (view == null)
            {
                return;
            }
            if (view.Source == null)
            {
                return;
            }
            EntityId entityId = view.Source.GetEntityIdFromNode(item.Node);
            Object obj = EditorUtility.EntityIdToObject(entityId);
            if (obj is not GameObject gameObject)
            {
                return;
            }
            ApplyFolderMarking(item, gameObject);
        }

        private static void ApplyFolderMarking(HierarchyViewItem item, GameObject gameObject)
        {
            EditorAttributesSettingsAsset settings = EditorAttributesSettingsAsset.GetSettings();
            if (settings.TryGetHirarchyColorBySpecialName(gameObject.name, out Color color, out bool isBold))
            {
                RowGradientView rowGradientView = item.RowContainer.Q<RowGradientView>() ?? new();
                rowGradientView.Color = color;
                rowGradientView.RemoveFromHierarchy();
                item.RowContainer.Insert(0, rowGradientView);
                item.Name.EnableInClassList(UssBoldName, isBold);
            }
            else
            {
                RowGradientView rowGradientView = item.RowContainer.Q<RowGradientView>();
                item.Name.RemoveFromClassList(UssBoldName);
                rowGradientView?.RemoveFromHierarchy();
            }
        }
    }
}
