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
    public class ScrollSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        // needed because of reversed behaviour of axis Y compared to X
        // (positions of children lower in children list in horizontal directions grows when in vertical it gets smaller)
        public enum ScrollDirection
        {
            Horizontal,
            Vertical
        }

        public delegate void PageSnapChange(int page);

        public event PageSnapChange onPageChange;

        public ScrollDirection direction = ScrollDirection.Horizontal;

        protected ScrollRect scrollRect;

        protected RectTransform scrollRectTransform;

        protected Transform listContainerTransform;

        protected RectTransform rectTransform;

        int pages;

        protected int startingPage = 0;

        // anchor points to lerp to to see child on certain indexes
        protected Vector3[] pageAnchorPositions;

        protected Vector3 lerpTarget;

        protected bool lerp;

        // item list related
        protected float listContainerMinPosition;

        protected float listContainerMaxPosition;

        protected float listContainerSize;

        protected RectTransform listContainerRectTransform;

        protected Vector2 listContainerCachedSize;

        protected float itemSize;

        protected int itemsCount = 0;

        [Tooltip("Button to go to the next page. (optional)")]
        public Button nextButton;

        [Tooltip("Button to go to the previous page. (optional)")]
        public Button prevButton;

        [Tooltip("Number of items visible in one page of scroll frame.")]
        [RangeAttribute(1, 100)]
        public int itemsVisibleAtOnce = 1;

        [Tooltip("Sets minimum width of list items to 1/itemsVisibleAtOnce.")]
        public bool autoLayoutItems = true;

        [Tooltip("If you wish to update scrollbar numberOfSteps to number of active children on list.")]
        public bool linkScrolbarSteps = false;

        [Tooltip("If you wish to update scrollrect sensitivity to size of list element.")]
        public bool linkScrolrectScrollSensitivity = false;

        public Boolean useFastSwipe = true;

        public int fastSwipeThreshold = 100;

        // drag related
        protected bool startDrag = true;

        protected Vector3 positionOnDragStart = new Vector3();

        protected int pageOnDragStart;

        protected bool fastSwipeTimer = false;

        protected int fastSwipeCounter = 0;

        protected int fastSwipeTarget = 10;

        // Use this for initialization
        void Awake()
        {
            lerp = false;

            scrollRect = gameObject.GetComponent<ScrollRect>();
            scrollRectTransform = gameObject.GetComponent<RectTransform>();
            listContainerTransform = scrollRect.content;
            listContainerRectTransform = listContainerTransform.GetComponent<RectTransform>();

            rectTransform = listContainerTransform.gameObject.GetComponent<RectTransform>();
            UpdateListItemsSize();
            UpdateListItemPositions();

            PageChanged(CurrentPage());

            if (nextButton)
            {
                nextButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    NextScreen();
                });
            }

            if (prevButton)
            {
                prevButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    PreviousScreen();
                });
            }
        }

        void Start()
        {
            Awake();
        }

        public void UpdateListItemsSize()
        {
            float size = 0;
            float currentSize = 0;
            if (direction == ScrollSnap.ScrollDirection.Horizontal)
            {
                size = scrollRectTransform.rect.width / itemsVisibleAtOnce;
                currentSize = listContainerRectTransform.rect.width / itemsCount;
            }
            else
            {
                size = scrollRectTransform.rect.height / itemsVisibleAtOnce;
                currentSize = listContainerRectTransform.rect.height / itemsCount;
            }

            itemSize = size;

            if (linkScrolrectScrollSensitivity)
            {
                scrollRect.scrollSensitivity = itemSize;
            }

            if (autoLayoutItems && currentSize != size && itemsCount > 0)
            {
                if (direction == ScrollSnap.ScrollDirection.Horizontal)
                {
                    foreach (var tr in listContainerTransform)
                    {
                        GameObject child = ((Transform)tr).gameObject;
                        if (child.activeInHierarchy)
                        {
                            var childLayout = child.GetComponent<LayoutElement>();

                            if (childLayout == null)
                            {
                                childLayout = child.AddComponent<LayoutElement>();
                            }

                            childLayout.minWidth = itemSize;
                        }
                    }
                }
                else
                {
                    foreach (var tr in listContainerTransform)
                    {
                        GameObject child = ((Transform)tr).gameObject;
                        if (child.activeInHierarchy)
                        {
                            var childLayout = child.GetComponent<LayoutElement>();

                            if (childLayout == null)
                            {
                                childLayout = child.AddComponent<LayoutElement>();
                            }

                            childLayout.minHeight = itemSize;
                        }
                    }
                }
            }
        }

        public void UpdateListItemPositions()
        {
            if (!listContainerRectTransform.rect.size.Equals(listContainerCachedSize))
            {
                // checking how many children of list are active
                int activeCount = 0;

                foreach (var tr in listContainerTransform)
                {
                    if (((Transform)tr).gameObject.activeInHierarchy)
                    {
                        activeCount++;
                    }
                }

                // if anything changed since last check reinitialize anchors list
                itemsCount = 0;
                Array.Resize(ref pageAnchorPositions, activeCount);

                if (activeCount > 0)
                {
                    pages = Mathf.Max(activeCount - itemsVisibleAtOnce + 1, 1);

                    if (direction == ScrollDirection.Horizontal)
                    {
                        // looking for list spanning range min/max
                        scrollRect.horizontalNormalizedPosition = 0;
                        listContainerMaxPosition = listContainerTransform.localPosition.x;
                        scrollRect.horizontalNormalizedPosition = 1;
                        listContainerMinPosition = listContainerTransform.localPosition.x;

                        listContainerSize = listContainerMaxPosition - listContainerMinPosition;

                        for (var i = 0; i < pages; i++)
                        {
                            pageAnchorPositions[i] = new Vector3(
                                listContainerMaxPosition - itemSize * i,
                                listContainerTransform.localPosition.y,
                                listContainerTransform.localPosition.z
                            );
                        }
                    }
                    else
                    {
                        //Debug.Log ("-------------looking for list spanning range----------------");
                        // looking for list spanning range
                        scrollRect.verticalNormalizedPosition = 1;
                        listContainerMinPosition = listContainerTransform.localPosition.y;
                        scrollRect.verticalNormalizedPosition = 0;
                        listContainerMaxPosition = listContainerTransform.localPosition.y;

                        listContainerSize = listContainerMaxPosition - listContainerMinPosition;

                        for (var i = 0; i < pages; i++)
                        {
                            pageAnchorPositions[i] = new Vector3(
                                listContainerTransform.localPosition.x,
                                listContainerMinPosition + itemSize * i,
                                listContainerTransform.localPosition.z
                            );
                        }
                    }

                    UpdateScrollbar(linkScrolbarSteps);
                    startingPage = Mathf.Min(startingPage, pages);
                    ResetPage();
                }

                if (itemsCount != activeCount)
                {
                    PageChanged(CurrentPage());
                }

                itemsCount = activeCount;
                listContainerCachedSize.Set(listContainerRectTransform.rect.size.x, listContainerRectTransform.rect.size.y);
            }

        }

        public void ResetPage()
        {
            if (direction == ScrollDirection.Horizontal)
            {
                scrollRect.horizontalNormalizedPosition = pages > 1 ? (float)startingPage / (float)(pages - 1) : 0;
            }
            else
            {
                scrollRect.verticalNormalizedPosition = pages > 1 ? (float)(pages - startingPage - 1) / (float)(pages - 1) : 0;
            }
        }

        protected void UpdateScrollbar(bool linkSteps)
        {
            if (linkSteps)
            {
                if (direction == ScrollDirection.Horizontal)
                {
                    if (scrollRect.horizontalScrollbar != null)
                    {
                        scrollRect.horizontalScrollbar.numberOfSteps = pages;
                    }
                }
                else
                {
                    if (scrollRect.verticalScrollbar != null)
                    {
                        scrollRect.verticalScrollbar.numberOfSteps = pages;
                    }
                }
            }
            else
            {
                if (direction == ScrollDirection.Horizontal)
                {
                    if (scrollRect.horizontalScrollbar != null)
                    {
                        scrollRect.horizontalScrollbar.numberOfSteps = 0;
                    }
                }
                else
                {
                    if (scrollRect.verticalScrollbar != null)
                    {
                        scrollRect.verticalScrollbar.numberOfSteps = 0;
                    }
                }
            }
        }

        void LateUpdate()
        {
            UpdateListItemsSize();
            UpdateListItemPositions();

            if (lerp)
            {
                UpdateScrollbar(false);

                listContainerTransform.localPosition = Vector3.Lerp(listContainerTransform.localPosition, lerpTarget, 7.5f * Time.deltaTime);

                if (Vector3.Distance(listContainerTransform.localPosition, lerpTarget) < 0.001f)
                {
                    listContainerTransform.localPosition = lerpTarget;
                    lerp = false;

                    UpdateScrollbar(linkScrolbarSteps);
                }

                //change the info bullets at the bottom of the screen. Just for visual effect
                if (Vector3.Distance(listContainerTransform.localPosition, lerpTarget) < 10f)
                {
                    PageChanged(CurrentPage());
                }
            }

            if (fastSwipeTimer)
            {
                fastSwipeCounter++;
            }
        }

        private bool fastSwipe = false; //to determine if a fast swipe was performed


        //Function for switching screens with buttons
        public void NextScreen()
        {
            UpdateListItemPositions();

            if (CurrentPage() < pages - 1)
            {
                lerp = true;
                lerpTarget = pageAnchorPositions[CurrentPage() + 1];

                PageChanged(CurrentPage() + 1);
            }
        }

        //Function for switching screens with buttons
        public void PreviousScreen()
        {
            UpdateListItemPositions();

            if (CurrentPage() > 0)
            {
                lerp = true;
                lerpTarget = pageAnchorPositions[CurrentPage() - 1];

                PageChanged(CurrentPage() - 1);
            }
        }

        //Because the CurrentScreen function is not so reliable, these are the functions used for swipes
        private void NextScreenCommand()
        {
            if (pageOnDragStart < pages - 1)
            {
                int targetPage = Mathf.Min(pages - 1, pageOnDragStart + itemsVisibleAtOnce);
                lerp = true;

                lerpTarget = pageAnchorPositions[targetPage];

                PageChanged(targetPage);
            }
        }

        //Because the CurrentScreen function is not so reliable, these are the functions used for swipes
        private void PrevScreenCommand()
        {
            if (pageOnDragStart > 0)
            {
                int targetPage = Mathf.Max(0, pageOnDragStart - itemsVisibleAtOnce);
                lerp = true;

                lerpTarget = pageAnchorPositions[targetPage];

                PageChanged(targetPage);
            }
        }


        //returns the current screen that the is seeing
        public int CurrentPage()
        {
            float pos;

            if (direction == ScrollDirection.Horizontal)
            {
                pos = listContainerMaxPosition - listContainerTransform.localPosition.x;
                pos = Mathf.Clamp(pos, 0, listContainerSize);
            }
            else
            {
                pos = listContainerTransform.localPosition.y - listContainerMinPosition;
                pos = Mathf.Clamp(pos, 0, listContainerSize);
            }

            float page = pos / itemSize;

            return Mathf.Clamp(Mathf.RoundToInt(page), 0, pages);
        }

        public void ChangePage(int page)
        {
            if (0 <= page && page < pages)
            {
                lerp = true;

                lerpTarget = pageAnchorPositions[page];

                PageChanged(page);
            }
        }

        //changes the bullets on the bottom of the page - pagination
        private void PageChanged(int currentPage)
        {
            startingPage = currentPage;

            if (nextButton)
            {
                nextButton.interactable = currentPage < pages - 1;
            }

            if (prevButton)
            {
                prevButton.interactable = currentPage > 0;
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

            fastSwipeCounter = 0;
            fastSwipeTimer = true;

            positionOnDragStart = eventData.position;
            pageOnDragStart = CurrentPage();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            startDrag = true;
            float change = 0;

            if (direction == ScrollDirection.Horizontal)
            {
                change = positionOnDragStart.x - eventData.position.x;
            }
            else
            {
                change = -positionOnDragStart.y + eventData.position.y;
            }

            if (useFastSwipe)
            {
                fastSwipe = false;
                fastSwipeTimer = false;

                if (fastSwipeCounter <= fastSwipeTarget)
                {
                    if (Math.Abs(change) > fastSwipeThreshold)
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
                    lerp = true;
                    lerpTarget = pageAnchorPositions[CurrentPage()];
                }
            }
            else
            {
                lerp = true;
                lerpTarget = pageAnchorPositions[CurrentPage()];
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            lerp = false;

            if (startDrag)
            {
                OnBeginDrag(eventData);
                startDrag = false;
            }
        }
        #endregion
    }
}