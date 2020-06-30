///Credit perchik
///Sourced from - http://forum.unity3d.com/threads/receive-onclick-event-and-pass-it-on-to-lower-ui-elements.293642/

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(RectTransform), typeof(Button))]
    public class DropDownListButton
    {
        public RectTransform rectTransform;
        public Button btn;
        public Text txt;
        public Image btnImg;
        public Image img;
        public GameObject gameobject;

        public DropDownListButton(GameObject btnObj)
        {
            gameobject = btnObj;
            rectTransform = btnObj.GetComponent<RectTransform>();
            btnImg = btnObj.GetComponent<Image>();
            btn = btnObj.GetComponent<Button>();
            txt = rectTransform.Find("Text").GetComponent<Text>();
            img = rectTransform.Find("Image").GetComponent<Image>();
        }
    }
}