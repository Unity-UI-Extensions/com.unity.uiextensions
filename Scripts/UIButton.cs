/// Credit AriathTheWise
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-1796783
/// Extended to include a HELD state that continually fires while the button is held down.

using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// UIButton
    /// </summary>
    [AddComponentMenu("UI/Extensions/UI Button")]
    public class UIButton : Button, IPointerDownHandler, IPointerUpHandler
    {
        #region Sub-Classes
        [System.Serializable]
        public class UIButtonEvent : UnityEvent<PointerEventData.InputButton> { }
        #endregion

        #region Events
		[Tooltip("Event that fires when a button is clicked")]
        public UIButtonEvent OnButtonClick;
		[Tooltip("Event that fires when a button is initially pressed down")]
        public UIButtonEvent OnButtonPress;
		[Tooltip("Event that fires when a button is released")]
        public UIButtonEvent OnButtonRelease;
		[Tooltip("Event that continually fires while a button is held down")]
        public UIButtonEvent OnButtonHeld;
        #endregion
		
		private bool _pressed;
        private PointerEventData _heldEventData;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnSubmit(eventData);

            if (OnButtonClick != null)
            {
                OnButtonClick.Invoke(eventData.button);
            }
        }


        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            DoStateTransition(SelectionState.Pressed, false);

            if (OnButtonPress != null)
            {
                OnButtonPress.Invoke(eventData.button);
            }
			_pressed = true;
            _heldEventData = eventData;
        }


        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            DoStateTransition(SelectionState.Normal, false);

            if (OnButtonRelease != null)
            {
                OnButtonRelease.Invoke(eventData.button);
            }
 			_pressed = false;
            _heldEventData = null;
       }
	   
	    void Update()
		{
			if (!_pressed)
				return;
			
			if (OnButtonHeld != null)
            {
                OnButtonHeld.Invoke(_heldEventData.button);
            }
 
		}
    }
}