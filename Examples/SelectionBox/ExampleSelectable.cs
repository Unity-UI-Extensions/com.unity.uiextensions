/// Original Credit Korindian
/// Sourced from - http://forum.unity3d.com/threads/rts-style-drag-selection-box.265739/
/// Updated Credit BenZed
/// Sourced from - http://forum.unity3d.com/threads/color-picker.267043/


namespace UnityEngine.UI.Extensions
{
    public class ExampleSelectable : MonoBehaviour, IBoxSelectable
    {

        #region Implemented members of IBoxSelectable
        bool _selected = false;
        public bool selected
        {
            get
            {
                return _selected;
            }

            set
            {
                _selected = value;
            }
        }

        bool _preSelected = false;
        public bool preSelected
        {
            get
            {
                return _preSelected;
            }

            set
            {
                _preSelected = value;
            }
        }
        #endregion

        //We want the test object to be either a UI element, a 2D element or a 3D element, so we'll get the appropriate components
        SpriteRenderer spriteRenderer;
        Image image;
        Text text;

        void Start()
        {
            spriteRenderer = transform.GetComponent<SpriteRenderer>();
            image = transform.GetComponent<Image>();
            text = transform.GetComponent<Text>();
        }

        void Update()
        {

            //What the game object does with the knowledge that it is selected is entirely up to it.
            //In this case we're just going to change the color.

            //White if deselected.
            Color color = Color.white;

            if (preSelected)
            {
                //Yellow if preselected
                color = Color.yellow;
            }
            if (selected)
            {
                //And green if selected.
                color = Color.green;
            }

            //Set the color depending on what the game object has.
            if (spriteRenderer)
            {
                spriteRenderer.color = color;
            }
            else if (text)
            {
                text.color = color;
            }
            else if (image)
            {
                image.color = color;
            }
            else if (GetComponent<UnityEngine.Renderer>())
            {
                GetComponent<UnityEngine.Renderer>().material.color = color;
            }


        }
    }
}