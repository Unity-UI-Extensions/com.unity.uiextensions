/// Credit drHogan 
/// Sourced from - http://forum.unity3d.com/threads/screenspace-camera-tooltip-controller-sweat-and-tears.293991/#post-1938929
/// updated simonDarksideJ - refactored code to be more performant.
/// updated lucasvinbr - mixed with BoundTooltip, should work with Screenspace Camera (non-rotated) and Overlay
/// *Note - only works for non-rotated Screenspace Camera and Screenspace Overlay canvases at present, needs updating to include rotated Screenspace Camera and Worldspace!

//ToolTip is written by Emiliano Pastorelli, H&R Tallinn (Estonia), http://www.hammerandravens.com
//Copyright (c) 2015 Emiliano Pastorelli, H&R - Hammer&Ravens, Tallinn, Estonia.
//All rights reserved.

//Redistribution and use in source and binary forms are permitted
//provided that the above copyright notice and this paragraph are
//duplicated in all such forms and that any documentation,
//advertising materials, and other materials related to such
//distribution and use acknowledge that the software was developed
//by H&R, Hammer&Ravens. The name of the
//H&R, Hammer&Ravens may not be used to endorse or promote products derived
//from this software without specific prior written permission.
//THIS SOFTWARE IS PROVIDED ``AS IS'' AND WITHOUT ANY EXPRESS OR
//IMPLIED WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Extensions/Tooltip/Tooltip")]
    public class ToolTip : MonoBehaviour
    {
        //text of the tooltip
        private Text _text;
        private RectTransform _rectTransform, canvasRectTransform;

        [Tooltip("The canvas used by the tooltip as positioning and scaling reference. Should usually be the root Canvas of the hierarchy this component is in")]
        public Canvas canvas;

        [Tooltip("Sets if tooltip triggers will run ForceUpdateCanvases and refresh the tooltip's layout group " +
            "(if any) when hovered, in order to prevent momentousness misplacement sometimes caused by ContentSizeFitters")]
        public bool tooltipTriggersCanForceCanvasUpdate = false;

        /// <summary>
        /// the tooltip's Layout Group, if any
        /// </summary>
        private LayoutGroup _layoutGroup;

        //if the tooltip is inside a UI element
        private bool _inside;

        private float width, height;//, canvasWidth, canvasHeight;

        public float YShift,xShift;

        [HideInInspector]
        public RenderMode guiMode;

        private Camera _guiCamera;

        public Camera GuiCamera
        {
            get
            {
                if (!_guiCamera) {
                    _guiCamera = Camera.main;
                }

                return _guiCamera;
            }
        }

        private Vector3 screenLowerLeft, screenUpperRight, shiftingVector;

        /// <summary>
        /// a screen-space point where the tooltip would be placed before applying X and Y shifts and border checks
        /// </summary>
        private Vector3 baseTooltipPos;

        private Vector3 newTTPos;
        private Vector3 adjustedNewTTPos;
        private Vector3 adjustedTTLocalPos;
        private Vector3 shifterForBorders;

        private float borderTest;

        // Standard Singleton Access
        private static ToolTip instance;
        
        public static ToolTip Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<ToolTip>();
                return instance;
            }
        }

        
        void Reset() {
            canvas = GetComponentInParent<Canvas>();
            canvas = canvas.rootCanvas;
        }

        // Use this for initialization
        public void Awake()
        {
            instance = this;
            if (!canvas) {
                canvas = GetComponentInParent<Canvas>();
                canvas = canvas.rootCanvas;
            }

            _guiCamera = canvas.worldCamera;
            guiMode = canvas.renderMode;
            _rectTransform = GetComponent<RectTransform>();
            canvasRectTransform = canvas.GetComponent<RectTransform>();
            _layoutGroup = GetComponentInChildren<LayoutGroup>();

            _text = GetComponentInChildren<Text>();

            _inside = false;

            this.gameObject.SetActive(false);
        }

        //Call this function externally to set the text of the template and activate the tooltip
        public void SetTooltip(string ttext, Vector3 basePos, bool refreshCanvasesBeforeGetSize = false)
        {

            baseTooltipPos = basePos;

            //set the text
            if (_text) {
                _text.text = ttext;
            }
            else {
                Debug.LogWarning("[ToolTip] Couldn't set tooltip text, tooltip has no child Text component");
            }

            ContextualTooltipUpdate(refreshCanvasesBeforeGetSize);

        }

        //call this function on mouse exit to deactivate the template
        public void HideTooltip()
        {
            gameObject.SetActive(false);
            _inside = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (_inside)
            {
                ContextualTooltipUpdate();
            }
        }

        /// <summary>
        /// forces rebuilding of Canvases in order to update the tooltip's content size fitting.
        /// Can prevent the tooltip from being visibly misplaced for one frame when being resized.
        /// Only runs if tooltipTriggersCanForceCanvasUpdate is true
        /// </summary>
        public void RefreshTooltipSize() {
            if (tooltipTriggersCanForceCanvasUpdate) {
                Canvas.ForceUpdateCanvases();

                if (_layoutGroup) {
                    _layoutGroup.enabled = false;
                    _layoutGroup.enabled = true;
                }
                
            }
            
        }

        /// <summary>
        /// Runs the appropriate tooltip placement method, according to the parent canvas's render mode
        /// </summary>
        /// <param name="refreshCanvasesBeforeGettingSize"></param>
        public void ContextualTooltipUpdate(bool refreshCanvasesBeforeGettingSize = false) {
            switch (guiMode) {
                case RenderMode.ScreenSpaceCamera:
                    OnScreenSpaceCamera(refreshCanvasesBeforeGettingSize);
                    break;
                case RenderMode.ScreenSpaceOverlay:
                    OnScreenSpaceOverlay(refreshCanvasesBeforeGettingSize);
                    break;
            }
        }

        //main tooltip edge of screen guard and movement - camera
        public void OnScreenSpaceCamera(bool refreshCanvasesBeforeGettingSize = false)
        {
            shiftingVector.x = xShift;
            shiftingVector.y = YShift;

            baseTooltipPos.z = canvas.planeDistance;

            newTTPos = GuiCamera.ScreenToViewportPoint(baseTooltipPos - shiftingVector);
            adjustedNewTTPos = GuiCamera.ViewportToWorldPoint(newTTPos);

            gameObject.SetActive(true);

            if (refreshCanvasesBeforeGettingSize) RefreshTooltipSize();

            //consider scaled dimensions when comparing against the edges
            width = transform.lossyScale.x * _rectTransform.sizeDelta[0];
            height = transform.lossyScale.y * _rectTransform.sizeDelta[1];

            // check and solve problems for the tooltip that goes out of the screen on the horizontal axis

            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, Vector2.zero, GuiCamera, out screenLowerLeft);
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, new Vector2(Screen.width, Screen.height), GuiCamera, out screenUpperRight);


            //check for right edge of screen
            borderTest = (adjustedNewTTPos.x + width / 2);
            if (borderTest > screenUpperRight.x)
            {
                shifterForBorders.x = borderTest - screenUpperRight.x;
                adjustedNewTTPos.x -= shifterForBorders.x;
            }
            //check for left edge of screen
            borderTest = (adjustedNewTTPos.x - width / 2);
            if (borderTest < screenLowerLeft.x)
            {
                shifterForBorders.x = screenLowerLeft.x - borderTest;
                adjustedNewTTPos.x += shifterForBorders.x;
            }

            // check and solve problems for the tooltip that goes out of the screen on the vertical axis

            //check for lower edge of the screen
            borderTest = (adjustedNewTTPos.y - height / 2);
            if (borderTest < screenLowerLeft.y) {
                shifterForBorders.y = screenLowerLeft.y - borderTest;
                adjustedNewTTPos.y += shifterForBorders.y;
            }

            //check for upper edge of the screen
            borderTest = (adjustedNewTTPos.y + height / 2);
            if (borderTest > screenUpperRight.y)
            {
                shifterForBorders.y = borderTest - screenUpperRight.y;
                adjustedNewTTPos.y -= shifterForBorders.y;
            }

            //failed attempt to circumvent issues caused when rotating the camera
            adjustedNewTTPos = transform.rotation * adjustedNewTTPos;

            transform.position = adjustedNewTTPos;
            adjustedTTLocalPos = transform.localPosition;
            adjustedTTLocalPos.z = 0;
            transform.localPosition = adjustedTTLocalPos;

            _inside = true;
        }


        //main tooltip edge of screen guard and movement - overlay
        public void OnScreenSpaceOverlay(bool refreshCanvasesBeforeGettingSize = false) {
            shiftingVector.x = xShift;
            shiftingVector.y = YShift;
            newTTPos = (baseTooltipPos - shiftingVector) / canvas.scaleFactor;
            adjustedNewTTPos = newTTPos;

            gameObject.SetActive(true);

            if (refreshCanvasesBeforeGettingSize) RefreshTooltipSize();

            width = _rectTransform.sizeDelta[0];
            height = _rectTransform.sizeDelta[1];

            // check and solve problems for the tooltip that goes out of the screen on the horizontal axis
            //screen's 0 = overlay canvas's 0 (always?)
            screenLowerLeft = Vector3.zero;
            screenUpperRight = canvasRectTransform.sizeDelta;

            //check for right edge of screen
            borderTest = (newTTPos.x + width / 2);
            if (borderTest > screenUpperRight.x) {
                shifterForBorders.x = borderTest - screenUpperRight.x;
                adjustedNewTTPos.x -= shifterForBorders.x;
            }
            //check for left edge of screen
            borderTest = (adjustedNewTTPos.x - width / 2);
            if (borderTest < screenLowerLeft.x) {
                shifterForBorders.x = screenLowerLeft.x - borderTest;
                adjustedNewTTPos.x += shifterForBorders.x;
            }

            // check and solve problems for the tooltip that goes out of the screen on the vertical axis

            //check for lower edge of the screen
            borderTest = (adjustedNewTTPos.y - height / 2);
            if (borderTest < screenLowerLeft.y) {
                shifterForBorders.y = screenLowerLeft.y - borderTest;
                adjustedNewTTPos.y += shifterForBorders.y;
            }

            //check for upper edge of the screen
            borderTest = (adjustedNewTTPos.y + height / 2);
            if (borderTest > screenUpperRight.y) {
                shifterForBorders.y = borderTest - screenUpperRight.y;
                adjustedNewTTPos.y -= shifterForBorders.y;
            }

            //remove scale factor for the actual positioning of the TT
            adjustedNewTTPos *= canvas.scaleFactor;
            transform.position = adjustedNewTTPos;

            _inside = true;
        }
    }
}