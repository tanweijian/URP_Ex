using UnityEditor;

namespace BedRockEditor.UI
{
    public static class MenuOptions
    {
        [MenuItem("Window/BedRock/UI Settings")]
        private static void OpenUISettingsWindow()
        {
            UISettingsEditor.Open();
        }
    }
}