using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal sealed class SortingLayerField : AdvancedPopupField<int>
    {
        internal SortingLayerField(string label) : base(label)
        {
            formatListItemCallback = FormatListItem;
            formatSelectedValueCallback = FormatSelectedValue;
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        internal event Action ChoicedUpdated;

        private protected override void OnCreateMenu() => UpdateChoices();

        private protected override void OnBeforeMenuShow(AbstractGenericMenu menu)
        {
            menu.AddSeparator(string.Empty);
            menu.AddItem("Add Sorting Layer...", false, OpenBuildProfiles);
        }

        private void UpdateChoices()
        {
            List<int> choices = new();
            for (int i = 0; i < SortingLayer.layers.Length; i += 1)
            {
                choices.Add(i);
            }
            this.choices = choices;
            ChoicedUpdated?.Invoke();
        }

        private static string FormatSelectedValue(int value)
        {
            if ((value >= SortingLayer.layers.Length) || (value < 0))
            {
                return string.Empty;
            }
            return SortingLayer.layers[value].name;
        }

        #region EventHandlers
        private void OnAttachToPanel(AttachToPanelEvent context)
        {
            SortingLayer.onLayerAdded += OnSortingLayersListChanged;
            SortingLayer.onLayerRemoved += OnSortingLayersListChanged;
        }

        private void OnDetachFromPanel(DetachFromPanelEvent context)
        {
            SortingLayer.onLayerAdded -= OnSortingLayersListChanged;
            SortingLayer.onLayerRemoved -= OnSortingLayersListChanged;
        }

        private void OnSortingLayersListChanged(SortingLayer sortingLayer) => UpdateChoices();
        #endregion


        private static string FormatListItem(int value) => $"{value}: {SortingLayer.layers[value].name}";

        private static void OpenBuildProfiles()
        {
            Type tagManagerInspectorType = typeof(UnityEditor.Editor)
                .Assembly
                .GetType("UnityEditor.TagManagerInspector");
            Type enumType = tagManagerInspectorType.GetNestedType(
                "InitialExpansionState",
                BindingFlags.Public | BindingFlags.NonPublic);
            FieldInfo sortingLayersValue = enumType.GetField("SortingLayers");
            MethodInfo method = tagManagerInspectorType.GetMethod(
                "ShowWithInitialExpansion",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            object sortingLayers = sortingLayersValue.GetValue(null);
            object[] parameters = new object[]
            {
                sortingLayers
            };
            method.Invoke(null, parameters);
        }
    }
}
