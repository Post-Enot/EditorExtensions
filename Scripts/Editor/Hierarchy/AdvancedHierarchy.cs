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
            view.RegisterCallback<GeometryChangedEvent, HierarchyView>(OnGeometryChangedEvent, view);
        }

        private static void OnGeometryChangedEvent(GeometryChangedEvent evt, HierarchyView view)
        {
            view.UnregisterCallback<GeometryChangedEvent, HierarchyView>(OnGeometryChangedEvent);
        }

        private static void ViewOnBindViewItem(HierarchyView view, HierarchyViewItem item)
        {
            var entityId = item.View.Source.GetEntityIdFromNode(item.Node);
            var obj = EditorUtility.EntityIdToObject(entityId);
            var gameObject = obj as GameObject;
            if (gameObject == null)
                return;

            ApplyFolderMarking(item, gameObject);
        }

        private static void ApplyFolderMarking(HierarchyViewItem item, GameObject gameObject)
        {
            EditorAttributesSettingsAsset settings = EditorAttributesSettingsAsset.GetSettings();
            if (settings.TryGetHirarchyColorBySpecialName(gameObject.name, out Color color))
            {
                RowGradientView rowGradientView = item.RowContainer.Q<RowGradientView>() ?? new();
                rowGradientView.Color = color;
                rowGradientView.RemoveFromHierarchy();
                item.RowContainer.Insert(0, rowGradientView);
            }
            else
            {
                RowGradientView rowGradientView = item.RowContainer.Q<RowGradientView>();
                rowGradientView?.RemoveFromHierarchy();
            }
        }

        private static void HierarchyWindowOnUnbindView(HierarchyWindow window, HierarchyView view)
            => view.BindViewItem -= ViewOnBindViewItem;
    }
}
