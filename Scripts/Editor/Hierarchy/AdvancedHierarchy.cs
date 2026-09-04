using Unity.Hierarchy;
using Unity.Hierarchy.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [InitializeOnLoad]
    internal static class AdvancedHierarchy
    {
        public static string UssBoldName => $"pe-hierarchy-item-view__name--bold";

        static AdvancedHierarchy() => Initialize();

        public static void Initialize()
        {
            HierarchyWindow.BindView -= HierarchyWindowOnBindView;
            HierarchyWindow.BindView += HierarchyWindowOnBindView;

            HierarchyWindow.UnbindView -= HierarchyWindowOnUnbindView;
            HierarchyWindow.UnbindView += HierarchyWindowOnUnbindView;
        }

        private static void HierarchyWindowOnBindView(HierarchyWindow window, HierarchyView view)
        {
            view.BindViewItem += ViewOnBindViewItem;
            EditorAttributesSettingsAsset settings = EditorAttributesSettingsAsset.GetSettings();
            if (settings.AdvancedHierarchyStyleSheet != null)
            {
                view.styleSheets.Add(settings.AdvancedHierarchyStyleSheet);
            }
            settings.SpecialNamesChanged += () =>
            {
                UQueryState<HierarchyViewItem> items = view.Query<HierarchyViewItem>().Build();
                foreach (HierarchyViewItem item in items)
                {
                    EntityId entityId = item.View.Source.GetEntityIdFromNode(item.Node);
                    Object obj = EditorUtility.EntityIdToObject(entityId);
                    if (obj is not GameObject gameObject)
                    {
                        continue;
                    }
                    ApplyFolderMarking(item, gameObject);
                }
            };
        }

        private static void ViewOnBindViewItem(HierarchyView view, HierarchyViewItem item)
        {
            var entityId = item.View.Source.GetEntityIdFromNode(item.Node);
            var obj = EditorUtility.EntityIdToObject(entityId);
            var gameObject = obj as GameObject;
            if (gameObject == null)
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

        private static void HierarchyWindowOnUnbindView(HierarchyWindow window, HierarchyView view)
        {
            view.BindViewItem -= ViewOnBindViewItem;
            EditorAttributesSettingsAsset settings = EditorAttributesSettingsAsset.GetSettings();
        }
    }
}
