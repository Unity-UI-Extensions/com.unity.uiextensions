/// Credit David Gileadi
/// Sourced from - https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/pull-requests/12

using System.Collections;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Segmented Control/Segment")]
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
        internal SegmentedControl segmentedControl;

        internal bool leftmost
        {
            get { return index == 0; }
        }
        internal bool rightmost
        {
            get { return index == segmentedControl.segments.Length - 1; }
        }

        public bool selected
        {
            get { return segmentedControl.selectedSegment == this.button; }
            set { SetSelected(value); }
        }

        internal Selectable button
        {
            get { return GetComponent<Selectable>(); }
        }

        internal Sprite cutSprite;

        protected Segment()
        { }

        protected override void Start()
        {
            StartCoroutine(DelayedInit());
        }

        IEnumerator DelayedInit()
        {
            yield return null;
            yield return null;

            button.image.overrideSprite = cutSprite;
            if (selected)
                MaintainSelection();
        }

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

        protected override void OnEnable()
        {
            base.OnEnable();
            if (segmentedControl)
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
                if (segmentedControl.selectedSegment == this.button)
                {
                    if (segmentedControl.allowSwitchingOff)
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
                    if (segmentedControl.selectedSegment)
                    {
                        var segment = segmentedControl.selectedSegment.GetComponent<Segment>();
                        segmentedControl.selectedSegment = null;
                        if (segment)
                        {
                            segment.TransitionButton();
                        }
                    }

                    segmentedControl.selectedSegment = this.button;
                    TransitionButton();
                    segmentedControl.onValueChanged.Invoke(index);
                }
            }
            else if (segmentedControl.selectedSegment == this.button)
            {
                Deselect();
            }
        }

        private void Deselect()
        {
            segmentedControl.selectedSegment = null;
            TransitionButton();
            segmentedControl.onValueChanged.Invoke(-1);
        }

        void MaintainSelection()
        {
            if (button != segmentedControl.selectedSegment)
                return;

            TransitionButton(true);
        }

        internal void TransitionButton()
        {
            TransitionButton(false);
        }

        internal void TransitionButton(bool instant)
        {
            Color tintColor = selected ? button.colors.pressedColor : button.colors.normalColor;
            Color textColor = selected ? button.colors.normalColor : button.colors.pressedColor;
            Sprite transitionSprite = selected ? button.spriteState.pressedSprite : cutSprite;
            string triggerName = selected ? button.animationTriggers.pressedTrigger : button.animationTriggers.normalTrigger;

            switch (button.transition)
            {
                case Selectable.Transition.ColorTint:
                    button.image.overrideSprite = cutSprite;
                    StartColorTween(tintColor * button.colors.colorMultiplier, instant);
                    ChangeTextColor(textColor * button.colors.colorMultiplier);
                    break;
                case Selectable.Transition.SpriteSwap:
                    if (transitionSprite != cutSprite)
                        transitionSprite = SegmentedControl.CutSprite(transitionSprite, leftmost, rightmost);
                    DoSpriteSwap(transitionSprite);
                    break;
                case Selectable.Transition.Animation:
                    button.image.overrideSprite = cutSprite;
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

        void ChangeTextColor(Color targetColor)
        {
#if UNITY_2022_1_OR_NEWER
            var text = GetComponentInChildren<TMPro.TMP_Text>();
#else
            var text = GetComponentInChildren<Text>();
#endif
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