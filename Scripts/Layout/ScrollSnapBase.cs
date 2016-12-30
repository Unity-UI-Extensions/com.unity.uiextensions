using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    public class ScrollSnapBase : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        internal RectTransform _screensContainer;
        internal bool isVertical;

        internal int _screens = 1;

        internal float _scrollStartPosition;
        internal float _childSize;
        private float _childPos;
        internal Vector2 childAnchorPoint;
        internal ScrollRect _scroll_rect;
        internal Vector3 _lerp_target;
        internal bool _lerp;
        internal Vector3 _startPosition = new Vector3();
        [Tooltip("The currently active page")]
        internal int _currentPage;
        internal int _previousPage;
        internal int HalfNoVisibleItems;

        [Serializable]
        public class SelectionChangeStartEvent : UnityEvent { }
        [Serializable]
        public class SelectionPageChangedEvent : UnityEvent<int> { }
        [Serializable]
        public class SelectionChangeEndEvent : UnityEvent<int> { }

        [Tooltip("The screen / page to start the control on\n*Note, this is a 0 indexed array")]
        [SerializeField]
        public int StartingScreen = 0;

        [Tooltip("The distance between two pages based on page height, by default pages are next to each other")]
        [SerializeField]
        [Range(1, 8)]
        public float PageStep = 1;

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

        [Tooltip("The visible bounds area, controls which items are visible/enabled. *Note Should use a RectMask. (optional)")]
        public RectTransform MaskArea;

        [Tooltip("Pixel size to buffer arround Mask Area. (optional)")]
        public float MaskBuffer = 1;
        
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
                    if(MaskArea) UpdateVisible();
                    ScreenChange();
                    ChangeBulletsInfo(_currentPage);
                }
            }
        }

        [Tooltip("(Experimental)\nBy default, child array objects will use the parent transform\nHowever you can disable this for some interesting effects")]
        public bool UseParentTransform = true;

        [Tooltip("Scroll Snap children. (optional)\nEither place objects in the scene as children OR\nPrefabs in this array, NOT BOTH")]
        public GameObject[] ChildObjects;
        
        [SerializeField]
        private SelectionChangeStartEvent m_OnSelectionChangeStartEvent = new SelectionChangeStartEvent();
        public SelectionChangeStartEvent OnSelectionChangeStartEvent { get { return m_OnSelectionChangeStartEvent; } set { m_OnSelectionChangeStartEvent = value; } }

        [SerializeField]
        private SelectionPageChangedEvent m_OnSelectionPageChangedEvent = new SelectionPageChangedEvent();
        public SelectionPageChangedEvent OnSelectionPageChangedEvent { get { return m_OnSelectionPageChangedEvent; } set { m_OnSelectionPageChangedEvent = value; } }

        [SerializeField]
        private SelectionChangeEndEvent m_OnSelectionChangeEndEvent = new SelectionChangeEndEvent();
        public SelectionChangeEndEvent OnSelectionChangeEndEvent { get { return m_OnSelectionChangeEndEvent; } set { m_OnSelectionChangeEndEvent = value; } }


        // Use this for initialization
        void Awake()
        {
            _scroll_rect = gameObject.GetComponent<ScrollRect>();

            if (_scroll_rect.horizontalScrollbar || _scroll_rect.verticalScrollbar)
            {
                Debug.LogWarning("Warning, using scrollbars with the Scroll Snap controls is not advised as it causes unpredictable results");
            }

            if (StartingScreen < 0)
            {
                StartingScreen = 0;
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
            RectTransform childRect;
            GameObject child;
            for (int i = 0; i < childCount; i++)
            {
                child = GameObject.Instantiate(ChildObjects[i]);
                //Optionally, use original GO transform when initialising, by default will use parent RectTransform position/rotation
                if (UseParentTransform)
                {
                    childRect = child.GetComponent<RectTransform>();
                    childRect.rotation = _screensContainer.rotation;
                    childRect.localScale = _screensContainer.localScale;
                    childRect.position = _screensContainer.position;
                }
                child.transform.SetParent(_screensContainer.transform);
                ChildObjects[i] = child;
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
            //If there are no objects in the scene, exit
            if (ChildObjects == null || ChildObjects.Length < 1 || _screensContainer.childCount < 1) return;

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
            if (_screensContainer || ChildObjects != null)
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
            if (MaskBuffer <= 0)
            {
                MaskBuffer = 1;
            }
        }

        /// <summary>
        /// Event fires when the user starts to change the page, either via swipe or button
        /// </summary>
        internal void StartScreenChange()
        {
            OnSelectionChangeStartEvent.Invoke();
        }

        /// <summary>
        /// Event fires when the currently viewed page changes, also updates while the scroll is moving
        /// </summary>
        internal void ScreenChange()
        {
            OnSelectionPageChangedEvent.Invoke(_currentPage);
        }

        /// <summary>
        /// Event fires when control settles on a page, outputs the new page number
        /// </summary>
        internal void EndScreenChange()
        {
            OnSelectionChangeEndEvent.Invoke(_currentPage);
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
        #endregion
    }
}