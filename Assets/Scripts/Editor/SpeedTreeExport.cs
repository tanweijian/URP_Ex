using System.IO;
using UnityEditor;
using UnityEngine;

public static class SpeedTreeExport
{
    public static void ExportAssets(GameObject speedTree, string outputPath)
    {
        AssetDatabase.StartAssetEditing();

        outputPath += $"/{speedTree.name}";
        Directory.CreateDirectory(outputPath);

        LODGroup lodGroup = speedTree.GetComponent<LODGroup>();
        int lodCount = lodGroup.lodCount;
        GameObject prefab = new GameObject(speedTree.name);
        LODGroup prefabLODGroup =  prefab.AddComponent<LODGroup>();
        prefabLODGroup.fadeMode = LODFadeMode.SpeedTree;
        prefabLODGroup.animateCrossFading = true;
        prefabLODGroup.lastLODBillboard = true;
        GameObject lodNode = new GameObject();
        lodNode.AddComponent<MeshFilter>();
        lodNode.AddComponent<MeshRenderer>();
        
        var lods = lodGroup.GetLODs();
        for (int i = 0; i < lodGroup.lodCount; i++)
        {
            LOD lod = lods[i];
            GameObject node = Object.Instantiate(lodNode, prefab.transform);
            node.name = $"{speedTree.name}_LOD{i}";
        }

        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }

    [MenuItem("tanweijian/test")]
    public static void Test()
    {
        GameObject sclected = Selection.activeGameObject;
        ExportAssets(sclected, "Assets/UAssets/Models/Tree");
    }
}
