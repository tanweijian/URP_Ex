namespace UnityEngine.Rendering.Universal.Internal
{
    public class BlitPass : ScriptableRenderPass
    {
        private RTHandle m_Source;
        private RTHandle m_Destination;

        private readonly PassData m_PassData;

        public BlitPass( string profilerTag, RenderPassEvent evt, Material blitMaterial, bool lieartosrgb, bool srgbtolinear)
        {
            profilingSampler = new ProfilingSampler(profilerTag);
            m_PassData = new PassData
            {
                blitMaterial = blitMaterial,
                enableLinearToSrgbKeyword = lieartosrgb,
                enableSrgbToLinearKeyword = srgbtolinear
            };

            renderPassEvent = evt;
            useNativeRenderPass = false;
        }

        public void Setup(RTHandle inSource, RTHandle inDestination)
        {
            m_Source = inSource;
            m_Destination = inDestination;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            ConfigureTarget(m_Destination, k_CameraTarget);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            m_PassData.cmd = renderingData.commandBuffer;
            ExecutePass(context, m_PassData, m_Source);
        }

        private void ExecutePass(ScriptableRenderContext context, PassData passData, RTHandle source)
        {
            CommandBuffer cmd = passData.cmd;
            Material blitMaterial = passData.blitMaterial;
            
            using (new ProfilingScope(cmd, profilingSampler))
            {
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.LinearToSRGBConversion, passData.enableLinearToSrgbKeyword);
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.SRGBToLinearConversion, passData.enableSrgbToLinearKeyword);
                Vector2 viewportScale = source.useScaling ? new Vector2(source.rtHandleProperties.rtHandleScale.x, source.rtHandleProperties.rtHandleScale.y) : Vector2.one;
                Blitter.BlitTexture(cmd, source, viewportScale, blitMaterial, 0);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                
                // clear up
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.LinearToSRGBConversion, false);
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.SRGBToLinearConversion, false);
            }
        }

        private class PassData
        {
            internal CommandBuffer cmd;
            internal Material blitMaterial;
            internal bool enableLinearToSrgbKeyword;
            internal bool enableSrgbToLinearKeyword;
        }
    }
}
