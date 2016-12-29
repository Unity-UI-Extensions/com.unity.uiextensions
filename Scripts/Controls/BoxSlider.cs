///Credit judah4
///Sourced from - http://forum.unity3d.com/threads/color-picker.267043/

using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Extensions/BoxSlider")]
    public class BoxSlider : Selectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
    {
        public enum Direction
        {
            LeftToRight,
            RightToLeft,
            BottomToTop,
            TopToBottom,
        }

        [Serializable]
        public class BoxSliderEvent : UnityEvent<float, float> { }

        [SerializeField]
        private RectTransform m_HandleRect;
        public RectTransform HandleRect { get { return m_HandleRect; } set { if (SetClass(ref m_HandleRect, value)) { UpdateCachedReferences(); UpdateVisuals(); } } }

        [Space(6)]

        [SerializeField]
        private float m_MinValue = 0;
        public float MinValue { get { return m_MinValue; } set { if (SetStruct(ref m_MinValue, value)) { SetX(m_ValueX); SetY(m_ValueY); UpdateVisuals(); } } }

        [SerializeField]
        private float m_MaxValue = 1;
        public float MaxValue { get { return m_MaxValue; } set { if (SetStruct(ref m_MaxValue, value)) { SetX(m_ValueX); SetY(m_ValueY); UpdateVisuals(); } } }

        [SerializeField]
        private bool m_WholeNumbers = false;
        public bool WholeNumbers { get { return m_WholeNumbers; } set { if (SetStruct(ref m_WholeNumbers, value)) { SetX(m_ValueX); SetY(m_ValueY); UpdateVisuals(); } } }

        [SerializeField]
        private float m_ValueX = 1f;
        public float ValueX
        {
            get
            {
                if (WholeNumbers)
                    return Mathf.Round(m_ValueX);
                return m_ValueX;
            }
            set
            {
                SetX(value);
            }
        }

        public float NormalizedValueX
        {
            get
            {
                if (Mathf.Approximately(MinValue, MaxValue))
                    return 0;
                return Mathf.InverseLerp(MinValue, MaxValue, ValueX);
            }
            set
            {
                this.ValueX = Mathf.Lerp(MinValue, MaxValue, value);
            }
        }

        [SerializeField]
        private float m_ValueY = 1f;
        public float ValueY
        {
            get
            {
                if (WholeNumbers)
                    return Mathf.Round(m_ValueY);
                return m_ValueY;
            }
            set
            {
                SetY(value);
            }
        }

        public float NormalizedValueY
        {
            get
            {
                if (Mathf.Approximately(MinValue, MaxValue))
                    return 0;
                return Mathf.InverseLerp(MinValue, MaxValue, ValueY);
            }
            set
            {
                this.ValueY = Mathf.Lerp(MinValue, MaxValue, value);
            }
        }

        [Space(6)]

        // Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
        [SerializeField]
        private BoxSliderEvent m_OnValueChanged = new BoxSliderEvent();
        public BoxSliderEvent OnValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }

        // Private fields

        private Transform m_HandleTransform;
        private RectTransform m_HandleContainerRect;

        // The offset from handle position to mouse down position
        private Vector2 m_Offset = Vector2.zero;

        private DrivenRectTransformTracker m_Tracker;

        // Size of each step.
        float StepSize { get { return WholeNumbers ? 1 : (MaxValue - MinValue) * 0.1f; } }

        protected BoxSlider()
        { }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (WholeNumbers)
            {
                m_MinValue = Mathf.Round(m_MinValue);
                m_MaxValue = Mathf.Round(m_MaxValue);
            }
            UpdateCachedReferences();
            SetX(m_ValueX, false);
            SetY(m_ValueY, false);
            // Update rects since other things might affect them even if value didn't change.
            UpdateVisuals();

            var prefabType = UnityEditor.PrefabUtility.GetPrefabType(this);
            if (prefabType != UnityEditor.PrefabType.Prefab && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

#endif // if UNITY_EDITOR

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                OnValueChanged.Invoke(ValueX, ValueY);
#endif
        }

        public void LayoutComplete()
        {

        }

        public void GraphicUpdateComplete()
        {

        }

        public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return false;

            currentValue = newValue;
            return true;
        }

        public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (currentValue.Equals(newValue))
                return false;

            currentValue = newValue;
            return true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateCachedReferences();
            SetX(m_ValueX, false);
            SetY(m_ValueY, false);
            // Update rects since they need to be initialized correctly.
            UpdateVisuals();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            base.OnDisable();
        }

        void UpdateCachedReferences()
        {

            if (m_HandleRect)
            {
                m_HandleTransform = m_HandleRect.transform;
                if (m_HandleTransform.parent != null)
                    m_HandleContainerRect = m_HandleTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                m_HandleContainerRect = null;
            }
        }

        // Set the valueUpdate the visible Image.
        void SetX(float input)
        {
            SetX(input, true);
        }

        void SetX(float input, bool sendCallback)
        {
            // Clamp the input
            float newValue = Mathf.Clamp(input, MinValue, MaxValue);
            if (WholeNumbers)
                newValue = Mathf.Round(newValue);

            // If the stepped value doesn't match the last one, it's time to update
            if (m_ValueX == newValue)
                return;

            m_ValueX = newValue;
            UpdateVisuals();
            if (sendCallback)
                m_OnValueChanged.Invoke(newValue, ValueY);
        }

        void SetY(float input)
        {
            SetY(input, true);
        }

        void SetY(float input, bool sendCallback)
        {
            // Clamp the input
            float newValue = Mathf.Clamp(input, MinValue, MaxValue);
            if (WholeNumbers)
                newValue = Mathf.Round(newValue);

            // If the stepped value doesn't match the last one, it's time to update
            if (m_ValueY == newValue)
                return;

            m_ValueY = newValue;
            UpdateVisuals();
            if (sendCallback)
                m_OnValueChanged.Invoke(ValueX, newValue);
        }


        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            UpdateVisuals();
        }

        enum Axis
        {
            Horizontal = 0,
            Vertical = 1
        }


        // Force-update the slider. Useful if you've changed the properties and want it to update visually.
        private void UpdateVisuals()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateCachedReferences();
#endif

            m_Tracker.Clear();


            //to business!
            if (m_HandleContainerRect != null)
            {
                m_Tracker.Add(this, m_HandleRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;
                anchorMin[0] = anchorMax[0] = (NormalizedValueX);
                anchorMin[1] = anchorMax[1] = (NormalizedValueY);

                m_HandleRect.anchorMin = anchorMin;
                m_HandleRect.anchorMax = anchorMax;
            }
        }

        // Update the slider's position based on the mouse.
        void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = m_HandleContainerRect;
            if (clickRect != null && clickRect.rect.size[0] > 0)
            {
                Vector2 localCursor;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, eventData.position, cam, out localCursor))
                    return;
                localCursor -= clickRect.rect.position;

                float val = Mathf.Clamp01((localCursor - m_Offset)[0] / clickRect.rect.size[0]);
                NormalizedValueX = (val);

                float valY = Mathf.Clamp01((localCursor - m_Offset)[1] / clickRect.rect.size[1]);
                NormalizedValueY = (valY);

            }
        }

        private bool CanDrag(PointerEventData eventData)
        {
            return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!CanDrag(eventData))
                return;

            base.OnPointerDown(eventData);

            m_Offset = Vector2.zero;
            if (m_HandleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(m_HandleRect, eventData.position, eventData.enterEventCamera))
            {
                Vector2 localMousePos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandleRect, eventData.position, eventData.pressEventCamera, out localMousePos))
                    m_Offset = localMousePos;
                m_Offset.y = -m_Offset.y;
            }
            else
            {
                // Outside the slider handle - jump to this point instead
                UpdateDrag(eventData, eventData.pressEventCamera);
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!CanDrag(eventData))
                return;

            UpdateDrag(eventData, eventData.pressEventCamera);
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

    }
}
