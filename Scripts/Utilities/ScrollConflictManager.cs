/// Credit srinivas sunil 
/// sourced from: https://bitbucket.org/ddreaper/unity-ui-extensions/pull-requests/21/develop_53/diff

using UnityEngine.EventSystems;

/// <summary>
/// This is the most efficient way to handle scroll conflicts when there are multiple scroll rects, this is useful when there is a vertical scrollrect in/on a horizontal scrollrect or vice versa
/// Attach the script to the  rect scroll and assign other rectscroll in the inspecter (one is verticle and other is horizontal) gathered and modified from unity answers(delta snipper)
/// </summary>
namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(ScrollRect))]
    [AddComponentMenu("UI/Extensions/Scrollrect Conflict Manager")]
    public class ScrollConflictManager : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public ScrollRect ParentScrollRect;
		public ScrollSnap ParentScrollSnap;
        private ScrollRect _myScrollRect;
        //This tracks if the other one should be scrolling instead of the current one.
        private bool scrollOther;
        //This tracks wether the other one should scroll horizontally or vertically.
        private bool scrollOtherHorizontally;

        void Awake()
        {
            //Get the current scroll rect so we can disable it if the other one is scrolling
            _myScrollRect = this.GetComponent<ScrollRect>();
            //If the current scroll Rect has the vertical checked then the other one will be scrolling horizontally.
            scrollOtherHorizontally = _myScrollRect.vertical;
            //Check some attributes to let the user know if this wont work as expected
            if (scrollOtherHorizontally)
            {
                if (_myScrollRect.horizontal)
                    Debug.Log("You have added the SecondScrollRect to a scroll view that already has both directions selected");
                if (!ParentScrollRect.horizontal)
                    Debug.Log("The other scroll rect doesnt support scrolling horizontally");
            }
            else if (!ParentScrollRect.vertical)
            {
                Debug.Log("The other scroll rect doesnt support scrolling vertically");
            }
        }

        //IBeginDragHandler
        public void OnBeginDrag(PointerEventData eventData)
        {
            //Get the absolute values of the x and y differences so we can see which one is bigger and scroll the other scroll rect accordingly
            float horizontal = Mathf.Abs(eventData.position.x - eventData.pressPosition.x);
            float vertical = Mathf.Abs(eventData.position.y - eventData.pressPosition.y);
            if (scrollOtherHorizontally)
            {
                if (horizontal > vertical)
                {
                    scrollOther = true;
                    //disable the current scroll rect so it doesnt move.
                    _myScrollRect.enabled = false;
                    ParentScrollRect.OnBeginDrag(eventData);
					ParentScrollSnap.OnBeginDrag(eventData);
                }
            }
            else if (vertical > horizontal)
            {
                scrollOther = true;
                //disable the current scroll rect so it doesnt move.
                _myScrollRect.enabled = false;
                ParentScrollRect.OnBeginDrag(eventData);
				ParentScrollSnap.OnBeginDrag(eventData);
            }
        }

        //IEndDragHandler
        public void OnEndDrag(PointerEventData eventData)
        {
            if (scrollOther)
            {
                scrollOther = false;
                _myScrollRect.enabled = true;
                ParentScrollRect.OnEndDrag(eventData);
				ParentScrollSnap.OnEndDrag(eventData);
            }
        }

        //IDragHandler
        public void OnDrag(PointerEventData eventData)
        {
            if (scrollOther)
            {
                ParentScrollRect.OnDrag(eventData);
				ParentScrollSnap.OnDrag(eventData);
            }
        }
    }
}