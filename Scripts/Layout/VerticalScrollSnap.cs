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
    public class VerticalScrollSnap : ScrollSnapBase, IEndDragHandler
    {
        void Start()
        {
            isVertical = true;
            DistributePages();
            CalculateVisible();
            _lerp = false;
            _currentScreen = StartingScreen - 1;
            _scrollStartPosition = _screensContainer.localPosition.y;
            _scroll_rect.verticalNormalizedPosition = (float)(_currentScreen) / (float)(_screens - 1);
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
            //else if ((_scroll_rect.velocity.y > 0 && _scroll_rect.velocity.y > SwipeVelocityThreshold) ||
            //    _scroll_rect.velocity.y < 0 && _scroll_rect.velocity.y < -SwipeVelocityThreshold)
            //{
                CurrentPage = GetPageforPosition(_screensContainer.localPosition);
            //}
            if(!_pointerDown)
            {
                ScrollToClosestElement();
            }
        }

        //used for changing between screen resolutions
        public void DistributePages()
        {
            _screens = _screensContainer.childCount;

            float _offset = 0;
            float _dimension = 0;
            Rect panelDimensions = gameObject.GetComponent<RectTransform>().rect;
            float currentYPosition = 0;
            var pageStepValue = _childSize = (int)panelDimensions.height * ((PageStep == 0) ? 3 : PageStep);

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
                CurrentPage = _screens - 1;
            }

            _scroll_rect.verticalNormalizedPosition = (float)(_currentScreen) / (_screens - 1);
        }


        #region Interfaces
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

        #endregion
    }
}