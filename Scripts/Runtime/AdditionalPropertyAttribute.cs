using UnityEngine;

namespace PostEnot.Toolkits
{
    /// <summary>
    /// Класс-основа для реализации атрибутов, меняющих отображение в инспекторе, но не изменяющих структуру отображаемого элемента.
    /// </summary>
    public abstract class AdditionalPropertyAttribute : PropertyAttribute
    {
        public AdditionalPropertyAttribute(bool applyToCollection = false) : base(applyToCollection) {}
    }
}
