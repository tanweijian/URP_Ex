using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(RectTransform))]
public class SyncCanvasSize : MonoBehaviour
{
    public Canvas mAssociatedCanvas;
    
    private Camera associatedPerspectiveCamera;
    private RectTransform associatedRectTran;
    
    private Canvas synchronousCanvas;
    private Camera synchronousCamera;
    private RectTransform synchronousRectTran;

#if UNITY_EDITOR
    
    private void OnValidate()
    {
        if (mAssociatedCanvas == null)
        {
            return;
        }

        if (mAssociatedCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            mAssociatedCanvas = null;
            return;
        }

        if (mAssociatedCanvas.worldCamera != null && !mAssociatedCanvas.worldCamera.orthographic)
        {
            associatedPerspectiveCamera = mAssociatedCanvas.worldCamera;
        }

        if (mAssociatedCanvas != null)
        {
            associatedRectTran = mAssociatedCanvas.GetComponent<RectTransform>();
        }
    }

    private void Update()
    {
        if (mAssociatedCanvas == null || associatedPerspectiveCamera == null)
        {
            return;
        }

        if (synchronousCanvas == null)
        {
            synchronousCanvas = GetComponent<Canvas>();
            synchronousRectTran = GetComponent<RectTransform>();
            synchronousCamera = synchronousCanvas.worldCamera;
        }

        if (synchronousCamera == null)
        {
            return;
        }

        RenderMode renderMode= synchronousCanvas.renderMode;
        if (renderMode == RenderMode.ScreenSpaceCamera)
        {
            float distance = mAssociatedCanvas.planeDistance;
            synchronousCanvas.planeDistance = distance;
            float orthographicSize = distance * Mathf.Tan(Mathf.Deg2Rad * associatedPerspectiveCamera.fieldOfView * 0.5f);
            synchronousCamera.orthographicSize = orthographicSize;
        }
        else if (renderMode == RenderMode.WorldSpace)
        {
            synchronousRectTran.position = associatedRectTran.position;
            synchronousRectTran.sizeDelta = new Vector2(associatedRectTran.rect.width * associatedRectTran.localScale.x / 0.01f, associatedRectTran.rect.height * associatedRectTran.localScale.y / 0.01f);
            synchronousRectTran.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }
    }
    
#endif
}