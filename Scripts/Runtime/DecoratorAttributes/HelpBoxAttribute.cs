using UnityEngine.UIElements;

namespace PostEnot.Toolkits
{
    public sealed class HelpBoxAttribute : DecoratorPropertyAttribute
    {
        public HelpBoxAttribute(
            string text,
            HelpBoxMessageType messageType = HelpBoxMessageType.None,
            AttributeDrawMode attributeDrawMode = AttributeDrawMode.Before) : base(attributeDrawMode)
        {
            Text = text;
            MessageType = messageType;
        }

        public string Text { get; }
        public HelpBoxMessageType MessageType { get; }
    }
}
