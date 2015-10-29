using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ReorderableList : MonoBehaviour
{
    public LayoutGroup ContentLayout;
    
    public bool IsDraggable = true;
    public RectTransform DraggableArea;
    public bool CloneDraggedObject = false;

    public bool IsDropable = true;


    public ReorderableListHandler OnElementDropped = new ReorderableListHandler();

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

    private void Awake()
    {
        if (ContentLayout == null)
        {
            Debug.LogError("You need to have a LayoutGroup content set for the list", gameObject);
            return;
        }
        if (DraggableArea == null)
        {
            Debug.LogError("You need to set a draggable area for the list", gameObject);
            return;
        }
        _listContent = ContentLayout.gameObject.AddComponent<ReorderableListContent>();
        _listContent.Init(this);
    }

    #region Nested type: ReorderableListEventStruct

    [Serializable]
    public struct ReorderableListEventStruct
    {
        public GameObject DropedObject;
        public int FromIndex;
        public ReorderableList FromList;
        public bool IsAClone;
        public GameObject SourceObject;
        public int ToIndex;
        public ReorderableList ToList;
    }

    #endregion

    #region Nested type: ReorderableListHandler

    [Serializable]
    public class ReorderableListHandler : UnityEvent<ReorderableListEventStruct>
    {
    }

    #endregion
}