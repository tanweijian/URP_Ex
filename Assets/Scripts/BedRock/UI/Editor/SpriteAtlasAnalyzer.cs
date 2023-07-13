using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.U2D;
using UnityEngine.U2D;
using System.Collections.Generic;

namespace BedRockEditor.UI
{
    public static class SpriteAtlasAnalyzer
    {
        internal static void Analyze(string atlasOutputPath, IEnumerable<string> samples)
        {
            var spriteCounter = new Dictionary<Sprite, int>();
            var spriteRecorder = new Dictionary<string, List<Sprite>>();
            foreach (string sample in samples)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(sample);
                if (prefab == null)
                {
                    continue;
                }

                var list = new List<Sprite>();
                var images = prefab.GetComponentsInChildren<Image>(true);
                foreach (Image image in images)
                {
                    Sprite sprite = image.sprite;
                    if (sprite != null && !list.Contains(sprite))
                    {
                        if (spriteCounter.ContainsKey(sprite))
                        {
                            spriteCounter[sprite]++;
                        }
                        else
                        {
                            spriteCounter.Add(sprite, 1);
                        }
                        list.Add(sprite);
                    }
                }
                spriteRecorder.Add(prefab.name, list);
            }

            foreach (var recorder in spriteRecorder)
            {
                string atlasPath = $"{atlasOutputPath}/{recorder.Key}.spriteatlasv2";
                SpriteAtlasAsset atlasAsset = SpriteAtlasAsset.Load(atlasPath);
                if (atlasAsset == null)
                {
                    atlasAsset = CreateNewSpriteAtlasAsset();
                }
                var sprites = recorder.Value.ToArray() as Object[];
                atlasAsset.Add(sprites);
                SpriteAtlasAsset.Save(atlasAsset, atlasPath);
            }
            AssetDatabase.Refresh();
        }

        private static SpriteAtlasAsset CreateNewSpriteAtlasAsset()
        {
            SpriteAtlasAsset atlasAsset = new SpriteAtlasAsset();
            SpriteAtlas masterAtlas = new SpriteAtlas();
            atlasAsset.SetMasterAtlas(masterAtlas);
            return atlasAsset;
        }
        
        [MenuItem("tanweijian/test")]
        private static void Test()
        {
            Analyze("Assets/ArtAssets/UI/Atlas", new[] { "Assets/ArtAssets/UI/Prefab/UITestView.prefab" });
        }
    }
}