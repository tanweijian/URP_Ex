using UnityEditor;

namespace BedRockEditor.AssetProcess
{
    public static class MenuOptions
    {
        [MenuItem("Menu(BedRock)/Model/Mesh Export")]
        static void ModelMeshExport()
        {
            AssetProcess.ModelMeshExport.MeshExportSelected();
        }
    }
}
