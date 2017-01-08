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
            _isVertical = false;
            _childAnchorPoint = new Vector2(0, 0.5f);
            _currentPage = StartingScreen;
            UpdateLayout();
        }

        void Update()
        {
            if (!_lerp && _scroll_rect.velocity == Vector2.zero)
            {
                if (!_settled && !_pointerDown)
                {
                    if (!IsRectSettledOnaPage(_screensContainer.localPosition))
                    {
                        ScrollToClosestElement();
                    }
                }
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
            if (!_pointerDown)
            {
                if (_scroll_rect.velocity.x > 0.01 || _scroll_rect.velocity.x < 0.01)
                {
                    //if the pointer is released and is moving slower than the threshold, then just land on a page
                    if (IsRectMovingSlowerThanThreshold(0))
                    {
                        ScrollToClosestElement();
                    }
                }
            }
        }

        private bool IsRectMovingSlowerThanThreshold(float startingSpeed)
        {
            return (_scroll_rect.velocity.x > startingSpeed && _scroll_rect.velocity.x < SwipeVelocityThreshold) ||
                                (_scroll_rect.velocity.x < startingSpeed && _scroll_rect.velocity.x > -SwipeVelocityThreshold);
        }

        private void DistributePages()
        {
            _screens = _screensContainer.childCount;
            _scroll_rect.horizontalNormalizedPosition = 0;

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
                child.anchorMin = child.anchorMax = child.pivot = _childAnchorPoint;
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
            AddChild(GO, false);
        }

        /// <summary>
        /// Add a new child to this Scroll Snap and recalculate it's children
        /// </summary>
        /// <param name="GO">GameObject to add to the ScrollSnap</param>
        /// <param name="WorldPositionStays">Should the world position be updated to it's parent transform?</param>
        public void AddChild(GameObject GO, bool WorldPositionStays)
        {
            _scroll_rect.horizontalNormalizedPosition = 0;
            GO.transform.SetParent(_screensContainer, WorldPositionStays);
            DistributePages();
            if (MaskArea) UpdateVisible();

            SetScrollContainerPosition();
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

            Transform child = _screensContainer.transform.GetChild(index);
            child.SetParent(null);
            ChildRemoved = child.gameObject;

            DistributePages();
            if (MaskArea) UpdateVisible();

            if (_currentPage > _screens - 1)
            {
                CurrentPage = _screens - 1;
            }

            SetScrollContainerPosition();
        }

        /// <summary>
        /// Remove all children from this ScrollSnap
        /// </summary>
        /// <param name="ChildrenRemoved"></param>
        public void RemoveAllChildren(out GameObject[] ChildrenRemoved)
        {
            var _screenCount = _screensContainer.childCount;
            ChildrenRemoved = new GameObject[_screenCount];

            for (int i = _screenCount - 1; i >= 0; i--)
            {
                ChildrenRemoved[i] = _screensContainer.GetChild(i).gameObject;
                ChildrenRemoved[i].transform.SetParent(null);
            }

            _scroll_rect.horizontalNormalizedPosition = 0;
            CurrentPage = 0;
            InitialiseChildObjectsFromScene();
            DistributePages();
            if (MaskArea) UpdateVisible();
        }

        private void SetScrollContainerPosition()
        {
            _scrollStartPosition = _screensContainer.localPosition.x;
            _scroll_rect.horizontalNormalizedPosition = (float)(_currentPage) / (_screens - 1);
        }

        /// <summary>
        /// used for changing / updating between screen resolutions
        /// </summary>
        public void UpdateLayout()
        {
            _lerp = false;
            DistributePages();
            if (MaskArea) UpdateVisible();
            SetScrollContainerPosition();
            ChangeBulletsInfo(_currentPage);
        }

        private void OnRectTransformDimensionsChange()
        {
            if (_childAnchorPoint != Vector2.zero)
            {
                UpdateLayout();
            }
        }

        #region Interfaces
        /// <summary>
        /// Release screen to swipe
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            _pointerDown = false;

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