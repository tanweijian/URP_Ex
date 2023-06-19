using UnityEngine;

namespace BedRockRuntime.UI
{
    [DefaultExecutionOrder(-999)]
    public class UIRootBinder : MonoBehaviour
    {
        public Camera mUICamera;
        public Canvas mRootCanvas;
        public Canvas mHUDCanvas;
        public Canvas mMainCanvas;
        public Canvas mSecondaryCanvas;
        public Canvas mPopupCanvas;
        public Canvas mOverlayCanvas;

        private void Awake()
        {
            UIRoot.SUICamera = mUICamera;
            UIRoot.SRootCanvas = mRootCanvas;
            UIRoot.SHUDCanvas = mHUDCanvas;
            UIRoot.SMainCanvas = mMainCanvas;
            UIRoot.SSecondaryCanvas = mSecondaryCanvas;
            UIRoot.SPopupCanvas = mPopupCanvas;
            UIRoot.SOverlayCanvas = mOverlayCanvas;
        }
    }
}
