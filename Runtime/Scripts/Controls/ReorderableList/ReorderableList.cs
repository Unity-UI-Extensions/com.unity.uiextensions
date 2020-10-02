/// Credit Ziboo
/// Sourced from - http://forum.unity3d.com/threads/free-reorderable-list.364600/

using System;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{

    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    [AddComponentMenu("UI/Extensions/Re-orderable list")]
    public class ReorderableList : MonoBehaviour
    {
        [Tooltip("Child container with re-orderable items in a layout group")]
        public LayoutGroup ContentLayout;
        [Tooltip("Parent area to draw the dragged element on top of containers. Defaults to the root Canvas")]
        public RectTransform DraggableArea;

        [Tooltip("Can items be dragged from the container?")]
        public bool IsDraggable = true;

        [Tooltip("Should the draggable components be removed or cloned?")]
        public bool CloneDraggedObject = false;

        [Tooltip("Can new draggable items be dropped in to the container?")]
        public bool IsDropable = true;

        [Tooltip("Should dropped items displace a current item if the list is full?\n " +
            "Depending on the dropped items origin list, the displaced item may be added, dropped in space or deleted.")]
        public bool IsDisplacable = false;
        
        // This sets every item size (when being dragged over this list) to the current size of the first element of this list
        [Tooltip("Should items being dragged over this list have their sizes equalized?")]
        public bool EqualizeSizesOnDrag = false; 

        public int maxItems = int.MaxValue;


        [Header("UI Re-orderable Events")]
        public ReorderableListHandler OnElementDropped = new ReorderableListHandler();
        public ReorderableListHandler OnElementGrabbed = new ReorderableListHandler();
        public ReorderableListHandler OnElementRemoved = new ReorderableListHandler();
        public ReorderableListHandler OnElementAdded = new ReorderableListHandler();
        public ReorderableListHandler OnElementDisplacedFrom = new ReorderableListHandler();
        public ReorderableListHandler OnElementDisplacedTo = new ReorderableListHandler();
        public ReorderableListHandler OnElementDisplacedFromReturned = new ReorderableListHandler();
        public ReorderableListHandler OnElementDisplacedToReturned = new ReorderableListHandler();
        public ReorderableListHandler OnElementDroppedWithMaxItems = new ReorderableListHandler();

        private RectTransform _content;
        private ReorderableListContent _listContent;

        public RectTransform Content
        {
            get
            {
                if (_content == null)
                {
                    _content = ContentLayout.GetComponent<RectTransform>();
                }
                return _content;
            }
        }

        Canvas GetCanvas()
        {
            Transform t = transform;
            Canvas canvas = null;


            int lvlLimit = 100;
            int lvl = 0;

            while (canvas == null && lvl < lvlLimit)
            {
                canvas = t.gameObject.GetComponent<Canvas>();
                if (canvas == null)
                {
                    t = t.parent;
                }

                lvl++;
            }
            return canvas;
        }

        /// <summary>
        /// Refresh related list content
        /// </summary>
        public void Refresh()
        {
            _listContent = ContentLayout.gameObject.GetOrAddComponent<ReorderableListContent>();
            _listContent.Init(this);
        }

        private void Start()
        {

            if (ContentLayout == null)
            {
                Debug.LogError("You need to have a child LayoutGroup content set for the list: " + name, gameObject);
                return;
            }
            if (DraggableArea == null)
            {
                DraggableArea = transform.root.GetComponentInChildren<Canvas>().GetComponent<RectTransform>();
            }
            if (IsDropable && !GetComponent<Graphic>())
            {
                Debug.LogError("You need to have a Graphic control (such as an Image) for the list [" + name + "] to be droppable", gameObject);
                return;
            }

            Refresh();
        }


        #region Nested type: ReorderableListEventStruct

        [Serializable]
        public struct ReorderableListEventStruct
        {
            public GameObject DroppedObject;
            public int FromIndex;
            public ReorderableList FromList;
            public bool IsAClone;
            public GameObject SourceObject;
            public int ToIndex;
            public ReorderableList ToList;

            public void Cancel()
            {
                SourceObject.GetComponent<ReorderableListElement>().isValid = false;
            }
        }

        #endregion


        #region Nested type: ReorderableListHandler

        [Serializable]
        public class ReorderableListHandler : UnityEvent<ReorderableListEventStruct>
        {
        }

        public void TestReOrderableListTarget(ReorderableListEventStruct item)
        {
            Debug.Log("Event Received");
            Debug.Log("Hello World, is my item a clone? [" + item.IsAClone + "]");
        }

        #endregion
    }
}
