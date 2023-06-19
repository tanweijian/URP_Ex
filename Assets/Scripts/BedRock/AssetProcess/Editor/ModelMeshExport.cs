using System.IO;
using UnityEditor;
using UnityEngine;

namespace BedRockEditor.AssetProcess
{
    public static class ModelMeshExport
    {
        internal static void MeshExportSelected()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected != null)
            {
                MeshExport(selected);
            }
        }
        
        public static void MeshExport(GameObject model)
        {
            string path = AssetDatabase.GetAssetPath(model);
            int lastDelimiter = path.LastIndexOf('/');
            string savingPath = path[..(lastDelimiter + 1)] + "Mesh";
            if (!Directory.Exists(savingPath))
            {
                Directory.CreateDirectory(savingPath);
                AssetDatabase.ImportAsset(savingPath, ImportAssetOptions.ForceUpdate);
            }
            AssetDatabase.StartAssetEditing();
            var filters = model.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter filter in filters)
            {
                if (filter == null || filter.sharedMesh == null)
                {
                    continue;
                }
                Mesh @new = Object.Instantiate(filter.sharedMesh);
                AssetDatabase.CreateAsset(@new, savingPath + $"/{filter.sharedMesh.name}.mesh");
            }
            AssetDatabase.StopAssetEditing();
            AssetDatabase.ImportAsset(savingPath, ImportAssetOptions.ImportRecursive);
        }
    }
}
