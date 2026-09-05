using UnityEngine.UIElements;

namespace PostEnot.Toolkits
{
    public class WarningHelpBoxAttribute : HelpBoxAttribute
    {
        public WarningHelpBoxAttribute(
            string text,
            AttributeDrawMode attributeDrawMode = AttributeDrawMode.Before)
            : base(text, HelpBoxMessageType.Warning, attributeDrawMode) { }

        public WarningHelpBoxAttribute(
            string text,
            string methodName,
            AttributeDrawMode attributeDrawMode = AttributeDrawMode.Before)
            : base(text, methodName, HelpBoxMessageType.Warning, attributeDrawMode) { }

        public WarningHelpBoxAttribute(
            string text,
            string buttonText,
            string methodName,
            AttributeDrawMode attributeDrawMode = AttributeDrawMode.Before)
            : base(text, buttonText, methodName, HelpBoxMessageType.Warning, attributeDrawMode) { }
    }
}
