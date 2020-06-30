///Original Credit Korindian
///Sourced from - http://forum.unity3d.com/threads/rts-style-drag-selection-box.265739/
///Updated Credit BenZed
///Sourced from - http://forum.unity3d.com/threads/color-picker.267043/

/*
 * What the SelectionBox component does is allow the game player to select objects using an RTS style click and drag interface:
 * 
 * We want to be able to select Game Objects of any type,
 * We want to be able to drag select a group of game objects to select them,
 * We want to be able to hold the shift key and drag select a group of game objects to add them to the current selection,
 * We want to be able to single click a game object to select it,
 * We want to be able to hold the shift key and single click a game object to add it to the current selection, 
 * We want to be able to hold the shift key and single click an already selected game object to remove it from the current selection.
 * 
 * Most importantly, we want this behaviour to work with UI, 2D or 3D gameObjects, so it has to be smart about considering their respective screen spaces.
 * 
 * Add this component to a Gameobject with a Canvas with RenderMode.ScreenSpaceOverlay
 * And implement the IBoxSelectable interface on any MonoBehaviour to make it selectable.
 * 
 * Improvements that could be made:
 * 
 * Control clicking a game object to select all objects of that type or tag.
 * Compatibility with Canvas Scaling
 * Filtering single click selections of objects occupying the same space. (So that, for example, you're only click selecting the game object found closest to the camera)
 * 
 */

using System.Collections.Generic;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(Canvas))]
	[AddComponentMenu("UI/Extensions/Selection Box")]
	public class SelectionBox : MonoBehaviour
	{
		
		// The color of the selection box.
		public Color color;
		
		// An optional parameter, but you can add a sprite to the selection box to give it a border or a stylized look.
		// It's suggested you use a monochrome sprite so that the selection
		// Box color is still relevent.
		public Sprite art;
		
		// Will store the location of wherever we first click before dragging.
		private Vector2 origin;
		
		// A rectTransform set by the User that can limit which part of the screen is eligable for drag selection
		public RectTransform selectionMask;
		
		//Stores the rectTransform connected to the generated gameObject being used for the selection box visuals
		private RectTransform boxRect;
		
		// Stores all of the selectable game objects
		private IBoxSelectable[] selectables;
		
		// A secondary storage of objects that the user can manually set.
		private MonoBehaviour[] selectableGroup;
		
		//Stores the selectable that was touched when the mouse button was pressed down
		private IBoxSelectable clickedBeforeDrag;
		
		//Stores the selectable that was touched when the mouse button was released
		private IBoxSelectable clickedAfterDrag;
		
		//Custom UnityEvent so we can add Listeners to this instance when Selections are changed.
		public class SelectionEvent : UnityEvent<IBoxSelectable[]> {}
		public SelectionEvent onSelectionChange = new SelectionEvent();
		
		//Ensures that the canvas that this component is attached to is set to the correct render mode. If not, it will not render the selection box properly.
		void ValidateCanvas(){
			var canvas = gameObject.GetComponent<Canvas>();
			
			if (canvas.renderMode != RenderMode.ScreenSpaceOverlay) {
				throw new System.Exception("SelectionBox component must be placed on a canvas in Screen Space Overlay mode.");
			}
			
			var canvasScaler = gameObject.GetComponent<CanvasScaler>();
			
			if (canvasScaler && canvasScaler.enabled && (!Mathf.Approximately(canvasScaler.scaleFactor, 1f) || canvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ConstantPixelSize)) {
				Destroy(canvasScaler);
				Debug.LogWarning("SelectionBox component is on a gameObject with a Canvas Scaler component. As of now, Canvas Scalers without the default settings throw off the coordinates of the selection box. Canvas Scaler has been removed.");
			}
		}
		
		/*
	 * The user can manually set a group of objects with monoBehaviours to be the pool of objects considered to be selectable. The benefits of this are two fold:
	 *
	 * 1) The default behaviour is to check every game object in the scene, which is much slower.
	 * 2) The user can filter which objects should be selectable, for example units versus menu selections
	 *
	 */
		void SetSelectableGroup(IEnumerable<MonoBehaviour> behaviourCollection) {
			
			// If null, the selectionbox reverts to it's default behaviour
			if (behaviourCollection == null) {
				selectableGroup = null;
				
				return;
			}
			
			//Runs a double check to ensure each of the objects in the collection can be selectable, and doesn't include them if not.
			var behaviourList = new List<MonoBehaviour>();
			
			foreach(var behaviour in behaviourCollection) {
				if (behaviour as IBoxSelectable != null) {
					behaviourList.Add (behaviour);
				}
			}
			
			selectableGroup = behaviourList.ToArray();
		}
		
		void CreateBoxRect(){
			var selectionBoxGO = new GameObject();
			
			selectionBoxGO.name = "Selection Box";
			selectionBoxGO.transform.parent = transform;
			selectionBoxGO.AddComponent<Image>();
			
			boxRect = selectionBoxGO.transform as RectTransform;
			
		}
		
		//Set all of the relevant rectTransform properties to zero, 
		//finally deactivates the boxRect gameobject since it doesn't
		//need to be enabled when not in a selection action.
		void ResetBoxRect(){
			
			//Update the art and color on the off chance they've changed
			Image image = boxRect.GetComponent<Image>();
			image.color = color;
			image.sprite = art;
			
			origin = Vector2.zero;
			
			boxRect.anchoredPosition = Vector2.zero;
			boxRect.sizeDelta = Vector2.zero;
			boxRect.anchorMax = Vector2.zero;
			boxRect.anchorMin = Vector2.zero;
			boxRect.pivot = Vector2.zero;
			boxRect.gameObject.SetActive(false);
		}
		
		
		void BeginSelection(){
			// Click somewhere in the Game View.
			if (!Input.GetMouseButtonDown(0))
				return;
			
			//The boxRect will be inactive up until the point we start selecting
			boxRect.gameObject.SetActive(true);
			
			// Get the initial click position of the mouse. 
			origin = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			
			//If the initial click point is not inside the selection mask, we abort the selection
			if (!PointIsValidAgainstSelectionMask(origin)) {
				ResetBoxRect();
				return;
			}
			
			// The anchor is set to the same place.
			boxRect.anchoredPosition = origin;
			
			MonoBehaviour[] behavioursToGetSelectionsFrom;
			
			// If we do not have a group of selectables already set, we'll just loop through every object that's a monobehaviour, and look for selectable interfaces in them
			if (selectableGroup == null) {
				behavioursToGetSelectionsFrom = GameObject.FindObjectsOfType<MonoBehaviour>();
			} else {
				behavioursToGetSelectionsFrom = selectableGroup;
			}
			
			//Temporary list to store the found selectables before converting to the main selectables array
			List<IBoxSelectable> selectableList = new List<IBoxSelectable>();
			
			foreach (MonoBehaviour behaviour in behavioursToGetSelectionsFrom) {
				
				//If the behaviour implements the selectable interface, we add it to the selectable list
				IBoxSelectable selectable = behaviour as IBoxSelectable;
				if (selectable != null) {
					selectableList.Add (selectable); 
					
					//We're using left shift to act as the "Add To Selection" command. So if left shift isn't pressed, we want everything to begin deselected
					if (!Input.GetKey (KeyCode.LeftShift)) {
						selectable.selected = false;
					}
				}
				
			}
			selectables = selectableList.ToArray();
			
			//For single-click actions, we need to get the selectable that was clicked when selection began (if any)
			clickedBeforeDrag = GetSelectableAtMousePosition();
			
		}
		
		bool PointIsValidAgainstSelectionMask(Vector2 screenPoint){
			//If there is no seleciton mask, any point is valid
			if (!selectionMask) {
				return true;
			}
			
			Camera screenPointCamera = GetScreenPointCamera(selectionMask);
			
			return RectTransformUtility.RectangleContainsScreenPoint(selectionMask, screenPoint, screenPointCamera);
		}
		
		IBoxSelectable GetSelectableAtMousePosition() {
			//Firstly, we cannot click on something that is not inside the selection mask (if we have one)
			if (!PointIsValidAgainstSelectionMask(Input.mousePosition)) {
				return null;
			}
			
			//This gets a bit tricky, because we have to make considerations depending on the heirarchy of the selectable's gameObject
			foreach (var selectable in selectables) {
				
				//First we check to see if the selectable has a rectTransform
				var rectTransform = (selectable.transform as RectTransform);
				
				if (rectTransform) {
					//Because if it does, the camera we use to calulate it's screen point will vary
					var screenCamera = GetScreenPointCamera(rectTransform);
					
					//Once we've found the rendering camera, we check if the selectables rectTransform contains the click. That way we
					//Can click anywhere on a rectTransform to select it.
					if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, screenCamera)) {
						
						//And if it does, we select it and send it back
						return selectable;
					}
				} else {
					//If it doesn't have a rectTransform, we need to get the radius so we can use it as an area around the center to detect a click.
					//This works because a 2D or 3D renderer will both return a radius
					var radius = selectable.transform.GetComponent<UnityEngine.Renderer>().bounds.extents.magnitude;
					
					var selectableScreenPoint = GetScreenPointOfSelectable(selectable);
					
					//Check that the click fits within the screen-radius of the selectable
					if (Vector2.Distance(selectableScreenPoint, Input.mousePosition) <= radius) {
						
						//And if it does, we select it and send it back
						return selectable;
					}
					
				}
			}
			
			return null;
		}
		
		
		void DragSelection(){
			//Return if we're not dragging or if the selection has been aborted (BoxRect disabled)
			if (!Input.GetMouseButton(0) || !boxRect.gameObject.activeSelf)
				return;
			
			// Store the current mouse position in screen space.
			Vector2 currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			
			// How far have we moved the mouse?
			Vector2 difference = currentMousePosition - origin;
			
			// Copy the initial click position to a new variable. Using the original variable will cause
			// the anchor to move around to wherever the current mouse position is,
			// which isn't desirable.
			Vector2 startPoint = origin;
			
			// The following code accounts for dragging in various directions.
			if (difference.x < 0)
			{
				startPoint.x = currentMousePosition.x;
				difference.x = -difference.x;
			}
			if (difference.y < 0)
			{
				startPoint.y = currentMousePosition.y;
				difference.y = -difference.y;
			}
			
			// Set the anchor, width and height every frame.
			boxRect.anchoredPosition = startPoint;
			boxRect.sizeDelta = difference;
			
			//Then we check our list of Selectables to see if they're being preselected or not.
			foreach(var selectable in selectables) {
				
				Vector3 screenPoint = GetScreenPointOfSelectable(selectable);
				
				//If the box Rect contains the selectabels screen point and that point is inside a valid selection mask, it's being preselected, otherwise it is not.
				selectable.preSelected = RectTransformUtility.RectangleContainsScreenPoint(boxRect, screenPoint, null) && PointIsValidAgainstSelectionMask(screenPoint);
				
			}
			
			//Finally, since it's possible for our first clicked object to not be within the bounds of the selection box
			//If it exists, we always ensure that it is preselected.
			if (clickedBeforeDrag != null) {
				clickedBeforeDrag.preSelected = true;
			}
		}
		
		void ApplySingleClickDeselection(){
			
			//If we didn't touch anything with the original mouse press, we don't need to continue checking
			if (clickedBeforeDrag == null)
				return;
			
			//If we clicked a selectable without dragging, and that selectable was previously selected, we must be trying to deselect it.
			if (clickedAfterDrag != null && clickedBeforeDrag.selected && clickedBeforeDrag.transform == clickedAfterDrag.transform ) {
				clickedBeforeDrag.selected = false;
				clickedBeforeDrag.preSelected = false;
				
			}
			
		}
		
		void ApplyPreSelections(){
			
			foreach(var selectable in selectables) {
				
				//If the selectable was preSelected, we finalize it as selected.
				if (selectable.preSelected) {
					selectable.selected = true;
					selectable.preSelected = false;
				}
			}
			
		}
		
		Vector2 GetScreenPointOfSelectable(IBoxSelectable selectable) {
			//Getting the screen point requires it's own function, because we have to take into consideration the selectables heirarchy.
			
			//Cast the transform as a rectTransform
			var rectTransform = selectable.transform as RectTransform;
			
			//If it has a rectTransform component, it must be in the heirarchy of a canvas, somewhere.
			if (rectTransform) {
				
				//And the camera used to calculate it's screen point will vary.
				Camera renderingCamera = GetScreenPointCamera(rectTransform);
				
				return RectTransformUtility.WorldToScreenPoint(renderingCamera, selectable.transform.position);
			}
			
			//If it's no in the heirarchy of a canvas, the regular Camera.main.WorldToScreenPoint will do.
			return Camera.main.WorldToScreenPoint(selectable.transform.position);				                                         
			
		}
		
		/*
	 * Finding the camera used to calculate the screenPoint of an object causes a couple of problems:
	 * 
	 * If it has a rectTransform, the root Canvas that the rectTransform is a descendant of will give unusable
	 * screen points depending on the Canvas.RenderMode, if we don't do any further calculation.
	 * 
	 * This function solves that problem. 
	 */
		Camera GetScreenPointCamera(RectTransform rectTransform) {
			
			Canvas rootCanvas = null;
			RectTransform rectCheck = rectTransform;
			
			//We're going to check all the canvases in the heirarchy of this rectTransform until we find the root.
			do {
				rootCanvas = rectCheck.GetComponent<Canvas>();
				
				//If we found a canvas on this Object, and it's not the rootCanvas, then we don't want to keep it
				if (rootCanvas && !rootCanvas.isRootCanvas) {
					rootCanvas = null;
				}
				
				//Then we promote the rect we're checking to it's parent.
				rectCheck = (RectTransform)rectCheck.parent;
				
			} while (rootCanvas == null);
			
			//Once we've found the root Canvas, we return a camera depending on it's render mode.
			switch (rootCanvas.renderMode) {
			case RenderMode.ScreenSpaceOverlay:
				//If we send back a camera when set to screen space overlay, the coordinates will not be accurate. If we return null, they will be.
				return null;
				
			case RenderMode.ScreenSpaceCamera:
				//If it's set to screen space we use the world Camera that the Canvas is using.
				//If it doesn't have one set, however, we have to send back the current camera. otherwise the coordinates will not be accurate.
				return (rootCanvas.worldCamera) ? rootCanvas.worldCamera : Camera.main;
				
			default:
			case RenderMode.WorldSpace:
				//World space always uses the current camera.
				return Camera.main;
			}
			
		}
		
		public IBoxSelectable[] GetAllSelected(){
			if (selectables == null) {
				return new IBoxSelectable[0];
			}
			
			var selectedList = new List<IBoxSelectable>();
			
			foreach(var selectable in selectables) {
				if (selectable.selected) {
					selectedList.Add (selectable);
				}
			}
			
			return selectedList.ToArray();
		}
		
		void EndSelection(){
			//Get out if we haven't finished selecting, or if the selection has been aborted (boxRect disabled)
			if (!Input.GetMouseButtonUp(0) || !boxRect.gameObject.activeSelf)
				return;
			
			clickedAfterDrag = GetSelectableAtMousePosition();
			
			ApplySingleClickDeselection();
			ApplyPreSelections();
			ResetBoxRect();
			onSelectionChange.Invoke(GetAllSelected());
		}
		
		void Start(){
			ValidateCanvas();
			CreateBoxRect();
			ResetBoxRect();
		}
		
		void Update() {
			BeginSelection ();
			DragSelection ();
			EndSelection ();
		}
	}
}
