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
            return root;
        }
    }
}
