using UnityEngine.UIElements;

namespace PostEnot.Toolkits
{
    public class HelpBoxAttribute : DecoratorPropertyAttribute
    {
        public HelpBoxAttribute(
            string text,
            HelpBoxMessageType messageType = HelpBoxMessageType.None,
            AttributeDrawMode attributeDrawMode = AttributeDrawMode.Before) : base(attributeDrawMode)
        {
            Text = text;
            MessageType = messageType;
        }

        public HelpBoxAttribute(
            string text,
            string methodName,
            HelpBoxMessageType messageType = HelpBoxMessageType.None,
            AttributeDrawMode attributeDrawMode = AttributeDrawMode.Before) : this(text, messageType, attributeDrawMode)
        {
            MethodName = methodName;
        }

        public HelpBoxAttribute(
            string text,
            string buttonText,
            string methodName,
            HelpBoxMessageType messageType = HelpBoxMessageType.None,
            AttributeDrawMode attributeDrawMode = AttributeDrawMode.Before) : this(text, methodName, messageType, attributeDrawMode)
        {
            ButtonText = buttonText;
        }

        public string Text { get; }
        public string MethodName { get; }
        public string ButtonText { get; }
        public HelpBoxMessageType MessageType { get; }
    }
}
