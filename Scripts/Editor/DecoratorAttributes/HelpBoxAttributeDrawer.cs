using PostEnot.Toolkits;
using UnityEditor;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public sealed class HelpBoxAttributeDrawer : DecoratorDrawer
    {
        public override VisualElement CreatePropertyGUI()
        {
            VisualElement temp = new()
            {
                name = "TEMP",
                userData = attribute
            };
            temp.RegisterCallbackOnce<AttachToPanelEvent>(OnAttachToPanel);
            return temp;
        }

        private void OnAttachToPanel(AttachToPanelEvent context)
        {
            VisualElement temp = context.target as VisualElement;
            HelpBoxAttribute helpBoxAttribute = temp.userData as HelpBoxAttribute;
            temp.schedule.Execute(() => AfterAttach(helpBoxAttribute, temp));
        }

        private void AfterAttach(HelpBoxAttribute helpBoxAttribute, VisualElement temp)
        {
            HelpBox helpBox = new(helpBoxAttribute.Text, helpBoxAttribute.MessageType);
            DrawerUtility.ApplyDrawMode(helpBoxAttribute.DrawMode, temp, helpBox);
        }
    }
}
