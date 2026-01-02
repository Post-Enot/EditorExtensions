using PostEnot.Toolkits;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(WithoutFoldoutAttribute))]
    internal sealed class WithoutFoldoutAttributePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new();
            DrawChildrenProperties(container, property);
            return container;
        }

        private void DrawChildrenProperties(VisualElement container, SerializedProperty property)
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
