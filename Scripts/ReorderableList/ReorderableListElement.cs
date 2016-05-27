/// Credit Ziboo, Andrew Quesenberry 
/// Sourced from - http://forum.unity3d.com/threads/free-reorderable-list.364600/
/// Last Child Fix - https://bitbucket.org/ddreaper/unity-ui-extensions/issues/70/all-re-orderable-lists-cause-a-transform

using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{

    [RequireComponent(typeof(RectTransform))]
    public class ReorderableListElement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();
        private ReorderableList _currentReorderableListRaycasted;
        private RectTransform _draggingObject;
        private LayoutElement _draggingObjectLE;
        private Vector2 _draggingObjectOriginalSize;
        private RectTransform _fakeElement;
        private LayoutElement _fakeElementLE;
        private int _fromIndex;
        private bool _isDragging;
        private RectTransform _rect;
        private ReorderableList _reorderableList;

        #region IBeginDragHandler Members

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
                //Send OnElementRemoved Event
                if (_reorderableList.OnElementRemoved != null)
                {
                    _reorderableList.OnElementRemoved.Invoke(new ReorderableList.ReorderableListEventStruct
                    {
                        DroppedObject = _draggingObject.gameObject,
                        IsAClone = _reorderableList.CloneDraggedObject,
                        SourceObject = _reorderableList.CloneDraggedObject ? gameObject : _draggingObject.gameObject,
                        FromList = _reorderableList,
                        FromIndex = _fromIndex,
                    });
                }
            }
            //Else Duplicate
            else
            {
                GameObject clone = (GameObject)Instantiate(gameObject);
                _draggingObject = clone.GetComponent<RectTransform>();
            }

            //Put _dragging object into the draggin area
            _draggingObjectOriginalSize = gameObject.GetComponent<RectTransform>().rect.size;
            _draggingObjectLE = _draggingObject.GetComponent<LayoutElement>();
            _draggingObject.SetParent(_reorderableList.DraggableArea, false);
            _draggingObject.SetAsLastSibling();

            //Create a fake element for previewing placement
            _fakeElement = new GameObject("Fake").AddComponent<RectTransform>();
            _fakeElementLE = _fakeElement.gameObject.AddComponent<LayoutElement>();


            RefreshSizes();

            //Send OnElementGrabbed Event
            if (_reorderableList.OnElementGrabbed != null)
            {
                _reorderableList.OnElementGrabbed.Invoke(new ReorderableList.ReorderableListEventStruct
                {
                    DroppedObject = _draggingObject.gameObject,
                    IsAClone = _reorderableList.CloneDraggedObject,
                    SourceObject = _reorderableList.CloneDraggedObject ? gameObject : _draggingObject.gameObject,
                    FromList = _reorderableList,
                    FromIndex = _fromIndex,
                });
            }

            _isDragging = true;
        }

        #endregion

        #region IDragHandler Members

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
                RefreshSizes();
                _fakeElement.transform.SetParent(_reorderableList.DraggableArea, false);

            }
            //Else find the best position on the list and put fake element on the right index  
            else
            {
                if (_fakeElement.parent != _currentReorderableListRaycasted)
                    _fakeElement.SetParent(_currentReorderableListRaycasted.Content, false);

                float minDistance = float.PositiveInfinity;
                int targetIndex = 0;
                float dist = 0;
                for (int j = 0; j < _currentReorderableListRaycasted.Content.childCount; j++)
                {
                    var c = _currentReorderableListRaycasted.Content.GetChild(j).GetComponent<RectTransform>();

                    if (_currentReorderableListRaycasted.ContentLayout is VerticalLayoutGroup)
                        dist = Mathf.Abs(c.position.y - eventData.position.y);
                    else if (_currentReorderableListRaycasted.ContentLayout is HorizontalLayoutGroup)
                        dist = Mathf.Abs(c.position.x - eventData.position.x);
                    else if (_currentReorderableListRaycasted.ContentLayout is GridLayoutGroup)
                        dist = (Mathf.Abs(c.position.x - eventData.position.x) + Mathf.Abs(c.position.y - eventData.position.y));

                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        targetIndex = j;
                    }
                }

                RefreshSizes();
                _fakeElement.SetSiblingIndex(targetIndex);
                _fakeElement.gameObject.SetActive(true);

            }
        }

        #endregion

        #region IEndDragHandler Members

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;

            if (_draggingObject != null)
            {
                //If we have a, ReorderableList that is dropable
                //Put the dragged object into the content and at the right index
                if (_currentReorderableListRaycasted != null && _currentReorderableListRaycasted.IsDropable)
                {
                    RefreshSizes();
                    _draggingObject.SetParent(_currentReorderableListRaycasted.Content, false);
                    _draggingObject.SetSiblingIndex(_fakeElement.GetSiblingIndex());


                    //Send OnelementDropped Event
                    if (_reorderableList.OnElementDropped != null)
                    {
                        _reorderableList.OnElementDropped.Invoke(new ReorderableList.ReorderableListEventStruct
                        {
                            DroppedObject = _draggingObject.gameObject,
                            IsAClone = _reorderableList.CloneDraggedObject,
                            SourceObject = _reorderableList.CloneDraggedObject ? gameObject : _draggingObject.gameObject,
                            FromList = _reorderableList,
                            FromIndex = _fromIndex,
                            ToList = _currentReorderableListRaycasted,
                            ToIndex = _fakeElement.GetSiblingIndex() - 1
                        });
                    }
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
                        RefreshSizes();
                        _draggingObject.SetParent(_reorderableList.Content, false);
                        _draggingObject.SetSiblingIndex(_fromIndex);
                    }
                }
            }

            //Delete fake element
            if (_fakeElement != null)
                Destroy(_fakeElement.gameObject);
        }

        #endregion

        private void RefreshSizes()
        {
            Vector2 size = _draggingObjectOriginalSize;

            if (_currentReorderableListRaycasted != null && _currentReorderableListRaycasted.IsDropable && _currentReorderableListRaycasted.Content.childCount > 0)
            {
                var firstChild = _currentReorderableListRaycasted.Content.GetChild(0);
                if (firstChild != null)
                {
                    size = firstChild.GetComponent<RectTransform>().rect.size;
                }
            }

            _draggingObject.sizeDelta = size;
            _fakeElementLE.preferredHeight = _draggingObjectLE.preferredHeight = size.y;
            _fakeElementLE.preferredWidth = _draggingObjectLE.preferredWidth = size.x;
        }

        public void Init(ReorderableList reorderableList)
        {
            _reorderableList = reorderableList;
            _rect = GetComponent<RectTransform>();
        }
    }
}