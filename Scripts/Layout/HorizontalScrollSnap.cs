/// Credit BinaryX 
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-1945602
/// Updated by ddreaper - removed dependency on a custom ScrollRect script. Now implements drag interfaces and standard Scroll Rect.

using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{

    [RequireComponent(typeof(ScrollRect))]
    [AddComponentMenu("Layout/Extensions/Horizontal Scroll Snap")]
    public class HorizontalScrollSnap : ScrollSnapBase, IEndDragHandler
    {

        // Use this for initialization
        void Awake()
        {
            _scroll_rect = gameObject.GetComponent<ScrollRect>();

            if (_scroll_rect.horizontalScrollbar || _scroll_rect.verticalScrollbar)
            {
                Debug.LogWarning("Warning, using scrollbars with the Scroll Snap controls is not advised as it causes unpredictable results");
            }

            //Should this derrive from ScrolRect?
            isVertical = false;

            _screensContainer = _scroll_rect.content;

            DistributePages();

            if (NextButton)
                NextButton.GetComponent<Button>().onClick.AddListener(() => { NextScreen(); });

            if (PrevButton)
                PrevButton.GetComponent<Button>().onClick.AddListener(() => { PreviousScreen(); });
        }

        void Start()
        {
            _lerp = false;
            _currentScreen = StartingScreen - 1;
            _scrollStartPosition = _screensContainer.localPosition.x;
            _scroll_rect.horizontalNormalizedPosition = (float)(_currentScreen) / (_screens - 1);

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
            else if ((_scroll_rect.velocity.x > 0 && _scroll_rect.velocity.x > SwipeVelocityThreshold) ||
                _scroll_rect.velocity.x < 0 && _scroll_rect.velocity.x < -SwipeVelocityThreshold)
            {
                _currentScreen = GetPageforPosition(_screensContainer.localPosition);
                if (_currentScreen != _previousScreen)
                {
                    _previousScreen = _currentScreen;
                    ChangeBulletsInfo(_currentScreen);
                }
            }
            else if (!_pointerDown)
            {
                ScrollToClosestElement();
            }
        }

        //used for changing between screen resolutions
        private void DistributePages()
        {
            _screens = _screensContainer.childCount;

            int _offset = 0;
            float _dimension = 0;
            Rect panelDimensions = gameObject.GetComponent<RectTransform>().rect;
            float currentXPosition = 0;
            var pageStepValue = _childSize = (int)panelDimensions.width * ((PageStep == 0) ? 3 : PageStep);


            for (int i = 0; i < _screensContainer.transform.childCount; i++)
            {
                RectTransform child = _screensContainer.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
                currentXPosition = _offset + (int)(i * pageStepValue);
                child.sizeDelta = new Vector2(panelDimensions.width, panelDimensions.height);
                child.anchoredPosition = new Vector2(currentXPosition, 0f);
                child.anchorMin = new Vector2(0f, child.anchorMin.y);
                child.anchorMax = new Vector2(0f, child.anchorMax.y);
                child.pivot = new Vector2(0f, child.pivot.y);
            }

            _dimension = currentXPosition + _offset * -1;

            _screensContainer.GetComponent<RectTransform>().offsetMax = new Vector2(_dimension, 0f);
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
        /// <summary>
        /// Release screen to swipe
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (_scroll_rect.horizontal)
            {
                if (UseFastSwipe)
                {
                    //If using fastswipe - then a swipe does page next / previous
                    if (_scroll_rect.velocity.x > SwipeVelocityThreshold)
                    {
                        _scroll_rect.velocity = Vector3.zero;
                        if (_startPosition.x - _screensContainer.localPosition.x > 0)
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

         #endregion
    }
}