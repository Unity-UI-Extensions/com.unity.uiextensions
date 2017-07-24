using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI.Extensions
{
    // Segmented control, like a group of buttons
    [AddComponentMenu("UI/Extensions/Segmented Control")]
    [RequireComponent(typeof(RectTransform))]
    public class SegmentedControl : UIBehaviour
    {
        public Button[] segments
        {
            get
            {
                if (_segments == null || _segments.Length == 0)
                {
                    _segments = GetChildSegments();
                }
                return _segments;
            }
        }
        private Button[] _segments;

        [SerializeField]
        public Color selectedColor;

        [SerializeField]
        private Graphic _separator;
        public Graphic separator { get { return _separator; } set { _separator = value; _separatorWidth = 0; LayoutSegments(); } }

        private float _separatorWidth = 0;
        private float separatorWidth
        {
            get
            {
                if (_separatorWidth == 0 && separator)
                {
                    _separatorWidth = separator.rectTransform.rect.width;
                    var image = separator.GetComponent<Image>();
                    if (image)
                        _separatorWidth /= image.pixelsPerUnit;
                }
                return _separatorWidth;
            }
        }

        [SerializeField] private bool _allowSwitchingOff = false;
        public bool allowSwitchingOff { get { return _allowSwitchingOff; } set { _allowSwitchingOff = value; } }

        protected internal Button selectedSegment;

        [SerializeField]
        private int _selectedSegmentIndex = -1;
        public int selectedSegmentIndex
        {
            get { return Array.IndexOf(segments, selectedSegment); }
            set
            {
                value = Math.Max(value, -1);
                value = Math.Min(value, segments.Length - 1);
                _selectedSegmentIndex = value;
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

        [Serializable]
        public class SegmentSelectedEvent : UnityEvent<int> { }

        // Event delegates triggered on click.
        [SerializeField]
        private SegmentSelectedEvent _onValueChanged = new SegmentSelectedEvent();

        public SegmentSelectedEvent onValueChanged
        {
            get { return _onValueChanged; }
            set { _onValueChanged = value; }
        }

        protected SegmentedControl()
        { }

        protected override void Start()
        {
            base.Start();

            LayoutSegments();

            if (_selectedSegmentIndex != -1)
                selectedSegmentIndex = _selectedSegmentIndex;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (separator)
                LayoutSegments();

            if (_selectedSegmentIndex != -1)
                selectedSegmentIndex = _selectedSegmentIndex;
        }
#endif

        private Button[] GetChildSegments()
        {
            var buttons = GetComponentsInChildren<Button>();
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

        public void LayoutSegments()
        {
            RectTransform transform = this.transform as RectTransform;
            float width = (transform.rect.width / segments.Length) - (separatorWidth * (segments.Length - 1));

            for (int i = 0; i < segments.Length; i++)
            {
                float leftX = 0;
                float rightX = 0;
                if (segments[i].image != null && segments[i].image.hasBorder)
                {
                    if (i > 0)
                    {
                        leftX = segments[i].image.sprite.border.x / segments[i].image.pixelsPerUnit;
                    }
                    if (i < segments.Length - 1)
                    {
                        rightX = segments[i].image.sprite.border.z / segments[i].image.pixelsPerUnit;
                    }
                }

                float insetX = ((width + separatorWidth) * i) - leftX;

                var rectTransform = segments[i].GetComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;
                rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, insetX, width + leftX + rightX);
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
                    sep.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, insetX + leftX - separatorWidth, separatorWidth);
                    sep.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, transform.rect.height);
                }
// TODO: maybe adjust text position
            }
        }
    }

    [RequireComponent(typeof(Button))]
    public class Segment :
        UIBehaviour,
        IMeshModifier,
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

        internal Button button
        {
            get { return GetComponent<Button>(); }
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

        public void ModifyMesh(Mesh mesh)
        {
            throw new NotSupportedException();
        }

        public void ModifyMesh(VertexHelper verts)
        {
            if (verts.currentIndexCount != 54)
            {
                return;
            }

            List<UIVertex> vertexList = new List<UIVertex>();
            verts.GetUIVertexStream(vertexList);

            if (!leftmost)
            {
                vertexList.RemoveRange(0, 18);
            }
            if (!rightmost)
            {
                vertexList.RemoveRange(vertexList.Count - 18, 18);
            }

            verts.Clear();
            verts.AddUIVertexTriangleStream(vertexList);
        }
    }
}