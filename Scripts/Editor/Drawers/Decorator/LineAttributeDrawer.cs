using PostEnot.Toolkits;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomPropertyDrawer(typeof(LineAttribute))]
    internal sealed class LineAttributeDrawer : DecoratorDrawer
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
            LineAttribute lineAttribute = temp.userData as LineAttribute;
            temp.schedule.Execute(() => AfterAttach(lineAttribute, temp));
        }

        private void AfterAttach(LineAttribute lineAttribute, VisualElement temp)
        {
            VisualElement lineDecorator = new()
            {
                name = "line-decorator"
            };
            EditorAttributesSettingsAsset settingsAsset = EditorAttributesSettingsAsset.GetSettings();
            lineDecorator.AddToClassList("pe-line-decorator");
            lineDecorator.styleSheets.Add(settingsAsset.LineDecoratorStyleSheet);
            if (lineAttribute.HeightInPixels > 0)
            {
                lineDecorator.style.height = new Length(lineAttribute.HeightInPixels, LengthUnit.Pixel);
            }
            if (ColorUtility.TryParseHtmlString(lineAttribute.HexColor, out Color color))
            {
                lineDecorator.style.backgroundColor = color;
            }
            UIUtility.ApplyDrawMode(lineAttribute.DrawMode, temp, lineDecorator);
        }
    }
}
