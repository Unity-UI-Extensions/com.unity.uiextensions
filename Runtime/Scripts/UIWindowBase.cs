/// Credit GXMark, alexzzzz, CaoMengde777, TroyDavis  
/// Original Sourced from (GXMark) - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-1834806 (with corrections)
/// Scaling fixed for non overlay canvases (alexzzzz) - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/#post-1780612
/// Canvas border fix (CaoMengde777) - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/#post-1781658
/// Canvas reset position fix (TroyDavis)- http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/#post-1782214


using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Includes a few fixes of my own, mainly to tidy up duplicates, remove unneeded stuff and testing. (nothing major, all the crew above did the hard work!)
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Extensions/UI Window Base")]
    public class UIWindowBase : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private bool _isDragging = false;
        public static bool ResetCoords = false;
        private Vector3 m_originalCoods = Vector3.zero;
        private Canvas m_canvas;
        private RectTransform m_canvasRectTransform;

        [Tooltip("Number of pixels of the window that must stay inside the canvas view.")]
        public int KeepWindowInCanvas = 5;

        [Tooltip("The transform that is moved when dragging, can be left empty in which case its own transform is used.")]
        public RectTransform RootTransform = null;

        // Use this for initialization
        void Start()
        {
            if (RootTransform == null)
            {
                RootTransform = GetComponent<RectTransform>();
            }

            m_originalCoods = RootTransform.position;
            m_canvas = GetComponentInParent<Canvas>();
            m_canvasRectTransform = m_canvas.GetComponent<RectTransform>();
        }

        void Update()
        {
            if (ResetCoords)
                resetCoordinatePosition();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isDragging)
            {
                var delta = ScreenToCanvas(eventData.position) - ScreenToCanvas(eventData.position - eventData.delta);
                RootTransform.localPosition += delta;
            }
        }

        //Note, the begin drag and end drag aren't actually needed to control the drag.  However, I'd recommend keeping it in case you want to do something else when dragging starts and stops
        public void OnBeginDrag(PointerEventData eventData)
        {

            if (eventData.pointerCurrentRaycast.gameObject == null)
                return;

            if (eventData.pointerCurrentRaycast.gameObject.name == name)
            {
                _isDragging = true;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;
        }

        void resetCoordinatePosition()
        {
            RootTransform.position = m_originalCoods;
            ResetCoords = false;
        }

        private Vector3 ScreenToCanvas(Vector3 screenPosition)
        {
            Vector3 localPosition;
            Vector2 min;
            Vector2 max;
            var canvasSize = m_canvasRectTransform.sizeDelta;

            if (m_canvas.renderMode == RenderMode.ScreenSpaceOverlay || (m_canvas.renderMode == RenderMode.ScreenSpaceCamera && m_canvas.worldCamera == null))
            {
                localPosition = screenPosition;

                min = Vector2.zero;
                max = canvasSize;
            }
            else
            {
                var ray = m_canvas.worldCamera.ScreenPointToRay(screenPosition);
                var plane = new Plane(m_canvasRectTransform.forward, m_canvasRectTransform.position);

                float distance;
                if (plane.Raycast(ray, out distance) == false)
                {
                    throw new Exception("Is it practically possible?");
                };
                var worldPosition = ray.origin + ray.direction * distance;
                localPosition = m_canvasRectTransform.InverseTransformPoint(worldPosition);

                min = -Vector2.Scale(canvasSize, m_canvasRectTransform.pivot);
                max = Vector2.Scale(canvasSize, Vector2.one - m_canvasRectTransform.pivot);
            }

            // keep window inside canvas
            localPosition.x = Mathf.Clamp(localPosition.x, min.x + KeepWindowInCanvas, max.x - KeepWindowInCanvas);
            localPosition.y = Mathf.Clamp(localPosition.y, min.y + KeepWindowInCanvas, max.y - KeepWindowInCanvas);

            return localPosition;
        }
    }
}