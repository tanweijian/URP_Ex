using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomPropertyDrawer(typeof(DisplayNameAttr))]
    public class DisplayNameAttrDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is DisplayNameAttr displayNameAttr && !string.IsNullOrEmpty(displayNameAttr.Name))
            {
                label.text = displayNameAttr.Name;
            }
            EditorGUI.PropertyField(position, property, label);
        }
    }
}