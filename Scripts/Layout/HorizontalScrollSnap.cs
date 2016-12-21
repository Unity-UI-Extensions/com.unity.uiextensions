/// Credit BinaryX 
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-1945602
/// Updated by ddreaper - removed dependency on a custom ScrollRect script. Now implements drag interfaces and standard Scroll Rect.

using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{

    [RequireComponent(typeof(ScrollRect))]
    [AddComponentMenu("Layout/Extensions/Horizontal Scroll Snap")]
    public class HorizontalScrollSnap : ScrollSnapBase, IEndDragHandler
    {
        void Start()
        {
            isVertical = false;
            DistributePages();
            if(MaskArea) CalculateVisible();
            _lerp = false;
            _currentPage = StartingScreen - 1;
            _scrollStartPosition = _screensContainer.localPosition.x;
            _scroll_rect.horizontalNormalizedPosition = (float)(_currentPage) / (_screens - 1);
        }

        void Update()
        {
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

            CurrentPage = GetPageforPosition(_screensContainer.localPosition);
            
            //If the container is moving check if it needs to settle on a page
            if (!_pointerDown && (_scroll_rect.velocity.x > 0.01 || _scroll_rect.velocity.x < 0.01))
            {
                // if the pointer is released and is moving slower than the threshold, then just land on a page
                if ((_scroll_rect.velocity.x > 0 && _scroll_rect.velocity.x < SwipeVelocityThreshold) ||
                    (_scroll_rect.velocity.x < 0 && _scroll_rect.velocity.x > -SwipeVelocityThreshold))
                {
                    ScrollToClosestElement();
                }
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

            _scroll_rect.horizontalNormalizedPosition = (float)(_currentPage) / (_screens - 1);
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

            if (_currentPage > _screens - 1)
            {
                CurrentPage = _screens - 1;
            }

            _scroll_rect.horizontalNormalizedPosition = (float)(_currentPage) / (_screens - 1);
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
                    if ((_scroll_rect.velocity.x > 0 &&_scroll_rect.velocity.x > FastSwipeThreshold) ||
                        _scroll_rect.velocity.x < 0 && _scroll_rect.velocity.x < -FastSwipeThreshold)
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