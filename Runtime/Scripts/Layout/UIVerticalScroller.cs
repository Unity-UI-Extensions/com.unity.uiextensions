/// Credit Mrs. YakaYocha 
/// Sourced from - https://www.youtube.com/channel/UCHp8LZ_0-iCvl-5pjHATsgw
/// Please donate: https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=RJ8D9FRFQF9VS

using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(ScrollRect))]
    [AddComponentMenu("Layout/Extensions/Vertical Scroller")]
    public class UIVerticalScroller : MonoBehaviour
    {
        [Tooltip("desired ScrollRect")]
        public ScrollRect scrollRect;
        [Tooltip("Center display area (position of zoomed content)")]
        public RectTransform center;
        [Tooltip("Size / spacing of elements")]
        public RectTransform elementSize;
        [Tooltip("Scale = 1/ (1+distance from center * shrinkage)")]
        public Vector2 elementShrinkage = new Vector2(1f / 200, 1f / 200);
        [Tooltip("Minimum element scale (furthest from center)")]
        public Vector2 minScale = new Vector2(0.7f, 0.7f);
        [Tooltip("Select the item to be in center on start.")]
        public int startingIndex = -1;
        [Tooltip("Stop scrolling past last element from inertia.")]
        public bool stopMomentumOnEnd = true;
        [Tooltip("Set Items out of center to not interactible.")]
        public bool disableUnfocused = true;
        [Tooltip("Button to go to the next page. (optional)")]
        public GameObject scrollUpButton;
        [Tooltip("Button to go to the previous page. (optional)")]
        public GameObject scrollDownButton;
        [Tooltip("Event fired when a specific item is clicked, exposes index number of item. (optional)")]
        public IntEvent OnButtonClicked;
        [Tooltip("Event fired when the focused item is Changed. (optional)")]
        public IntEvent OnFocusChanged;
        [HideInInspector]
        public GameObject[] _arrayOfElements;

        public int focusedElementIndex { get; private set; }

        public string result { get; private set; }

        private float[] distReposition;
        private float[] distance;
        //private int elementsDistance;


        //Scrollable area (content of desired ScrollRect)
        [HideInInspector]
        public RectTransform scrollingPanel{ get { return scrollRect.content; } }


        /// <summary>
        /// Constructor when not used as component but called from other script, don't forget to set the non-optional properties.
        /// </summary>
        public UIVerticalScroller()
        {
        }

        /// <summary>
        /// Constructor when not used as component but called from other script
        /// </summary>
        public UIVerticalScroller(RectTransform center, RectTransform elementSize, ScrollRect scrollRect, GameObject[] arrayOfElements)
        {
            this.center = center;
            this.elementSize = elementSize;
            this.scrollRect = scrollRect;
            _arrayOfElements = arrayOfElements;
        }

        /// <summary>
        /// Awake this instance.
        /// </summary>
        public void Awake()
        {
            if (!scrollRect)
            {
                scrollRect = GetComponent<ScrollRect>();
            }
            if (!center)
            {
                Debug.LogError("Please define the RectTransform for the Center viewport of the scrollable area");
            }
            if (!elementSize)
            {
                elementSize = center;
            }
            if (_arrayOfElements == null || _arrayOfElements.Length == 0)
            {
                _arrayOfElements = new GameObject[scrollingPanel.childCount];
                for (int i = 0; i < scrollingPanel.childCount; i++)
                {
                    _arrayOfElements[i] = scrollingPanel.GetChild(i).gameObject;
                }     
            }
        }

        /// <summary>
        /// Recognises and resizes the children.
        /// </summary>
        /// <param name="startingIndex">Starting index.</param>
        /// <param name="arrayOfElements">Array of elements.</param>
        public void updateChildren(int startingIndex = -1, GameObject[] arrayOfElements = null)
        {
            // Set _arrayOfElements to arrayOfElements if given, otherwise to child objects of the scrolling panel.
            if (arrayOfElements != null)
            {
                _arrayOfElements = arrayOfElements;
            }
            else
            {
                _arrayOfElements = new GameObject[scrollingPanel.childCount];
                for (int i = 0; i < scrollingPanel.childCount; i++)
                {
                    _arrayOfElements[i] = scrollingPanel.GetChild(i).gameObject;
                }
            }

            // resize the elements to match elementSize rect
            for (var i = 0; i < _arrayOfElements.Length; i++)
            {
                int j = i;
                _arrayOfElements[i].GetComponent<Button>().onClick.RemoveAllListeners();
                if (OnButtonClicked != null)
                {
                    _arrayOfElements[i].GetComponent<Button>().onClick.AddListener(() => OnButtonClicked.Invoke(j));
                }
                RectTransform r = _arrayOfElements[i].GetComponent<RectTransform>();
                r.anchorMax = r.anchorMin = r.pivot = new Vector2(0.5f, 0.5f);
                r.localPosition = new Vector2(0, i * elementSize.rect.size.y);
                r.sizeDelta = elementSize.rect.size;
            }

            // prepare for scrolling
            distance = new float[_arrayOfElements.Length];
            distReposition = new float[_arrayOfElements.Length];
            focusedElementIndex = -1;

            //scrollRect.scrollSensitivity = elementSize.rect.height / 5;

            // if starting index is given, snap to respective element
            if (startingIndex > -1)
            {
                startingIndex = startingIndex > _arrayOfElements.Length ? _arrayOfElements.Length - 1 : startingIndex;
                SnapToElement(startingIndex);
            }
        }

        public void Start()
        {

            if (scrollUpButton)
                scrollUpButton.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ScrollUp();
                    });

            if (scrollDownButton)
                scrollDownButton.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ScrollDown();
                    });
            updateChildren(startingIndex, _arrayOfElements);
        }


        public void Update()
        {
            if (_arrayOfElements.Length < 1)
            {
                return;
            }

            for (var i = 0; i < _arrayOfElements.Length; i++)
            {
                distReposition[i] = center.GetComponent<RectTransform>().position.y - _arrayOfElements[i].GetComponent<RectTransform>().position.y;
                distance[i] = Mathf.Abs(distReposition[i]);

                //Magnifying effect
                Vector2 scale = Vector2.Max(minScale, new Vector2(1 / (1 + distance[i] * elementShrinkage.x), (1 / (1 + distance[i] * elementShrinkage.y))));
                _arrayOfElements[i].GetComponent<RectTransform>().transform.localScale = new Vector3(scale.x, scale.y, 1f);
            }

            // detect focused element
            float minDistance = Mathf.Min(distance);
            int oldFocusedElement = focusedElementIndex;
            for (var i = 0; i < _arrayOfElements.Length; i++)
            {
                _arrayOfElements[i].GetComponent<CanvasGroup>().interactable = !disableUnfocused || minDistance == distance[i];
                if (minDistance == distance[i])
                {
                    focusedElementIndex = i;
                    result = _arrayOfElements[i].GetComponentInChildren<Text>().text;
                }
            }
            if (focusedElementIndex != oldFocusedElement && OnFocusChanged != null)
            {
                OnFocusChanged.Invoke(focusedElementIndex);
            }


            if (!UIExtensionsInputManager.GetMouseButton(0))
            {
                // scroll slowly to nearest element when not dragged
                ScrollingElements();
            }


            // stop scrolling past last element from inertia
            if (stopMomentumOnEnd
                && (_arrayOfElements[0].GetComponent<RectTransform>().position.y > center.position.y
                || _arrayOfElements[_arrayOfElements.Length - 1].GetComponent<RectTransform>().position.y < center.position.y))
            {
                scrollRect.velocity = Vector2.zero;
            }
        }

        private void ScrollingElements()
        {
            float newY = Mathf.Lerp(scrollingPanel.anchoredPosition.y, scrollingPanel.anchoredPosition.y + distReposition[focusedElementIndex], Time.deltaTime * 2f);
            Vector2 newPosition = new Vector2(scrollingPanel.anchoredPosition.x, newY);
            scrollingPanel.anchoredPosition = newPosition;
        }

        public void SnapToElement(int element)
        {
            float deltaElementPositionY = elementSize.rect.height * element;
            Vector2 newPosition = new Vector2(scrollingPanel.anchoredPosition.x, -deltaElementPositionY);
            scrollingPanel.anchoredPosition = newPosition;

        }

        public void ScrollUp()
        {
            float deltaUp = elementSize.rect.height / 1.2f;
            Vector2 newPositionUp = new Vector2(scrollingPanel.anchoredPosition.x, scrollingPanel.anchoredPosition.y - deltaUp);
            scrollingPanel.anchoredPosition = Vector2.Lerp(scrollingPanel.anchoredPosition, newPositionUp, 1);
        }

        public void ScrollDown()
        {
            float deltaDown = elementSize.rect.height / 1.2f;
            Vector2 newPositionDown = new Vector2(scrollingPanel.anchoredPosition.x, scrollingPanel.anchoredPosition.y + deltaDown);
            scrollingPanel.anchoredPosition = newPositionDown;
        }

        [System.Serializable]
        public class IntEvent:UnityEvent<int>
        {

        }
    }
}