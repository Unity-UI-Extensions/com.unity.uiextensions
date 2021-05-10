/// Credit Tomasz Schelenz 
/// Sourced from - https://bitbucket.org/SimonDarksideJ/unity-ui-extensions/issues/82/scrollrectocclusion
/// Demo - https://youtu.be/uVTV7Udx78k?t=39s ScrollRectOcclusion - disables the objects outside of the scrollrect viewport. Useful for scrolls with lots of content, reduces geometry and drawcalls (if content is not batched) In some cases it might create a bit of spikes, especially if you have lots of UI.Text objects in the childs. In that case consider to Add CanvasGroup to your childs and instead of calling setActive on game object change CanvasGroup.alpha value. At 0 it is not being rendered hence will also optimize the performance. 

using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// ScrollRectOcclusion - disables the objects outside of the scrollrect viewport. 
    /// Useful for scrolls with lots of content, reduces geometry and drawcalls (if content is not batched)
    /// 
    /// Fields
    /// - InitByUSer - in case your scrollrect is populated from code, you can explicitly Initialize the infinite scroll after your scroll is ready
    /// by calling Init() method
    /// 
    /// Notes
    /// - In some cases it might create a bit of spikes, especially if you have lots of UI.Text objects in the child's. In that case consider to Add 
    /// CanvasGroup to your child's and instead of calling setActive on game object change CanvasGroup.alpha value. At 0 it is not being rendered hence will 
    /// also optimize the performance. 
    /// - works for both vertical and horizontal scrolls, even at the same time (grid layout)
    /// - in order to work it disables layout components and size fitter if present (automatically)
    /// </summary>

    [AddComponentMenu("UI/Extensions/UI Scrollrect Occlusion")]
    public class UI_ScrollRectOcclusion : MonoBehaviour
    {
        //if true user will need to call Init() method manually (in case the contend of the scrollview is generated from code or requires special initialization)
        public bool InitByUser = false;
        private bool _initialised = false;
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
        private List<RectTransform> _items = new List<RectTransform>();
        private bool _reset = false;

        void Awake()
        {
            if (InitByUser)
                return;

            Init();
        }

        public void Init()
        {
            if (_initialised)
            {
                Debug.LogError("Control already initialized\nYou have to enable the InitByUser setting on the control in order to use Init() when running");
                return;
            }

            if (GetComponent<ScrollRect>() != null)
            {
                _initialised = true;
                _scrollRect = GetComponent<ScrollRect>();
                _scrollRect.onValueChanged.AddListener(OnScroll);

                _isHorizontal = _scrollRect.horizontal;
                _isVertical = _scrollRect.vertical;

                for (int i = 0; i < _scrollRect.content.childCount; i++)
                {
                    _items.Add(_scrollRect.content.GetChild(i).GetComponent<RectTransform>());
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
            }
            else
            {
                Debug.LogError("UI_ScrollRectOcclusion => No ScrollRect component found");
            }
        }

        void ToggleGridComponents(bool toggle)
        {
            if (_isVertical)
                _disableMarginY = _scrollRect.GetComponent<RectTransform>().rect.height / 2 + _items[0].sizeDelta.y;

            if (_isHorizontal)
                _disableMarginX = _scrollRect.GetComponent<RectTransform>().rect.width / 2 + _items[0].sizeDelta.x;

            if (_verticalLayoutGroup)
            {
                _verticalLayoutGroup.enabled = toggle;
            }
            if (_horizontalLayoutGroup)
            {
                _horizontalLayoutGroup.enabled = toggle;
            }
            if (_contentSizeFitter)
            {
                _contentSizeFitter.enabled = toggle;
            }
            if (_gridLayoutGroup)
            {
                _gridLayoutGroup.enabled = toggle;
            }
            _hasDisabledGridComponents = !toggle;
        }

        public void OnScroll(Vector2 pos)
        {
            if (_reset)
            {
                return;
            }

            if (!_hasDisabledGridComponents)
            {
                ToggleGridComponents(false);
            }

            for (int i = 0; i < _items.Count; i++)
            {
                if (_isVertical && _isHorizontal)
                {
                    if (_scrollRect.transform.InverseTransformPoint(_items[i].position).y < -_disableMarginY || _scrollRect.transform.InverseTransformPoint(_items[i].position).y > _disableMarginY
                    || _scrollRect.transform.InverseTransformPoint(_items[i].position).x < -_disableMarginX || _scrollRect.transform.InverseTransformPoint(_items[i].position).x > _disableMarginX)
                    {
                        _items[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        _items[i].gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (_isVertical)
                    {
                        if (_scrollRect.transform.InverseTransformPoint(_items[i].position).y < -_disableMarginY || _scrollRect.transform.InverseTransformPoint(_items[i].position).y > _disableMarginY)
                        {
                            _items[i].gameObject.SetActive(false);
                        }
                        else
                        {
                            _items[i].gameObject.SetActive(true);
                        }
                    }

                    if (_isHorizontal)
                    {
                        if (_scrollRect.transform.InverseTransformPoint(_items[i].position).x < -_disableMarginX || _scrollRect.transform.InverseTransformPoint(_items[i].position).x > _disableMarginX)
                        {
                            _items[i].gameObject.SetActive(false);
                        }
                        else
                        {
                            _items[i].gameObject.SetActive(true);
                        }
                    }
                }
            }
        }

        public void SetDirty()
        {
            _reset = true;
        }

        private void LateUpdate()
        {
            if (_reset)
            {
                _reset = false;
                _items.Clear();

                for (int i = 0; i < _scrollRect.content.childCount; i++)
                {
                    _items.Add(_scrollRect.content.GetChild(i).GetComponent<RectTransform>());
                    _items[i].gameObject.SetActive(true);
                }

                ToggleGridComponents(true);
            }
        }
    }
}