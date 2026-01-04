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
    [CustomPropertyDrawer(typeof(TableAttribute))]
    internal sealed class TableAttributeDrawer : BasePropertyDrawer<TableAttribute>
    {
        private protected override VisualElement CreateProperty(SerializedProperty property, FieldInfo fieldInfo, TableAttribute attribute)
        {
            if (!property.isArray)
            {
                return new Label("Use Table with collections.");
            }
            List<FieldInfo> elementFieldInfos = GetSerializableFieldsFromCollectionElement(fieldInfo);
            MultiColumnListView multiColumnListView = new()
            {
                showAlternatingRowBackgrounds = attribute.AlternatingRows,
                reorderable = attribute.Reorderable,
                reorderMode = attribute.ReorderMode,
                showAddRemoveFooter = attribute.ShowAddRemoveFooter,
                showFoldoutHeader = attribute.ShowFoldout,
                showBoundCollectionSize = attribute.ShowBoundCollectionSize,
                showBorder = attribute.ShowBorder,
                allowAdd = attribute.AllowAdd,
                allowRemove = attribute.AllowRemove,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                headerTitle = preferredLabel
            };
            HashSet<string> hiddenColumnPathes = GetHiddenColumns(fieldInfo);
            foreach (FieldInfo elementFieldInfo in elementFieldInfos)
            {
                if (hiddenColumnPathes.Contains(elementFieldInfo.Name))
                {
                    continue;
                }
                Column column = new()
                {
                    bindingPath = elementFieldInfo.Name,
                    title = ObjectNames.NicifyVariableName(elementFieldInfo.Name),
                    stretchable = true
                };
                multiColumnListView.columns.Add(column);
            }
            if (Settings.TableStyleSheet != null)
            {
                multiColumnListView.styleSheets.Add(Settings.TableStyleSheet);
            }
            multiColumnListView.BindProperty(property);
            return multiColumnListView;
        }

        private static HashSet<string> GetHiddenColumns(FieldInfo fieldInfo)
        {
            HashSet<string> hiddenColumnPathes = new();
            IEnumerable<HideColumnAttribute> hiddenColumns = fieldInfo.GetCustomAttributes<HideColumnAttribute>();
            foreach (HideColumnAttribute hiddenColumn in hiddenColumns)
            {
                _ = hiddenColumnPathes.Add(hiddenColumn.BindingPath);
            }
            return hiddenColumnPathes;
        }

        private static List<FieldInfo> GetSerializableFieldsFromCollectionElement(FieldInfo fieldInfo)
        {
            Type elementType = SerializationUtility.GetElementTypeOfSerializedCollection(fieldInfo.FieldType);
            FieldInfo[] allFields = elementType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            List<FieldInfo> result = new();
            foreach (FieldInfo field in allFields)
            {
                if (HasSerializationAttribute(field))
                {
                    result.Add(field);
                }
            }
            return result;
        }

        private static bool HasSerializationAttribute(FieldInfo field)
        {
            object[] attributes = field.GetCustomAttributes(false);
            bool result = false;
            foreach (object attribute in attributes)
            {
                if (attribute is HideInTableAttribute)
                {
                    return false;
                }
                else if (attribute is SerializeField or SerializeReference)
                {
                    result = true;
                }
            }
            return result;
        }
    }
}
