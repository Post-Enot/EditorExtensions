using PostEnot.Toolkits;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    internal static class DrawerUtility
    {
        private const string _footerDecoratorDrawerContainerName = "footer-decorator-drawers-container";
        private const string _decoratorDrawerContainerUss = "unity-decorator-drawers-container";

        internal static void ApplyDrawMode(AttributeDrawMode drawMode, VisualElement temp, VisualElement decorator)
        {
            PropertyField propertyField = temp.GetFirstAncestorOfType<PropertyField>();
            if (drawMode is AttributeDrawMode.After)
            {
                VisualElement footerDecoratorContainer = propertyField.Q<VisualElement>(
                    _footerDecoratorDrawerContainerName,
                    _decoratorDrawerContainerUss);
                if (footerDecoratorContainer == null)
                {
                    footerDecoratorContainer = new VisualElement()
                    {
                        name = _footerDecoratorDrawerContainerName
                    };
                    footerDecoratorContainer.AddToClassList(_decoratorDrawerContainerUss);
                    propertyField.Add(footerDecoratorContainer);
                }
                footerDecoratorContainer.Insert(0, decorator);
            }
            else if (drawMode is AttributeDrawMode.Before)
            {
                int index = temp.parent.IndexOf(temp);
                temp.parent.Insert(index, decorator);
            }
        }
    }
}
