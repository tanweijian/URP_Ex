using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace BedRockEditor.UI
{
    public static class SpriteAtlasAnalyzer
    {
        public static void Analyze(string atlasOutputPath, string[] samples)
        {
            foreach (string sample in samples)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(sample);
                if (prefab == null)
                {
                    continue;
                }
                
                var images = prefab.GetComponentsInChildren<Image>(true);
                foreach (Image image in images)
                {
                    if (image.sprite != null)
                    {
                    }
                }

                // SpritePackerMode packerMode = EditorSettings.spritePackerMode;
                // string atlasPath = $"{atlasOutputPath}/{prefab.name}.spriteatlasv2";
                // SpriteAtlasAsset spriteAtlas = new SpriteAtlasAsset();
                // SpriteAtlasAsset.Save(spriteAtlas, atlasPath);
                // spriteAtlas = SpriteAtlasAsset.Load(atlasPath);
            }
        }
    }
}