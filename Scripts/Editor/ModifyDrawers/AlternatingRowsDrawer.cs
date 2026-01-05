using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(AlternatingRowsAttribute))]
    internal sealed class AlternatingRowsDrawer : ModifyAttributeDrawer<AlternatingRowsAttribute>
    {
        private protected override void OnAttach(
            SerializedProperty property,
            PropertyField propertyField,
            FieldInfo fieldInfo,
            AlternatingRowsAttribute attribute)
        {
            if (propertyField == null)
            {
                return;
            }
            BaseVerticalCollectionView verticalCollectionView = propertyField.Q<BaseVerticalCollectionView>();
            if (verticalCollectionView == null)
            {
                return;                
            }
            verticalCollectionView.showAlternatingRowBackgrounds = attribute.ContentOnly
                ? AlternatingRowBackground.ContentOnly
                : AlternatingRowBackground.All;
        }
    }
}
