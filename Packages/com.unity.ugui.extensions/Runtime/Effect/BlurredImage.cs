using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.UI
{
    public class BlurredImage : RawImage
    {
        [SerializeField] private SpecifiedCameraType m_SpecifiedCameraType;

        private CustomCapturePass _customCapturePass = new("BlurredCapture", RenderPassEvent.AfterRendering + 1);

        private Camera specifiedCamera;
        private ScriptableRenderer _scriptableRenderer;
        
        private RTHandle captureTexture;
        
        public enum SpecifiedCameraType
        {
            MainCamera,
            InFrontOfUICamera,
            UICamera,
            LastCameraInStack,
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return;
            }
            
            if (m_SpecifiedCameraType == SpecifiedCameraType.MainCamera)
            {
                specifiedCamera = mainCamera;
            }
            else
            {
                UniversalAdditionalCameraData baseCameraAdditionalCameraData = mainCamera.GetComponent<UniversalAdditionalCameraData>();
                var cameraStack = baseCameraAdditionalCameraData.cameraStack;
                if (m_SpecifiedCameraType == SpecifiedCameraType.LastCameraInStack)
                {
                    specifiedCamera = cameraStack.Last((cam) => cam.isActiveAndEnabled);
                }
                else
                {
                    for (int i = cameraStack.Count - 1; i >= 0; i--)
                    {
                        Camera cam = cameraStack[i];
                        if (cam.CompareTag("UICamera"))
                        {
                            if (m_SpecifiedCameraType == SpecifiedCameraType.UICamera)
                            {
                                specifiedCamera = cam;
                            }
                            else if (m_SpecifiedCameraType == SpecifiedCameraType.InFrontOfUICamera)
                            {
                                specifiedCamera = cameraStack[i - 1];
                            }
                            break;
                        }
                    }
                }
            }

            if (specifiedCamera != null)
            {
                UniversalAdditionalCameraData additionalCameraData = specifiedCamera.GetComponent<UniversalAdditionalCameraData>();
                _scriptableRenderer = additionalCameraData.scriptableRenderer;
                _customCapturePass.Setup("_CaptureTexture");
                RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            }
            else
            {
                Debug.LogError("BlurredImage: Can not find the specified camera！");
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (specifiedCamera != null)
            {
                RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
                specifiedCamera = null;
            }
        }

        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
        {
            if (_scriptableRenderer != null && specifiedCamera != null && specifiedCamera == cam)
            {
                _scriptableRenderer.EnqueuePass(_customCapturePass);
            }
        }
    }

    internal class CustomCapturePass : ScriptableRenderPass
    {
        private string _destinationName;
        private RTHandle _destinationTex;

        public CustomCapturePass(string profilingID, RenderPassEvent evt)
        {
            profilingSampler = new ProfilingSampler(profilingID);
            renderPassEvent = evt;
        }

        public void Setup(string destinationName)
        {
            _destinationName = destinationName;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.msaaSamples = 1;
            descriptor.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref _destinationTex, descriptor, FilterMode.Point, name: _destinationName);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            cmd.Clear();
            //using (new ProfilingScope(cmd, profilingSampler))
            {
                RTHandle sourceHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
                cmd.Blit(sourceHandle.nameID, _destinationTex.nameID);
                // Blitter.BlitTexture(cmd, sourceHandle, _destinationTex, null, 0);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
            }
            CommandBufferPool.Release(cmd);
        }
    }
}