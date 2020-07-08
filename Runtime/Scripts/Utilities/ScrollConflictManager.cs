/// Credit srinivas sunil 
/// sourced from: https://bitbucket.org/SimonDarksideJ/unity-ui-extensions/pull-requests/21/develop_53/diff
/// Updated by Hiep Eldest : https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/issues/300/scrollconflictmanager-not-working-if

using UnityEngine.EventSystems;

/// <summary>
/// This is the most efficient way to handle scroll conflicts when there are multiple scroll rects, this is useful when there is a vertical scrollrect in/on a horizontal scrollrect or vice versa
/// Attach the script to the  rect scroll and assign other rectscroll in the inspector (one is vertical and other is horizontal) gathered and modified from unity answers(delta snipper)
/// </summary>
namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(ScrollRect))]
    [AddComponentMenu("UI/Extensions/Scrollrect Conflict Manager")]
    public class ScrollConflictManager : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [Tooltip("The parent ScrollRect control hosting this ScrollSnap")]
        public ScrollRect ParentScrollRect;

        [Tooltip("The parent ScrollSnap control hosting this Scroll Snap.\nIf left empty, it will use the ScrollSnap of the ParentScrollRect")]
        public ScrollSnapBase ParentScrollSnap;

        private ScrollRect _myScrollRect;
        private IBeginDragHandler[] _beginDragHandlers;
        private IEndDragHandler[] _endDragHandlers;
        private IDragHandler[] _dragHandlers;
        //This tracks if the other one should be scrolling instead of the current one.
        private bool scrollOther;
        //This tracks whether the other one should scroll horizontally or vertically.
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
                    Debug.LogError("You have added the SecondScrollRect to a scroll view that already has both directions selected");
                if (!ParentScrollRect.horizontal)
                    Debug.LogError("The other scroll rect does not support scrolling horizontally");
            }
            else if (!ParentScrollRect.vertical)
            {
                Debug.LogError("The other scroll rect does not support scrolling vertically");
            }

            if (ParentScrollRect && !ParentScrollSnap)
            {
                ParentScrollSnap = ParentScrollRect.GetComponent<ScrollSnapBase>();
            }
        }

        void Start()
        {
            _beginDragHandlers = ParentScrollRect.GetComponents<IBeginDragHandler>();
            _dragHandlers = ParentScrollRect.GetComponents<IDragHandler>();
            _endDragHandlers = ParentScrollRect.GetComponents<IEndDragHandler>();
        }

        #region DragHandler

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
                    //disable the current scroll rect so it does not move.
                    _myScrollRect.enabled = false;
                    for (int i = 0, length = _beginDragHandlers.Length; i < length; i++)
                    {
                        _beginDragHandlers[i].OnBeginDrag(eventData);
                        if(ParentScrollSnap) ParentScrollSnap.OnBeginDrag(eventData);
                    }
                }
            }
            else if (vertical > horizontal)
            {
                scrollOther = true;
                //disable the current scroll rect so it does not move.
                _myScrollRect.enabled = false;
                for (int i = 0, length = _beginDragHandlers.Length; i < length; i++)
                {
                    _beginDragHandlers[i].OnBeginDrag(eventData);
                    if (ParentScrollSnap) ParentScrollSnap.OnBeginDrag(eventData);
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (scrollOther)
            {
                _myScrollRect.enabled = true;
                scrollOther = false;
                for (int i = 0, length = _endDragHandlers.Length; i < length; i++)
                {
                    _endDragHandlers[i].OnEndDrag(eventData);
                    if (ParentScrollSnap) ParentScrollSnap.OnEndDrag(eventData);
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (scrollOther)
            {
                for (int i = 0, length = _endDragHandlers.Length; i < length; i++)
                {
                    _dragHandlers[i].OnDrag(eventData);
                    if (ParentScrollSnap) ParentScrollSnap.OnDrag(eventData);
                }
            }
        }

        #endregion DragHandler
    }
}