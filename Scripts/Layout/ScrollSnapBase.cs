using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{
    public class ScrollSnapBase : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        internal RectTransform _screensContainer;
        internal bool isVertical;

        internal int _screens = 1;

        internal float _scrollStartPosition;
        internal float _childSize;
        private float _childPos;
        internal ScrollRect _scroll_rect;
        internal Vector3 _lerp_target;
        internal bool _lerp;
        internal bool _pointerDown = false;
        internal Vector3 _startPosition = new Vector3();
        [Tooltip("The currently active page")]
        internal int _currentPage;
        internal int _previousPage;



        [Serializable]
        public class SelectionChangeStartEvent : UnityEvent { }
        [Serializable]
        public class SelectionPageChangedEvent : UnityEvent<int> { }
        [Serializable]
        public class SelectionChangeEndEvent : UnityEvent { }

        [Tooltip("The visible bounds area, controls which items are visible/enabled. *Note Should use a RectMask. (optional)")]
        public RectTransform MaskArea;
        [Tooltip("Pixel size to buffer arround Mask Area. (optional)")]
        public float MaskBuffer = 1;
        public int HalfNoVisibleItems;

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
        [Tooltip("Speed at which the ScrollRect will keep scrolling before slowing down and stopping (optional)")]
        public int SwipeVelocityThreshold = 200;

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
                return _currentPage;
            }
            internal set
            {
                if (value != _currentPage)
                {
                    _previousPage = _currentPage;
                    _currentPage = value;
                    ChangeBulletsInfo(_currentPage);
                    if(MaskArea) UpdateVisible();
                }
            }
        }

        [SerializeField]
        private SelectionChangeStartEvent m_OnSelectionChangeStartEvent = new SelectionChangeStartEvent();
        public SelectionChangeStartEvent OnSelectionChangeStartEvent { get { return m_OnSelectionChangeStartEvent; } set { m_OnSelectionChangeStartEvent = value; } }

        [SerializeField]
        private SelectionPageChangedEvent m_OnSelectionPageChangedEvent = new SelectionPageChangedEvent();
        public SelectionPageChangedEvent OnSelectionPageChangedEvent { get { return m_OnSelectionPageChangedEvent; } set { m_OnSelectionPageChangedEvent = value; } }

        [SerializeField]
        private SelectionChangeEndEvent m_OnSelectionChangeEndEvent = new SelectionChangeEndEvent();
        public SelectionChangeEndEvent OnSelectionChangeEndEvent { get { return m_OnSelectionChangeEndEvent; } set { m_OnSelectionChangeEndEvent = value; } }

        public GameObject[] ChildObjects;

        // Use this for initialization
        void Awake()
        {
            _scroll_rect = gameObject.GetComponent<ScrollRect>();

            if (_scroll_rect.horizontalScrollbar || _scroll_rect.verticalScrollbar)
            {
                Debug.LogWarning("Warning, using scrollbars with the Scroll Snap controls is not advised as it causes unpredictable results");
            }

            _screensContainer = _scroll_rect.content;
            if (ChildObjects != null && ChildObjects.Length > 0)
            {
                if (_screensContainer.transform.childCount > 0)
                {
                    Debug.LogError("ScrollRect Content has children, this is not supported when using managed Child Objects\n Either remove the ScrollRect Content children or clear the ChildObjects array");
                    return;
                }
                InitialiseChildObjectsFromArray();
            }
            else
            {
                InitialiseChildObjectsFromScene();
            }

            if (NextButton)
                NextButton.GetComponent<Button>().onClick.AddListener(() => { NextScreen(); });

            if (PrevButton)
                PrevButton.GetComponent<Button>().onClick.AddListener(() => { PreviousScreen(); });
        }

        internal void InitialiseChildObjectsFromScene()
        {
            int childCount = _screensContainer.childCount;
            ChildObjects = new GameObject[childCount];
            for (int i = 0; i < childCount; i++)
            {
                ChildObjects[i] = _screensContainer.transform.GetChild(i).gameObject;
                if (MaskArea && ChildObjects[i].activeSelf)
                {
                    ChildObjects[i].SetActive(false);
                }
            }
        }

        internal void InitialiseChildObjectsFromArray()
        {
            int childCount = ChildObjects.Length;
            for (int i = 0; i < childCount; i++)
            {
                ChildObjects[i] = GameObject.Instantiate(ChildObjects[i]);
                ChildObjects[i].transform.SetParent(_screensContainer.transform);
                if (MaskArea && ChildObjects[i].activeSelf)
                {
                    ChildObjects[i].SetActive(false);
                }
            }
        }

        internal void CalculateVisible()
        {
            float MaskSize = isVertical ? MaskArea.rect.height : MaskArea.rect.width;
            HalfNoVisibleItems = (int)Math.Round(MaskSize / (_childSize * MaskBuffer), MidpointRounding.AwayFromZero) / 2 + 2;
            int StartingItemsBefore = StartingScreen - HalfNoVisibleItems < 0 ? 0 : HalfNoVisibleItems;
            int StartingItemsAfter = _screensContainer.childCount - StartingScreen < HalfNoVisibleItems ? _screensContainer.childCount - StartingScreen : HalfNoVisibleItems;
            for (int i = StartingScreen - StartingItemsBefore; i < StartingScreen + StartingItemsAfter - 1; i++)
            {
                ChildObjects[i].SetActive(true);
            }
        }

        internal void UpdateVisible()
        {
            int BottomItem = _currentPage - HalfNoVisibleItems < 0 ? 0 : HalfNoVisibleItems;
            int TopItem = _screensContainer.childCount - _currentPage < HalfNoVisibleItems ? _screensContainer.childCount - _currentPage : HalfNoVisibleItems;

            for (int i = CurrentPage - BottomItem; i < CurrentPage + TopItem; i++)
            {
                ChildObjects[i].SetActive(true);
            }

            if (_screensContainer.childCount - _currentPage > HalfNoVisibleItems + 1) ChildObjects[CurrentPage + TopItem + 1].SetActive(false);
            if(_currentPage - HalfNoVisibleItems > 0) ChildObjects[CurrentPage - BottomItem - 1].SetActive(false);
        }


        //Function for switching screens with buttons
        public void NextScreen()
        {
            if (_currentPage < _screens - 1)
            {
                if (!_lerp) StartScreenChange();

                _lerp = true;
                CurrentPage = _currentPage + 1;
                GetPositionforPage(_currentPage, ref _lerp_target);
            }
        }

        //Function for switching screens with buttons
        public void PreviousScreen()
        {
            if (_currentPage > 0)
            {
                if (!_lerp) StartScreenChange();

                _lerp = true;
                CurrentPage = _currentPage - 1;
                GetPositionforPage(_currentPage, ref _lerp_target);
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
                CurrentPage = screenIndex;
                GetPositionforPage(_currentPage, ref _lerp_target);

            }
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
            CurrentPage = GetPageforPosition(_screensContainer.localPosition);
            GetPositionforPage(_currentPage, ref _lerp_target);
            ChangeBulletsInfo(_currentPage);
        }

        //changes the bullets on the bottom of the page - pagination
        internal void ChangeBulletsInfo(int targetScreen)
        {
            if (Pagination)
                for (int i = 0; i < Pagination.transform.childCount; i++)
                {
                    Pagination.transform.GetChild(i).GetComponent<Toggle>().isOn = (targetScreen == i)
                        ? true
                        : false;
                }
        }

        private void OnValidate()
        {
            var childCount = ChildObjects == null ? _screensContainer.childCount : ChildObjects.Length;
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

        internal void ScreenChange(int previousScreen)
        {

            OnSelectionPageChangedEvent.Invoke(_currentPage);
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