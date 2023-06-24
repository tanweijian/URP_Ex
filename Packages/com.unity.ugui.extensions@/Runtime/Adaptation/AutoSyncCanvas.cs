using System;

namespace UnityEngine.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(RectTransform))]
    public class AutoSyncCanvas : MonoBehaviour
    {
        private enum RenderMode
        {
            Inherit,
            World
        }

        [SerializeField] private RenderMode m_RenderMode;

        private Canvas canvas;
        private RectTransform rectTransform;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            SyncRectTransform();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Awake();
            SyncRectTransform();
        }

        private void Update()
        {
            SyncRectTransform();
        }
#endif

        private void SyncRectTransform()
        {
            Canvas rootCanvas = canvas.rootCanvas;
            if (rootCanvas == null)
            {
                return;
            }
            RectTransform rootRect = rootCanvas.GetComponent<RectTransform>();
            switch (m_RenderMode)
            {
                case RenderMode.Inherit:
                    rectTransform.position = rootRect.position;
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                    rectTransform.localScale = Vector3.one;
                    break;
                case RenderMode.World:
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    float rootScale = rootRect.localScale.x;
                    float scale = 0.01f / rootScale;
                    rectTransform.localScale = new Vector3(scale, scale, scale);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}