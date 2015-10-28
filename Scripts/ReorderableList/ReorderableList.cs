using System;
using UnityEngine;
using UnityEngine.Events;

public class ReorderableList : MonoBehaviour
{
    public RectTransform Content;
    
    public bool IsDraggable = true;
    public RectTransform DraggableArea;
    public bool CloneDraggedObject = false;

    public bool IsDropable = true;
   
   
    public ReorderableListHandler OnElementDropped = new ReorderableListHandler();

    private ReorderableListContent _listContent;

    private void Awake()
    {
        if (Content == null)
        {
            Debug.LogError("You need to set the content for the list", gameObject);
            return;
        }
        if (DraggableArea == null)
        {
            Debug.LogError("You need to set a draggable area for the list", gameObject);
            return;
        }
        _listContent = Content.gameObject.AddComponent<ReorderableListContent>();
        _listContent.Init(this);
    }

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

    [Serializable]
    public class ReorderableListHandler : UnityEvent<ReorderableListEventStruct>
    {
    }
}