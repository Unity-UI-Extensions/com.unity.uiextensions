/// Credit BinaryX 
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-1945602
/// Updated by ddreaper - removed dependency on a custom ScrollRect script. Now implements drag interfaces and standard Scroll Rect.

using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{

    [RequireComponent(typeof(ScrollRect))]
    [AddComponentMenu("Layout/Extensions/Horizontal Scroll Snap")]
    public class HorizontalScrollSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private Transform _screensContainer;

        private int _screens = 1;

        private bool _fastSwipeTimer = false;
        private int _fastSwipeCounter = 0;
        private int _fastSwipeTarget = 30;


        private System.Collections.Generic.List<Vector3> _positions;
        private ScrollRect _scroll_rect;
        private Vector3 _lerp_target;
        private bool _lerp;

        [Tooltip("The gameobject that contains toggles which suggest pagination. (optional)")]
        public GameObject Pagination;

        [Tooltip("Button to go to the next page. (optional)")]
        public GameObject NextButton;
        [Tooltip("Button to go to the previous page. (optional)")]
        public GameObject PrevButton;
        [Tooltip("Transition speed between pages. (optional)")]
        public float transitionSpeed = 7.5f;

        public Boolean UseFastSwipe = true;
        public int FastSwipeThreshold = 100;

        private bool _startDrag = true;
        private Vector3 _startPosition = new Vector3();

        [Tooltip("The currently active page")]
        [SerializeField]
        private int _currentScreen;

        [Tooltip("The screen / page to start the control on")]
        public int StartingScreen = 1;

        [Tooltip("The distance between two pages, by default 3 times the height of the control")]
        public int PageStep = 0;

        public int CurrentPage
        {
            get
            {
                return _currentScreen;
            }
        }



        // Use this for initialization
        void Start()
        {
            _scroll_rect = gameObject.GetComponent<ScrollRect>();

            if (_scroll_rect.horizontalScrollbar || _scroll_rect.verticalScrollbar)
            {
                Debug.LogWarning("Warning, using scrollbors with the Scroll Snap controls is not advised as it causes unpredictable results");
            }

            _screensContainer = _scroll_rect.content;
            if (PageStep == 0)
            {
                PageStep = (int)_scroll_rect.GetComponent<RectTransform>().rect.width * 3;
            }
            DistributePages();

            _lerp = false;
            _currentScreen = StartingScreen;

            _scroll_rect.horizontalNormalizedPosition = (float)(_currentScreen - 1) / (_screens - 1);

            ChangeBulletsInfo(_currentScreen);

            if (NextButton)
                NextButton.GetComponent<Button>().onClick.AddListener(() => { NextScreen(); });

            if (PrevButton)
                PrevButton.GetComponent<Button>().onClick.AddListener(() => { PreviousScreen(); });
        }

        void Update()
        {
            if (_lerp)
            {
                _screensContainer.localPosition = Vector3.Lerp(_screensContainer.localPosition, _lerp_target, transitionSpeed * Time.deltaTime);
                if (Vector3.Distance(_screensContainer.localPosition, _lerp_target) < 0.005f)
                {
                    _lerp = false;
                }

                //change the info bullets at the bottom of the screen. Just for visual effect
                if (Vector3.Distance(_screensContainer.localPosition, _lerp_target) < 10f)
                {
                    ChangeBulletsInfo(CurrentScreen());
                }
            }

            if (_fastSwipeTimer)
            {
                _fastSwipeCounter++;
            }
        }

        private bool fastSwipe = false; //to determine if a fast swipe was performed


        //Function for switching screens with buttons
        public void NextScreen()
        {
            if (_currentScreen < _screens - 1)
            {
                _currentScreen++;
                _lerp = true;
                _lerp_target = _positions[_currentScreen];

                ChangeBulletsInfo(_currentScreen);
            }
        }

        //Function for switching screens with buttons
        public void PreviousScreen()
        {
            if (_currentScreen > 0)
            {
                _currentScreen--;
                _lerp = true;
                _lerp_target = _positions[_currentScreen];

                ChangeBulletsInfo(_currentScreen);
            }
        }

        //Function for switching to a specific screen
        public void GoToScreen(int screenIndex)
        {
            if (screenIndex <= _screens && screenIndex >= 0)
            {
                _lerp = true;
                _lerp_target = _positions[screenIndex];

                ChangeBulletsInfo(screenIndex);
            }
        }

        //Because the CurrentScreen function is not so reliable, these are the functions used for swipes
        private void NextScreenCommand()
        {
            if (_currentScreen < _screens - 1)
            {
                _lerp = true;
                _lerp_target = _positions[_currentScreen + 1];

                ChangeBulletsInfo(_currentScreen + 1);
            }
        }

        //Because the CurrentScreen function is not so reliable, these are the functions used for swipes
        private void PrevScreenCommand()
        {
            if (_currentScreen > 0)
            {
                _lerp = true;
                _lerp_target = _positions[_currentScreen - 1];

                ChangeBulletsInfo(_currentScreen - 1);
            }
        }


        //find the closest registered point to the releasing point
        private Vector3 FindClosestFrom(Vector3 start, System.Collections.Generic.List<Vector3> positions)
        {
            Vector3 closest = Vector3.zero;
            float distance = Mathf.Infinity;

            foreach (Vector3 position in _positions)
            {
                if (Vector3.Distance(start, position) < distance)
                {
                    distance = Vector3.Distance(start, position);
                    closest = position;
                }
            }

            return closest;
        }


        //returns the current screen that the is seeing
        public int CurrentScreen()
        {
            var pos = FindClosestFrom(_screensContainer.localPosition, _positions);
            return _currentScreen = GetPageforPosition(pos);
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
        private void DistributePages()
        {
            int _offset = 0;
            int _dimension = 0;
            Vector2 panelDimensions = gameObject.GetComponent<RectTransform>().sizeDelta;
            int currentXPosition = 0;

            for (int i = 0; i < _screensContainer.transform.childCount; i++)
            {
                RectTransform child = _screensContainer.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
                currentXPosition = _offset + i * PageStep;
                child.sizeDelta = new Vector2(panelDimensions.x, panelDimensions.y);
                child.anchoredPosition = new Vector2(currentXPosition, 0f);
            }

            _dimension = currentXPosition + _offset * -1;

            _screensContainer.GetComponent<RectTransform>().offsetMax = new Vector2(_dimension, 0f);

            _screens = _screensContainer.childCount;

            _positions = new System.Collections.Generic.List<Vector3>();

            if (_screens > 0)
            {
                for (float i = 0; i < _screens; ++i)
                {
                    _scroll_rect.horizontalNormalizedPosition = i / (_screens - 1);
                    _positions.Add(_screensContainer.localPosition);
                }
            }
        }

        int GetPageforPosition(Vector3 pos)
        {
            for (int i = 0; i < _positions.Count; i++)
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
            if (StartingScreen > childCount)
            {
                StartingScreen = childCount;
            }
            if (StartingScreen < 1)
            {
                StartingScreen = 1;
            }
        }

        /// <summary>
        /// Add a new child to this Scroll Snap and recalculate it's children
        /// </summary>
        /// <param name="GO">GameObject to add to the ScrollSnap</param>
        public void AddChild(GameObject GO)
        {
            _scroll_rect.horizontalNormalizedPosition = 0;
            GO.transform.SetParent(_screensContainer);
            DistributePages();

            _scroll_rect.horizontalNormalizedPosition = (float)(_currentScreen) / (_screens - 1);
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
            _scroll_rect.horizontalNormalizedPosition = 0;
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

            _scroll_rect.horizontalNormalizedPosition = (float)(_currentScreen) / (_screens - 1);
        }



        #region Interfaces
        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPosition = _screensContainer.localPosition;
            _fastSwipeCounter = 0;
            _fastSwipeTimer = true;
            _currentScreen = CurrentScreen();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _startDrag = true;
            if (_scroll_rect.horizontal)
            {
                if (UseFastSwipe)
                {
                    fastSwipe = false;
                    _fastSwipeTimer = false;
                    if (_fastSwipeCounter <= _fastSwipeTarget)
                    {
                        if (Math.Abs(_startPosition.x - _screensContainer.localPosition.x) > FastSwipeThreshold)
                        {
                            fastSwipe = true;
                        }
                    }
                    if (fastSwipe)
                    {
                        if (_startPosition.x - _screensContainer.localPosition.x > 0)
                        {
                            NextScreenCommand();
                        }
                        else
                        {
                            PrevScreenCommand();
                        }
                    }
                    else
                    {
                        _lerp = true;
                        _lerp_target = FindClosestFrom(_screensContainer.localPosition, _positions);
                        _currentScreen = GetPageforPosition(_lerp_target);
                    }
                }
                else
                {
                    _lerp = true;
                    _lerp_target = FindClosestFrom(_screensContainer.localPosition, _positions);
                    _currentScreen = GetPageforPosition(_lerp_target);
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            _lerp = false;
            if (_startDrag)
            {
                OnBeginDrag(eventData);
                _startDrag = false;
            }
        }
        #endregion


    }
}