using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    /// <summary>
    /// Simple toggle -- something that has an 'on' and 'off' states: checkbox, toggle button, radio button, etc.
    /// </summary>
    [AddComponentMenu("UI/Extensions/Extensions Toggle", 31)]
    [RequireComponent(typeof(RectTransform))]
    public class ExtensionsToggle : Selectable, IPointerClickHandler, ISubmitHandler, ICanvasElement
    {
        /// <summary>
        /// Variable to identify this script, change the datatype if needed to fit your use case 
        /// </summary>
        public string UniqueID;

        public enum ToggleTransition
        {
            None,
            Fade
        }

        [Serializable]
        public class ToggleEvent : UnityEvent<bool>
        { }

        [Serializable]
        public class ToggleEventObject : UnityEvent<ExtensionsToggle>
        { }

        /// <summary>
        /// Transition type.
        /// </summary>
        public ToggleTransition toggleTransition = ToggleTransition.Fade;

        /// <summary>
        /// Graphic the toggle should be working with.
        /// </summary>
        public Graphic graphic;

        // group that this toggle can belong to
        [SerializeField]
        private ExtensionsToggleGroup m_Group;

        public ExtensionsToggleGroup Group
        {
            get { return m_Group; }
            set
            {
                m_Group = value;
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                {
                    SetToggleGroup(m_Group, true);
                    PlayEffect(true);
                }
            }
        }

        /// <summary>
        /// Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
        /// </summary>
        [Tooltip("Use this event if you only need the bool state of the toggle that was changed")]
        public ToggleEvent onValueChanged = new ToggleEvent();


        /// <summary>
        /// Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
        /// </summary>
        [Tooltip("Use this event if you need access to the toggle that was changed")]
        public ToggleEventObject onToggleChanged = new ToggleEventObject();

        // Whether the toggle is on
        [FormerlySerializedAs("m_IsActive")]
        [Tooltip("Is the toggle currently on or off?")]
        [SerializeField]
        private bool m_IsOn;

        protected ExtensionsToggle()
        { }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            Set(m_IsOn, false);
            PlayEffect(toggleTransition == ToggleTransition.None);
#if UNITY_2018_3_OR_NEWER
            if (!Application.isPlaying)
#else
            var prefabType = UnityEditor.PrefabUtility.GetPrefabType(this);
            if (prefabType != UnityEditor.PrefabType.Prefab && !Application.isPlaying)
#endif
            {
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
            }
        }

#endif // if UNITY_EDITOR

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
            {
                onValueChanged.Invoke(m_IsOn);
                onToggleChanged.Invoke(this);
            }
#endif
        }

        public virtual void LayoutComplete()
        { }

        public virtual void GraphicUpdateComplete()
        { }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetToggleGroup(m_Group, false);
            PlayEffect(true);
        }

        protected override void OnDisable()
        {
            SetToggleGroup(null, false);
            base.OnDisable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            // Check if isOn has been changed by the animation.
            // Unfortunately there is no way to check if we don't have a graphic.
            if (graphic != null)
            {
                bool oldValue = !Mathf.Approximately(graphic.canvasRenderer.GetColor().a, 0);
                if (m_IsOn != oldValue)
                {
                    m_IsOn = oldValue;
                    Set(!oldValue);
                }
            }

            base.OnDidApplyAnimationProperties();
        }

        private void SetToggleGroup(ExtensionsToggleGroup newGroup, bool setMemberValue)
        {
            ExtensionsToggleGroup oldGroup = m_Group;

            // Sometimes IsActive returns false in OnDisable so don't check for it.
            // Rather remove the toggle too often than too little.
            if (m_Group != null)
                m_Group.UnregisterToggle(this);

            // At runtime the group variable should be set but not when calling this method from OnEnable or OnDisable.
            // That's why we use the setMemberValue parameter.
            if (setMemberValue)
                m_Group = newGroup;

            // Only register to the new group if this Toggle is active.
            if (m_Group != null && IsActive())
                m_Group.RegisterToggle(this);

            // If we are in a new group, and this toggle is on, notify group.
            // Note: Don't refer to m_Group here as it's not guaranteed to have been set.
            if (newGroup != null && newGroup != oldGroup && IsOn && IsActive())
                m_Group.NotifyToggleOn(this);
        }

        /// <summary>
        /// Whether the toggle is currently active.
        /// </summary>
        public bool IsOn
        {
            get { return m_IsOn; }
            set
            {
                Set(value);
            }
        }

        void Set(bool value)
        {
            Set(value, true);
        }

        void Set(bool value, bool sendCallback)
        {
            if (m_IsOn == value)
                return;

            // if we are in a group and set to true, do group logic
            m_IsOn = value;
            if (m_Group != null && IsActive())
            {
                if (m_IsOn || (!m_Group.AnyTogglesOn() && !m_Group.AllowSwitchOff))
                {
                    m_IsOn = true;
                    m_Group.NotifyToggleOn(this);
                }
            }

            // Always send event when toggle is clicked, even if value didn't change
            // due to already active toggle in a toggle group being clicked.
            // Controls like Dropdown rely on this.
            // It's up to the user to ignore a selection being set to the same value it already was, if desired.
            PlayEffect(toggleTransition == ToggleTransition.None);
            if (sendCallback)
            {
                onValueChanged.Invoke(m_IsOn);
                onToggleChanged.Invoke(this);
            }
        }

        /// <summary>
        /// Play the appropriate effect.
        /// </summary>
        private void PlayEffect(bool instant)
        {
            if (graphic == null)
                return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                graphic.canvasRenderer.SetAlpha(m_IsOn ? 1f : 0f);
            else
#endif
                graphic.CrossFadeAlpha(m_IsOn ? 1f : 0f, instant ? 0f : 0.1f, true);
        }

        /// <summary>
        /// Assume the correct visual state.
        /// </summary>
        protected override void Start()
        {
            PlayEffect(true);
        }

        private void InternalToggle()
        {
            if (!IsActive() || !IsInteractable())
                return;

            IsOn = !IsOn;
        }

        /// <summary>
        /// React to clicks.
        /// </summary>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            InternalToggle();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            InternalToggle();
        }
    }
}
