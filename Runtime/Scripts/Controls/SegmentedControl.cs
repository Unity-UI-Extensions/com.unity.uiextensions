/// Credit David Gileadi
/// Sourced from - https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/pull-requests/12

using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    // Segmented control, like a group of buttons
    [AddComponentMenu("UI/Extensions/Segmented Control/Segmented Control")]
    [RequireComponent(typeof(RectTransform))]
    public class SegmentedControl : UIBehaviour
    {
        private Selectable[] m_segments;
        [SerializeField]
        [Tooltip("A GameObject with an Image to use as a separator between segments. Size of the RectTransform will determine the size of the separator used.\nNote, make sure to disable the separator GO so that it does not affect the scene")]
        private Graphic m_separator;
        private float m_separatorWidth = 0;
        [SerializeField]
        [Tooltip("When True, it allows each button to be toggled on/off")]
        private bool m_allowSwitchingOff = false;
        [SerializeField]
        [Tooltip("The selected default for the control (zero indexed array)")]
        private int m_selectedSegmentIndex = -1;
        // Event delegates triggered on click.
        [SerializeField]
        [Tooltip("Event to fire once the selection has been changed")]
        private SegmentSelectedEvent m_onValueChanged = new SegmentSelectedEvent();

        internal Selectable selectedSegment;

        protected float SeparatorWidth
        {
            get
            {
                if (m_separatorWidth == 0 && separator)
                {
                    m_separatorWidth = separator.rectTransform.rect.width;
                    var image = separator.GetComponent<Image>();
                    if (image)
                        m_separatorWidth /= image.pixelsPerUnit;
                }
                return m_separatorWidth;
            }
        }

        [Serializable]
        public class SegmentSelectedEvent : UnityEvent<int> { }

        public Selectable[] segments
        {
            get
            {
                if (m_segments == null || m_segments.Length == 0)
                {
                    m_segments = GetChildSegments();
                }
                return m_segments;
            }
        }

        public Graphic separator { get { return m_separator; } set { m_separator = value; m_separatorWidth = 0; LayoutSegments(); } }

        public bool allowSwitchingOff { get { return m_allowSwitchingOff; } set { m_allowSwitchingOff = value; } }

        public int selectedSegmentIndex
        {
            get { return Array.IndexOf(segments, selectedSegment); }
            set
            {
                value = Math.Max(value, -1);
                value = Math.Min(value, segments.Length - 1);

                if (m_selectedSegmentIndex == value)
                {
                    return;
                }

                m_selectedSegmentIndex = value;

                if (selectedSegment)
                {
                    var segment = selectedSegment.GetComponent<Segment>();
                    if (segment)
                    {
                        segment.selected = false;
                    }
                    selectedSegment = null;
                }

                if (value != -1)
                {
                    selectedSegment = segments[value];
                    var segment = selectedSegment.GetComponent<Segment>();
                    if (segment)
                    {
                        segment.selected = true;
                    }
                }
            }
        }

        public SegmentSelectedEvent onValueChanged
        {
            get { return m_onValueChanged; }
            set { m_onValueChanged = value; }
        }

        protected SegmentedControl()
        { }

        protected override void Start()
        {
            base.Start();

            if (isActiveAndEnabled)
                StartCoroutine(DelayedInit());
        }

        protected override void OnEnable()
        {
            StartCoroutine(DelayedInit());
        }

        IEnumerator DelayedInit()
        {
            yield return null;

            LayoutSegments();

            if (m_selectedSegmentIndex != -1)
                selectedSegmentIndex = m_selectedSegmentIndex;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (isActiveAndEnabled)
                StartCoroutine(DelayedInit());

            if (m_selectedSegmentIndex > transform.childCount)
            {
                selectedSegmentIndex = transform.childCount - 1;
            }
        }
#endif

        private Selectable[] GetChildSegments()
        {
            var buttons = GetComponentsInChildren<Selectable>();
            if (buttons.Length < 2)
            {
                throw new InvalidOperationException("A segmented control must have at least two Button children");
            }

            for (int i = 0; i < buttons.Length; i++)
            {
                var segment = buttons[i].GetComponent<Segment>();
                if (segment != null)
                {
                    segment.index = i;
                    segment.segmentedControl = this;
                }
            }

            return buttons;
        }

        private void RecreateSprites()
        {
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i].image == null)
                    continue;

                var sprite = CutSprite(segments[i].image.sprite, i == 0, i == segments.Length - 1);
                var segment = segments[i].GetComponent<Segment>();
                if (segment)
                {
                    segment.cutSprite = sprite;
                }
                segments[i].image.overrideSprite = sprite;
            }
        }

        static internal Sprite CutSprite(Sprite sprite, bool leftmost, bool rightmost)
        {
            if (sprite.border.x == 0 || sprite.border.z == 0)
                return sprite;

            var rect = sprite.rect;
            var border = sprite.border;

            if (!leftmost)
            {
                rect.xMin = border.x;
                border.x = 0;
            }
            if (!rightmost)
            {
                rect.xMax = border.z;
                border.z = 0;
            }

            return Sprite.Create(sprite.texture, rect, sprite.pivot, sprite.pixelsPerUnit, 0, SpriteMeshType.FullRect, border);
        }

        public void LayoutSegments()
        {
            RecreateSprites();

            RectTransform transform = this.transform as RectTransform;
            float width = (transform.rect.width / segments.Length) - (SeparatorWidth * (segments.Length - 1));

            for (int i = 0; i < segments.Length; i++)
            {
                float insetX = ((width + SeparatorWidth) * i);

                var rectTransform = segments[i].GetComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;
                rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, insetX, width);
                rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, transform.rect.height);

                if (separator && i > 0)
                {
                    var sepTransform = gameObject.transform.Find("Separator " + i);
                    Graphic sep = (sepTransform != null) ? sepTransform.GetComponent<Graphic>() : (GameObject.Instantiate(separator.gameObject) as GameObject).GetComponent<Graphic>();
                    sep.gameObject.name = "Separator " + i;
                    sep.gameObject.SetActive(true);
                    sep.rectTransform.SetParent(this.transform, false);
                    sep.rectTransform.anchorMin = Vector2.zero;
                    sep.rectTransform.anchorMax = Vector2.zero;
                    sep.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, insetX - SeparatorWidth, SeparatorWidth);
                    sep.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, transform.rect.height);
                }
// TODO: maybe adjust text position
            }
        }
    }
}