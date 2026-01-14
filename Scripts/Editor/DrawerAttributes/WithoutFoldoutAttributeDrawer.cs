using PostEnot.Toolkits;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(WithoutFoldoutAttribute))]
    internal sealed class WithoutFoldoutAttributeDrawer : BasePropertyDrawer<WithoutFoldoutAttribute>
    {
        private protected override VisualElement CreateProperty(
            SerializedProperty property,
            FieldInfo fieldInfo,
            WithoutFoldoutAttribute attribute)
        {
            VisualElement container = new();
            DrawChildrenProperties(container, property);
            return container;
        }

        private static void DrawChildrenProperties(VisualElement container, SerializedProperty property)
        {
            int depth = property.depth;
            if (!property.NextVisible(true))
            {
                return;
            }
            do
            {
                if (property.depth != (depth + 1))
                {
                    break;
                }
                PropertyField propertyField = new();
                propertyField.BindProperty(property);
                container.Add(propertyField);
            }
            while (property.NextVisible(false));
        }
    }
}
