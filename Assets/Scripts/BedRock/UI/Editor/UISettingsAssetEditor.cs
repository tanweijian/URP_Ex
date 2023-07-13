using UnityEditor;
using UnityEngine;
using BedRockRuntime.UI;

namespace BedRockEditor.UI
{
    [CustomEditor(typeof(UISettingsAsset))]
    public class UISettingsAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Setting"))
            {
                EditorPrefs.SetString(PrefsKey.UISettingsAseetPath, AssetDatabase.GetAssetPath(target));
                UISettingsEditor.Open();
            }
        }
    }
}