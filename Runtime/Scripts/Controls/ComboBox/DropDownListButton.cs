///Credit perchik
///Sourced from - http://forum.unity3d.com/threads/receive-onclick-event-and-pass-it-on-to-lower-ui-elements.293642/

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(RectTransform), typeof(Button))]
    public class DropDownListButton
    {
        public RectTransform rectTransform;
        public Button btn;
#if UNITY_2022_1_OR_NEWER
        public TMPro.TMP_Text txt;
#else
        public Text txt;
#endif
        public Image btnImg;
        public Image img;
        public GameObject gameobject;

        public DropDownListButton(GameObject btnObj)
        {
            gameobject = btnObj;
            rectTransform = btnObj.GetComponent<RectTransform>();
            btnImg = btnObj.GetComponent<Image>();
            btn = btnObj.GetComponent<Button>();
#if UNITY_2022_1_OR_NEWER
            txt = rectTransform.Find("Text").GetComponent<TMPro.TMP_Text>();
#else
            txt = rectTransform.Find("Text").GetComponent<Text>();
#endif
            img = rectTransform.Find("Image").GetComponent<Image>();
        }
    }
}