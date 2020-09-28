/// Credit BinaryX, SimonDarksideJ 
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-1945602
/// Updated by SimonDarksideJ - removed dependency on a custom ScrollRect script. Now implements drag interfaces and standard Scroll Rect.
/// Updated by SimonDarksideJ - major refactoring on updating current position and scroll management

using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(ScrollRect))]
    [AddComponentMenu("Layout/Extensions/Vertical Scroll Snap")]
    public class VerticalScrollSnap : ScrollSnapBase
    {
        private bool updated = true;

        void Start()
        {
            _isVertical = true;
            _childAnchorPoint = new Vector2(0.5f,0);
            _currentPage = StartingScreen;
            panelDimensions = gameObject.GetComponent<RectTransform>().rect;
            UpdateLayout();
        }

        void Update()
        {
            updated = false;

            if (!_lerp && _scroll_rect.velocity == Vector2.zero)
            {
                if (!_settled && !_pointerDown)
                {
                    if (!IsRectSettledOnaPage(_screensContainer.anchoredPosition))
                    {
                        ScrollToClosestElement();
                    }
                }
                return;
            }
            else if (_lerp)
            {
                _screensContainer.anchoredPosition = Vector3.Lerp(_screensContainer.anchoredPosition, _lerp_target, transitionSpeed * (UseTimeScale ? Time.deltaTime : Time.unscaledDeltaTime));
                if (Vector3.Distance(_screensContainer.anchoredPosition, _lerp_target) < 0.1f)
                {
                    _screensContainer.anchoredPosition = _lerp_target;
                    _lerp = false;
                    EndScreenChange();
                }
            }

            if (UseHardSwipe) return;

            CurrentPage = GetPageforPosition(_screensContainer.anchoredPosition);

            //If the container is moving check if it needs to settle on a page
            if (!_pointerDown)
            {
                if (_scroll_rect.velocity.y > 0.01 || _scroll_rect.velocity.y < -0.01)
            {
                    // if the pointer is released and is moving slower than the threshold, then just land on a page
                    if (IsRectMovingSlowerThanThreshold(0))
                    {
                        ScrollToClosestElement();
                    }
                }
            }
        }

        private bool IsRectMovingSlowerThanThreshold(float startingSpeed)
        {
            return (_scroll_rect.velocity.y > startingSpeed && _scroll_rect.velocity.y < SwipeVelocityThreshold) ||
                                (_scroll_rect.velocity.y < startingSpeed && _scroll_rect.velocity.y > -SwipeVelocityThreshold);
        }

        public void DistributePages()
        {
            _screens = _screensContainer.childCount;
            _scroll_rect.verticalNormalizedPosition = 0;

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
                child.anchorMin = child.anchorMax = child.pivot = _childAnchorPoint;
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
            AddChild(GO, false);
        }

        /// <summary>
        /// Add a new child to this Scroll Snap and recalculate it's children
        /// </summary>
        /// <param name="GO">GameObject to add to the ScrollSnap</param>
        /// <param name="WorldPositionStays">Should the world position be updated to it's parent transform?</param>
        public void AddChild(GameObject GO, bool WorldPositionStays)
        {
            _scroll_rect.verticalNormalizedPosition = 0;
            GO.transform.SetParent(_screensContainer, WorldPositionStays);
            InitialiseChildObjectsFromScene();
            DistributePages();
            if (MaskArea) UpdateVisible();

            SetScrollContainerPosition();
        }

        /// <summary>
        /// Remove a new child to this Scroll Snap and recalculate it's children 
        /// *Note, this is an index address (0-x)
        /// </summary>
        /// <param name="index">Index element of child to remove</param>
        /// <param name="ChildRemoved">Resulting removed GO</param>
        public void RemoveChild(int index, out GameObject ChildRemoved)
        {
            RemoveChild(index, false, out ChildRemoved);
        }

        /// <summary>
        /// Remove a new child to this Scroll Snap and recalculate it's children 
        /// *Note, this is an index address (0-x)
        /// </summary>
        /// <param name="index">Index element of child to remove</param>
        /// <param name="WorldPositionStays">If true, the parent-relative position, scale and rotation are modified such that the object keeps the same world space position, rotation and scale as before</param>
        /// <param name="ChildRemoved">Resulting removed GO</param>
        public void RemoveChild(int index, bool WorldPositionStays, out GameObject ChildRemoved)
        {
            ChildRemoved = null;
            if (index < 0 || index > _screensContainer.childCount)
            {
                return;
            }
            _scroll_rect.verticalNormalizedPosition = 0;

            Transform child = _screensContainer.transform.GetChild(index);
            child.SetParent(null, WorldPositionStays);
            ChildRemoved = child.gameObject;
            InitialiseChildObjectsFromScene();
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
        /// <param name="ChildrenRemoved">Array of child GO's removed</param>
        public void RemoveAllChildren(out GameObject[] ChildrenRemoved)
        {
            RemoveAllChildren(false, out ChildrenRemoved);
        }

        /// <summary>
        /// Remove all children from this ScrollSnap
        /// </summary>
        /// <param name="WorldPositionStays">If true, the parent-relative position, scale and rotation are modified such that the object keeps the same world space position, rotation and scale as before</param>
        /// <param name="ChildrenRemoved">Array of child GO's removed</param>
        public void RemoveAllChildren(bool WorldPositionStays, out GameObject[] ChildrenRemoved)
        {
            var _screenCount = _screensContainer.childCount;
            ChildrenRemoved = new GameObject[_screenCount];

            for (int i = _screenCount - 1; i >= 0; i--)
            {
                ChildrenRemoved[i] = _screensContainer.GetChild(i).gameObject;
                ChildrenRemoved[i].transform.SetParent(null, WorldPositionStays);
            }

            _scroll_rect.verticalNormalizedPosition = 0;
            CurrentPage = 0;
            InitialiseChildObjectsFromScene();
            DistributePages();
            if (MaskArea) UpdateVisible();
        }

        private void SetScrollContainerPosition()
        {
            _scrollStartPosition = _screensContainer.anchoredPosition.y;
            _scroll_rect.verticalNormalizedPosition = (float)(_currentPage) / (_screens - 1);
            OnCurrentScreenChange(_currentPage);
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
            OnCurrentScreenChange(_currentPage);
        }

        private void OnRectTransformDimensionsChange()
        {
            if (_childAnchorPoint != Vector2.zero)
            {
                UpdateLayout();
            }
        }

        private void OnEnable()
        {
            InitialiseChildObjectsFromScene();
            DistributePages();
            if (MaskArea)
                UpdateVisible();

            if (JumpOnEnable || !RestartOnEnable)
                SetScrollContainerPosition();
            if (RestartOnEnable)
                GoToScreen(StartingScreen);
        }

        /// <summary>
        /// Release screen to swipe
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (updated)
            {
                return;
            }

            // to prevent double dragging, only act on EndDrag once per frame
            updated = true;

            _pointerDown = false;

            if (_scroll_rect.vertical)
            {
                if (UseSwipeDeltaThreshold && Math.Abs(eventData.delta.y) < SwipeDeltaThreshold)
                {
                    ScrollToClosestElement();
                }
                else
                {
                    var distance = Vector3.Distance(_startPosition, _screensContainer.anchoredPosition);
                    if (UseHardSwipe)
                    {
                        _scroll_rect.velocity = Vector3.zero;

                        if (distance > FastSwipeThreshold)
                        {
                            if (_startPosition.y - _screensContainer.anchoredPosition.y > 0)
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
                    else
                    {
                        if (UseFastSwipe && distance < panelDimensions.height + FastSwipeThreshold && distance >= 1f)
                        {
                            _scroll_rect.velocity = Vector3.zero;
                            if (_startPosition.y - _screensContainer.anchoredPosition.y > 0)
                            {
                                if (_startPosition.y - _screensContainer.anchoredPosition.y > _childSize / 3)
                                {
                                    ScrollToClosestElement();
                                }
                                else
                                {
                                    NextScreen();
                                }
                            }
                            else
                            {
                                if (_startPosition.y - _screensContainer.anchoredPosition.y > -_childSize / 3)
                                {
                                    ScrollToClosestElement();
                                }
                                else
                                {
                                    PreviousScreen();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
