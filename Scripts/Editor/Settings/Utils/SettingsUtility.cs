#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEditor.SettingsManagement;
using UnityEngine;

namespace PostEnot.EditorExtensions.Editor
{
    public static class SettingsUtility
    {
        public static T GetOrCreateSettingsAsset<T>(
            Settings settings,
            string keyAssetGuid,
            string creationAssetPath)
            where T : ScriptableObject
        {
            return GetOrCreateSettingsAsset<T>(
                settings,
                keyAssetGuid,
                creationAssetPath,
                () => ScriptableObject.CreateInstance<T>());
        }

        public static T GetOrCreateSettingsAsset<T>(
            Settings settings,
            string keyAssetGuid,
            string creationAssetPath,
            T defaultAsset)
            where T : ScriptableObject
        {
            return GetOrCreateSettingsAsset<T>(
                settings,
                keyAssetGuid,
                creationAssetPath,
                () => UnityEngine.Object.Instantiate(defaultAsset));
        }

        public static T GetOrCreateSettingsAsset<T>(
            Settings settings,
            string keyAssetGuid,
            string creationAssetPath,
            string defaultAssetGuid)
            where T : ScriptableObject
        {
            return GetOrCreateSettingsAsset<T>(
                settings,
                keyAssetGuid,
                creationAssetPath,
                () =>
                {
                    if (TryGetSettingsAssetByGUID(defaultAssetGuid, out T? defaultAsset))
                    {
                        return UnityEngine.Object.Instantiate(defaultAsset);
                    }

                    return ScriptableObject.CreateInstance<T>();
                });
        }

        public static void SetSettingsAsset<T>(T settingsAsset, Settings settings, string keyAssetGuid) where T : ScriptableObject
        {
            if (settingsAsset == null)
            {
                settings.DeleteKey<string>(keyAssetGuid, SettingsScope.Project);
            }
            else
            {
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(settingsAsset, out string settingsGuid, out _))
                {
                    settings.Set(keyAssetGuid, settingsGuid, SettingsScope.Project);
                    settings.Save();
                }
            }
        }


        private static T GetOrCreateSettingsAsset<T>(
            Settings settings,
            string keyAssetGuid,
            string creationAssetPath,
            Func<T> createNewAsset)
            where T : ScriptableObject
        {
            T? settingsAsset;

            if (settings.ContainsKey<string>(keyAssetGuid, SettingsScope.Project))
            {
                string guid = settings.Get<string>(keyAssetGuid, SettingsScope.Project);
                if (TryGetSettingsAssetByGUID(guid, out settingsAsset))
                {
                    return settingsAsset;
                }
            }

            settingsAsset = AssetDatabase.LoadAssetAtPath<T>(creationAssetPath);

            if (settingsAsset == null)
            {
                settingsAsset = createNewAsset();
                AssetDatabase.CreateAsset(settingsAsset, creationAssetPath);
                AssetDatabase.SaveAssets();
            }

            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(settingsAsset, out string settingsGuid, out _))
            {
                settings.Set(keyAssetGuid, settingsGuid, SettingsScope.Project);
                settings.Save();
            }

            return settingsAsset;
        }

        public static bool TryGetSettingsAssetByGUID<T>(
            string strGuid,
            [NotNullWhen(true)] out T? asset)
            where T : ScriptableObject
        {
            if (strGuid == null)
            {
                throw new ArgumentNullException(nameof(strGuid));
            }

            if (GUID.TryParse(strGuid, out GUID guid))
            {
                asset = AssetDatabase.LoadAssetByGUID<T>(guid);
                return asset != null;
            }

            asset = null;
            return false;
        }
    }
}
