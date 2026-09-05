using UnityEngine.UIElements;

namespace PostEnot.Toolkits
{
    public class InfoHelpBoxAttribute : HelpBoxAttribute
    {
        public InfoHelpBoxAttribute(
            string text,
            AttributeDrawMode attributeDrawMode = AttributeDrawMode.Before)
            : base(text, HelpBoxMessageType.Info, attributeDrawMode) { }

        public InfoHelpBoxAttribute(
            string text,
            string methodName,
            AttributeDrawMode attributeDrawMode = AttributeDrawMode.Before)
            : base(text, methodName, HelpBoxMessageType.Info, attributeDrawMode) { }

        public InfoHelpBoxAttribute(
            string text,
            string buttonText,
            string methodName,
            AttributeDrawMode attributeDrawMode = AttributeDrawMode.Before)
            : base(text, buttonText, methodName, HelpBoxMessageType.Info, attributeDrawMode) { }
    }
}
