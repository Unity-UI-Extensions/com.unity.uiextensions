using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof (RectTransform))]
public class ReorderableListElement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();
    private ReorderableList _currentReorderableListRaycasted;
    private RectTransform _draggingObject;
    private ReorderableList _reorderableList;
    private RectTransform _fakeElement;
    private int _fromIndex;
    private bool _isDragging;
    private RectTransform _rect;


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_reorderableList == null)
            return;

        //Can't drag, return...
        if (!_reorderableList.IsDraggable)
        {
            _draggingObject = null;
            return;
        }

        //If CloneDraggedObject  just set draggingObject to this gameobject
        if (_reorderableList.CloneDraggedObject == false)
        {
            _draggingObject = _rect;
            _fromIndex = _rect.GetSiblingIndex();
        }
            //Else Duplicate
        else
        {
            GameObject clone = Instantiate(gameObject);
            _draggingObject = clone.GetComponent<RectTransform>();
            _draggingObject.sizeDelta = gameObject.GetComponent<RectTransform>().sizeDelta;
        }

        //Put _dragging object into the draggin area
        _draggingObject.SetParent(_reorderableList.DraggableArea, false);
        _draggingObject.SetAsLastSibling();

        //Create a fake element for previewing placement
        _fakeElement = new GameObject("Fake").AddComponent<RectTransform>();
        _fakeElement.gameObject.AddComponent<LayoutElement>().preferredHeight = _rect.rect.height;


        _isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging)
            return;

        //Set dragging object on cursor
        _draggingObject.position = eventData.position;


        //Check everything under the cursor to find a ReorderableList
        EventSystem.current.RaycastAll(eventData, _raycastResults);
        for (int i = 0; i < _raycastResults.Count; i++)
        {
            _currentReorderableListRaycasted = _raycastResults[i].gameObject.GetComponent<ReorderableList>();
            if (_currentReorderableListRaycasted != null)
            {
                break;
            }
        }

        //If nothing found or the list is not dropable, put the fake element outsite
        if (_currentReorderableListRaycasted == null || _currentReorderableListRaycasted.IsDropable == false)
        {
            _fakeElement.transform.SetParent(_reorderableList.DraggableArea, false);
        }
            //Else find the best position on the list and put fake element on the right index  
        else
        {
            if (_fakeElement.parent != _currentReorderableListRaycasted)
                _fakeElement.SetParent(_currentReorderableListRaycasted.Content, false);

            float minDistance = float.PositiveInfinity;
            int targetIndex = 0;
            for (int j = 0; j < _currentReorderableListRaycasted.Content.childCount; j++)
            {
                var c = _currentReorderableListRaycasted.Content.GetChild(j).GetComponent<RectTransform>();
                float dist = Mathf.Abs(c.position.y - eventData.position.y);

                if (dist < minDistance)
                {
                    minDistance = dist;
                    targetIndex = j;
                }
            }

            _fakeElement.SetSiblingIndex(targetIndex);
            _fakeElement.gameObject.SetActive(true);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;

        if (_draggingObject != null)
        {
            //If we have a, ReorderableList that is dropable
            //Put the dragged object into the content and at the right index
            if (_currentReorderableListRaycasted != null && _currentReorderableListRaycasted.IsDropable)
            {
                _draggingObject.SetParent(_currentReorderableListRaycasted.Content, false);
                _draggingObject.SetSiblingIndex(_fakeElement.GetSiblingIndex());

                //Send OnelementDropped Event
                _reorderableList.OnElementDropped.Invoke(new ReorderableList.ReorderableListEventStruct
                {
                    DropedObject = _draggingObject.gameObject,
                    IsAClone = _reorderableList.CloneDraggedObject,
                    SourceObject = _reorderableList.CloneDraggedObject ? gameObject : _draggingObject.gameObject,
                    FromList = _reorderableList,
                    FromIndex = _fromIndex,
                    ToList = _currentReorderableListRaycasted,
                    ToIndex = _fakeElement.GetSiblingIndex() - 1
                });
            }
            //We don't have an ReorderableList
            else
            {
                //If it's a clone, delete it
                if (_reorderableList.CloneDraggedObject)
                {
                    Destroy(_draggingObject.gameObject);
                }
                //Else replace the draggedObject to his first place
                else
                {
                    _draggingObject.SetParent(_reorderableList.Content, false);
                    _draggingObject.SetSiblingIndex(_fromIndex);
                }
            }
        }

        //Delete fake element
        if (_fakeElement != null)
            Destroy(_fakeElement.gameObject);
    }

    public void Init(ReorderableList reorderableList)
    {
        _reorderableList = reorderableList;
        _rect = GetComponent<RectTransform>();
    }
}