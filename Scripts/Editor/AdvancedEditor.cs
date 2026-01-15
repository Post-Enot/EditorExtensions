using PostEnot.Toolkits;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.EditorExtensions.Editor
{
    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class AdvancedEditor : UnityEditor.Editor
    {
        private static Dictionary<Type, AdvancedModificatorDrawer> _modificatorDrawers;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();
            Type targetType = serializedObject.targetObject.GetType();
            IEnumerable<Attribute> classAttributes = targetType.GetCustomAttributes<Attribute>();
            bool containsHideScriptAttribute = false;
            foreach (Attribute classAttribute in classAttributes)
            {
                if (classAttribute is DisableInspectorAttribute)
                {
                    root.SetEnabled(false);
                }
                else if (classAttribute is HideInspectorAttribute)
                {
                    return new VisualElement();
                }
                else if (classAttribute is HideClassFieldAttribute)
                {
                    containsHideScriptAttribute = true;
                }
            }

            if (containsHideScriptAttribute)
            {
                InspectorElement.FillDefaultInspector(root, serializedObject, this, "m_Script");
            }
            else
            {
                InspectorElement.FillDefaultInspector(root, serializedObject, this);
            }
            List<PropertyField> propertyFields = new();
            foreach (VisualElement child in root.Children())
            {
                if ((child is PropertyField propertyField) && (propertyField.bindingPath != "m_Script"))
                {
                    propertyFields.Add(propertyField);
                }
            }
            ModificatorPass(serializedObject, propertyFields);
            return root;
        }

        private static void InitModificatorDrawers() =>
            _modificatorDrawers ??= new Dictionary<Type, AdvancedModificatorDrawer>
            {
                { typeof(ReadOnlyAttribute), new ReadOnlyAttributeDrawer() },
                { typeof(LabelAttribute), new LabelAttributeDrawer() }
            };

        private static AdvancedModificatorDrawer GetModificatorDrawerFor(Type attributeType)
        {
            InitModificatorDrawers();
            while (attributeType != null)
            {
                if (_modificatorDrawers.TryGetValue(attributeType, out AdvancedModificatorDrawer drawer))
                {
                    return drawer;
                }
                attributeType = attributeType.BaseType;
            }
            return null;
        }

        private static void ModificatorPass(SerializedObject serializedObject, IReadOnlyList<PropertyField> propertyFields)
        {
            List<ModifyPropertyAttribute> attributes = new();
            foreach (PropertyField propertyField in propertyFields)
            {
                SerializedProperty property = serializedObject.FindProperty(propertyField.bindingPath);
                FieldInfo fieldInfo = SerializationUtility.GetFieldInfo(property);
                SerializationUtility.GetFieldAttributes(fieldInfo, attributes);
                foreach (ModifyPropertyAttribute attribute in attributes)
                {
                    Type attributeType = attribute.GetType();
                    AdvancedModificatorDrawer drawer = GetModificatorDrawerFor(attributeType);
                    drawer?.ModifyProperty(property, propertyField, fieldInfo, attribute);
                }
            }
        }
    }
}
