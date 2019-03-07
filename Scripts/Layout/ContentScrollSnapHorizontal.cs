using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

namespace UnityEngine.UI.Extensions
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(ScrollRect))]
    [AddComponentMenu("UI/Extensions/ContentSnapScrollHorizontal")]
    public class ContentScrollSnapHorizontal : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {

        [Serializable]
        public class StartMovementEvent : UnityEvent { }
        [Serializable]
        public class CurrentItemChangeEvent : UnityEvent<int> { }
        [Serializable]
        public class FoundItemToSnapToEvent : UnityEvent<int> { }
        [Serializable]
        public class SnappedToItemEvent : UnityEvent<int> { }

        public bool ignoreInactiveItems = true;
        public MoveInfo startInfo = new MoveInfo(MoveInfo.IndexType.positionIndex, 0);
        public GameObject prevButton;
        public GameObject nextButton;
        public GameObject pagination;
        [Tooltip("The velocity below which the scroll rect will start to snap")]
        public int snappingVelocityThreshold = 50;

        [Header("Paging Info")]
        [Tooltip("Should the pagination & buttons jump or lerp to the items")]
        public bool jumpToItem = false;
        [Tooltip("The time it will take for the pagination or buttons to move between items")]
        public float lerpTime = .3f;

        [Header("Events")]
        [SerializeField]
        [Tooltip("Event is triggered whenever the scroll rect starts to move, even when triggered programatically")]
        private StartMovementEvent m_StartMovementEvent = new StartMovementEvent();
        public StartMovementEvent MovementStarted
        {
            get
            {
                return m_StartMovementEvent;
            }
            set
            {
                m_StartMovementEvent = value;
            }
        }

        [SerializeField]
        [Tooltip("Event is triggered whenever the closest item to the center of the scrollrect changes")]
        private CurrentItemChangeEvent m_CurrentItemChangeEvent = new CurrentItemChangeEvent();
        public CurrentItemChangeEvent CurrentItemChanged
        {
            get
            {
                return m_CurrentItemChangeEvent;
            }
            set
            {
                m_CurrentItemChangeEvent = value;
            }
        }

        [SerializeField]
        [Tooltip("Event is triggered when the ContentSnapScroll decides which item it is going to snap to. Returns the index of the closest position.")]
        private FoundItemToSnapToEvent m_FoundItemToSnapToEvent = new FoundItemToSnapToEvent();
        public FoundItemToSnapToEvent ItemFoundToSnap
        {
            get
            {
                return m_FoundItemToSnapToEvent;
            }
            set
            {
                m_FoundItemToSnapToEvent = value;
            }
        }

        [SerializeField]
        [Tooltip("Event is triggered when we finally settle on an element. Returns the index of the item's position.")]
        private SnappedToItemEvent m_SnappedToItemEvent = new SnappedToItemEvent();
        public SnappedToItemEvent ItemSnappedTo
        {
            get
            {
                return m_SnappedToItemEvent;
            }
            set
            {
                m_SnappedToItemEvent = value;
            }
        }

        private ScrollRect scrollRect = null;
        private RectTransform scrollRectTransform = null;
        private RectTransform contentTransform = null;
        private List<Vector3> contentPositions = null;
        private Vector3 lerpTarget = Vector3.zero;
        private float totalScrollableWidth = 0;
        private DrivenRectTransformTracker tracker ;
        private float mLerpTime = 0;
        private int _closestItem = 0;
        private bool mSliding = false;
        private bool mLerping = false;
        private bool ContentIsHorizonalLayoutGroup
        {
            get
            {
                return contentTransform.GetComponent<HorizontalLayoutGroup>() != null;
            }
        }

        #region Public Info
        /// <summary>
        /// Returns if the SnapScroll is moving
        /// </summary>
        public bool Moving
        {
            get
            {
                return Sliding || Lerping;
            }
        }

        /// <summary>
        /// Returns if the SnapScroll is moving because of a touch
        /// </summary>
        public bool Sliding
        {
            get
            {
                return mSliding;
            }
        }
        /// <summary>
        /// Returns if the SnapScroll is moving programmatically
        /// </summary>
        public bool Lerping
        {
            get
            {
                return mLerping;
            }
        }

        /// <summary>
        /// Returns the closest item's index
        /// *Note this is zero based, and based on position not child order
        /// </summary>
        public int ClosestItemIndex
        {
            get
            {
                return contentPositions.IndexOf(FindClosestFrom(contentTransform.localPosition));
            }
        }
        /// <summary>
        /// Returns the lerpTarget's index
        /// *Note this is zero-based, and based on position not child order
        /// </summary>
        public int LerpTargetIndex
        {
            get
            {
                return contentPositions.IndexOf(lerpTarget);
            }
        }
        #endregion

        #region Setup
        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            scrollRectTransform = (RectTransform) scrollRect.transform;
            contentTransform = scrollRect.content;

            if (nextButton)
                nextButton.GetComponent<Button>().onClick.AddListener(() => { NextItem(); });

            if (prevButton)
                prevButton.GetComponent<Button>().onClick.AddListener(() => { PreviousItem(); });

            SetupDrivenTransforms();
            SetupSnapScroll();
            scrollRect.horizontalNormalizedPosition = 0;
            _closestItem = 0;
            GoTo(startInfo);
        }

        private void OnDisable()
        {
            tracker.Clear();
        }

        private void SetupDrivenTransforms()
        {
            tracker = new DrivenRectTransformTracker();
            tracker.Clear();

            //So that we can calculate everything correctly
            foreach (RectTransform child in contentTransform)
            {
                tracker.Add(this, child, DrivenTransformProperties.Anchors);

                child.anchorMax = new Vector2(0, 1);
                child.anchorMin = new Vector2(0, 1);
            }
        }

        private void SetupSnapScroll()
        {
            if (ContentIsHorizonalLayoutGroup)
            {
                //because you can't get the anchored positions of UI elements
                //when they are in a layout group (as far as I could tell)
                SetupWithHorizontalLayoutGroup();
            }
            else
            {
                SetupWithCalculatedSpacing();
            }
        }

        private void SetupWithHorizontalLayoutGroup()
        {
            HorizontalLayoutGroup horizLayoutGroup = contentTransform.GetComponent<HorizontalLayoutGroup>();
            float childTotalWidths = 0;
            int activeChildren = 0;
            for (int i = 0; i < contentTransform.childCount; i++)
            {
                if (!ignoreInactiveItems || contentTransform.GetChild(i).gameObject.activeInHierarchy)
                {
                    childTotalWidths += ((RectTransform)contentTransform.GetChild(i)).sizeDelta.x;
                    activeChildren++;
                }
            }
            float spacingTotal = (activeChildren - 1) * horizLayoutGroup.spacing;
            float totalWidth = childTotalWidths + spacingTotal + horizLayoutGroup.padding.left + horizLayoutGroup.padding.right;

            contentTransform.sizeDelta = new Vector2(totalWidth, contentTransform.sizeDelta.y);
            float scrollRectWidth = Mathf.Min(((RectTransform)contentTransform.GetChild(0)).sizeDelta.x, ((RectTransform)contentTransform.GetChild(contentTransform.childCount - 1)).sizeDelta.x);

            /*---If the scroll view is set to stretch width this breaks stuff---*/
            scrollRectTransform.sizeDelta = new Vector2(scrollRectWidth, scrollRectTransform.sizeDelta.y);

            contentPositions = new List<Vector3>();
            float widthOfScrollRect = scrollRectTransform.sizeDelta.x;
            totalScrollableWidth = totalWidth - widthOfScrollRect;
            float checkedChildrenTotalWidths = horizLayoutGroup.padding.left;
            int activeChildrenBeforeSelf = 0;
            for (int i = 0; i < contentTransform.childCount; i++)
            {
                if (!ignoreInactiveItems || contentTransform.GetChild(i).gameObject.activeInHierarchy)
                {
                    float widthOfSelf = ((RectTransform)contentTransform.GetChild(i)).sizeDelta.x;
                    float offset = checkedChildrenTotalWidths + (horizLayoutGroup.spacing * activeChildrenBeforeSelf) + ((widthOfSelf - widthOfScrollRect) / 2);
                    scrollRect.horizontalNormalizedPosition = offset / totalScrollableWidth;
                    contentPositions.Add(contentTransform.localPosition);

                    checkedChildrenTotalWidths += widthOfSelf;
                    activeChildrenBeforeSelf++;
                }
            }
        }

        private void SetupWithCalculatedSpacing()
        {
            //we need them in order from left to right for pagination & buttons & our scrollRectWidth
            List<RectTransform> childrenFromLeftToRight = new List<RectTransform>();
            for (int i = 0; i < contentTransform.childCount; i++)
            {
                if (!ignoreInactiveItems || contentTransform.GetChild(i).gameObject.activeInHierarchy)
                {
                    RectTransform childBeingSorted = ((RectTransform)contentTransform.GetChild(i));
                    int insertIndex = childrenFromLeftToRight.Count;
                    for (int j = 0; j < childrenFromLeftToRight.Count; j++)
                    {
                        if (DstFromTopLeftOfTransformToTopLeftOfParent(childBeingSorted).x < DstFromTopLeftOfTransformToTopLeftOfParent(childrenFromLeftToRight[j]).x)
                        {
                            insertIndex = j;
                            break;
                        }
                    }
                    childrenFromLeftToRight.Insert(insertIndex, childBeingSorted);
                }
            }
            RectTransform childFurthestToTheRight = childrenFromLeftToRight[childrenFromLeftToRight.Count - 1];
            float totalWidth = DstFromTopLeftOfTransformToTopLeftOfParent(childFurthestToTheRight).x + childFurthestToTheRight.sizeDelta.x;

            contentTransform.sizeDelta = new Vector2(totalWidth, contentTransform.sizeDelta.y);
            float scrollRectWidth = Mathf.Min(childrenFromLeftToRight[0].sizeDelta.x, childrenFromLeftToRight[childrenFromLeftToRight.Count - 1].sizeDelta.x);

            // Note: sizeDelta will not be calculated properly if the scroll view is set to stretch width.
            scrollRectTransform.sizeDelta = new Vector2(scrollRectWidth, scrollRectTransform.sizeDelta.y);

            contentPositions = new List<Vector3>();
            float widthOfScrollRect = scrollRectTransform.sizeDelta.x;
            totalScrollableWidth = totalWidth - widthOfScrollRect;
            for (int i = 0; i < childrenFromLeftToRight.Count; i++)
            {
                float offset = DstFromTopLeftOfTransformToTopLeftOfParent(childrenFromLeftToRight[i]).x + ((childrenFromLeftToRight[i].sizeDelta.x - widthOfScrollRect) / 2);
                scrollRect.horizontalNormalizedPosition = offset / totalScrollableWidth;
                contentPositions.Add(contentTransform.localPosition);
            }
        }
        #endregion

        #region Public Movement Functions
        /// <summary>
        /// Function for going to a specific screen.
        /// *Note the index is based on a zero-starting index.
        /// </summary>
        /// <param name="info">All of the info about how you want it to move</param>
        public void GoTo(MoveInfo info)
        {
            if (!Moving && info.index != ClosestItemIndex)
            {
                MovementStarted.Invoke();
            }

            if (info.indexType == MoveInfo.IndexType.childIndex)
            {
                mLerpTime = info.duration;
                GoToChild(info.index, info.jump);
            }
            else if (info.indexType == MoveInfo.IndexType.positionIndex)
            {
                mLerpTime = info.duration;
                GoToContentPos(info.index, info.jump);
            }
        }

        private void GoToChild(int index, bool jump)
        {
            int clampedIndex = Mathf.Clamp(index, 0, contentPositions.Count - 1); //contentPositions amount == the amount of available children

            if (ContentIsHorizonalLayoutGroup) //the contentPositions are in child order
            {
                lerpTarget = contentPositions[clampedIndex];
                if (jump)
                {
                    contentTransform.localPosition = lerpTarget;
                }
                else
                {
                    StopMovement();
                    StartCoroutine("LerpToContent");
                }
            }
            else //the contentPositions are in order from left -> right;
            {
                int availableChildIndex = 0; //an available child is one we can snap to
                Vector3 previousContentTransformPos = contentTransform.localPosition;
                for (int i = 0; i < contentTransform.childCount; i++)
                {
                    if (!ignoreInactiveItems || contentTransform.GetChild(i).gameObject.activeInHierarchy)
                    {
                        if (availableChildIndex == clampedIndex)
                        {
                            RectTransform startChild = (RectTransform) contentTransform.GetChild(i);
                            float offset = DstFromTopLeftOfTransformToTopLeftOfParent(startChild).x + ((startChild.sizeDelta.x - scrollRectTransform.sizeDelta.x) / 2);
                            scrollRect.horizontalNormalizedPosition = offset / totalScrollableWidth;
                            lerpTarget = contentTransform.localPosition;
                            if (!jump)
                            {
                                contentTransform.localPosition = previousContentTransformPos;
                                StopMovement();
                                StartCoroutine("LerpToContent");
                            }
                            return;
                        }
                        availableChildIndex++;
                    }
                }
            }
        }

        private void GoToContentPos(int index, bool jump)
        {
            int clampedIndex = Mathf.Clamp(index, 0, contentPositions.Count - 1); //contentPositions amount == the amount of available children

            //the content positions are all in order from left -> right
            //which is what we want so there's no need to check

            lerpTarget = contentPositions[clampedIndex];
            if (jump)
            {
                contentTransform.localPosition = lerpTarget;
            }
            else
            {
                StopMovement();
                StartCoroutine("LerpToContent");
            }
        }

        /// <summary>
        /// Function for going to the next item
        /// *Note the next item is the item to the right of the current item, this is not based on child order
        /// </summary>
        public void NextItem()
        {
            int index;
            if (Sliding)
            {
                index = ClosestItemIndex + 1;
            }
            else
            {
                index = LerpTargetIndex + 1;
            }
            MoveInfo info = new MoveInfo(MoveInfo.IndexType.positionIndex, index, jumpToItem, lerpTime);
            GoTo(info);
        }

        /// <summary>
        /// Function for going to the previous item
        /// *Note the next item is the item to the left of the current item, this is not based on child order
        /// </summary>
        public void PreviousItem()
        {
            int index;
            if (Sliding)
            {
                index = ClosestItemIndex - 1;
            }
            else
            {
                index = LerpTargetIndex - 1;
            }
            MoveInfo info = new MoveInfo(MoveInfo.IndexType.positionIndex, index, jumpToItem, lerpTime);
            GoTo(info);
        }

        /// <summary>
        /// Function for recalculating the size of the content & the snap positions, such as when you remove or add a child
        /// </summary>
        public void UpdateLayout()
        {
            SetupDrivenTransforms();
            SetupSnapScroll();
        }

        /// <summary>
        /// Recalculates the size of the content & snap positions, and moves to a new item afterwards.
        /// </summary>
        /// <param name="info">All of the info about how you want it to move</param>
        public void UpdateLayoutAndMoveTo(MoveInfo info)
        {
            SetupDrivenTransforms();
            SetupSnapScroll();
            GoTo(info);
        }
        #endregion

        #region Behind the Scenes Movement stuff
        public void OnBeginDrag(PointerEventData ped)
        {
            StopMovement();
            if (!Moving)
            {
                MovementStarted.Invoke();
            }
        }

        public void OnEndDrag(PointerEventData ped)
        {
            StartCoroutine("SlideAndLerp");
        }

        private void Update()
        {
            if (_closestItem != ClosestItemIndex)
            {
                CurrentItemChanged.Invoke(ClosestItemIndex);
                ChangePaginationInfo(ClosestItemIndex);
                _closestItem = ClosestItemIndex;
            }
        }

        private IEnumerator SlideAndLerp()
        {
            mSliding = true;
            while (Mathf.Abs(scrollRect.velocity.x) > snappingVelocityThreshold)
            {
                yield return null;
            }

            lerpTarget = FindClosestFrom(contentTransform.localPosition);
            ItemFoundToSnap.Invoke(LerpTargetIndex);

            while (Vector3.Distance(contentTransform.localPosition, lerpTarget) > 1)
            {
                contentTransform.localPosition = Vector3.Lerp(scrollRect.content.localPosition, lerpTarget, 7.5f * Time.deltaTime);
                yield return null;
            }
            mSliding = false;
            scrollRect.velocity = Vector2.zero;
            contentTransform.localPosition = lerpTarget;
            ItemSnappedTo.Invoke(LerpTargetIndex);
        }

        private IEnumerator LerpToContent()
        {
            ItemFoundToSnap.Invoke(LerpTargetIndex);
            mLerping = true;
            Vector3 originalContentPos = contentTransform.localPosition;
            float elapsedTime = 0;
            while (elapsedTime < mLerpTime)
            {
                elapsedTime += Time.deltaTime;
                contentTransform.localPosition = Vector3.Lerp(originalContentPos, lerpTarget, (elapsedTime / mLerpTime));
                yield return null;
            }
            ItemSnappedTo.Invoke(LerpTargetIndex);
            mLerping = false;
        }
        #endregion

        private void StopMovement()
        {
            scrollRect.velocity = Vector2.zero;
            StopCoroutine("SlideAndLerp");
            StopCoroutine("LerpToContent");
        }

        private void ChangePaginationInfo(int targetScreen)
        {
            if (pagination)
                for (int i = 0; i < pagination.transform.childCount; i++)
                {
                    pagination.transform.GetChild(i).GetComponent<Toggle>().isOn = (targetScreen == i);
                }
        }

        private Vector2 DstFromTopLeftOfTransformToTopLeftOfParent(RectTransform rt)
        {
            //gets rid of any pivot weirdness
            return new Vector2(rt.anchoredPosition.x - (rt.sizeDelta.x * rt.pivot.x), rt.anchoredPosition.y + (rt.sizeDelta.y * (1 - rt.pivot.y)));
        }

        private Vector3 FindClosestFrom(Vector3 start)
        {
            Vector3 closest = Vector3.zero;
            float distance = Mathf.Infinity;

            foreach (Vector3 position in contentPositions)
            {
                if (Vector3.Distance(start, position) < distance)
                {
                    distance = Vector3.Distance(start, position);
                    closest = position;
                }
            }
            return closest;
        }

        [System.Serializable]
        public struct MoveInfo
        {
            public enum IndexType { childIndex, positionIndex }
            [Tooltip("Child Index means the Index coresponds to the content item at that index in the hierarchy.\n" +
                "Position Index means the Index corresponds to the content item in that snap position.\n" +
                "A higher Position Index in a Horizonal Scroll Snap means it would be further to the right.")]
            public IndexType indexType;
            [Tooltip("Zero based")]
            public int index;
            [Tooltip("If this is true the snap scroll will jump to the index, otherwise it will lerp there.")]
            public bool jump;
            [Tooltip("If jump is false this is the time it will take to lerp to the index")]
            public float duration;

            /// <summary>
            /// Creates a MoveInfo that jumps to the index
            /// </summary>
            /// <param name="_indexType">Whether you want to get the child at the index or the snap position at the index</param>
            /// <param name="_index">Where you want it to jump</param>
            public MoveInfo(IndexType _indexType, int _index)
            {
                indexType = _indexType;
                index = _index;
                jump = true;
                duration = 0;
            }

            /// <summary>
            /// Creates a MoveInfo
            /// </summary>
            /// <param name="_indexType">Whether you want to get the child at the index or the snap position at the index</param>
            /// <param name="_index">Where you want it to jump</param>
            /// <param name="_jump">Whether you want it to jump or lerp to the index</param>
            /// <param name="_duration">How long it takes to lerp to the index</param>
            public MoveInfo(IndexType _indexType, int _index, bool _jump, float _duration)
            {
                indexType = _indexType;
                index = _index;
                jump = _jump;
                duration = _duration;
            }
        }
    }
}
