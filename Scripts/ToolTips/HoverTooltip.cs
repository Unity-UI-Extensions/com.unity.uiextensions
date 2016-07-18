/// Credit drHogan 
/// Sourced from - http://www.hammerandravens.com/multi-use-tooltip-system-in-unity3d/

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/HoverTooltip")]
    public class HoverTooltip : MonoBehaviour
    {
        //manually selectable padding for the background image
        public int horizontalPadding;
        public int verticalPadding;

        //tooltip text
        public Text thisText;

        //horizontal layout of the tooltip
        public HorizontalLayoutGroup hlG;

        //tooltip background image
        public RectTransform bgImage;
        Image bgImageSource;

        //needed as the layout refreshes only on the first Update() call
        bool firstUpdate;

        //if the tooltip is inside a UI element
        bool inside;

        //size of the tooltip, needed to track if out of screen
        // public float width;
        // public float height;

        //detect canvas mode so to apply different behaviors to different canvas modes, currently only RenderMode.ScreenSpaceCamera implemented
        //int canvasMode;
        RenderMode GUIMode;

        //the scene GUI camera
        Camera GUICamera;

        //the default tooltip object has the following pivots, so that the offset from the mouse is always proportional to the screen resolution (the y pivot)
        //Pivot(0.5,-0.5)

        //screen viewport corners for out of screen detection
        Vector3 lowerLeft;
        Vector3 upperRight;

        //scale factor of proportionality to the reference resolution (1280x720)
        float currentYScaleFactor;
        float currentXScaleFactor;

        //standard X and Y offsets of the new tooltip
        float defaultYOffset;
        float defaultXOffset;

        //real on screen sizes of the tooltip object
        float tooltipRealHeight;
        float tooltipRealWidth;

        // Use this for initialization
        void Start()
        {
            //in this line you need to change the string in order to get your Camera //TODO MAYBE DO IT FROM THE INSPECTOR
            GUICamera = GameObject.Find("GUICamera").GetComponent<Camera>();
            GUIMode = this.transform.parent.parent.GetComponent<Canvas>().renderMode;

            bgImageSource = bgImage.GetComponent<Image>();

            //at start the pointer is never to be considered over and UI element
            inside = false;

            //assign the tooltip to the singleton GUI class manager for fast access
            //TacticalGUIManager.tgm.mmttp = this;

            //hide the tooltip
            HideTooltipVisibility();
            this.transform.parent.gameObject.SetActive(false);
        }


        //single string input tooltip
        public void SetTooltip(string text)
        {
            NewTooltip();

            //init tooltip string
            thisText.text = text;

            //call the position function
            OnScreenSpaceCamera();
        }


        //multi string/line input tooltip (each string of the input array is a new line)
        public void SetTooltip(string[] texts)
        {
            NewTooltip();

            //build up the tooltip line after line with the input
            string tooltipText = "";
            int index = 0;
            foreach (string newLine in texts)
            {
                if (index == 0)
                {
                    tooltipText += newLine;
                }
                else
                {
                    tooltipText += ("\n" + newLine);
                }
                index++;
            }

            //init tooltip string
            thisText.text = tooltipText;

            //call the position function
            OnScreenSpaceCamera();
        }



        //temporary call to don't fuck up old code, will be removed
        public void SetTooltip(string text, bool test)
        {
            NewTooltip();

            //init tooltip string
            thisText.text = text;

            //call the position function
            OnScreenSpaceCamera();
        }


        //position function, currently not working correctly due to the use of pivots and not manual offsets, soon to be fixed
        public void OnScreenSpaceCamera()
        {
            //get the dynamic position of the pous in viewport coordinates
            Vector3 newPos = GUICamera.ScreenToViewportPoint(Input.mousePosition);

            // store in val the updated position (x or y) of the tooltip edge of interest
            float val;

            //store the new offset to impose in case of out of screen
            float yOffSet = 0f;
            float xOffSet = 0f;

            //check for right edge of screen
            //obtain the x coordinate of the right edge of the tooltip
            val = ((GUICamera.ViewportToScreenPoint(newPos).x) + (tooltipRealWidth * bgImage.pivot.x));

            //evaluate if the right edge of the tooltip goes out of screen
            if (val > (upperRight.x))
            {
                float distFromRight = upperRight.x - val;

                if (distFromRight > (defaultXOffset * 0.75))
                {
                    //shorten the temporary offset up to a certain distance from the tooltip
                    xOffSet = distFromRight;
                }
                else
                {
                    //if the distance becomes too short flip the tooltip to below the pointer (by offset+twice the height of the tooltip)
                    xOffSet = ((defaultXOffset) - (tooltipRealWidth) * 2f);
                }

                //assign the new modified coordinates to the tooltip and convert to screen coordinates
                Vector3 newTooltipPos = new Vector3(GUICamera.ViewportToScreenPoint(newPos).x + xOffSet, 0f, 0f);

                newPos.x = GUICamera.ScreenToViewportPoint(newTooltipPos).x;
            }

            //check for left edge of screen
            //obtain the x coordinate of the left edge of the tooltip
            val = ((GUICamera.ViewportToScreenPoint(newPos).x) - (tooltipRealWidth * bgImage.pivot.x));

            //evaluate if the left edge of the tooltip goes out of screen
            if (val < (lowerLeft.x))
            {
                float distFromLeft = lowerLeft.x - val;

                if (distFromLeft < (defaultXOffset * 0.75 - tooltipRealWidth))
                {
                    //shorten the temporary offset up to a certain distance from the tooltip
                    xOffSet = -distFromLeft;
                }
                else
                {
                    //if the distance becomes too short flip the tooltip to above the pointer (by twice the height of the tooltip)
                    xOffSet = ((tooltipRealWidth) * 2f);
                }

                //assign the new modified coordinates to the tooltip and convert to screen coordinates
                Vector3 newTooltipPos = new Vector3(GUICamera.ViewportToScreenPoint(newPos).x - xOffSet, 0f, 0f);

                newPos.x = GUICamera.ScreenToViewportPoint(newTooltipPos).x;
            }

            //check for upper edge of the screen
            //obtain the y coordinate of the upper edge of the tooltip
            val = ((GUICamera.ViewportToScreenPoint(newPos).y) - ((bgImage.sizeDelta.y * currentYScaleFactor * (bgImage.pivot.y)) - (tooltipRealHeight)));
            //evaluate if the upper edge of the tooltip goes out of screen
            if (val > (upperRight.y))
            {
                float distFromUpper = upperRight.y - val;
                yOffSet = (bgImage.sizeDelta.y * currentYScaleFactor * (bgImage.pivot.y));

                if (distFromUpper > (defaultYOffset * 0.75))
                {
                    //shorten the temporary offset up to a certain distance from the tooltip
                    yOffSet = distFromUpper;
                }
                else
                {
                    //if the distance becomes too short flip the tooltip to below the pointer (by offset+twice the height of the tooltip)
                    yOffSet = ((defaultYOffset) - (tooltipRealHeight) * 2f);
                }

                //assign the new modified coordinates to the tooltip and convert to screen coordinates
                Vector3 newTooltipPos = new Vector3(newPos.x, GUICamera.ViewportToScreenPoint(newPos).y + yOffSet, 0f);
                newPos.y = GUICamera.ScreenToViewportPoint(newTooltipPos).y;
            }

            //check for lower edge of the screen
            //obtain the y coordinate of the lower edge of the tooltip
            val = ((GUICamera.ViewportToScreenPoint(newPos).y) - ((bgImage.sizeDelta.y * currentYScaleFactor * (bgImage.pivot.y))));

            //evaluate if the upper edge of the tooltip goes out of screen
            if (val < (lowerLeft.y))
            {
                float distFromLower = lowerLeft.y - val;
                yOffSet = (bgImage.sizeDelta.y * currentYScaleFactor * (bgImage.pivot.y));

                if (distFromLower < (defaultYOffset * 0.75 - tooltipRealHeight))
                {
                    //shorten the temporary offset up to a certain distance from the tooltip
                    yOffSet = distFromLower;
                }
                else
                {
                    //if the distance becomes too short flip the tooltip to above the pointer (by twice the height of the tooltip)
                    yOffSet = ((tooltipRealHeight) * 2f);
                }

                //assign the new modified coordinates to the tooltip and convert to screen coordinates
                Vector3 newTooltipPos = new Vector3(newPos.x, GUICamera.ViewportToScreenPoint(newPos).y + yOffSet, 0f);
                newPos.y = GUICamera.ScreenToViewportPoint(newTooltipPos).y;
            }

            this.transform.parent.transform.position = new Vector3(GUICamera.ViewportToWorldPoint(newPos).x, GUICamera.ViewportToWorldPoint(newPos).y, 0f);
            this.transform.parent.gameObject.SetActive(true);
            inside = true;
        }

        //call to hide tooltip when hovering out from the object
        public void HideTooltip()
        {
            if (GUIMode == RenderMode.ScreenSpaceCamera)
            {
                if (this != null)
                {
                    this.transform.parent.gameObject.SetActive(false);
                    inside = false;
                    HideTooltipVisibility();
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            LayoutInit();
            if (inside)
            {
                if (GUIMode == RenderMode.ScreenSpaceCamera)
                {
                    OnScreenSpaceCamera();
                }
            }
        }

        //this function is used in order to setup the size of the tooltip by cheating on the HorizontalLayoutBehavior. The resize is done in the first update.
        void LayoutInit()
        {
            if (firstUpdate)
            {
                firstUpdate = false;

                bgImage.sizeDelta = new Vector2(hlG.preferredWidth + horizontalPadding, hlG.preferredHeight + verticalPadding);

                defaultYOffset = (bgImage.sizeDelta.y * currentYScaleFactor * (bgImage.pivot.y));
                defaultXOffset = (bgImage.sizeDelta.x * currentXScaleFactor * (bgImage.pivot.x));

                tooltipRealHeight = bgImage.sizeDelta.y * currentYScaleFactor;
                tooltipRealWidth = bgImage.sizeDelta.x * currentXScaleFactor;

                ActivateTooltipVisibility();
            }
        }

        //init basic variables on a new tooltip set
        void NewTooltip()
        {
            firstUpdate = true;

            lowerLeft = GUICamera.ViewportToScreenPoint(new Vector3(0.0f, 0.0f, 0.0f));
            upperRight = GUICamera.ViewportToScreenPoint(new Vector3(1.0f, 1.0f, 0.0f));

            currentYScaleFactor = Screen.height / this.transform.root.GetComponent<CanvasScaler>().referenceResolution.y;
            currentXScaleFactor = Screen.width / this.transform.root.GetComponent<CanvasScaler>().referenceResolution.x;

        }

        //used to visualize the tooltip one update call after it has been built (to avoid flickers)
        public void ActivateTooltipVisibility()
        {
            Color textColor = thisText.color;
            thisText.color = new Color(textColor.r, textColor.g, textColor.b, 1f);
            bgImageSource.color = new Color(bgImageSource.color.r, bgImageSource.color.g, bgImageSource.color.b, 0.8f);
        }

        //used to hide the tooltip so that it can be made visible one update call after it has been built (to avoid flickers)
        public void HideTooltipVisibility()
        {
            Color textColor = thisText.color;
            thisText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
            bgImageSource.color = new Color(bgImageSource.color.r, bgImageSource.color.g, bgImageSource.color.b, 0f);
        }

    }
}