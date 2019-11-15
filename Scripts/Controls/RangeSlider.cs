/// Credit Ben MacKinnon @Dover8
/// Sourced from - https://github.com/Dover8/Unity-UI-Extensions/tree/range-slider
/// Usage: Extension of the standard slider. Two handles determine a low and high value between a Min and Max. 
/// Raises a UnityEvent passing the low and high values

using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Range Slider", 34)]
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class RangeSlider : Selectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
    {

        [Serializable]
        public class RangeSliderEvent : UnityEvent<float, float> { }

        [SerializeField]
        private RectTransform m_FillRect;

        public RectTransform FillRect { get { return m_FillRect; } set { if (SetClass(ref m_FillRect, value)) { UpdateCachedReferences(); UpdateVisuals(); } } }

        [SerializeField]
        private RectTransform m_LowHandleRect;

        public RectTransform LowHandleRect { get { return m_LowHandleRect; } set { if (SetClass(ref m_LowHandleRect, value)) { UpdateCachedReferences(); UpdateVisuals(); } } }

        [SerializeField]
        private RectTransform m_HighHandleRect;

        public RectTransform HighHandleRect { get { return m_HighHandleRect; } set { if (SetClass(ref m_HighHandleRect, value)) { UpdateCachedReferences(); UpdateVisuals(); } } }

        [Space]

        [SerializeField]
        private float m_MinValue = 0;

        public float MinValue { get { return m_MinValue; } set { if (SetStruct(ref m_MinValue, value)) { SetLow(m_LowValue); SetHigh(m_HighValue); UpdateVisuals(); } } }


        [SerializeField]
        private float m_MaxValue = 1;

        public float MaxValue { get { return m_MaxValue; } set { if (SetStruct(ref m_MaxValue, value)) { SetLow(m_LowValue); SetHigh(m_HighValue); UpdateVisuals(); } } }

        [SerializeField]
        private bool m_WholeNumbers = false;

        public bool WholeNumbers { get { return m_WholeNumbers; } set { if (SetStruct(ref m_WholeNumbers, value)) { SetLow(m_LowValue); SetHigh(m_HighValue); UpdateVisuals(); } } }

        [SerializeField]
        private float m_LowValue;
        public virtual float LowValue
        {
            get
            {
                if (WholeNumbers)
                {
                    return Mathf.Round(m_LowValue);
                }

                return m_LowValue;
            }
            set
            {
                SetLow(value);
            }
        }

        public float NormalizedLowValue
        {
            get
            {
                if (Mathf.Approximately(MinValue, MaxValue))
                {
                    return 0;
                }
                return Mathf.InverseLerp(MinValue, MaxValue, LowValue);
            }
            set
            {
                this.LowValue = Mathf.Lerp(MinValue, MaxValue, value);
            }
        }


        [SerializeField]
        private float m_HighValue;
        public virtual float HighValue
        {
            get
            {
                if (WholeNumbers)
                {
                    return Mathf.Round(m_HighValue);
                }

                return m_HighValue;
            }
            set
            {
                SetHigh(value);
            }
        }

        public float NormalizedHighValue
        {
            get
            {
                if (Mathf.Approximately(MinValue, MaxValue))
                {
                    return 0;
                }
                return Mathf.InverseLerp(MinValue, MaxValue, HighValue);
            }
            set
            {
                this.HighValue = Mathf.Lerp(MinValue, MaxValue, value);
            }
        }

        /// <summary>
        /// Set the value of the slider without invoking onValueChanged callback.
        /// </summary>
        /// <param name="input">The new value for the slider.</param>
        public virtual void SetValueWithoutNotify(float low, float high)
        {
            SetLow(low, false);
            SetHigh(high, false);
        }

        [Space]

        [SerializeField]
        private RangeSliderEvent m_OnValueChanged = new RangeSliderEvent();

        public RangeSliderEvent OnValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }

        // Private fields

        /// <summary>
        /// An Enum that says in what state we and interacting with the slider
        /// </summary>
        private enum InteractionState
        {
            Low,
            High,
            Bar,
            None
        }

        private InteractionState interactionState = InteractionState.None;

        private Image m_FillImage;
        private Transform m_FillTransform;
        private RectTransform m_FillContainerRect;
        private Transform m_HighHandleTransform;
        private RectTransform m_HighHandleContainerRect;
        private Transform m_LowHandleTransform;
        private RectTransform m_LowHandleContainerRect;

        // The offset from handle position to mouse down position
        private Vector2 m_LowOffset = Vector2.zero;
        // The offset from handle position to mouse down position
        private Vector2 m_HighOffset = Vector2.zero;

        private DrivenRectTransformTracker m_Tracker;

        // This "delayed" mechanism is required for case 1037681.
        private bool m_DelayedUpdateVisuals = false;

        // Size of each step.
        float StepSize { get { return WholeNumbers ? 1 : (MaxValue - MinValue) * 0.1f; } }

        protected RangeSlider()
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

            if (IsActive())
            {
                UpdateCachedReferences();
                SetLow(m_LowValue, false);
                SetHigh(m_HighValue, false);
                //Update rects since other things might affect them even if value didn't change
                m_DelayedUpdateVisuals = true;
            }

            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
            {
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
            }
        }
#endif

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
            {
                OnValueChanged.Invoke(LowValue, HighValue);
            }
#endif
        }

        /// <summary>
        /// See ICanvasElement.LayoutComplete
        /// </summary>
        public virtual void LayoutComplete()
        { }

        /// <summary>
        /// See ICanvasElement.GraphicUpdateComplete
        /// </summary>
        public virtual void GraphicUpdateComplete()
        { }

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
            SetLow(LowValue, false);
            SetHigh(HighValue, false);
            // Update rects since they need to be initialized correctly.
            UpdateVisuals();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            base.OnDisable();
        }

        /// <summary>
        /// Update the rect based on the delayed update visuals.
        /// Got around issue of calling sendMessage from onValidate.
        /// </summary>
        protected virtual void Update()
        {
            if (m_DelayedUpdateVisuals)
            {
                m_DelayedUpdateVisuals = false;
                UpdateVisuals();
            }
        }

        protected override void OnDidApplyAnimationProperties()
        {
            base.OnDidApplyAnimationProperties();
        }

        void UpdateCachedReferences()
        {
            if (m_FillRect && m_FillRect != (RectTransform)transform)
            {
                m_FillTransform = m_FillRect.transform;
                m_FillImage = m_FillRect.GetComponent<Image>();
                if (m_FillTransform.parent != null)
                    m_FillContainerRect = m_FillTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                m_FillRect = null;
                m_FillContainerRect = null;
                m_FillImage = null;
            }

            if (m_HighHandleRect && m_HighHandleRect != (RectTransform)transform)
            {
                m_HighHandleTransform = m_HighHandleRect.transform;
                if (m_HighHandleTransform.parent != null)
                    m_HighHandleContainerRect = m_HighHandleTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                m_HighHandleRect = null;
                m_HighHandleContainerRect = null;
            }

            if (m_LowHandleRect && m_LowHandleRect != (RectTransform)transform)
            {
                m_LowHandleTransform = m_LowHandleRect.transform;
                if (m_LowHandleTransform.parent != null)
                {
                    m_LowHandleContainerRect = m_LowHandleTransform.parent.GetComponent<RectTransform>();
                }
            }
            else
            {
                m_LowHandleRect = null;
                m_LowHandleContainerRect = null;
            }
        }
        
        void SetLow(float input)
        {
            SetLow(input, true);
        }

        protected virtual void SetLow(float input, bool sendCallback)
        {
            // Clamp the input
            float newValue = Mathf.Clamp(input, MinValue, HighValue); //clamp between min and High
            if (WholeNumbers)
            {
                newValue = Mathf.Round(newValue);
            }

            // If the stepped value doesn't match the last one, it's time to update
            if (m_LowValue == newValue)
                return;

            m_LowValue = newValue;
            UpdateVisuals();
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("RangeSlider.lowValue", this);
                m_OnValueChanged.Invoke(newValue, HighValue);
            }
        }

        void SetHigh(float input)
        {
            SetHigh(input, true);
        }

        protected virtual void SetHigh(float input, bool sendCallback)
        {
            // Clamp the input
            float newValue = Mathf.Clamp(input, LowValue, MaxValue); //clamp between min and High
            if (WholeNumbers)
            {
                newValue = Mathf.Round(newValue);
            }

            // If the stepped value doesn't match the last one, it's time to update
            if (m_HighValue == newValue)
                return;

            m_HighValue = newValue;
            UpdateVisuals();
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("RangeSlider.highValue", this);
                m_OnValueChanged.Invoke(LowValue, newValue);
            }
        }


        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            //This can be invoked before OnEnabled is called. So we shouldn't be accessing other objects, before OnEnable is called.
            if (!IsActive())
                return;

            UpdateVisuals();
        }


        // Force-update the slider. Useful if you've changed the properties and want it to update visually.
        private void UpdateVisuals()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateCachedReferences();
#endif

            m_Tracker.Clear();

            if (m_FillContainerRect != null)
            {
                m_Tracker.Add(this, m_FillRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;

                //this is where some new magic must happen. Slider just uses a filled image
                //and changes the % of fill. We must move the image anchors to be between the two handles.
                anchorMin[0] = NormalizedLowValue;
                anchorMax[0] = NormalizedHighValue;

                m_FillRect.anchorMin = anchorMin;
                m_FillRect.anchorMax = anchorMax;
            }

            if (m_LowHandleContainerRect != null)
            {
                m_Tracker.Add(this, m_LowHandleRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;
                anchorMin[0] = anchorMax[0] = NormalizedLowValue;
                m_LowHandleRect.anchorMin = anchorMin;
                m_LowHandleRect.anchorMax = anchorMax;
            }

            if (m_HighHandleContainerRect != null)
            {
                m_Tracker.Add(this, m_HighHandleRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;
                anchorMin[0] = anchorMax[0] = NormalizedHighValue;
                m_HighHandleRect.anchorMin = anchorMin;
                m_HighHandleRect.anchorMax = anchorMax;
            }
        }

        // Update the slider's position based on the mouse.
        void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            //this needs to differ from slider in that we have two handles, and need to move the right one. 
            //and if it was neither handle, we will have a seperate case where both handles move uniformly 
            //moving the entire range

            //this is where we use our interationState
            switch (interactionState)
            {
                case InteractionState.Low:
                    NormalizedLowValue = CalculateDrag(eventData, cam, m_LowHandleContainerRect, m_LowOffset);
                    break;
                case InteractionState.High:
                    NormalizedHighValue = CalculateDrag(eventData, cam, m_HighHandleContainerRect, m_HighOffset);
                    break;
                case InteractionState.Bar:
                    //special case
                    CalculateBarDrag(eventData, cam);
                    break;
                case InteractionState.None:
                    break;
            }
        }

        private float CalculateDrag(PointerEventData eventData, Camera cam, RectTransform containerRect, Vector2 offset)
        { 
            RectTransform clickRect = containerRect ?? m_FillContainerRect;
            if (clickRect != null && clickRect.rect.size[0] > 0)
            {
                Vector2 localCursor;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, eventData.position, cam, out localCursor))
                {
                    return 0f;
                }
                localCursor -= clickRect.rect.position;

                float val = Mathf.Clamp01((localCursor - offset)[0] / clickRect.rect.size[0]);

                return val;
            }
            return 0;
        }

        private void CalculateBarDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = m_FillContainerRect;
            if (clickRect != null && clickRect.rect.size[0] > 0)
            {
                Vector2 localCursor;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, eventData.position, cam, out localCursor))
                {
                    return;
                }
                localCursor -= clickRect.rect.position;

                //now we need to get the delta drag on the bar
                //and move both the normalized low and high values by this amount
                //but also check that neither is going beyond the bounds
                if (NormalizedLowValue >= 0 && NormalizedHighValue <= 1)
                {
                    //find the mid point on the current bar
                    float mid = (NormalizedHighValue + NormalizedLowValue)/2;
                    //find where the new mid point should be
                    float val = Mathf.Clamp01((localCursor)[0] / clickRect.rect.size[0]);
                    //calculate the delta
                    float delta = val - mid;
                    //check the clamp range
                    if (NormalizedLowValue + delta < 0)
                    {
                        delta = -NormalizedLowValue;
                    }
                    else if (NormalizedHighValue + delta > 1)
                    {
                        delta = 1 - NormalizedHighValue;
                    }

                    //adjust both ends
                    NormalizedLowValue += delta;
                    NormalizedHighValue += delta;
                }
            }
        }

        private bool MayDrag(PointerEventData eventData)
        {
            return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;


            //HANDLE DRAG EVENTS
            m_LowOffset = m_HighOffset = Vector2.zero;
            Vector2 localMousePos;
            if (m_HighHandleRect != null && RectTransformUtility.RectangleContainsScreenPoint(m_HighHandleRect, eventData.position, eventData.enterEventCamera))
            {
                //dragging the high value handle
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HighHandleRect, eventData.position, eventData.pressEventCamera, out localMousePos))
                {
                    m_HighOffset = localMousePos;
                }
                interactionState = InteractionState.High;
                if (transition == Transition.ColorTint)
                {
                    targetGraphic = m_HighHandleRect.GetComponent<Graphic>();
                }
            }
            else if (m_LowHandleRect != null && RectTransformUtility.RectangleContainsScreenPoint(m_LowHandleRect, eventData.position, eventData.enterEventCamera))
            {
                //dragging the low value handle
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_LowHandleRect, eventData.position, eventData.pressEventCamera, out localMousePos))
                {
                    m_LowOffset = localMousePos;
                }
                interactionState = InteractionState.Low;
                if (transition == Transition.ColorTint)
                {
                    targetGraphic = m_LowHandleRect.GetComponent<Graphic>();
                }
            }
            else
            {
                //outside the handles, move the entire slider along
                UpdateDrag(eventData, eventData.pressEventCamera);
                interactionState = InteractionState.Bar;
                if (transition == Transition.ColorTint)
                {
                    targetGraphic = m_FillImage;
                }
            }
            base.OnPointerDown(eventData);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
            {
                return;
            }
            UpdateDrag(eventData, eventData.pressEventCamera);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            interactionState = InteractionState.None;
        }

        public override void OnMove(AxisEventData eventData)
        {
            //this requires further investigation
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }
    }
}
