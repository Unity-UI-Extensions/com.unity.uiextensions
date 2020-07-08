/// Credit zero3growlithe
/// sourced from: http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-2011648
/// Update by xesenix - based on UIScrollToSelection centers on selected element in scrollrect which can move in XY 
///		you can restrict movement by locking axis on ScrollRect component

/*USAGE:
Simply place the script on the ScrollRect that contains the selectable children we'll be scrolling to
and drag'n'drop the RectTransform of the options "container" that we'll be scrolling.*/

using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/UI ScrollTo Selection XY")]
    [RequireComponent(typeof(ScrollRect))]
    public class UIScrollToSelectionXY : MonoBehaviour
    {

        #region Variables

        // settings
        public float scrollSpeed = 10f;

        [SerializeField]
        private RectTransform layoutListGroup = null;

        // temporary variables
        private RectTransform targetScrollObject;
        private bool scrollToSelection = true;

        // references
        private RectTransform scrollWindow = null;
        private ScrollRect targetScrollRect = null;
        #endregion

        // Use this for initialization
        private void Start()
        {
            targetScrollRect = GetComponent<ScrollRect>();
            scrollWindow = targetScrollRect.GetComponent<RectTransform>();
        }

        // Update is called once per frame
        private void Update()
        {
            ScrollRectToLevelSelection();
        }

        private void ScrollRectToLevelSelection()
        {
			// FIX: if you do not do that here events can have null value
			var events = EventSystem.current;

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

            if (selection != targetScrollObject)
			{
				scrollToSelection = true;
			}

            // check if scrolling is possible
            bool isScrollDirectionUnknown = (selection == null || scrollToSelection == false);

            if (isScrollDirectionUnknown == true || selection.transform.parent != layoutListGroup.transform)
			{
				return;
			}

			bool finishedX = false, finishedY = false;
            
			if (targetScrollRect.vertical)
			{
				// move the current scroll rect to correct position
				float selectionPos = -selection.anchoredPosition.y;

				float listPixelAnchor = layoutListGroup.anchoredPosition.y;

				// get the element offset value depending on the cursor move direction
				float offlimitsValue = 0;

				offlimitsValue = listPixelAnchor - selectionPos;
				// move the target scroll rect
				targetScrollRect.verticalNormalizedPosition += (offlimitsValue / layoutListGroup.sizeDelta.y) * Time.deltaTime * scrollSpeed;

				finishedY = Mathf.Abs(offlimitsValue) < 2f;
			}

			if (targetScrollRect.horizontal)
			{
				// move the current scroll rect to correct position
				float selectionPos = -selection.anchoredPosition.x;

				float listPixelAnchor = layoutListGroup.anchoredPosition.x;
				
				// get the element offset value depending on the cursor move direction
				float offlimitsValue = 0;
				
				offlimitsValue = listPixelAnchor - selectionPos;
				// move the target scroll rect
				targetScrollRect.horizontalNormalizedPosition += (offlimitsValue / layoutListGroup.sizeDelta.x) * Time.deltaTime * scrollSpeed;

				finishedX = Mathf.Abs(offlimitsValue) < 2f;
			}
			// check if we reached our destination
			if (finishedX && finishedY) {
				scrollToSelection = false;
			}
            // save last object we were "heading to" to prevent blocking
            targetScrollObject = selection;
        }
    }
}