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
        private float[] distReposition;
        private float[] distance;

        [SerializeField]
        [Tooltip("desired ScrollRect")]
        private ScrollRect scrollRect;

        [SerializeField]
        [Tooltip("Elements to populate inside the scroller")]
        private GameObject[] arrayOfElements;

        [SerializeField]
        [Tooltip("Center display area (position of zoomed content)")]
        private RectTransform center;

        [SerializeField]
        [Tooltip("Size / spacing of elements")]
        private RectTransform elementSize;

        [SerializeField]
        [Tooltip("Scale = 1/ (1+distance from center * shrinkage)")]
        private Vector2 elementShrinkage = new Vector2(1f / 200, 1f / 200);

        [SerializeField]
        [Tooltip("Minimum element scale (furthest from center)")]
        private Vector2 minScale = new Vector2(0.7f, 0.7f);

        [SerializeField]
        [Tooltip("Select the item to be in center on start.")]
        private int startingIndex = -1;

        [SerializeField]
        [Tooltip("Stop scrolling past last element from inertia.")]
        private bool stopMomentumOnEnd = true;

        [SerializeField]
        [Tooltip("Set Items out of center to not interactible.")]
        private bool disableUnfocused = true;

        [SerializeField]
        [Tooltip("Button to go to the next page. (optional)")]
        private GameObject scrollUpButton;

        [SerializeField]
        [Tooltip("Button to go to the previous page. (optional)")]
        private GameObject scrollDownButton;

        [SerializeField]
        [Tooltip("Event fired when a specific item is clicked, exposes index number of item. (optional)")]
        private UnityEvent<int> onButtonClicked;

        [SerializeField]
        [Tooltip("Event fired when the focused item is Changed. (optional)")]
        private UnityEvent<int> onFocusChanged;

        public int FocusedElementIndex { get; private set; }

        public RectTransform Center { get => center; set => center = value; }

        public string Result { get; private set; }

        //Scrollable area (content of desired ScrollRect)
        public RectTransform ScrollingPanel{ get { return scrollRect.content; } }

        /// <summary>
        /// Constructor when not used as component but called from other script
        /// </summary>
        public UIVerticalScroller(RectTransform center, RectTransform elementSize, ScrollRect scrollRect, GameObject[] arrayOfElements)
        {
            this.center = center;
            this.elementSize = elementSize;
            this.scrollRect = scrollRect;
            this.arrayOfElements = arrayOfElements;
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

            if (arrayOfElements == null || arrayOfElements.Length == 0)
            {
                var childCount = ScrollingPanel.childCount;
                if (childCount > 0)
                {
                    arrayOfElements = new GameObject[childCount];
                    for (int i = 0; i < childCount; i++)
                    {
                        arrayOfElements[i] = ScrollingPanel.GetChild(i).gameObject;
                    }
                }
            }
        }

        /// <summary>
        /// Recognises and resizes the children.
        /// </summary>
        /// <param name="startingIndex">Starting index.</param>
        /// <param name="arrayOfElements">Array of elements.</param>
        public void UpdateChildren(int startingIndex = -1, GameObject[] arrayOfElements = null)
        {
            // Set _arrayOfElements to arrayOfElements if given, otherwise to child objects of the scrolling panel.
            if (arrayOfElements != null)
            {
                this.arrayOfElements = arrayOfElements;
            }
            else
            {
                this.arrayOfElements = new GameObject[ScrollingPanel.childCount];
                for (int i = 0; i < ScrollingPanel.childCount; i++)
                {
                    this.arrayOfElements[i] = ScrollingPanel.GetChild(i).gameObject;
                }
            }

            // resize the elements to match elementSize rect
            for (var i = 0; i < this.arrayOfElements.Length; i++)
            {
                AddListener(arrayOfElements[i], i);

                RectTransform r = this.arrayOfElements[i].GetComponent<RectTransform>();
                r.anchorMax = r.anchorMin = r.pivot = new Vector2(0.5f, 0.5f);
                r.localPosition = new Vector2(0, i * elementSize.rect.size.y);
                r.sizeDelta = elementSize.rect.size;
            }

            // prepare for scrolling
            distance = new float[this.arrayOfElements.Length];
            distReposition = new float[this.arrayOfElements.Length];
            FocusedElementIndex = -1;

            // if starting index is given, snap to respective element
            if (startingIndex > -1)
            {
                startingIndex = startingIndex > this.arrayOfElements.Length ? this.arrayOfElements.Length - 1 : startingIndex;
                SnapToElement(startingIndex);
            }
        }
        private void AddListener(GameObject button, int index)
        {
            var buttonClick = button.GetComponent<Button>();
            buttonClick.onClick.RemoveAllListeners();
            buttonClick.onClick.AddListener(() => onButtonClicked?.Invoke(index));
        }

        public void Start()
        {
            if (scrollUpButton)
            {
                scrollUpButton.GetComponent<Button>().onClick.AddListener(() => ScrollUp());
            }
            if (scrollDownButton)
            {
                scrollDownButton.GetComponent<Button>().onClick.AddListener(() => ScrollDown());
            }
            UpdateChildren(startingIndex, arrayOfElements);
        }

        public void Update()
        {
            if (arrayOfElements.Length < 1)
            {
                return;
            }

            for (var i = 0; i < arrayOfElements.Length; i++)
            {
                var arrayElementRT = arrayOfElements[i].GetComponent<RectTransform>();

                distReposition[i] = center.position.y - arrayElementRT.position.y;
                distance[i] = Mathf.Abs(distReposition[i]);

                //Magnifying effect
                Vector2 scale = Vector2.Max(minScale, new Vector2(1 / (1 + distance[i] * elementShrinkage.x), (1 / (1 + distance[i] * elementShrinkage.y))));
                arrayElementRT.transform.localScale = new Vector3(scale.x, scale.y, 1f);
            }

            // detect focused element
            float minDistance = Mathf.Min(distance);
            int oldFocusedElement = FocusedElementIndex;

            for (var i = 0; i < arrayOfElements.Length; i++)
            {
                arrayOfElements[i].GetComponent<CanvasGroup>().interactable = !disableUnfocused || minDistance == distance[i];
                if (minDistance == distance[i])
                {
                    FocusedElementIndex = i;
#if UNITY_2022_1_OR_NEWER
                    var textComponentTxtMeshPro = arrayOfElements[i].GetComponentInChildren<TMPro.TMP_Text>();
                    if (textComponentTxtMeshPro != null)
                    {
                        Result = textComponentTxtMeshPro.text;
                    }
#else
                    var textComponent = arrayOfElements[i].GetComponentInChildren<Text>();
                    if (textComponent != null)
                    {
                        Result = textComponent.text;
                    }
#endif
                }
            }

            if (FocusedElementIndex != oldFocusedElement)
            {
                onFocusChanged?.Invoke(FocusedElementIndex);
            }

            if (!UIExtensionsInputManager.GetMouseButton(0))
            {
                // scroll slowly to nearest element when not dragged
                ScrollingElements();
            }

            // stop scrolling past last element from inertia
            if (stopMomentumOnEnd
                && (arrayOfElements[0].GetComponent<RectTransform>().position.y > center.position.y
                || arrayOfElements[arrayOfElements.Length - 1].GetComponent<RectTransform>().position.y < center.position.y))
            {
                scrollRect.velocity = Vector2.zero;
            }
        }

        private void ScrollingElements()
        {
            float newY = Mathf.Lerp(ScrollingPanel.anchoredPosition.y, ScrollingPanel.anchoredPosition.y + distReposition[FocusedElementIndex], Time.deltaTime * 2f);
            Vector2 newPosition = new Vector2(ScrollingPanel.anchoredPosition.x, newY);
            ScrollingPanel.anchoredPosition = newPosition;
        }

        public void SnapToElement(int element)
        {
            float deltaElementPositionY = elementSize.rect.height * element;
            Vector2 newPosition = new Vector2(ScrollingPanel.anchoredPosition.x, -deltaElementPositionY);
            ScrollingPanel.anchoredPosition = newPosition;
        }

        public void ScrollUp()
        {
            float deltaUp = elementSize.rect.height / 1.2f;
            Vector2 newPositionUp = new Vector2(ScrollingPanel.anchoredPosition.x, ScrollingPanel.anchoredPosition.y - deltaUp);
            ScrollingPanel.anchoredPosition = Vector2.Lerp(ScrollingPanel.anchoredPosition, newPositionUp, 1);
        }

        public void ScrollDown()
        {
            float deltaDown = elementSize.rect.height / 1.2f;
            Vector2 newPositionDown = new Vector2(ScrollingPanel.anchoredPosition.x, ScrollingPanel.anchoredPosition.y + deltaDown);
            ScrollingPanel.anchoredPosition = newPositionDown;
        }
    }
}