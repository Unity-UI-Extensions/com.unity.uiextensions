///Credit perchik
///Sourced from - http://forum.unity3d.com/threads/receive-onclick-event-and-pass-it-on-to-lower-ui-elements.293642/

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(RectTransform), typeof(Button))]
    public class DropDownListButton
    {
        public RectTransform rectTransform;
        public Button btn;
#if UNITY_6000_0_OR_NEWER
        public TMPro.TMP_Text txt;   // TextMeshPro caption (if present)
#endif
        public Text uiText;          // legacy Unity UI Text caption (if present)
        public Image btnImg;
        public Image img;
        public GameObject gameobject;

        public DropDownListButton(GameObject btnObj)
        {
            gameobject = btnObj;
            rectTransform = btnObj.GetComponent<RectTransform>();
            btnImg = btnObj.GetComponent<Image>();
            btn = btnObj.GetComponent<Button>();

            // Resolve the caption as either a TextMeshPro or a legacy UI Text component so the
            // control works whichever was authored on the "Text" child.
            Transform textTransform = rectTransform.Find("Text");
            if (textTransform != null)
            {
#if UNITY_6000_0_OR_NEWER
                txt = textTransform.GetComponent<TMPro.TMP_Text>();
#endif
                uiText = textTransform.GetComponent<Text>();
            }

            Transform imageTransform = rectTransform.Find("Image");
            if (imageTransform != null)
            {
                img = imageTransform.GetComponent<Image>();
            }
        }

        /// <summary>
        /// Gets or sets the caption on whichever text component (TextMeshPro or legacy UI Text) is present.
        /// </summary>
        public string Caption
        {
            get
            {
#if UNITY_6000_0_OR_NEWER
                if (txt != null) { return txt.text; }
#endif
                return uiText != null ? uiText.text : string.Empty;
            }
            set
            {
#if UNITY_6000_0_OR_NEWER
                if (txt != null) { txt.text = value; return; }
#endif
                if (uiText != null) { uiText.text = value; }
            }
        }

        /// <summary>
        /// Gets or sets the caption colour on whichever text component is present.
        /// </summary>
        public Color CaptionColor
        {
            get
            {
#if UNITY_6000_0_OR_NEWER
                if (txt != null) { return txt.color; }
#endif
                return uiText != null ? uiText.color : Color.white;
            }
            set
            {
#if UNITY_6000_0_OR_NEWER
                if (txt != null) { txt.color = value; return; }
#endif
                if (uiText != null) { uiText.color = value; }
            }
        }
    }
}