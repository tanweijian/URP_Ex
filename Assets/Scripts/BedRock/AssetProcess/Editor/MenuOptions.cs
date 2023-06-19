using UnityEditor;

namespace BedRockEditor.AssetProcess
{
    public static class MenuOptions
    {
        [MenuItem("BedRockTool/Model/Mesh Export")]
        static void ModelMeshExport()
        {
            AssetProcess.ModelMeshExport.MeshExportSelected();
        }
    }
}
