using UnityEngine.UIElements;

namespace PostEnot.Toolkits
{
    public class ErrorHelpBoxAttribute : HelpBoxAttribute
    {
        public ErrorHelpBoxAttribute(
            string text,
            AttributeDrawMode attributeDrawMode = AttributeDrawMode.Before)
            : base(text, HelpBoxMessageType.Error, attributeDrawMode) { }

        public ErrorHelpBoxAttribute(
            string text,
            string methodName,
            AttributeDrawMode attributeDrawMode = AttributeDrawMode.Before)
            : base(text, methodName, HelpBoxMessageType.Error, attributeDrawMode) { }

        public ErrorHelpBoxAttribute(
            string text,
            string buttonText,
            string methodName,
            AttributeDrawMode attributeDrawMode = AttributeDrawMode.Before)
            : base(text, buttonText, methodName, HelpBoxMessageType.Error, attributeDrawMode) { }
    }
}
