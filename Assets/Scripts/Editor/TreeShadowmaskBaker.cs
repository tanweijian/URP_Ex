using System.IO;
using UnityEditor;
using UnityEngine;

public class TreeShadowmaskBakerEditor : EditorWindow
{
    private Light bakeLight;
    private Renderer bakeRenderer;
    public string info;

    [MenuItem("tanweijian/bake tree shadowmask")]
    public static void OpenWindow()
    {
        TreeShadowmaskBakerEditor win = GetWindow<TreeShadowmaskBakerEditor>("烘焙树阴影");
        win.showInit();
    }

    private void showInit()
    {
        bakeLight = null;
        foreach (var item in FindObjectsOfType<Light>())
        {
            if (item.type == LightType.Directional)
            {
                bakeLight = item;
                break;
            }
        }

        Show();
    }

    private void OnGUI()
    {
        bakeLight = EditorGUILayout.ObjectField("烘焙阴影的灯光", bakeLight, typeof(Light), true) as Light;
        bakeRenderer = EditorGUILayout.ObjectField("烘焙阴影的树", bakeRenderer, typeof(Renderer), true) as Renderer;
        if (GUILayout.Button("烘焙阴影"))
        {
            bakeShadowMask();
        }
        EditorGUILayout.LabelField(info);
    }

    private void bakeShadowMask()
    {
        if (bakeRenderer == null || bakeLight == null)
        {
            info = "需要设置 灯光 和 烘焙对象 参数";
            return;
        }

        // 灯光的角度保持不变，改变方向烘焙
        Vector3 rot = bakeLight.transform.forward;
        float xzLen = Mathf.Sqrt(1 - rot.y * rot.y);
        Color32[][] colors = new Color32[4][];
        GameObjectUtility.SetStaticEditorFlags(bakeRenderer.gameObject,StaticEditorFlags.ContributeGI | StaticEditorFlags.ReflectionProbeStatic);
        for (int i = 0; i < 4; i++)
        {
            rot.z = Mathf.Cos(i * Mathf.PI / 2) * xzLen;
            rot.x = Mathf.Sin(i * Mathf.PI / 2) * xzLen;
            GameObject light = new GameObject();
            light.transform.rotation = Quaternion.LookRotation(rot.normalized, Vector3.up);
            bakeLight.transform.rotation = Quaternion.LookRotation(rot.normalized, Vector3.up);
            Lightmapping.Bake();
            if ((uint)bakeRenderer.lightmapIndex > LightmapSettings.lightmaps.Length)
            {
                info = "bakeRenderer lightmap index error";
                return;
            }
            
            var maskT = LightmapSettings.lightmaps[bakeRenderer.lightmapIndex].shadowMask;
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(maskT), "tempTreeMask");
            colors[i] = maskT.GetPixels32();
        }

        var mask0 = LightmapSettings.lightmaps[0].shadowMask;
        var finalMask = new Texture2D(mask0.width, mask0.height, TextureFormat.ARGB32, false, true);
        var finalColors = finalMask.GetPixels32();
        
        for (int i = 0; i < finalColors.Length; i++)
        {
            finalColors[i].r = colors[0][i].r;
            finalColors[i].g = colors[1][i].r;
            finalColors[i].b = colors[2][i].r;
            finalColors[i].a = colors[3][i].r;
        }
        
        finalMask.SetPixels32(finalColors);
        finalMask.Apply();
        string path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(bakeRenderer.sharedMaterial.mainTexture));
        
        File.WriteAllBytes(path + "/TreeShadowMask.png", finalMask.EncodeToPNG());
        AssetDatabase.Refresh();
        info = "烘焙成功 >>>>>>>>>>>>>  " + path + " / TreeShadowMask.png";
    }
}

public class TreeShadowmaskBakerImporter : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        string fileName = Path.GetFileName(assetPath);
        TextureImporter importer = TextureImporter.GetAtPath(assetPath) as TextureImporter;

        if (fileName.StartsWith("TreeShadowMask"))
        {
            importer.sRGBTexture = false;
            importer.maxTextureSize = 256;
        }

        if (fileName.StartsWith("tempTreeMask"))
        {
            importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
        }
    }
}