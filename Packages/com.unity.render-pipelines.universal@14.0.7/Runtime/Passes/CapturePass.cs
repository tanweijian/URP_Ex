using System;
using System.Collections.Generic;

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
        RTHandle m_CameraColorHandle;
        const string m_ProfilerTag = "Capture Pass";
        private static readonly ProfilingSampler m_ProfilingSampler = new ProfilingSampler(m_ProfilerTag);
// extensions modify begin;
        private IEnumerator<Action<RenderTargetIdentifier, CommandBuffer>> captureActions;
// extensions modify end;
        
        public CapturePass(RenderPassEvent evt)
        {
            base.profilingSampler = new ProfilingSampler(nameof(CapturePass));
            renderPassEvent = evt;
        }

// extensions modify begin;
        public void Setup(IEnumerator<Action<RenderTargetIdentifier, CommandBuffer>> actions)
        {
            captureActions = actions;
        }
// extensions modify end;

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
// extensions modify begin;
            CommandBuffer cmdBuf = renderingData.commandBuffer;

            m_CameraColorHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;

            using (new ProfilingScope(cmdBuf, m_ProfilingSampler))
            {
                RenderTargetIdentifier colorAttachmentIdentifier = m_CameraColorHandle.nameID;
                for (captureActions.Reset(); captureActions.MoveNext();)
                {
                    captureActions.Current?.Invoke(colorAttachmentIdentifier, renderingData.commandBuffer);
                }
            }
// extensions modify end;
        }
    }
}
