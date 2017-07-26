/// Credit David Gileadi
/// Sourced from - https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/pull-requests/12

using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    // Segmented control, like a group of buttons
    [AddComponentMenu("UI/Extensions/Segmented Control")]
    [RequireComponent(typeof(RectTransform))]
    public class SegmentedControl : UIBehaviour
    {
        private Selectable[] m_segments;
        [SerializeField]
        [Tooltip("A GameObject with an Image to use as a separator between segments. Size of the RectTransform will determine the size of the separator used.\nNote, make sure to disable the separator GO so that it does not affect the scene")]
        private Graphic m_separator;
        private float m_separatorWidth = 0;
        [SerializeField]
        [Tooltip("When True, it allows each button to be toggled on/off")]
        private bool m_allowSwitchingOff = false;
        [SerializeField]
        [Tooltip("The selected default for the control (zero indexed array)")]
        private int m_selectedSegmentIndex = -1;
        // Event delegates triggered on click.
        [SerializeField]
        [Tooltip("Event to fire once the selection has been changed")]
        private SegmentSelectedEvent m_onValueChanged = new SegmentSelectedEvent();

        protected internal Selectable selectedSegment;

        protected float SeparatorWidth
        {
            get
            {
                if (m_separatorWidth == 0 && separator)
                {
                    m_separatorWidth = separator.rectTransform.rect.width;
                    var image = separator.GetComponent<Image>();
                    if (image)
                        m_separatorWidth /= image.pixelsPerUnit;
                }
                return m_separatorWidth;
            }
        }

        [Serializable]
        public class SegmentSelectedEvent : UnityEvent<int> { }

        public Selectable[] segments
        {
            get
            {
                if (m_segments == null || m_segments.Length == 0)
                {
                    m_segments = GetChildSegments();
                }
                return m_segments;
            }
        }

        [SerializeField]
        public Color selectedColor;

        public Graphic separator { get { return m_separator; } set { m_separator = value; m_separatorWidth = 0; LayoutSegments(); } }

        public bool allowSwitchingOff { get { return m_allowSwitchingOff; } set { m_allowSwitchingOff = value; } }

        public int selectedSegmentIndex
        {
            get { return Array.IndexOf(segments, selectedSegment); }
            set
            {
                value = Math.Max(value, -1);
                value = Math.Min(value, segments.Length - 1);
                m_selectedSegmentIndex = value;
                if (value == -1)
                {
                    if (selectedSegment)
                    {
                        selectedSegment.GetComponent<Segment>().selected = false;
                        selectedSegment = null;
                    }
                }
                else
                {
#if UNITY_EDITOR
                    segments[value].GetComponent<Segment>().StoreTextColor();
#endif
                    segments[value].GetComponent<Segment>().selected = true;
                }
            }
        }

        public SegmentSelectedEvent onValueChanged
        {
            get { return m_onValueChanged; }
            set { m_onValueChanged = value; }
        }

        protected SegmentedControl()
        { }

        protected override void Start()
        {
            base.Start();

            LayoutSegments();

            if (m_selectedSegmentIndex != -1)
                selectedSegmentIndex = m_selectedSegmentIndex;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (separator)
                LayoutSegments();

            if (m_selectedSegmentIndex != -1)
                selectedSegmentIndex = m_selectedSegmentIndex;

            if (m_selectedSegmentIndex > transform.childCount)
            {
                selectedSegmentIndex = transform.childCount - 1;
            }

            if (selectedColor == new Color(0, 0, 0, 0))
            {
                selectedColor = new Color(0f, 0.455f, 0.894f);
            }
        }
#endif

        private Selectable[] GetChildSegments()
        {
            var buttons = GetComponentsInChildren<Selectable>();
            if (buttons.Length < 2)
            {
                throw new InvalidOperationException("A segmented control must have at least two Button children");
            }

            for (int i = 0; i < buttons.Length; i++)
            {
                var segment = buttons[i].GetComponent<Segment>();
                if (segment == null)
                {
                    segment = buttons[i].gameObject.AddComponent<Segment>();
                }
                segment.index = i;
            }

            return buttons;
        }

        public void SetAllSegmentsOff()
        {
            selectedSegment = null;
        }

        private void RecreateSprites()
        {
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i].image == null)
                    continue;

                var sprite = segments[i].image.sprite;
                if (sprite.border.x == 0 || sprite.border.z == 0)
                    continue;

                var rect = sprite.rect;
                var border = sprite.border;

                if (i > 0)
                {
                    rect.xMin = border.x;
                    border.x = 0;
                }
                if (i < segments.Length - 1)
                {
                    rect.xMax = border.z;
                    border.z = 0;
                }

                segments[i].image.sprite = Sprite.Create(sprite.texture, rect, sprite.pivot, sprite.pixelsPerUnit, 0, SpriteMeshType.FullRect, border);
            }
        }

        public void LayoutSegments()
        {
            RecreateSprites();

            RectTransform transform = this.transform as RectTransform;
            float width = (transform.rect.width / segments.Length) - (SeparatorWidth * (segments.Length - 1));

            for (int i = 0; i < segments.Length; i++)
            {
                float insetX = ((width + SeparatorWidth) * i);

                var rectTransform = segments[i].GetComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;
                rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, insetX, width);
                rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, transform.rect.height);

                if (separator && i > 0)
                {
                    var sepTransform = gameObject.transform.Find("Separator " + i);
                    Graphic sep = (sepTransform != null) ? sepTransform.GetComponent<Graphic>() : (GameObject.Instantiate(separator.gameObject) as GameObject).GetComponent<Graphic>();
                    sep.gameObject.name = "Separator " + i;
                    sep.gameObject.SetActive(true);
                    sep.rectTransform.SetParent(this.transform, false);
                    sep.rectTransform.anchorMin = Vector2.zero;
                    sep.rectTransform.anchorMax = Vector2.zero;
                    sep.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, insetX - SeparatorWidth, SeparatorWidth);
                    sep.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, transform.rect.height);
                }
// TODO: maybe adjust text position
            }
        }
    }

    [RequireComponent(typeof(Selectable))]
    public class Segment :
        UIBehaviour,
        IPointerClickHandler,
        ISubmitHandler,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler,
        ISelectHandler, IDeselectHandler
    {
        internal int index;

        internal bool leftmost
        {
            get { return index == 0; }
        }
        internal bool rightmost
        {
            get { return index == segmentControl.segments.Length - 1; }
        }

        public bool selected
        {
            get { return segmentControl.selectedSegment == this.button; }
            set { SetSelected(value); }
        }

        internal SegmentedControl segmentControl
        {
            get { return GetComponentInParent<SegmentedControl>(); }
        }

        internal Selectable button
        {
            get { return GetComponent<Selectable>(); }
        }

        [SerializeField]
        Color textColor;

        protected Segment()
        { }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            selected = true;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            MaintainSelection();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            MaintainSelection();
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            MaintainSelection();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            MaintainSelection();
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            MaintainSelection();
        }

        public virtual void OnDeselect(BaseEventData eventData)
        {
            MaintainSelection();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            selected = true;
        }

        private void SetSelected(bool value)
        {
            if (value && button.IsActive() && button.IsInteractable())
            {
                if (segmentControl.selectedSegment == this.button)
                {
                    if (segmentControl.allowSwitchingOff)
                    {
                        Deselect();
                    }
                    else
                    {
                        MaintainSelection();
                    }
                }
                else
                {
                    if (segmentControl.selectedSegment)
                    {
                        var segment = segmentControl.selectedSegment.GetComponent<Segment>();
                        segmentControl.selectedSegment = null;
                        segment.TransitionButton();
                    }

                    segmentControl.selectedSegment = this.button;
                    StoreTextColor();
                    TransitionButton();
                    segmentControl.onValueChanged.Invoke(index);
                }
            }
            else if (segmentControl.selectedSegment == this.button)
            {
                Deselect();
            }
        }

        private void Deselect()
        {
            segmentControl.selectedSegment = null;
            TransitionButton();
            segmentControl.onValueChanged.Invoke(-1);
        }

        void MaintainSelection()
        {
            if (button != segmentControl.selectedSegment)
                return;

            TransitionButton(true);
        }

        internal void TransitionButton()
        {
            TransitionButton(false);
        }

        internal void TransitionButton(bool instant)
        {
            Color tintColor = selected ? segmentControl.selectedColor : button.colors.normalColor;
            Color textColor = selected ? button.colors.normalColor : this.textColor;
            Sprite transitionSprite = selected ? button.spriteState.pressedSprite : null;
            string triggerName = selected ? button.animationTriggers.pressedTrigger : button.animationTriggers.normalTrigger;

            switch (button.transition)
            {
                case Selectable.Transition.ColorTint:
                    StartColorTween(tintColor * button.colors.colorMultiplier, instant);
                    ChangeTextColor(textColor * button.colors.colorMultiplier);
                    break;
                case Selectable.Transition.SpriteSwap:
                    DoSpriteSwap(transitionSprite);
                    break;
                case Selectable.Transition.Animation:
                    TriggerAnimation(triggerName);
                    break;
            }
        }

        void StartColorTween(Color targetColor, bool instant)
        {
            if (button.targetGraphic == null)
                return;

            button.targetGraphic.CrossFadeColor(targetColor, instant ? 0f : button.colors.fadeDuration, true, true);
        }

        internal void StoreTextColor()
        {
            var text = GetComponentInChildren<Text>();
            if (!text)
                return;

            textColor = text.color;
        }

        void ChangeTextColor(Color targetColor)
        {
            var text = GetComponentInChildren<Text>();
            if (!text)
                return;

            text.color = targetColor;
        }

        void DoSpriteSwap(Sprite newSprite)
        {
            if (button.image == null)
                return;

            button.image.overrideSprite = newSprite;
        }

        void TriggerAnimation(string triggername)
        {
            if (button.animator == null || !button.animator.isActiveAndEnabled || !button.animator.hasBoundPlayables || string.IsNullOrEmpty(triggername))
                return;

            button.animator.ResetTrigger(button.animationTriggers.normalTrigger);
            button.animator.ResetTrigger(button.animationTriggers.pressedTrigger);
            button.animator.ResetTrigger(button.animationTriggers.highlightedTrigger);
            button.animator.ResetTrigger(button.animationTriggers.disabledTrigger);

            button.animator.SetTrigger(triggername);
        }
    }
}