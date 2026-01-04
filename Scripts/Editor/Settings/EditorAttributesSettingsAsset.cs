using PostEnot.Toolkits;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal sealed class EditorAttributesSettingsAsset : ScriptableObject
    {
        private const string _defaultSettingsGUID = "da2cfa0f35ae5224d92ac9bba76ff5f8";
        private const string _currentSettingsGUIDPrefsKey = "PostEnot_EditorAttributes_CurrentSettingsGUIDPrefsKey";
        private const string _pathToCreateAsset = "Assets/EditorAttributesSettingsAsset.asset";

        #region Inspector
        [Header("Style Sheets:")]
        [SerializeField, Label("Line Decorator")] private StyleSheet lineDecoratorStyleSheet;
        [SerializeField, Label("Slider")]         private StyleSheet sliderStyleSheet;
        [SerializeField, Label("Vector Labels")]  private StyleSheet vectorLabelsStyleSheet;
        [SerializeField, Label("Table")]          private StyleSheet tableStyleSheet;
        #endregion

        public StyleSheet LineDecoratorStyleSheet => lineDecoratorStyleSheet;
        public StyleSheet SliderStyleSheet => sliderStyleSheet;
        public StyleSheet VectorLabelsStyleSheet => vectorLabelsStyleSheet;
        public StyleSheet TableStyleSheet => tableStyleSheet;

        private static EditorAttributesSettingsAsset _instance;

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
            EditorAttributesSettingsAsset settingsAsset;
            if (EditorPrefs.HasKey(_currentSettingsGUIDPrefsKey))
            {
                string strGUID = EditorPrefs.GetString(_currentSettingsGUIDPrefsKey);
                if (TryGetSettingsByGUID(strGUID, out settingsAsset))
                {
                    return settingsAsset;
                }
            }
            if (TryGetSettingsByGUID(_defaultSettingsGUID, out settingsAsset))
            {
                return settingsAsset;
            }
            settingsAsset = CreateInstance<EditorAttributesSettingsAsset>();
            AssetDatabase.CreateAsset(settingsAsset, _pathToCreateAsset);
            AssetDatabase.SaveAssets();
            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(settingsAsset, out string strGuid, out long localId))
            {
                EditorPrefs.SetString(_currentSettingsGUIDPrefsKey, strGuid);
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
