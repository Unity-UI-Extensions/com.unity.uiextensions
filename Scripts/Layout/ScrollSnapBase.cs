using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{
    public class ScrollSnapBase : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        internal struct VisibleItem
        {
            public Vector3 Position;
            public int OriginalPosition;
        }

        internal RectTransform _screensContainer;
        internal bool isVertical;

        internal int _screens = 1;

        internal float _scrollStartPosition;
        internal float _childSize;
        private float _childPos;
        //internal Vector3[] _positions;
        //internal VisibleItem[] _visiblePositions;
        internal ScrollRect _scroll_rect;
        internal Vector3 _lerp_target;
        internal bool _lerp;
        internal bool _pointerDown = false;

        [Serializable]
        public class SelectionChangeStartEvent : UnityEvent { }
        [Serializable]
        public class SelectionChangeEndEvent : UnityEvent { }

        [Tooltip("The visible bounds area, controls which items are visible/enabled. *Note Should use a RectMask. (optional)")]
        public RectTransform MaskArea;
        [Tooltip("Pixel size to buffer arround Mask Area. (optional)")]
        public float MaskBuffer;

        [Tooltip("The gameobject that contains toggles which suggest pagination. (optional)")]
        public GameObject Pagination;

        [Tooltip("Button to go to the next page. (optional)")]
        public GameObject NextButton;
        [Tooltip("Button to go to the previous page. (optional)")]
        public GameObject PrevButton;
        [Tooltip("Transition speed between pages. (optional)")]
        public float transitionSpeed = 7.5f;

        [Tooltip("Fast Swipe makes swiping page next / previous (optional)")]
        public Boolean UseFastSwipe = false;
        [Tooltip("How far swipe has to travel to initiate a page change (optional)")]
        public int FastSwipeThreshold = 100;
        [Tooltip("How fast can a user swipe to be a swipe (optional)")]
        public int SwipeVelocityThreshold = 200;

        internal Vector3 _startPosition = new Vector3();

        [Tooltip("The currently active page")]
        internal int _currentScreen;
        internal int _previousScreen;

        [Tooltip("The screen / page to start the control on")]
        [SerializeField]
        public int StartingScreen = 1;

        [Tooltip("The distance between two pages based on page height, by default pages are next to each other")]
        [SerializeField]
        [Range(1, 8)]
        public float PageStep = 1;

        public int CurrentPage
        {
            get
            {
                return _currentScreen;
            }
        }

        [SerializeField]
        private SelectionChangeStartEvent m_OnSelectionChangeStartEvent = new SelectionChangeStartEvent();
        public SelectionChangeStartEvent OnSelectionChangeStartEvent { get { return m_OnSelectionChangeStartEvent; } set { m_OnSelectionChangeStartEvent = value; } }

        [SerializeField]
        private SelectionChangeEndEvent m_OnSelectionChangeEndEvent = new SelectionChangeEndEvent();
        public SelectionChangeEndEvent OnSelectionChangeEndEvent { get { return m_OnSelectionChangeEndEvent; } set { m_OnSelectionChangeEndEvent = value; } }

        //Function for switching screens with buttons
        public void NextScreen()
        {
            if (_currentScreen < _screens - 1)
            {
                if (!_lerp) StartScreenChange();

                _lerp = true;
                _currentScreen++;
                GetPositionforPage(_currentScreen, ref _lerp_target);
 //               _lerp_target = _positions[_currentScreen];

                ChangeBulletsInfo(_currentScreen);
            }
        }

        //Function for switching screens with buttons
        public void PreviousScreen()
        {
            if (_currentScreen > 0)
            {
                if (!_lerp) StartScreenChange();

                _lerp = true;
                _currentScreen--;
                GetPositionforPage(_currentScreen, ref _lerp_target);
//                _lerp_target = _positions[_currentScreen];

                ChangeBulletsInfo(_currentScreen);
            }
        }

        /// <summary>
        /// Function for switching to a specific screen
        /// *Note, this is based on a 0 starting index - 0 to x
        /// </summary>
        /// <param name="screenIndex">0 starting index of page to jump to</param>
        public void GoToScreen(int screenIndex)
        {
            if (screenIndex <= _screens - 1 && screenIndex >= 0)
            {
                if (!_lerp) StartScreenChange();

                _lerp = true;
                _currentScreen = screenIndex;
                GetPositionforPage(_currentScreen, ref _lerp_target);
 //               _lerp_target = _positions[_currentScreen];

                ChangeBulletsInfo(_currentScreen);
            }
        }

        //find the closest registered point to the releasing point
        internal Vector3 FindClosestFrom(Vector3 start, VisibleItem[] positions)
        {

            Vector3 closestPosition = Vector3.zero;
            float closest = Mathf.Infinity;
            float distanceToTarget = 0;

            for (int i = 0; i < positions.Length; i++)
            {
                distanceToTarget = Vector3.Distance(start, positions[i].Position);
                if (distanceToTarget < closest)
                {
                    closest = distanceToTarget;
                    closestPosition = positions[i].Position;
                }
            }
            float close = -(int)Math.Round((start.y - _scrollStartPosition) / _childSize);
            return closestPosition;
        }

        internal int GetPageforPosition(Vector3 pos)
        {
            return isVertical ? 
                -(int)Math.Round((pos.y - _scrollStartPosition) / _childSize) :
                -(int)Math.Round((pos.x - _scrollStartPosition) / _childSize);
        }

        internal void GetPositionforPage(int page, ref Vector3 target)
        {
            _childPos = -_childSize * page;
            if (isVertical)
            {
                target.y = _childPos + _scrollStartPosition;
            }
            else
            {
                target.x = _childPos + _scrollStartPosition;
            }
        }

        internal void ScrollToClosestElement()
        {
            _lerp = true;
            //_lerp_target = FindClosestFrom(_screensContainer.localPosition, _visiblePositions);
            _currentScreen = GetPageforPosition(_screensContainer.localPosition);
            GetPositionforPage(_currentScreen, ref _lerp_target);
            ChangeBulletsInfo(_currentScreen);
        }

        //changes the bullets on the bottom of the page - pagination
        internal void ChangeBulletsInfo(int currentScreen)
        {
            if (Pagination)
                for (int i = 0; i < Pagination.transform.childCount; i++)
                {
                    Pagination.transform.GetChild(i).GetComponent<Toggle>().isOn = (currentScreen == i)
                        ? true
                        : false;
                }
        }

        void OnValidate()
        {
            var childCount = gameObject.GetComponent<ScrollRect>().content.childCount;
            if (StartingScreen > childCount - 1)
            {
                StartingScreen = childCount - 1;
            }
            if (StartingScreen < 0)
            {
                StartingScreen = 0;
            }
        }

        internal void StartScreenChange()
        {
            OnSelectionChangeStartEvent.Invoke();
        }

        internal void EndScreenChange()
        {
            OnSelectionChangeEndEvent.Invoke();
        }

        #region Interfaces
        /// <summary>
        /// Touch screen to start swiping
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            StartScreenChange();
            _startPosition = _screensContainer.localPosition;
        }

        /// <summary>
        /// While dragging do
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            _lerp = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _pointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _pointerDown = false;
        }
        #endregion


    }
}