namespace UnityEngine.Rendering.Universal
{
    /// <summary>
    /// Let customizable actions inject commands to capture the camera output.
    ///
    /// You can use this pass to inject capture commands into a command buffer
    /// with the goal of having camera capture happening in external code.
    /// </summary>
    internal class CapturePass : ScriptableRenderPass
    {
// extensions modify begin;
        // RTHandle m_CameraColorHandle;
        // const string m_ProfilerTag = "Capture Pass";
        // private static readonly ProfilingSampler m_ProfilingSampler = new ProfilingSampler(m_ProfilerTag);
// extensions modify end;

        public CapturePass(RenderPassEvent evt)
        {
            base.profilingSampler = new ProfilingSampler(nameof(CapturePass));
            renderPassEvent = evt;
        }

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
// extensions modify begin;
            CommandBuffer cmd = renderingData.commandBuffer;

            RTHandle cameraColorHandle = renderingData.cameraData.renderer.GetCameraColorBackBuffer(cmd);

            using (new ProfilingScope(cmd, profilingSampler))
            {
                var captureActions = renderingData.cameraData.captureActions;
                for (captureActions.Reset(); captureActions.MoveNext();)
                    captureActions.Current?.Invoke(cameraColorHandle, context, cmd);
            }
// extensions modify end;
        }
    }
}
