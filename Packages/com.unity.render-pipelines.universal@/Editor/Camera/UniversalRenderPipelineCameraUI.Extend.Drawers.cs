using UnityEngine.Rendering.Universal;

namespace UnityEditor.Rendering.Universal
{
    using CED = CoreEditorDrawer<UniversalRenderPipelineSerializedCamera>;

    static partial class UniversalRenderPipelineCameraUI
    {
        public partial class Extend
        {
            public static readonly CED.IDrawer Drawer;

            static Extend()
            {
                Drawer = CED.Conditional(
                    (serialized, owner) => (CameraRenderType)serialized.cameraType.intValue == CameraRenderType.Overlay,
                    CED.FoldoutGroup(Styles.headerTr, Expandable.Extend, k_ExpandedState, FoldoutOption.Indent,
                        CED.Group(DrawerForceGammaColorSpaceOption))
                );
            }

            private static void DrawerForceGammaColorSpaceOption(UniversalRenderPipelineSerializedCamera p, Editor owner)
            {
                EditorGUILayout.PropertyField(p.renderingColorSpaceGamma, Styles.RenderingColorSpaceGammaTr);
            }
        }
    }
}
