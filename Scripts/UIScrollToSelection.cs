/// Credit zero3growlithe
/// sourced from: http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-2011648

/*USAGE:
Simply place the script on the ScrollRect that contains the selectable children we'll be scroling to
and drag'n'drop the RectTransform of the options "container" that we'll be scrolling.*/

using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(ScrollRect))]
    public class UIScrollToSelection : MonoBehaviour
    {

        #region Variables

        // settings
        public float scrollSpeed = 10f;

        [SerializeField]
        private RectTransform layoutListGroup;

        // temporary variables
        private RectTransform targetScrollObject;
        private bool scrollToSelection = true;

        // references
        private RectTransform scrollWindow;
        private RectTransform currentCanvas;
        private ScrollRect targetScrollRect;

        private EventSystem events = EventSystem.current;
        #endregion

        // Use this for initialization
        private void Start()
        {
            targetScrollRect = GetComponent<ScrollRect>();
            scrollWindow = targetScrollRect.GetComponent<RectTransform>();
            currentCanvas = transform.root.GetComponent<RectTransform>();
        }

        // Update is called once per frame
        private void Update()
        {
            ScrollRectToLevelSelection();
        }

        private void ScrollRectToLevelSelection()
        {
            // check main references
            bool referencesAreIncorrect =
                (targetScrollRect == null || layoutListGroup == null || scrollWindow == null);
            if (referencesAreIncorrect == true)
            {
                return;
            }

            // get calculation references
            RectTransform selection = events.currentSelectedGameObject != null ?
                events.currentSelectedGameObject.GetComponent<RectTransform>() :
                null;
            RectTransform lastSelection = events.lastSelectedGameObject != null ?
                events.lastSelectedGameObject.GetComponent<RectTransform>() :
                selection;

            if (selection != targetScrollObject)
                scrollToSelection = true;

            // check if scrolling is possible
            bool isScrollDirectionUnknown =
                (selection == null || lastSelection == null || scrollToSelection == false);

            if (isScrollDirectionUnknown == true || selection.transform.parent != layoutListGroup.transform)
                return;

            // move the current scroll rect to correct position
            float selectionPos = -selection.anchoredPosition.y;
            int direction = (int)Mathf.Sign(selection.anchoredPosition.y - lastSelection.anchoredPosition.y);

            float elementHeight = layoutListGroup.sizeDelta.y / layoutListGroup.transform.childCount;
            float maskHeight = currentCanvas.sizeDelta.y + scrollWindow.sizeDelta.y;
            float listPixelAnchor = layoutListGroup.anchoredPosition.y;

            // get the element offset value depending on the cursor move direction
            float offlimitsValue = 0;
            if (direction > 0 && selectionPos < listPixelAnchor)
            {
                offlimitsValue = listPixelAnchor - selectionPos;
            }
            if (direction < 0 && selectionPos + elementHeight > listPixelAnchor + maskHeight)
            {
                offlimitsValue = (listPixelAnchor + maskHeight) - (selectionPos + elementHeight);
            }
            // move the target scroll rect
            targetScrollRect.verticalNormalizedPosition +=
                (offlimitsValue / layoutListGroup.sizeDelta.y) * Time.deltaTime * scrollSpeed;
            // check if we reached our destination
            if (Mathf.Abs(offlimitsValue) < 2f)
                scrollToSelection = false;
            // save last object we were "heading to" to prevent blocking
            targetScrollObject = selection;
        }
    }
}