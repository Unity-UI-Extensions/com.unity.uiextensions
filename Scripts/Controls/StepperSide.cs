/// Credit David Gileadi
/// Sourced from - https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/pull-requests/11

using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{

    [RequireComponent(typeof(Selectable))]
    public class StepperSide :
        UIBehaviour,
        IPointerClickHandler,
        ISubmitHandler,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler,
        ISelectHandler, IDeselectHandler
    {
        Selectable button { get { return GetComponent<Selectable>(); } }

        Stepper stepper { get { return GetComponentInParent<Stepper>(); } }

        bool leftmost { get { return button == stepper.sides[0]; } }

        internal Sprite cutSprite;

        protected StepperSide()
        { }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Press();
            AdjustSprite(false);
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();
            AdjustSprite(true);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            AdjustSprite(false);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            AdjustSprite(true);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            AdjustSprite(false);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            AdjustSprite(false);
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            AdjustSprite(false);
        }

        public virtual void OnDeselect(BaseEventData eventData)
        {
            AdjustSprite(true);
        }

        private void Press()
        {
            if (!button.IsActive() || !button.IsInteractable())
                return;

            if (leftmost)
            {
                stepper.StepDown();
            }
            else
            {
                stepper.StepUp();
            }
        }

        private void AdjustSprite(bool restore)
        {
            var image = button.image;
            if (!image || image.overrideSprite == cutSprite)
                return;

            if (restore)
                image.overrideSprite = cutSprite;
            else
                image.overrideSprite = Stepper.CutSprite(image.overrideSprite, leftmost);
        }
    }
}