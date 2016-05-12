/// Credit zero3growlithe
/// sourced from: http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-2011648

/*USAGE:
Simply place the script on the ScrollRect that contains the selectable children we'll be scroling to
and drag'n'drop the RectTransform of the options "container" that we'll be scrolling.*/

using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(ScrollRect))]
    [AddComponentMenu("UI/Extensions/UIScrollToSelection")]
    public class UIScrollToSelection : MonoBehaviour {
 
	//*** ATTRIBUTES ***//
		[Header("[ References ]")]
		[SerializeField]
		private RectTransform layoutListGroup;
	   
		[Header("[ Settings ]")]
		[SerializeField]
		private float scrollSpeed = 10f;
	 
	//*** PROPERTIES ***//
		// REFERENCES
		protected RectTransform LayoutListGroup {
				get {return layoutListGroup;}
		}
 
		// SETTINGS
		protected float ScrollSpeed {
				get {return scrollSpeed;}
		}
 
		// VARIABLES
		protected RectTransform TargetScrollObject {get; set;}
		protected RectTransform ScrollWindow {get; set;}
		protected ScrollRect TargetScrollRect {get; set;}
	 
	//*** METHODS - PUBLIC ***//
	 
	 
	//*** METHODS - PROTECTED ***//
		protected virtual void Awake (){
			TargetScrollRect = GetComponent<ScrollRect>();
			ScrollWindow = TargetScrollRect.GetComponent<RectTransform>();
		}
 
		protected virtual void Start (){
 
		}
	   
		protected virtual void Update (){
			ScrollRectToLevelSelection();
		}
	 
	//*** METHODS - PRIVATE ***//
		private void ScrollRectToLevelSelection (){
			// check main references
			bool referencesAreIncorrect = (TargetScrollRect == null || LayoutListGroup == null || ScrollWindow == null);

			if (referencesAreIncorrect == true){
					return;
			}

			// get calculation references
			EventSystem events = EventSystem.current;
			RectTransform selection =
					events.currentSelectedGameObject != null ?
					events.currentSelectedGameObject.GetComponent<RectTransform>() :
					null;

			// check if scrolling is possible
			if (selection == null ||
					selection.transform.parent != LayoutListGroup.transform)
			{
					return;
			}

			// move the current scroll rect to correct position
			float selectionPos = -selection.anchoredPosition.y;

			float elementHeight = LayoutListGroup.rect.height / LayoutListGroup.transform.childCount;
			float maskHeight = ScrollWindow.rect.height;
			float listPixelAnchor = LayoutListGroup.anchoredPosition.y;

			// get the element offset value depending on the cursor move direction
			float offlimitsValue = 0;

			if (selectionPos < listPixelAnchor){
					offlimitsValue = listPixelAnchor - selectionPos;
			} else if (selectionPos + elementHeight > listPixelAnchor + maskHeight){
					offlimitsValue = (listPixelAnchor + maskHeight) - (selectionPos + elementHeight);
			}

			// move the target scroll rect
			TargetScrollRect.verticalNormalizedPosition +=
					(offlimitsValue / LayoutListGroup.rect.height) * Time.deltaTime * scrollSpeed;
		   
			// save last object we were "heading to" to prevent blocking
			TargetScrollObject = selection;
		}
	}
}