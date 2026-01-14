namespace PostEnot.Toolkits
{
    public class LabelAttribute : ModifyPropertyAttribute
    {
        // Приведение null к string.Empty необходимо, т.к. при установка значения null для
        // PropertyField.label приводит не к сокрытию поля, а к возвращению его к стандартному значению.
        public LabelAttribute(string label) => Label = label ?? string.Empty;

        public string Label { get; }
    }
}
