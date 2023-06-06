namespace UnityEngine.UI
{
    public class DummyGraphic : Graphic
    {
        protected DummyGraphic()
        {
            useLegacyMeshGeneration = false;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}
