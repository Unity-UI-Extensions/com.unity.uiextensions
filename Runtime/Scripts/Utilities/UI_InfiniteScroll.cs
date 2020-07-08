/// Credit Tomasz Schelenz 
/// Sourced from - https://bitbucket.org/SimonDarksideJ/unity-ui-extensions/issues/81/infinite-scrollrect
/// Demo - https://www.youtube.com/watch?v=uVTV7Udx78k  - configures automatically.  - works in both vertical and horizontal (but not both at the same time)  - drag and drop  - can be initialized by code (in case you populate your scrollview content from code)

using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Infinite scroll view with automatic configuration 
    /// 
    /// Fields
    /// - InitByUSer - in case your scrollrect is populated from code, you can explicitly Initialize the infinite scroll after your scroll is ready
    /// by callin Init() method
    /// 
    /// Notes
    /// - doesn't work in both vertical and horizontal orientation at the same time.
    /// - in order to work it disables layout components and size fitter if present(automatically)
    /// 
    /// </summary>
    [AddComponentMenu("UI/Extensions/UI Infinite Scroll")]
    public class UI_InfiniteScroll : MonoBehaviour
    {
        //if true user will need to call Init() method manually (in case the contend of the scrollview is generated from code or requires special initialization)
        [Tooltip("If false, will Init automatically, otherwise you need to call Init() method")]
        public bool InitByUser = false;
        private ScrollRect _scrollRect;
        private ContentSizeFitter _contentSizeFitter;
        private VerticalLayoutGroup _verticalLayoutGroup;
        private HorizontalLayoutGroup _horizontalLayoutGroup;
        private GridLayoutGroup _gridLayoutGroup;
        private bool _isVertical = false;
        private bool _isHorizontal = false;
        private float _disableMarginX = 0;
        private float _disableMarginY = 0;
        private bool _hasDisabledGridComponents = false;
        private List<RectTransform> items = new List<RectTransform>();
        private Vector2 _newAnchoredPosition = Vector2.zero;
        //TO DISABLE FLICKERING OBJECT WHEN SCROLL VIEW IS IDLE IN BETWEEN OBJECTS
        private float _treshold = 100f;
        private int _itemCount = 0;
        private float _recordOffsetX = 0;
        private float _recordOffsetY = 0;

        void Awake()
        {
            if (!InitByUser)
                Init();
        }

        public void Init()
        {
            if (GetComponent<ScrollRect>() != null)
            {
                _scrollRect = GetComponent<ScrollRect>();
                _scrollRect.onValueChanged.AddListener(OnScroll);
                _scrollRect.movementType = ScrollRect.MovementType.Unrestricted;

                for (int i = 0; i < _scrollRect.content.childCount; i++)
                {
                    items.Add(_scrollRect.content.GetChild(i).GetComponent<RectTransform>());
                }
                if (_scrollRect.content.GetComponent<VerticalLayoutGroup>() != null)
                {
                    _verticalLayoutGroup = _scrollRect.content.GetComponent<VerticalLayoutGroup>();
                }
                if (_scrollRect.content.GetComponent<HorizontalLayoutGroup>() != null)
                {
                    _horizontalLayoutGroup = _scrollRect.content.GetComponent<HorizontalLayoutGroup>();
                }
                if (_scrollRect.content.GetComponent<GridLayoutGroup>() != null)
                {
                    _gridLayoutGroup = _scrollRect.content.GetComponent<GridLayoutGroup>();
                }
                if (_scrollRect.content.GetComponent<ContentSizeFitter>() != null)
                {
                    _contentSizeFitter = _scrollRect.content.GetComponent<ContentSizeFitter>();
                }

                _isHorizontal = _scrollRect.horizontal;
                _isVertical = _scrollRect.vertical;

                if (_isHorizontal && _isVertical)
                {
                    Debug.LogError("UI_InfiniteScroll doesn't support scrolling in both directions, please choose one direction (horizontal or vertical)");
                }

                _itemCount = _scrollRect.content.childCount;
            }
            else
            {
                Debug.LogError("UI_InfiniteScroll => No ScrollRect component found");
            }
        }

        void DisableGridComponents()
        {
            if (_isVertical)
            {
                _recordOffsetY = items[1].GetComponent<RectTransform>().anchoredPosition.y - items[0].GetComponent<RectTransform>().anchoredPosition.y;
                if (_recordOffsetY < 0)
                {
                    _recordOffsetY *= -1;
                }
                _disableMarginY = _recordOffsetY * _itemCount / 2;// _scrollRect.GetComponent<RectTransform>().rect.height/2 + items[0].sizeDelta.y;
            }
            if (_isHorizontal)
            {
                _recordOffsetX = items[1].GetComponent<RectTransform>().anchoredPosition.x - items[0].GetComponent<RectTransform>().anchoredPosition.x;
                if (_recordOffsetX < 0)
                {
                    _recordOffsetX *= -1;
                }
                _disableMarginX = _recordOffsetX * _itemCount / 2;//_scrollRect.GetComponent<RectTransform>().rect.width/2 + items[0].sizeDelta.x;
            }

            if (_verticalLayoutGroup)
            {
                _verticalLayoutGroup.enabled = false;
            }
            if (_horizontalLayoutGroup)
            {
                _horizontalLayoutGroup.enabled = false;
            }
            if (_contentSizeFitter)
            {
                _contentSizeFitter.enabled = false;
            }
            if (_gridLayoutGroup)
            {
                _gridLayoutGroup.enabled = false;
            }
            _hasDisabledGridComponents = true;
        }
        
        public void OnScroll(Vector2 pos)
        {
            if (!_hasDisabledGridComponents)
                DisableGridComponents();

            for (int i = 0; i < items.Count; i++)
            {
                if (_isHorizontal)
                {
                    if (_scrollRect.transform.InverseTransformPoint(items[i].gameObject.transform.position).x > _disableMarginX + _treshold)
                    {
                        _newAnchoredPosition = items[i].anchoredPosition;
                        _newAnchoredPosition.x -= _itemCount * _recordOffsetX;
                        items[i].anchoredPosition = _newAnchoredPosition;
                        _scrollRect.content.GetChild(_itemCount - 1).transform.SetAsFirstSibling();
                    }
                    else if (_scrollRect.transform.InverseTransformPoint(items[i].gameObject.transform.position).x < -_disableMarginX)
                    {
                        _newAnchoredPosition = items[i].anchoredPosition;
                        _newAnchoredPosition.x += _itemCount * _recordOffsetX;
                        items[i].anchoredPosition = _newAnchoredPosition;
                        _scrollRect.content.GetChild(0).transform.SetAsLastSibling();
                    }
                }

                if (_isVertical)
                {
                    if (_scrollRect.transform.InverseTransformPoint(items[i].gameObject.transform.position).y > _disableMarginY + _treshold)
                    {
                        _newAnchoredPosition = items[i].anchoredPosition;
                        _newAnchoredPosition.y -= _itemCount * _recordOffsetY;
                        items[i].anchoredPosition = _newAnchoredPosition;
                        _scrollRect.content.GetChild(_itemCount - 1).transform.SetAsFirstSibling();
                    }
                    else if (_scrollRect.transform.InverseTransformPoint(items[i].gameObject.transform.position).y < -_disableMarginY)
                    {
                        _newAnchoredPosition = items[i].anchoredPosition;
                        _newAnchoredPosition.y += _itemCount * _recordOffsetY;
                        items[i].anchoredPosition = _newAnchoredPosition;
                        _scrollRect.content.GetChild(0).transform.SetAsLastSibling();
                    }
                }
            }
        }
    }
}