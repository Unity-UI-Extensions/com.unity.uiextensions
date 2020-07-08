/// Credit .entity
/// Sourced from - http://forum.unity3d.com/threads/rescale-panel.309226/

using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/RescalePanels/RescaleDragPanel")]
    public class RescaleDragPanel : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        private Vector2 pointerOffset;
        private RectTransform canvasRectTransform;
        private RectTransform panelRectTransform;

        private Transform goTransform;

        void Awake()
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvasRectTransform = canvas.transform as RectTransform;
                panelRectTransform = transform.parent as RectTransform;
                goTransform = transform.parent;
            }
        }

        public void OnPointerDown(PointerEventData data)
        {
            panelRectTransform.SetAsLastSibling();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, data.position, data.pressEventCamera, out pointerOffset);
        }

        public void OnDrag(PointerEventData data)
        {
            if (panelRectTransform == null)
                return;

            Vector2 pointerPosition = ClampToWindow(data);

            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform, pointerPosition, data.pressEventCamera, out localPointerPosition
                ))
            {
                panelRectTransform.localPosition = localPointerPosition - new Vector2(pointerOffset.x * goTransform.localScale.x, pointerOffset.y * goTransform.localScale.y);
            }
        }

        Vector2 ClampToWindow(PointerEventData data)
        {
            Vector2 rawPointerPosition = data.position;

            Vector3[] canvasCorners = new Vector3[4];
            canvasRectTransform.GetWorldCorners(canvasCorners);

            float clampedX = Mathf.Clamp(rawPointerPosition.x, canvasCorners[0].x, canvasCorners[2].x);
            float clampedY = Mathf.Clamp(rawPointerPosition.y, canvasCorners[0].y, canvasCorners[2].y);

            Vector2 newPointerPosition = new Vector2(clampedX, clampedY);
            return newPointerPosition;
        }
    }
}