using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.UI
{
    public class BlurredImage : RawImage
    {
        [SerializeField] private SpecifiedCameraType m_SpecifiedCameraType;

        private Camera specifiedCamera;
        private RTHandle captureTexture;
        private Material blitMaterial;

        [Range(0f, 5f)] [SerializeField] private float m_BlurRadius = 0f;
        [Range(1, 10)] [SerializeField] private int m_Iteration = 2;
        [Range(1, 8)] [SerializeField] private int m_DownScaling = 2;

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
                UniversalAdditionalCameraData baseCameraAdditionalCameraData =
                    mainCamera.GetComponent<UniversalAdditionalCameraData>();
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
                // CameraCaptureBridge.AddCaptureAction(specifiedCamera, CaptureAction);
                blitMaterial = CoreUtils.CreateEngineMaterial("Hidden/Universal Render Pipeline/Blit");
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
                // CameraCaptureBridge.RemoveCaptureAction(specifiedCamera, CaptureAction);
                specifiedCamera = null;
                CoreUtils.Destroy(blitMaterial);
                blitMaterial = null;
            }
        }

        private void CaptureAction(RTHandle cameraColorHandle, ScriptableRenderContext context, CommandBuffer cmd)
        {
            cmd.Clear();
            RenderTextureDescriptor descriptor = cameraColorHandle.rt.descriptor;
            descriptor.msaaSamples = 1;
            descriptor.graphicsFormat = GraphicsFormat.B8G8R8A8_UNorm;
            RenderingUtils.ReAllocateIfNeeded(ref captureTexture, descriptor, name: "BlurredTexture");
            using (new ProfilingScope(cmd, new ProfilingSampler("BlurredCapture")))
            {
                Blitter.BlitTexture(cmd, cameraColorHandle, captureTexture, blitMaterial, 0);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
            }
        }
    }
}