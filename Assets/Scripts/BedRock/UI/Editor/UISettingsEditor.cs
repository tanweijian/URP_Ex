using UnityEditor;
using UnityEngine;

namespace BedRockEditor.UI
{
    public class UISettingsEditor : EditorWindow
    {
        internal static void Open()
        {
            UISettingsEditor window = GetWindow<UISettingsEditor>("UI Settings");
            window.ShowPopup();
        }

        private Vector2 scrollPosition;

        private void OnGUI()
        {
            using EditorGUILayout.ScrollViewScope scrollView = new(scrollPosition, false, false, GUILayout.Width(200), GUILayout.Height(50));
            scrollPosition = scrollView.scrollPosition;
        }
    }
}