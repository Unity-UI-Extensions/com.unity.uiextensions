/// Credit BinaryX, SimonDarksideJ 
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-1945602
/// Updated by SimonDarksideJ - removed dependency on a custom ScrollRect script. Now implements drag interfaces and standard Scroll Rect.
/// Updated by SimonDarksideJ - major refactoring on updating current position and scroll management

using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(ScrollRect))]
    [AddComponentMenu("Layout/Extensions/Vertical Scroll Snap")]
    public class VerticalScrollSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        private Transform _screensContainer;

        private int _screens = 1;

        private Vector3[] _positions;
        private Vector3[] _visiblePositions;
        private ScrollRect _scroll_rect;
        private Vector3 _lerp_target;
        private bool _lerp;
        private bool _pointerDown = false;

        [Serializable]
        public class SelectionChangeStartEvent : UnityEvent { }
        [Serializable]
        public class SelectionChangeEndEvent : UnityEvent { }

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

        private Vector3 _startPosition = new Vector3();

        [Tooltip("The currently active page")]
        private int _currentScreen;
        private int _previousScreen;

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

        // Use this for initialization
        void Awake()
        {
            _scroll_rect = gameObject.GetComponent<ScrollRect>();

            if (_scroll_rect.horizontalScrollbar || _scroll_rect.verticalScrollbar)
            {
                Debug.LogWarning("Warning, using scrollbars with the Scroll Snap controls is not advised as it causes unpredictable results");
            }

            _screensContainer = _scroll_rect.content;

            DistributePages();

            if (NextButton)
                NextButton.GetComponent<Button>().onClick.AddListener(() => { NextScreen(); });

            if (PrevButton)
                PrevButton.GetComponent<Button>().onClick.AddListener(() => { PreviousScreen(); });
        }

        void Start()
        {
            UpdateChildPositions();
            _lerp = false;
            _currentScreen = StartingScreen - 1;

            _scroll_rect.verticalNormalizedPosition = (float)(_currentScreen) / (float)(_screens - 1);

            ChangeBulletsInfo(_currentScreen);
        }

        void Update()
        {
            //Three Use cases:
            //1: Swipe Next - FastSwipeNextPrev
            //2: Swipe next while in motion - FastSwipeNextPrev
            //3: Swipe to end - default

            //If lerping, NOT swiping and (!fastswipenextprev & velocity < 200)
            //Aim is to settle on target "page"
            if (!_lerp && _scroll_rect.velocity == Vector2.zero)
            {
                return;
            }
            else if (_lerp)
            {
                _screensContainer.localPosition = Vector3.Lerp(_screensContainer.localPosition, _lerp_target, transitionSpeed * Time.deltaTime);
                if (Vector3.Distance(_screensContainer.localPosition, _lerp_target) < 0.1f)
                {
                    _lerp = false;
                    EndScreenChange();
                }
            }
            //If the container is moving faster than the threshold, then just update the pages as they pass
            else if ((_scroll_rect.velocity.y > 0 && _scroll_rect.velocity.y > SwipeVelocityThreshold) ||
                _scroll_rect.velocity.y < 0 && _scroll_rect.velocity.y < -SwipeVelocityThreshold)
            {
                _currentScreen = GetPageforPosition(FindClosestFrom(_screensContainer.localPosition, _visiblePositions));
                if (_currentScreen != _previousScreen)
                {
                    _previousScreen = _currentScreen;
                    ChangeBulletsInfo(_currentScreen);
                }
            }
            else if(!_pointerDown)
            {
                ScrollToClosestElement();
            }
        }

        //Function for switching screens with buttons
        public void NextScreen()
        {
            if (_currentScreen < _screens - 1)
            {
                if (!_lerp) StartScreenChange();

                _lerp = true;
                _currentScreen++;
                _lerp_target = _positions[_currentScreen];

                ChangeBulletsInfo(_currentScreen);
            }
        }

        //Function for switching screens with buttons
        public void PreviousScreen()
        {
            if (_currentScreen > 0)
            {
                if(!_lerp) StartScreenChange();

                _lerp = true;
                _currentScreen--;
                _lerp_target = _positions[_currentScreen];

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
                _lerp_target = _positions[_currentScreen];

                ChangeBulletsInfo(_currentScreen);
            }
        }

        //find the closest registered point to the releasing point
        private Vector3 FindClosestFrom(Vector3 start, Vector3[] positions)
        {
            Vector3 closestPosition = Vector3.zero;
            float closest = Mathf.Infinity;
            float distanceToTarget = 0;

            for (int i = 0; i < _screens; i++)
            {
                distanceToTarget = Vector3.Distance(start, positions[i]);
                if (distanceToTarget < closest)
                {
                    closest = distanceToTarget;
                    closestPosition = positions[i];
                }
            }

            return closestPosition;
        }

        //changes the bullets on the bottom of the page - pagination
        private void ChangeBulletsInfo(int currentScreen)
        {
            if (Pagination)
                for (int i = 0; i < Pagination.transform.childCount; i++)
                {
                    Pagination.transform.GetChild(i).GetComponent<Toggle>().isOn = (currentScreen == i)
                        ? true
                        : false;
                }
        }

        //used for changing between screen resolutions
        public void DistributePages()
        {
            float _offset = 0;
            float _dimension = 0;
            Rect panelDimensions = gameObject.GetComponent<RectTransform>().rect;
            float currentYPosition = 0;
            var pageStepValue = (int)panelDimensions.height * ((PageStep == 0) ? 3 : PageStep);

            for (int i = 0; i < _screensContainer.transform.childCount; i++)
            {
                RectTransform child = _screensContainer.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
                currentYPosition = _offset + i * pageStepValue;
                child.sizeDelta = new Vector2(panelDimensions.width, panelDimensions.height);
                child.anchoredPosition = new Vector2(0f, currentYPosition);
                child.anchorMin = new Vector2(child.anchorMin.x, 0f);
                child.anchorMax = new Vector2(child.anchorMax.x, 0f);
                child.pivot = new Vector2(child.pivot.x, 0f);
            }

            _dimension = currentYPosition + _offset * -1;

            _screensContainer.GetComponent<RectTransform>().offsetMax = new Vector2(0f, _dimension);
        }

        void UpdateChildPositions()
        {
            _screens = _screensContainer.childCount;

            _positions = new Vector3[_screens];

            if (_screens > 0)
            {
                for (int i = 0; i < _screens; ++i)
                {
                    _scroll_rect.verticalNormalizedPosition = (float)i / (float)(_screens - 1);
                    _positions[i] = _screensContainer.localPosition;
                }
            }

            //debug visible
            _visiblePositions = _positions;
        }
        
        int GetPageforPosition(Vector3 pos)
        {
            for (int i = 0; i < _positions.Length; i++)
            {
                if (_positions[i] == pos)
                {
                    return i;
                }
            }
            return 0;
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

        /// <summary>
        /// Add a new child to this Scroll Snap and recalculate it's children
        /// </summary>
        /// <param name="GO">GameObject to add to the ScrollSnap</param>
        public void AddChild(GameObject GO)
        {
            _scroll_rect.verticalNormalizedPosition = 0;
            GO.transform.SetParent(_screensContainer);
            DistributePages();

            _scroll_rect.verticalNormalizedPosition = (float)(_currentScreen) / (_screens - 1);
        }

        /// <summary>
        /// Remove a new child to this Scroll Snap and recalculate it's children 
        /// *Note, this is an index address (0-x)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="ChildRemoved"></param>
        public void RemoveChild(int index, out GameObject ChildRemoved)
        {
            ChildRemoved = null;
            if (index < 0 || index > _screensContainer.childCount)
            {
                return;
            }
            _scroll_rect.verticalNormalizedPosition = 0;
            var children = _screensContainer.transform;
            int i = 0;
            foreach (Transform child in children)
            {
                if (i == index)
                {
                    child.SetParent(null);
                    ChildRemoved = child.gameObject;
                    break;
                }
                i++;
            }
            DistributePages();
            if (_currentScreen > _screens - 1)
            {
                _currentScreen = _screens - 1;
            }

            _scroll_rect.verticalNormalizedPosition = (float)(_currentScreen) / (_screens - 1);
        }

        private void StartScreenChange()
        {
            OnSelectionChangeStartEvent.Invoke();
        }

        private void EndScreenChange()
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
        /// Release screen to swipe
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (_scroll_rect.vertical)
            {
                if (UseFastSwipe)
                {
                    //If using fastswipe - then a swipe does page next / previous
                    if (_scroll_rect.velocity.y > SwipeVelocityThreshold)
                    {
                        _scroll_rect.velocity = Vector3.zero;
                        if (_startPosition.y - _screensContainer.localPosition.y > 0)
                        {
                            NextScreen();
                        }
                        else
                        {
                            PreviousScreen();
                        }
                    }
                    else
                    {
                        ScrollToClosestElement();
                    }
                }
            }
        }

        private void ScrollToClosestElement()
        {
            _lerp = true;
            _lerp_target = FindClosestFrom(_screensContainer.localPosition, _visiblePositions);
            _currentScreen = GetPageforPosition(_lerp_target);
            ChangeBulletsInfo(_currentScreen);
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