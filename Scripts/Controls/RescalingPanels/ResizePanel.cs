/// Credit .entity
/// Sourced from - http://forum.unity3d.com/threads/rescale-panel.309226/

using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/RescalePanels/ResizePanel")]
    public class ResizePanel : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        public Vector2 minSize;
        public Vector2 maxSize;

        private RectTransform rectTransform;
        private Vector2 currentPointerPosition;
        private Vector2 previousPointerPosition;

        private float ratio;


        void Awake()
        {
            rectTransform = transform.parent.GetComponent<RectTransform>();
            float originalWidth;
            float originalHeight;
            originalWidth = rectTransform.rect.width;
            originalHeight = rectTransform.rect.height;
            ratio = originalHeight / originalWidth;
            minSize = new Vector2(0.1f * originalWidth, 0.1f * originalHeight);
            maxSize = new Vector2(10f * originalWidth, 10f * originalHeight);
        }

        public void OnPointerDown(PointerEventData data)
        {
            rectTransform.SetAsLastSibling();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out previousPointerPosition);
        }

        public void OnDrag(PointerEventData data)
        {
            if (rectTransform == null)
                return;

            Vector2 sizeDelta = rectTransform.sizeDelta;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out currentPointerPosition);
            Vector2 resizeValue = currentPointerPosition - previousPointerPosition;

            sizeDelta += new Vector2(resizeValue.x, ratio * resizeValue.x);
            sizeDelta = new Vector2(
                Mathf.Clamp(sizeDelta.x, minSize.x, maxSize.x),
                Mathf.Clamp(sizeDelta.y, minSize.y, maxSize.y)
                );

            rectTransform.sizeDelta = sizeDelta;

            previousPointerPosition = currentPointerPosition;
        }
    }
}