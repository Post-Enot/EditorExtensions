using PostEnot.Toolkits;
using System;
using System.Collections.Generic;
using Unity.Scripting.LifecycleManagement;
using UnityEditor;
using UnityEditor.SettingsManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal sealed partial class EditorAttributesSettingsAsset : ScriptableObject
    {
        private const string _prefKeySettingsGuid = "settingsGuid";
        private const string _packageName = "com.postenot.editorextensions";
        private const string _defaultCreateSettingsPath = "Assets/EditorExtensionsSettings.asset";
        private const string _defaultSettingsGUID = "da2cfa0f35ae5224d92ac9bba76ff5f8";

        #region Inspector
        [Header("Attributes Style Sheets")]
        [SerializeField, Label("Line Decorator")] private StyleSheet lineDecoratorStyleSheet;
        [SerializeField, Label("Slider")] private StyleSheet sliderStyleSheet;
        [SerializeField, Label("Vector Labels")] private StyleSheet vectorLabelsStyleSheet;
        [SerializeField, Label("Table")] private StyleSheet tableStyleSheet;
        [SerializeField, Label("Preview")] private StyleSheet previewStyleSheet;
        [SerializeField, Label("MinMaxSlider")] private StyleSheet minMaxSliderStyleSheet;

        [Header("Hierarchy Style Sheets")]
        [SerializeField, Label("Components Column")] private StyleSheet componentsColumnStyleSheet;
        [SerializeField, Label("Advanced Hierarchy")] private StyleSheet advancedHierarchyStyleSheet;
        [SerializeField, Table, Label("<b>Special Names</b>")] private List<HierarchySpecialNameData> hierarchySpecialNames = new();
        //{
        //    {
        //        "[red]",
        //        new Color(192f / 255f, 57f / 255f, 43f / 255f)
        //    },
        //    {
        //        "[orange]",
        //        new Color(192f / 255f, 86f / 255f, 0f / 255f)
        //    },
        //    {
        //        "[yellow]",
        //        new Color(166f / 255f, 124f / 255f, 0f / 255f)
        //    },
        //    {
        //        "[lightGreen]",
        //        new Color(56f / 255f, 142f / 255f, 60f / 255f)
        //    },
        //    {
        //        "[darkGreen]",
        //        new Color(27f / 255f, 94f / 255f, 32f / 255f)
        //    },
        //    {
        //        "[cyan]",
        //        new Color(0f / 255f, 131f / 255f, 143f / 255f)
        //    },
        //    {
        //        "[blue]",
        //        new Color(13f / 255f, 71f / 255f, 161f / 255f)
        //    },
        //    {
        //        "[pink]",
        //        new Color(194f / 255f, 24f / 255f, 91f / 255f)
        //    },
        //    {
        //        "[purple]",
        //        new Color(123f / 255f, 31f / 255f, 162f / 255f)
        //    },
        //    {
        //        "[black]",
        //        new Color(26f / 255f, 26f / 255f, 26f / 255f)
        //    }
        //};
        #endregion

        public StyleSheet LineDecoratorStyleSheet => lineDecoratorStyleSheet;
        public StyleSheet SliderStyleSheet => sliderStyleSheet;
        public StyleSheet VectorLabelsStyleSheet => vectorLabelsStyleSheet;
        public StyleSheet TableStyleSheet => tableStyleSheet;
        public StyleSheet PreviewStyleSheet => previewStyleSheet;
        public StyleSheet MinMaxSliderStyleSheet => minMaxSliderStyleSheet;
        public StyleSheet ComponentsColumnStyleSheet => componentsColumnStyleSheet;
        public StyleSheet AdvancedHierarchyStyleSheet => advancedHierarchyStyleSheet;

        [AutoStaticsCleanup] public static event Action SpecialNamesChanged;

        [AutoStaticsCleanup] private static EditorAttributesSettingsAsset _instance;

        public void InvokeSpecialNamesChanged() => SpecialNamesChanged?.Invoke();

        public bool TryGetHirarchyColorBySpecialName(string name, out Color color, out bool isBold)
        {
            if (hierarchySpecialNames != null)
            {
                foreach (HierarchySpecialNameData specialName in hierarchySpecialNames)
                {
                    if (string.IsNullOrWhiteSpace(specialName.Pattern))
                    {
                        continue;
                    }
                    if (IsMatch(name, specialName.Pattern, specialName.Mode))
                    {
                        color = specialName.Color;
                        isBold = specialName.IsBold;
                        return true;
                    }
                }
            }
            color = default;
            isBold = default;
            return false;
        }

        private bool IsMatch(string name, string pattern, HierarchySpecialNameMode mode) => mode switch
            {
                HierarchySpecialNameMode.Ordinal => string.Equals(name, pattern, StringComparison.Ordinal),
                HierarchySpecialNameMode.Prefix => name.StartsWith(pattern, StringComparison.Ordinal),
                HierarchySpecialNameMode.Suffix => name.EndsWith(pattern, StringComparison.Ordinal),
                HierarchySpecialNameMode.Regex => System.Text.RegularExpressions.Regex.IsMatch(name, pattern),
                HierarchySpecialNameMode.OrdinalIgnoreCase => string.Equals(name, pattern, StringComparison.OrdinalIgnoreCase),
                HierarchySpecialNameMode.PrefixIgnoreCase => name.StartsWith(pattern, StringComparison.OrdinalIgnoreCase),
                HierarchySpecialNameMode.SuffixIgnoreCase => name.EndsWith(pattern, StringComparison.OrdinalIgnoreCase),
                _ => throw new NotImplementedException()
            };

        public static EditorAttributesSettingsAsset GetSettings()
        {
            if (_instance == null)
            {
                _instance = GetOrCreateSettings();
            }
            return _instance;
        }

        private static EditorAttributesSettingsAsset GetOrCreateSettings()
        {
            Settings settings = new(_packageName);
            EditorAttributesSettingsAsset settingsAsset = null;
            if (settings.ContainsKey<string>(_prefKeySettingsGuid, SettingsScope.Project))
            {
                string guid = settings.Get<string>(_prefKeySettingsGuid, SettingsScope.Project);
                if (TryGetSettingsByGUID(guid, out settingsAsset))
                {
                    return settingsAsset;
                }
            }
            settingsAsset = AssetDatabase.LoadAssetAtPath<EditorAttributesSettingsAsset>(_defaultCreateSettingsPath);
            if (settingsAsset == null)
            {
                if (TryGetSettingsByGUID(_defaultSettingsGUID, out EditorAttributesSettingsAsset settingsTemplate))
                {
                    settingsAsset = Instantiate(settingsTemplate);
                }
                else
                {
                    settingsAsset = CreateInstance<EditorAttributesSettingsAsset>();

                }
                AssetDatabase.CreateAsset(settingsAsset, _defaultCreateSettingsPath);
                AssetDatabase.SaveAssets();
            }
            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(settingsAsset, out string settingsGuid, out _))
            {
                settings.Set(_prefKeySettingsGuid, settingsGuid, SettingsScope.Project);
                settings.Save();
            }
            return settingsAsset;
        }

        private static bool TryGetSettingsByGUID(string strGuid, out EditorAttributesSettingsAsset settingsAsset)
        {
            if (GUID.TryParse(strGuid, out GUID guid))
            {
                settingsAsset = AssetDatabase.LoadAssetByGUID<EditorAttributesSettingsAsset>(guid);
                return settingsAsset != null;
            }
            settingsAsset = null;
            return false;
        }
    }
}
