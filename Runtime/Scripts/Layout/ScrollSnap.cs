/// Credit BinaryX 
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-1945602
/// Updated by ddreaper - removed dependency on a custom ScrollRect script. Now implements drag interfaces and standard Scroll Rect.
/// Update by xesenix - rewrote almost the entire code 
/// - configuration for direction move instead of 2 concurrent class (easier to change direction in editor)
/// - supports list layout with horizontal or vertical layout need to match direction with type of layout used
/// - dynamic checks if scrolled list size changes and recalculates anchor positions 
///   and item size based on itemsVisibleAtOnce and size of root container
///   if you don't wish to use this auto resize turn of autoLayoutItems
/// - fixed current page made it independent from pivot
/// - replaced pagination with delegate function
using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(ScrollRect))]
    [AddComponentMenu("UI/Extensions/Scroll Snap")]
    public class ScrollSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollSnap
    {
        // needed because of reversed behaviour of axis Y compared to X
        // (positions of children lower in children list in horizontal directions grows when in vertical it gets smaller)
        public enum ScrollDirection
        {
            Horizontal,
            Vertical
        }

        private ScrollRect _scroll_rect;

        private RectTransform _scrollRectTransform;

        private Transform _listContainerTransform;

        //private RectTransform _rectTransform;

        private int _pages;

        private int _startingPage = 0;

        // anchor points to lerp to to see child on certain indexes
        private Vector3[] _pageAnchorPositions;

        private Vector3 _lerpTarget;

        private bool _lerp;

        // item list related
        private float _listContainerMinPosition;

        private float _listContainerMaxPosition;

        private float _listContainerSize;

        private RectTransform _listContainerRectTransform;

        private Vector2 _listContainerCachedSize;

        private float _itemSize;

        private int _itemsCount = 0;

        // drag related
        private bool _startDrag = true;

        private Vector3 _positionOnDragStart = new Vector3();

        private int _pageOnDragStart;

        private bool _fastSwipeTimer = false;

        private int _fastSwipeCounter = 0;

        private int _fastSwipeTarget = 10;

        [Tooltip("Button to go to the next page. (optional)")]
        public Button NextButton;

        [Tooltip("Button to go to the previous page. (optional)")]
        public Button PrevButton;

        [Tooltip("Number of items visible in one page of scroll frame.")]
        [RangeAttribute(1, 100)]
        public int ItemsVisibleAtOnce = 1;

        [Tooltip("Sets minimum width of list items to 1/itemsVisibleAtOnce.")]
        public bool AutoLayoutItems = true;

        [Tooltip("If you wish to update scrollbar numberOfSteps to number of active children on list.")]
        public bool LinkScrolbarSteps = false;

        [Tooltip("If you wish to update scrollrect sensitivity to size of list element.")]
        public bool LinkScrolrectScrollSensitivity = false;

        public Boolean UseFastSwipe = true;

        public int FastSwipeThreshold = 100;

        public delegate void PageSnapChange(int page);

        public event PageSnapChange onPageChange;

        public ScrollDirection direction = ScrollDirection.Horizontal;

        // Use this for initialization
        void Start()
        {
            _lerp = false;

            _scroll_rect = gameObject.GetComponent<ScrollRect>();
            _scrollRectTransform = gameObject.GetComponent<RectTransform>();
            _listContainerTransform = _scroll_rect.content;
            _listContainerRectTransform = _listContainerTransform.GetComponent<RectTransform>();

            //_rectTransform = _listContainerTransform.gameObject.GetComponent<RectTransform>();
            UpdateListItemsSize();
            UpdateListItemPositions();

            PageChanged(CurrentPage());

            if (NextButton)
            {
                NextButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    NextScreen();
                });
            }

            if (PrevButton)
            {
                PrevButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    PreviousScreen();
                });
            }
            if (_scroll_rect.horizontalScrollbar != null && _scroll_rect.horizontal)
            {
                
                   var hscroll = _scroll_rect.horizontalScrollbar.gameObject.GetOrAddComponent<ScrollSnapScrollbarHelper>();
                hscroll.ss = this;
            }
            if (_scroll_rect.verticalScrollbar != null && _scroll_rect.vertical)
            {
                var vscroll = _scroll_rect.verticalScrollbar.gameObject.GetOrAddComponent<ScrollSnapScrollbarHelper>();
                vscroll.ss = this;
            }
        }

        public void UpdateListItemsSize()
        {
            float size = 0;
            float currentSize = 0;
            if (direction == ScrollSnap.ScrollDirection.Horizontal)
            {
                size = _scrollRectTransform.rect.width / ItemsVisibleAtOnce;
                currentSize = _listContainerRectTransform.rect.width / _itemsCount;
            }
            else
            {
                size = _scrollRectTransform.rect.height / ItemsVisibleAtOnce;
                currentSize = _listContainerRectTransform.rect.height / _itemsCount;
            }

            _itemSize = size;

            if (LinkScrolrectScrollSensitivity)
            {
                _scroll_rect.scrollSensitivity = _itemSize;
            }

            if (AutoLayoutItems && currentSize != size && _itemsCount > 0)
            {
                if (direction == ScrollSnap.ScrollDirection.Horizontal)
                {
                    foreach (var tr in _listContainerTransform)
                    {
                        GameObject child = ((Transform)tr).gameObject;
                        if (child.activeInHierarchy)
                        {
                            var childLayout = child.GetComponent<LayoutElement>();

                            if (childLayout == null)
                            {
                                childLayout = child.AddComponent<LayoutElement>();
                            }

                            childLayout.minWidth = _itemSize;
                        }
                    }
                }
                else
                {
                    foreach (var tr in _listContainerTransform)
                    {
                        GameObject child = ((Transform)tr).gameObject;
                        if (child.activeInHierarchy)
                        {
                            var childLayout = child.GetComponent<LayoutElement>();

                            if (childLayout == null)
                            {
                                childLayout = child.AddComponent<LayoutElement>();
                            }

                            childLayout.minHeight = _itemSize;
                        }
                    }
                }
            }
        }

        public void UpdateListItemPositions()
        {
            if (!_listContainerRectTransform.rect.size.Equals(_listContainerCachedSize))
            {
                // checking how many children of list are active
                int activeCount = 0;

                foreach (var tr in _listContainerTransform)
                {
                    if (((Transform)tr).gameObject.activeInHierarchy)
                    {
                        activeCount++;
                    }
                }

                // if anything changed since last check reinitialize anchors list
                _itemsCount = 0;
                Array.Resize(ref _pageAnchorPositions, activeCount);

                if (activeCount > 0)
                {
                    _pages = Mathf.Max(activeCount - ItemsVisibleAtOnce + 1, 1);

                    if (direction == ScrollDirection.Horizontal)
                    {
                        // looking for list spanning range min/max
                        _scroll_rect.horizontalNormalizedPosition = 0;
                        _listContainerMaxPosition = _listContainerTransform.localPosition.x;
                        _scroll_rect.horizontalNormalizedPosition = 1;
                        _listContainerMinPosition = _listContainerTransform.localPosition.x;

                        _listContainerSize = _listContainerMaxPosition - _listContainerMinPosition;

                        for (var i = 0; i < _pages; i++)
                        {
                            _pageAnchorPositions[i] = new Vector3(
                                _listContainerMaxPosition - _itemSize * i,
                                _listContainerTransform.localPosition.y,
                                _listContainerTransform.localPosition.z
                            );
                        }
                    }
                    else
                    {
                        //Debug.Log ("-------------looking for list spanning range----------------");
                        // looking for list spanning range
                        _scroll_rect.verticalNormalizedPosition = 1;
                        _listContainerMinPosition = _listContainerTransform.localPosition.y;
                        _scroll_rect.verticalNormalizedPosition = 0;
                        _listContainerMaxPosition = _listContainerTransform.localPosition.y;

                        _listContainerSize = _listContainerMaxPosition - _listContainerMinPosition;

                        for (var i = 0; i < _pages; i++)
                        {
                            _pageAnchorPositions[i] = new Vector3(
                                _listContainerTransform.localPosition.x,
                                _listContainerMinPosition + _itemSize * i,
                                _listContainerTransform.localPosition.z
                            );
                        }
                    }

                    UpdateScrollbar(LinkScrolbarSteps);
                    _startingPage = Mathf.Min(_startingPage, _pages);
                    ResetPage();
                }

                if (_itemsCount != activeCount)
                {
                    PageChanged(CurrentPage());
                }

                _itemsCount = activeCount;
                _listContainerCachedSize.Set(_listContainerRectTransform.rect.size.x, _listContainerRectTransform.rect.size.y);
            }

        }

        public void ResetPage()
        {
            if (direction == ScrollDirection.Horizontal)
            {
                _scroll_rect.horizontalNormalizedPosition = _pages > 1 ? (float)_startingPage / (float)(_pages - 1) : 0;
            }
            else
            {
                _scroll_rect.verticalNormalizedPosition = _pages > 1 ? (float)(_pages - _startingPage - 1) / (float)(_pages - 1) : 0;
            }
        }

        private void UpdateScrollbar(bool linkSteps)
        {
            if (linkSteps)
            {
                if (direction == ScrollDirection.Horizontal)
                {
                    if (_scroll_rect.horizontalScrollbar != null)
                    {
                        _scroll_rect.horizontalScrollbar.numberOfSteps = _pages;
                    }
                }
                else
                {
                    if (_scroll_rect.verticalScrollbar != null)
                    {
                        _scroll_rect.verticalScrollbar.numberOfSteps = _pages;
                    }
                }
            }
            else
            {
                if (direction == ScrollDirection.Horizontal)
                {
                    if (_scroll_rect.horizontalScrollbar != null)
                    {
                        _scroll_rect.horizontalScrollbar.numberOfSteps = 0;
                    }
                }
                else
                {
                    if (_scroll_rect.verticalScrollbar != null)
                    {
                        _scroll_rect.verticalScrollbar.numberOfSteps = 0;
                    }
                }
            }
        }

        void LateUpdate()
        {
            UpdateListItemsSize();
            UpdateListItemPositions();

            if (_lerp)
            {
                UpdateScrollbar(false);

                _listContainerTransform.localPosition = Vector3.Lerp(_listContainerTransform.localPosition, _lerpTarget, 7.5f * Time.deltaTime);

                if (Vector3.Distance(_listContainerTransform.localPosition, _lerpTarget) < 0.001f)
                {
                    _listContainerTransform.localPosition = _lerpTarget;
                    _lerp = false;

                    UpdateScrollbar(LinkScrolbarSteps);
                }

                //change the info bullets at the bottom of the screen. Just for visual effect
                if (Vector3.Distance(_listContainerTransform.localPosition, _lerpTarget) < 10f)
                {
                    PageChanged(CurrentPage());
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
            UpdateListItemPositions();

            if (CurrentPage() < _pages - 1)
            {
                _lerp = true;
                _lerpTarget = _pageAnchorPositions[CurrentPage() + 1];

                PageChanged(CurrentPage() + 1);
            }
        }

        //Function for switching screens with buttons
        public void PreviousScreen()
        {
            UpdateListItemPositions();

            if (CurrentPage() > 0)
            {
                _lerp = true;
                _lerpTarget = _pageAnchorPositions[CurrentPage() - 1];

                PageChanged(CurrentPage() - 1);
            }
        }

        //Because the CurrentScreen function is not so reliable, these are the functions used for swipes
        private void NextScreenCommand()
        {
            if (_pageOnDragStart < _pages - 1)
            {
                int targetPage = Mathf.Min(_pages - 1, _pageOnDragStart + ItemsVisibleAtOnce);
                _lerp = true;

                _lerpTarget = _pageAnchorPositions[targetPage];

                PageChanged(targetPage);
            }
        }

        //Because the CurrentScreen function is not so reliable, these are the functions used for swipes
        private void PrevScreenCommand()
        {
            if (_pageOnDragStart > 0)
            {
                int targetPage = Mathf.Max(0, _pageOnDragStart - ItemsVisibleAtOnce);
                _lerp = true;

                _lerpTarget = _pageAnchorPositions[targetPage];

                PageChanged(targetPage);
            }
        }


        //returns the current screen that the is seeing
        public int CurrentPage()
        {
            float pos;

            if (direction == ScrollDirection.Horizontal)
            {
                pos = _listContainerMaxPosition - _listContainerTransform.localPosition.x;
                pos = Mathf.Clamp(pos, 0, _listContainerSize);
            }
            else
            {
                pos = _listContainerTransform.localPosition.y - _listContainerMinPosition;
                pos = Mathf.Clamp(pos, 0, _listContainerSize);
            }

            float page = pos / _itemSize;

            return Mathf.Clamp(Mathf.RoundToInt(page), 0, _pages);
        }

        /// <summary>
        /// Added to provide a uniform interface for the ScrollBarHelper
        /// </summary>
        public void SetLerp(bool value)
        {
            _lerp = value;
        }

        public void ChangePage(int page)
        {
            if (0 <= page && page < _pages)
            {
                _lerp = true;

                _lerpTarget = _pageAnchorPositions[page];

                PageChanged(page);
            }
        }

        //changes the bullets on the bottom of the page - pagination
        private void PageChanged(int currentPage)
        {
            _startingPage = currentPage;

            if (NextButton)
            {
                NextButton.interactable = currentPage < _pages - 1;
            }

            if (PrevButton)
            {
                PrevButton.interactable = currentPage > 0;
            }

            if (onPageChange != null)
            {
                onPageChange(currentPage);
            }
        }

        #region Interfaces
        public void OnBeginDrag(PointerEventData eventData)
        {
            UpdateScrollbar(false);

            _fastSwipeCounter = 0;
            _fastSwipeTimer = true;

            _positionOnDragStart = eventData.position;
            _pageOnDragStart = CurrentPage();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _startDrag = true;
            float change = 0;

            if (direction == ScrollDirection.Horizontal)
            {
                change = _positionOnDragStart.x - eventData.position.x;
            }
            else
            {
                change = -_positionOnDragStart.y + eventData.position.y;
            }

            if (UseFastSwipe)
            {
                fastSwipe = false;
                _fastSwipeTimer = false;

                if (_fastSwipeCounter <= _fastSwipeTarget)
                {
                    if (Math.Abs(change) > FastSwipeThreshold)
                    {
                        fastSwipe = true;
                    }
                }
                if (fastSwipe)
                {
                    if (change > 0)
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
                    _lerpTarget = _pageAnchorPositions[CurrentPage()];
                }
            }
            else
            {
                _lerp = true;
                _lerpTarget = _pageAnchorPositions[CurrentPage()];
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

        public void StartScreenChange() { }
        #endregion
    }
}