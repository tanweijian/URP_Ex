using UnityEditor;

namespace BedRockEditor.UI
{
    public static class MenuOptions
    {
        [MenuItem("Menu(BedRock)/UI/Settings")]
        private static void OpenUISettingsWindow()
        {
            UISettingsEditor.Open();
        }
    }
}