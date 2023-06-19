using UnityEditor;

namespace BedRockEditor.AssetProcess
{
    public static class MenuOptions
    {
        [MenuItem("ToolMenu(BedRock)/Model/Mesh Export")]
        static void ModelMeshExport()
        {
            AssetProcess.ModelMeshExport.MeshExportSelected();
        }
    }
}
