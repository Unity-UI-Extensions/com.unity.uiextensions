using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace UnityEditor.UI
{
    /// <summary>
    /// This script adds the Extensions UI menu options to the Unity Editor.
    /// </summary>

    static internal class ExtensionMenuOptions
	{
		#region Unity Builder section  - Do not change unless UI Source (Editor\MenuOptions) changes
		#region Unity Builder properties  - Do not change unless UI Source (Editor\MenuOptions) changes
		private const string kUILayerName = "UI";
		private const float kWidth = 160f;
		private const float kThickHeight = 30f;
		private const float kThinHeight = 20f;
		private const string kStandardSpritePath = "UI/Skin/UISprite.psd";
		private const string kBackgroundSpriteResourcePath = "UI/Skin/Background.psd";
		private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
		private const string kKnobPath = "UI/Skin/Knob.psd";
		private const string kCheckmarkPath = "UI/Skin/Checkmark.psd";

		private static Vector2 s_ThickGUIElementSize = new Vector2(kWidth, kThickHeight);
		private static Vector2 s_ThinGUIElementSize = new Vector2(kWidth, kThinHeight);
		private static Vector2 s_ImageGUIElementSize = new Vector2(100f, 100f);
		private static Color s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
		private static Color s_TextColor = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);
		#endregion
		#region Unity Builder methods - Do not change unless UI Source (Editor\MenuOptions) changes
		private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
		{
			// Find the best scene view
			SceneView sceneView = SceneView.lastActiveSceneView;
			if (sceneView == null && SceneView.sceneViews.Count > 0)
				sceneView = SceneView.sceneViews[0] as SceneView;

			// Couldn't find a SceneView. Don't set position.
			if (sceneView == null || sceneView.camera == null)
				return;

			// Create world space Plane from canvas position.
			Vector2 localPlanePosition;
			Camera camera = sceneView.camera;
			Vector3 position = Vector3.zero;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
			{
				// Adjust for canvas pivot
				localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
				localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

				localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
				localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

				// Adjust for anchoring
				position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
				position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

				Vector3 minLocalPosition;
				minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
				minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

				Vector3 maxLocalPosition;
				maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
				maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

				position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
				position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
			}

			itemTransform.anchoredPosition = position;
			itemTransform.localRotation = Quaternion.identity;
			itemTransform.localScale = Vector3.one;
		}

		private static GameObject CreateUIElementRoot(string name, MenuCommand menuCommand, Vector2 size)
		{
			GameObject parent = menuCommand.context as GameObject;
			if (parent == null || parent.GetComponentInParent<Canvas>() == null)
			{
				parent = GetOrCreateCanvasGameObject();
			}
			GameObject child = new GameObject(name);

			Undo.RegisterCreatedObjectUndo(child, "Create " + name);
			Undo.SetTransformParent(child.transform, parent.transform, "Parent " + child.name);
			GameObjectUtility.SetParentAndAlign(child, parent);

			RectTransform rectTransform = child.AddComponent<RectTransform>();
			rectTransform.sizeDelta = size;
			if (parent != menuCommand.context) // not a context click, so center in sceneview
			{
				SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), rectTransform);
			}
			Selection.activeGameObject = child;
			return child;
		}

		static GameObject CreateUIObject(string name, GameObject parent)
		{
			GameObject go = new GameObject(name);
			go.AddComponent<RectTransform>();
			GameObjectUtility.SetParentAndAlign(go, parent);
			return go;
		}

		static public void AddCanvas(MenuCommand menuCommand)
		{
			var go = CreateNewUI();
			GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
			if (go.transform.parent as RectTransform)
			{
				RectTransform rect = go.transform as RectTransform;
				rect.anchorMin = Vector2.zero;
				rect.anchorMax = Vector2.one;
				rect.anchoredPosition = Vector2.zero;
				rect.sizeDelta = Vector2.zero;
			}
			Selection.activeGameObject = go;
		}

		static public GameObject CreateNewUI()
		{
			// Root for the UI
			var root = new GameObject("Canvas");
			root.layer = LayerMask.NameToLayer(kUILayerName);
			Canvas canvas = root.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			root.AddComponent<CanvasScaler>();
			root.AddComponent<GraphicRaycaster>();
			Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

			// if there is no event system add one...
			CreateEventSystem(false);
			return root;
		}

		public static void CreateEventSystem(MenuCommand menuCommand)
		{
			GameObject parent = menuCommand.context as GameObject;
			CreateEventSystem(true, parent);
		}

		private static void CreateEventSystem(bool select)
		{
			CreateEventSystem(select, null);
		}

		private static void CreateEventSystem(bool select, GameObject parent)
		{
			var esys = Object.FindObjectOfType<EventSystem>();
			if (esys == null)
			{
				var eventSystem = new GameObject("EventSystem");
				GameObjectUtility.SetParentAndAlign(eventSystem, parent);
				esys = eventSystem.AddComponent<EventSystem>();
				eventSystem.AddComponent<StandaloneInputModule>();

				Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
			}

			if (select && esys != null)
			{
				Selection.activeGameObject = esys.gameObject;
			}
		}

		// Helper function that returns a Canvas GameObject; preferably a parent of the selection, or other existing Canvas.
		static public GameObject GetOrCreateCanvasGameObject()
		{
			GameObject selectedGo = Selection.activeGameObject;

			// Try to find a gameobject that is the selected GO or one if its parents.
			Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
			if (canvas != null && canvas.gameObject.activeInHierarchy)
				return canvas.gameObject;

			// No canvas in selection or its parents? Then use just any canvas..
			canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
			if (canvas != null && canvas.gameObject.activeInHierarchy)
				return canvas.gameObject;

			// No canvas in the scene at all? Then create a new one.
			return ExtensionMenuOptions.CreateNewUI();
		}

		private static void SetDefaultColorTransitionValues(Selectable slider)
		{
			ColorBlock colors = slider.colors;
			colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
			colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
			colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
		}

		private static void SetDefaultTextValues(Text lbl)
		{
			// Set text values we want across UI elements in default controls.
			// Don't set values which are the same as the default values for the Text component,
			// since there's no point in that, and it's good to keep them as consistent as possible.
			lbl.color = s_TextColor;
		}
		#endregion
		#endregion

		#region UI Extensions "Create" Menu items

		#region Scroll Snap controls
		[MenuItem("GameObject/UI/Extensions/Horizontal Scroll Snap", false)]
		static public void AddHorizontalScrollSnap(MenuCommand menuCommand)
		{
			GameObject horizontalScrollSnapRoot = CreateUIElementRoot("Horizontal Scroll Snap", menuCommand, s_ThickGUIElementSize);

			GameObject childContent = CreateUIObject("Content", horizontalScrollSnapRoot);

			GameObject childPage01 = CreateUIObject("Page_01", childContent);

			GameObject childText = CreateUIObject("Text", childPage01);

			// Set RectTransform to stretch
			RectTransform rectTransformScrollSnapRoot = horizontalScrollSnapRoot.GetComponent<RectTransform>();
			rectTransformScrollSnapRoot.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchoredPosition = Vector2.zero;
			rectTransformScrollSnapRoot.sizeDelta = new Vector2(300f, 150f);


			Image image = horizontalScrollSnapRoot.AddComponent<Image>();
			image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpriteResourcePath);
			image.type = Image.Type.Sliced;
			image.color = new Color(1f, 1f, 1f, 0.392f);

			ScrollRect sr = horizontalScrollSnapRoot.AddComponent<ScrollRect>();
			sr.vertical = false;
			sr.horizontal = true;
			horizontalScrollSnapRoot.AddComponent<HorizontalScrollSnap>();

			//Setup Content container
			RectTransform rectTransformContent = childContent.GetComponent<RectTransform>();
			rectTransformContent.anchorMin = Vector2.zero;
			rectTransformContent.anchorMax = new Vector2(1f, 1f);
			rectTransformContent.sizeDelta = Vector2.zero;

			sr.content = rectTransformContent;

			//Setup 1st Child
			Image pageImage = childPage01.AddComponent<Image>();
			pageImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
			pageImage.type = Image.Type.Sliced;
			pageImage.color = s_DefaultSelectableColor;

			RectTransform rectTransformPage01 = childPage01.GetComponent<RectTransform>();
			rectTransformPage01.anchorMin = new Vector2(0f, 0.5f);
			rectTransformPage01.anchorMax = new Vector2(0f, 0.5f);
			rectTransformPage01.pivot = new Vector2(0f, 0.5f);

			//Setup Text on Page01
			Text text = childText.AddComponent<Text>();
			text.text = "Page_01";
			text.alignment = TextAnchor.MiddleCenter;
			text.color = new Color(0.196f, 0.196f, 0.196f);

			//Setup Text 1st Child
			RectTransform rectTransformPage01Text = childText.GetComponent<RectTransform>();
			rectTransformPage01Text.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransformPage01Text.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransformPage01Text.pivot = new Vector2(0.5f, 0.5f);


			//Need to add example child components like in the Asset (SJ)
			Selection.activeGameObject = horizontalScrollSnapRoot;
		}

		[MenuItem("GameObject/UI/Extensions/Vertical Scroll Snap", false)]
		static public void AddVerticallScrollSnap(MenuCommand menuCommand)
		{
			GameObject verticalScrollSnapRoot = CreateUIElementRoot("Vertical Scroll Snap", menuCommand, s_ThickGUIElementSize);

			GameObject childContent = CreateUIObject("Content", verticalScrollSnapRoot);

			GameObject childPage01 = CreateUIObject("Page_01", childContent);

			GameObject childText = CreateUIObject("Text", childPage01);

			// Set RectTransform to stretch
			RectTransform rectTransformScrollSnapRoot = verticalScrollSnapRoot.GetComponent<RectTransform>();
			rectTransformScrollSnapRoot.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchoredPosition = Vector2.zero;
			rectTransformScrollSnapRoot.sizeDelta = new Vector2(300f, 150f);


			Image image = verticalScrollSnapRoot.AddComponent<Image>();
			image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpriteResourcePath);
			image.type = Image.Type.Sliced;
			image.color = new Color(1f, 1f, 1f, 0.392f);

			ScrollRect sr = verticalScrollSnapRoot.AddComponent<ScrollRect>();
			sr.vertical = true;
			sr.horizontal = false;
			verticalScrollSnapRoot.AddComponent<VerticalScrollSnap>();

			//Setup Content container
			RectTransform rectTransformContent = childContent.GetComponent<RectTransform>();
			rectTransformContent.anchorMin = Vector2.zero;
			rectTransformContent.anchorMax = new Vector2(1f, 1f);
			//rectTransformContent.anchoredPosition = Vector2.zero;
			rectTransformContent.sizeDelta = Vector2.zero;

			sr.content = rectTransformContent;

			//Setup 1st Child
			Image pageImage = childPage01.AddComponent<Image>();
			pageImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
			pageImage.type = Image.Type.Sliced;
			pageImage.color = s_DefaultSelectableColor;

			RectTransform rectTransformPage01 = childPage01.GetComponent<RectTransform>();
			rectTransformPage01.anchorMin = new Vector2(0.5f, 0f);
			rectTransformPage01.anchorMax = new Vector2(0.5f, 0f);
			rectTransformPage01.anchoredPosition = new Vector2(-rectTransformPage01.sizeDelta.x / 2, rectTransformPage01.sizeDelta.y / 2);
			//rectTransformPage01.anchoredPosition = Vector2.zero;
			//rectTransformPage01.sizeDelta = Vector2.zero;
			rectTransformPage01.pivot = new Vector2(0.5f, 0f);

			//Setup Text on Page01
			Text text = childText.AddComponent<Text>();
			text.text = "Page_01";
			text.alignment = TextAnchor.MiddleCenter;
			text.color = new Color(0.196f, 0.196f, 0.196f);

			//Setup Text 1st Child
			RectTransform rectTransformPage01Text = childText.GetComponent<RectTransform>();
			rectTransformPage01Text.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransformPage01Text.anchorMax = new Vector2(0.5f, 0.5f);
			//rectTransformPage01Text.anchoredPosition = Vector2.zero;
			//rectTransformPage01Text.sizeDelta = Vector2.zero;
			rectTransformPage01Text.pivot = new Vector2(0.5f, 0.5f);


			//Need to add example child components like in the Asset (SJ)

			Selection.activeGameObject = verticalScrollSnapRoot;
		}

		#region New ScrollSnapCode
		static public void FixedScrollSnapBase(MenuCommand menuCommand, string name, ScrollSnap.ScrollDirection direction, int itemVisible, int itemCount, Vector2 itemSize)
		{
			GameObject scrollSnapRoot = CreateUIElementRoot(name, menuCommand, s_ThickGUIElementSize);
			GameObject itemList = CreateUIObject("List", scrollSnapRoot);

			// Set RectTransform to stretch
			RectTransform rectTransformScrollSnapRoot = scrollSnapRoot.GetComponent<RectTransform>();
			rectTransformScrollSnapRoot.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchoredPosition = Vector2.zero;

			if (direction == ScrollSnap.ScrollDirection.Horizontal)
			{
				rectTransformScrollSnapRoot.sizeDelta = new Vector2(itemVisible * itemSize.x, itemSize.y);
			}
			else
			{
				rectTransformScrollSnapRoot.sizeDelta = new Vector2(itemSize.x, itemVisible * itemSize.y);
			}

			Image image = scrollSnapRoot.AddComponent<Image>();
			image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpriteResourcePath);
			image.type = Image.Type.Sliced;
			image.color = new Color(1f, 1f, 1f, 1f);

			Mask listMask = scrollSnapRoot.AddComponent<Mask>();
			listMask.showMaskGraphic = false;

			ScrollRect scrollRect = scrollSnapRoot.AddComponent<ScrollRect>();
			scrollRect.vertical = direction == ScrollSnap.ScrollDirection.Vertical;
			scrollRect.horizontal = direction == ScrollSnap.ScrollDirection.Horizontal;

			ScrollSnap scrollSnap = scrollSnapRoot.AddComponent<ScrollSnap>();
			scrollSnap.direction = direction;
			scrollSnap.ItemsVisibleAtOnce = itemVisible;

			//Setup Content container
			RectTransform rectTransformContent = itemList.GetComponent<RectTransform>();
			rectTransformContent.anchorMin = Vector2.zero;
			rectTransformContent.anchorMax = new Vector2(1f, 1f);
			rectTransformContent.sizeDelta = Vector2.zero;
			scrollRect.content = rectTransformContent;

			//Setup Item list container
			if (direction == ScrollSnap.ScrollDirection.Horizontal)
			{
				itemList.AddComponent<HorizontalLayoutGroup>();
				ContentSizeFitter sizeFitter = itemList.AddComponent<ContentSizeFitter>();
				sizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;
			}
			else
			{
				itemList.AddComponent<VerticalLayoutGroup>();
				ContentSizeFitter sizeFitter = itemList.AddComponent<ContentSizeFitter>();
				sizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
			}

			//Setup children
			for (var i = 0; i < itemCount; i++)
			{
				GameObject item = CreateUIObject(string.Format("Item_{0:00}", i), itemList);
				GameObject childText = CreateUIObject("Text", item);

				Image pageImage = item.AddComponent<Image>();
				pageImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
				pageImage.type = Image.Type.Sliced;
				pageImage.color = s_DefaultSelectableColor;

				LayoutElement elementLayout = item.AddComponent<LayoutElement>();
				if (direction == ScrollSnap.ScrollDirection.Horizontal)
				{
					elementLayout.minWidth = itemSize.x;
				}
				else
				{
					elementLayout.minHeight = itemSize.y;
				}

				RectTransform rectTransformPage01 = item.GetComponent<RectTransform>();
				rectTransformPage01.anchorMin = new Vector2(0f, 0.5f);
				rectTransformPage01.anchorMax = new Vector2(0f, 0.5f);
				rectTransformPage01.pivot = new Vector2(0f, 0.5f);

				//Setup Text on Page01
				Text text = childText.AddComponent<Text>();
				text.text = item.name;
				text.alignment = TextAnchor.MiddleCenter;
				text.color = new Color(0.196f, 0.196f, 0.196f);

				//Setup Text 1st Child
				RectTransform rectTransformPage01Text = childText.GetComponent<RectTransform>();
				rectTransformPage01Text.anchorMin = new Vector2(0.5f, 0.5f);
				rectTransformPage01Text.anchorMax = new Vector2(0.5f, 0.5f);
				rectTransformPage01Text.pivot = new Vector2(0.5f, 0.5f);
			}
			Selection.activeGameObject = scrollSnapRoot;
		}

		[MenuItem("GameObject/UI/Extensions/Fixed Item Scroll/Snap Horizontal Single Item", false)]
		static public void AddFixedItemScrollSnapHorizontalSingle(MenuCommand menuCommand)
		{
			FixedScrollSnapBase(menuCommand, "Scroll Snap Horizontal Single", ScrollSnap.ScrollDirection.Horizontal, 1, 3, new Vector2(100, 100));
		}

		[MenuItem("GameObject/UI/Extensions/Fixed Item Scroll/Snap Horizontal Multiple Items", false)]
		static public void AddFixedItemScrollSnapHorizontalMultiple(MenuCommand menuCommand)
		{
			FixedScrollSnapBase(menuCommand, "Scroll Snap Horizontal Multiple", ScrollSnap.ScrollDirection.Horizontal, 3, 15, new Vector2(100, 100));
		}

		[MenuItem("GameObject/UI/Extensions/Fixed Item Scroll/Snap Vertical Single Item", false)]
		static public void AddFixedItemScrollSnapVerticalSingle(MenuCommand menuCommand)
		{
			FixedScrollSnapBase(menuCommand, "Scroll Snap Vertical Multiple", ScrollSnap.ScrollDirection.Vertical, 1, 3, new Vector2(100, 100));
		}

		[MenuItem("GameObject/UI/Extensions/Fixed Item Scroll/Snap Vertical Multiple Items", false)]
		static public void AddFixedItemScrollSnapVerticalMultiple(MenuCommand menuCommand)
		{
			FixedScrollSnapBase(menuCommand, "Scroll Snap Vertical Multiple", ScrollSnap.ScrollDirection.Vertical, 3, 15, new Vector2(100, 100));
		}
		#endregion

		#endregion

		#region UIVertical Scroller
		[MenuItem("GameObject/UI/Extensions/UI Vertical Scroller", false)]
		static public void AddUIVerticallScroller(MenuCommand menuCommand)
		{
			GameObject uiVerticalScrollerRoot = CreateUIElementRoot("UI Vertical Scroller", menuCommand, s_ThickGUIElementSize);

			GameObject uiScrollerCenter = CreateUIObject("Center", uiVerticalScrollerRoot);

			GameObject childContent = CreateUIObject("Content", uiVerticalScrollerRoot);

			// Set RectTransform to stretch
			RectTransform rectTransformScrollSnapRoot = uiVerticalScrollerRoot.GetComponent<RectTransform>();
			rectTransformScrollSnapRoot.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchoredPosition = Vector2.zero;
			rectTransformScrollSnapRoot.sizeDelta = new Vector2(500f, 150f);

			// Add required ScrollRect
			ScrollRect sr = uiVerticalScrollerRoot.AddComponent<ScrollRect>();
			sr.vertical = true;
			sr.horizontal = false;
			sr.movementType = ScrollRect.MovementType.Unrestricted;
			var uiscr = uiVerticalScrollerRoot.AddComponent<UIVerticalScroller>();

			//Setup container center point
			RectTransform rectTransformCenter = uiScrollerCenter.GetComponent<RectTransform>();
			rectTransformCenter.anchorMin = new Vector2(0f, 0.3f);
			rectTransformCenter.anchorMax = new Vector2(1f, 0.6f);
			rectTransformCenter.sizeDelta = Vector2.zero;

			uiscr._center = uiScrollerCenter.GetComponent<RectTransform>();

			//Setup Content container
			RectTransform rectTransformContent = childContent.GetComponent<RectTransform>();
			rectTransformContent.anchorMin = Vector2.zero;
			rectTransformContent.anchorMax = new Vector2(1f, 1f);
			rectTransformContent.sizeDelta = Vector2.zero;

			sr.content = rectTransformContent;

			// Add sample children
			for (int i = 0; i < 10; i++)
			{
				GameObject childPage = CreateUIObject("Page_" + i, childContent);

				GameObject childText = CreateUIObject("Text", childPage);

				//Setup 1st Child
				Image pageImage = childPage.AddComponent<Image>();
				pageImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
				pageImage.type = Image.Type.Sliced;
				pageImage.color = s_DefaultSelectableColor;

				RectTransform rectTransformPage = childPage.GetComponent<RectTransform>();
				rectTransformPage.anchorMin = new Vector2(0f, 0.5f);
				rectTransformPage.anchorMax = new Vector2(1f, 0.5f);
				rectTransformPage.sizeDelta = new Vector2(0f, 80f);
				rectTransformPage.pivot = new Vector2(0.5f, 0.5f);
				rectTransformPage.localPosition = new Vector3(0, 80 * i, 0);
				childPage.AddComponent<Button>();

				var childCG = childPage.AddComponent<CanvasGroup>();
				childCG.interactable = false;

				//Setup Text on Item
				Text text = childText.AddComponent<Text>();
				text.text = "Item_" + i;
				text.alignment = TextAnchor.MiddleCenter;
				text.color = new Color(0.196f, 0.196f, 0.196f);

				//Setup Text on Item
				RectTransform rectTransformPageText = childText.GetComponent<RectTransform>();
				rectTransformPageText.anchorMin = new Vector2(0.5f, 0.5f);
				rectTransformPageText.anchorMax = new Vector2(0.5f, 0.5f);
				rectTransformPageText.pivot = new Vector2(0.5f, 0.5f);
			}

			Selection.activeGameObject = uiVerticalScrollerRoot;
		}
        #endregion

        #region UI Button
        [MenuItem("GameObject/UI/Extensions/UI Button", false)]
		static public void AddUIButton(MenuCommand menuCommand)
		{
			GameObject uiButtonRoot = CreateUIElementRoot("UI Button", menuCommand, s_ThickGUIElementSize);
			GameObject childText = CreateUIObject("Text", uiButtonRoot);

			Image image = uiButtonRoot.AddComponent<Image>();
			image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
			image.type = Image.Type.Sliced;
			image.color = s_DefaultSelectableColor;

			Button bt = uiButtonRoot.AddComponent<Button>();
			uiButtonRoot.AddComponent<UISelectableExtension>();
			SetDefaultColorTransitionValues(bt);

			Text text = childText.AddComponent<Text>();
			text.text = "Button";
			text.alignment = TextAnchor.MiddleCenter;
			text.color = new Color(0.196f, 0.196f, 0.196f);

			RectTransform textRectTransform = childText.GetComponent<RectTransform>();
			textRectTransform.anchorMin = Vector2.zero;
			textRectTransform.anchorMax = Vector2.one;
			textRectTransform.sizeDelta = Vector2.zero;

			Selection.activeGameObject = uiButtonRoot;
		}
        #endregion

        #region UI Flippable
        [MenuItem("GameObject/UI/Extensions/UI Flippable", false)]
		static public void AddUIFlippableImage(MenuCommand menuCommand)
		{
			GameObject go = CreateUIElementRoot("UI Flippable", menuCommand, s_ImageGUIElementSize);
			go.AddComponent<Image>();
			go.AddComponent<UIFlippable>();
			Selection.activeGameObject = go;
		}
        #endregion

        #region UI WindowBase
        [MenuItem("GameObject/UI/Extensions/UI Window Base", false)]
		static public void AddUIWindowBase(MenuCommand menuCommand)
		{
			GameObject go = CreateUIElementRoot("UI Window Base", menuCommand, s_ThickGUIElementSize);
			go.AddComponent<UIWindowBase>();
			go.AddComponent<Image>();
			Selection.activeGameObject = go;
		}
        #endregion

        #region Accordian
        [MenuItem("GameObject/UI/Extensions/Accordion/Accordion Group", false)]
		static public void AddAccordionGroup(MenuCommand menuCommand)
		{
			GameObject go = CreateUIElementRoot("Accordion Group", menuCommand, s_ThickGUIElementSize);
			go.AddComponent<VerticalLayoutGroup>();
			go.AddComponent<ContentSizeFitter>();
			go.AddComponent<ToggleGroup>();
			go.AddComponent<Accordion>();
			Selection.activeGameObject = go;
		}

		[MenuItem("GameObject/UI/Extensions/Accordion/Accordion Element", false)]
		static public void AddAccordionElement(MenuCommand menuCommand)
		{
			GameObject go = CreateUIElementRoot("Accordion Element", menuCommand, s_ThickGUIElementSize);
			go.AddComponent<LayoutElement>();
			go.AddComponent<AccordionElement>();
			Selection.activeGameObject = go;

		}
		#endregion

		#region Drop Down controls
		[MenuItem("GameObject/UI/Extensions/AutoComplete ComboBox", false)]
		static public void AddAutoCompleteComboBox(MenuCommand menuCommand)
		{
			GameObject autoCompleteComboBoxRoot = CreateUIElementRoot("AutoCompleteComboBox", menuCommand, s_ThickGUIElementSize);

			//Create Template
			GameObject itemTemplate = AddButtonAsChild(autoCompleteComboBoxRoot);

			//Create Inputfield
			GameObject inputField = AddInputFieldAsChild(autoCompleteComboBoxRoot);

			//Create Overlay
			GameObject overlay = CreateUIObject("Overlay", autoCompleteComboBoxRoot);
			GameObject overlayScrollPanel = CreateUIObject("ScrollPanel", overlay);
			GameObject overlayScrollPanelItems = CreateUIObject("Items", overlayScrollPanel);
			GameObject overlayScrollPanelScrollBar = AddScrollbarAsChild(overlayScrollPanel);

			//Create Arrow Button
			GameObject arrowButton = AddButtonAsChild(autoCompleteComboBoxRoot);

			//Setup ComboBox
			var autoCompleteComboBox = autoCompleteComboBoxRoot.AddComponent<AutoCompleteComboBox>();
			var cbbRT = autoCompleteComboBoxRoot.GetComponent<RectTransform>();

			//Setup Template
			itemTemplate.name = "ItemTemplate";
			var itemTemplateRT = itemTemplate.GetComponent<RectTransform>();
			itemTemplateRT.sizeDelta = cbbRT.sizeDelta;
			var itemTemplateButton = itemTemplate.GetComponent<Button>();
			itemTemplateButton.transition = Selectable.Transition.None;
			var itemTemplateLayoutElement = itemTemplate.AddComponent<LayoutElement>();
			itemTemplateLayoutElement.minHeight = cbbRT.rect.height;
			itemTemplate.SetActive(false);

			//Setup InputField
			var inputFieldRT = inputField.GetComponent<RectTransform>();
			inputFieldRT.anchorMin = Vector2.zero;
			inputFieldRT.anchorMax = Vector2.one;
			inputFieldRT.sizeDelta = Vector2.zero;
			Events.UnityEventTools.AddPersistentListener<string>(inputField.GetComponent<InputField>().onValueChanged, new UnityEngine.Events.UnityAction<string>(autoCompleteComboBox.OnValueChanged));

			//Setup Overlay
			var overlayRT = overlay.GetComponent<RectTransform>();
			overlayRT.anchorMin = new Vector2(0f, 1f);
			overlayRT.anchorMax = new Vector2(0f, 1f);
			overlayRT.sizeDelta = new Vector2(0f, 1f);
			overlayRT.pivot = new Vector2(0f, 1f);
			overlay.AddComponent<Image>().color = new Color(0.243f, 0.871f, 0f, 0f);
			Events.UnityEventTools.AddBoolPersistentListener(overlay.AddComponent<Button>().onClick, new UnityEngine.Events.UnityAction<bool>(autoCompleteComboBox.ToggleDropdownPanel), true);
			//Overlay Scroll Panel
			var overlayScrollPanelRT = overlayScrollPanel.GetComponent<RectTransform>();
			overlayScrollPanelRT.position += new Vector3(0, -cbbRT.sizeDelta.y, 0);
			overlayScrollPanelRT.anchorMin = new Vector2(0f, 1f);
			overlayScrollPanelRT.anchorMax = new Vector2(0f, 1f);
			overlayScrollPanelRT.sizeDelta = new Vector2(cbbRT.sizeDelta.x, cbbRT.sizeDelta.y * 3);
			overlayScrollPanelRT.pivot = new Vector2(0f, 1f);
			overlayScrollPanel.AddComponent<Image>();
			overlayScrollPanel.AddComponent<Mask>();
			var scrollRect = overlayScrollPanel.AddComponent<ScrollRect>();
			scrollRect.horizontal = false;
			scrollRect.movementType = ScrollRect.MovementType.Clamped;
			scrollRect.verticalScrollbar = overlayScrollPanelScrollBar.GetComponent<Scrollbar>();
			//Overlay Items list
			var overlayScrollPanelItemsRT = overlayScrollPanelItems.GetComponent<RectTransform>();
			overlayScrollPanelItemsRT.position += new Vector3(5, 0, 0);
			overlayScrollPanelItemsRT.anchorMin = new Vector2(0f, 1f);
			overlayScrollPanelItemsRT.anchorMax = new Vector2(0f, 1f);
			overlayScrollPanelItemsRT.sizeDelta = new Vector2(120f, 5f);
			overlayScrollPanelItemsRT.pivot = new Vector2(0f, 1f);
			scrollRect.content = overlayScrollPanelItemsRT;
			var overlayScrollPanelItemsVLG = overlayScrollPanelItems.AddComponent<VerticalLayoutGroup>();
			overlayScrollPanelItemsVLG.padding = new RectOffset(0, 0, 5, 0);
			var overlayScrollPanelItemsFitter = overlayScrollPanelItems.AddComponent<ContentSizeFitter>();
			overlayScrollPanelItemsFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
			//Overlay Scrollbar
			var overlayScrollPanelScrollbarRT = overlayScrollPanelScrollBar.GetComponent<RectTransform>();
			overlayScrollPanelScrollbarRT.anchorMin = new Vector2(1f, 0f);
			overlayScrollPanelScrollbarRT.anchorMax = Vector2.one;
			overlayScrollPanelScrollbarRT.sizeDelta = new Vector2(cbbRT.sizeDelta.y, 0f);
			overlayScrollPanelScrollbarRT.pivot = Vector2.one;
			overlayScrollPanelScrollbarRT.GetComponent<Scrollbar>().direction = Scrollbar.Direction.BottomToTop;
			overlayScrollPanelScrollBar.transform.GetChild(0).name = "SlidingArea";

			//Arrow Button
			arrowButton.name = "ArrowBtn";
			var arrowButtonRT = arrowButton.GetComponent<RectTransform>();
			arrowButtonRT.anchorMin = Vector2.one;
			arrowButtonRT.anchorMax = Vector2.one;
			arrowButtonRT.sizeDelta = new Vector2(cbbRT.sizeDelta.y, cbbRT.sizeDelta.y);
			arrowButtonRT.pivot = Vector2.one;
			Events.UnityEventTools.AddBoolPersistentListener(arrowButton.GetComponent<Button>().onClick, new UnityEngine.Events.UnityAction<bool>(autoCompleteComboBox.ToggleDropdownPanel), true);
			arrowButton.GetComponentInChildren<Text>().text = "▼";

			Selection.activeGameObject = autoCompleteComboBoxRoot;
		}

		[MenuItem("GameObject/UI/Extensions/ComboBox", false)]
		static public void AddComboBox(MenuCommand menuCommand)
		{
			GameObject comboBoxRoot = CreateUIElementRoot("ComboBox", menuCommand, s_ThickGUIElementSize);

			//Create Template
			GameObject itemTemplate = AddButtonAsChild(comboBoxRoot);

			//Create Inputfield
			GameObject inputField = AddInputFieldAsChild(comboBoxRoot);

			//Create Overlay
			GameObject overlay = CreateUIObject("Overlay", comboBoxRoot);
			GameObject overlayScrollPanel = CreateUIObject("ScrollPanel", overlay);
			GameObject overlayScrollPanelItems = CreateUIObject("Items", overlayScrollPanel);
			GameObject overlayScrollPanelScrollBar = AddScrollbarAsChild(overlayScrollPanel);

			//Create Arrow Button
			GameObject arrowButton = AddButtonAsChild(comboBoxRoot);

			//Setup ComboBox
			var comboBox = comboBoxRoot.AddComponent<ComboBox>();
			var cbbRT = comboBoxRoot.GetComponent<RectTransform>();

			//Setup Template
			itemTemplate.name = "ItemTemplate";
			var itemTemplateRT = itemTemplate.GetComponent<RectTransform>();
			itemTemplateRT.sizeDelta = cbbRT.sizeDelta;
			var itemTemplateButton = itemTemplate.GetComponent<Button>();
			itemTemplateButton.transition = Selectable.Transition.None;
			var itemTemplateLayoutElement = itemTemplate.AddComponent<LayoutElement>();
			itemTemplateLayoutElement.minHeight = cbbRT.rect.height;
			itemTemplate.SetActive(false);

			//Setup InputField
			var inputFieldRT = inputField.GetComponent<RectTransform>();
			inputFieldRT.anchorMin = Vector2.zero;
			inputFieldRT.anchorMax = Vector2.one;
			inputFieldRT.sizeDelta = Vector2.zero;
			Events.UnityEventTools.AddPersistentListener<string>(inputField.GetComponent<InputField>().onValueChanged, new UnityEngine.Events.UnityAction<string>(comboBox.OnValueChanged));

			//Setup Overlay
			var overlayRT = overlay.GetComponent<RectTransform>();
			overlayRT.anchorMin = new Vector2(0f, 1f);
			overlayRT.anchorMax = new Vector2(0f, 1f);
			overlayRT.sizeDelta = new Vector2(0f, 1f);
			overlayRT.pivot = new Vector2(0f, 1f);
			overlay.AddComponent<Image>().color = new Color(0.243f, 0.871f, 0f, 0f);
			Events.UnityEventTools.AddBoolPersistentListener(overlay.AddComponent<Button>().onClick, new UnityEngine.Events.UnityAction<bool>(comboBox.ToggleDropdownPanel), true);
			//Overlay Scroll Panel
			var overlayScrollPanelRT = overlayScrollPanel.GetComponent<RectTransform>();
			overlayScrollPanelRT.position += new Vector3(0, -cbbRT.sizeDelta.y, 0);
			overlayScrollPanelRT.anchorMin = new Vector2(0f, 1f);
			overlayScrollPanelRT.anchorMax = new Vector2(0f, 1f);
			overlayScrollPanelRT.sizeDelta = new Vector2(cbbRT.sizeDelta.x, cbbRT.sizeDelta.y * 3);
			overlayScrollPanelRT.pivot = new Vector2(0f, 1f);
			overlayScrollPanel.AddComponent<Image>();
			overlayScrollPanel.AddComponent<Mask>();
			var scrollRect = overlayScrollPanel.AddComponent<ScrollRect>();
			scrollRect.horizontal = false;
			scrollRect.movementType = ScrollRect.MovementType.Clamped;
			scrollRect.verticalScrollbar = overlayScrollPanelScrollBar.GetComponent<Scrollbar>();
			//Overlay Items list
			var overlayScrollPanelItemsRT = overlayScrollPanelItems.GetComponent<RectTransform>();
			overlayScrollPanelItemsRT.position += new Vector3(5, 0, 0);
			overlayScrollPanelItemsRT.anchorMin = new Vector2(0f, 1f);
			overlayScrollPanelItemsRT.anchorMax = new Vector2(0f, 1f);
			overlayScrollPanelItemsRT.sizeDelta = new Vector2(120f, 5f);
			overlayScrollPanelItemsRT.pivot = new Vector2(0f, 1f);
			scrollRect.content = overlayScrollPanelItemsRT;
			var overlayScrollPanelItemsVLG = overlayScrollPanelItems.AddComponent<VerticalLayoutGroup>();
			overlayScrollPanelItemsVLG.padding = new RectOffset(0, 0, 5, 0);
			var overlayScrollPanelItemsFitter = overlayScrollPanelItems.AddComponent<ContentSizeFitter>();
			overlayScrollPanelItemsFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
			//Overlay Scrollbar
			var overlayScrollPanelScrollbarRT = overlayScrollPanelScrollBar.GetComponent<RectTransform>();
			overlayScrollPanelScrollbarRT.anchorMin = new Vector2(1f, 0f);
			overlayScrollPanelScrollbarRT.anchorMax = Vector2.one;
			overlayScrollPanelScrollbarRT.sizeDelta = new Vector2(cbbRT.sizeDelta.y, 0f);
			overlayScrollPanelScrollbarRT.pivot = Vector2.one;
			overlayScrollPanelScrollbarRT.GetComponent<Scrollbar>().direction = Scrollbar.Direction.BottomToTop;
			overlayScrollPanelScrollBar.transform.GetChild(0).name = "SlidingArea";

			//Arrow Button
			arrowButton.name = "ArrowBtn";
			var arrowButtonRT = arrowButton.GetComponent<RectTransform>();
			arrowButtonRT.anchorMin = Vector2.one;
			arrowButtonRT.anchorMax = Vector2.one;
			arrowButtonRT.sizeDelta = new Vector2(cbbRT.sizeDelta.y, cbbRT.sizeDelta.y);
			arrowButtonRT.pivot = Vector2.one;
			Events.UnityEventTools.AddBoolPersistentListener(arrowButton.GetComponent<Button>().onClick, new UnityEngine.Events.UnityAction<bool>(comboBox.ToggleDropdownPanel), true);
			arrowButton.GetComponentInChildren<Text>().text = "▼";

			Selection.activeGameObject = comboBoxRoot;
		}

		[MenuItem("GameObject/UI/Extensions/DropDownList", false)]
		static public void AddDropDownList(MenuCommand menuCommand)
		{
			GameObject dropDownListRoot = CreateUIElementRoot("DropDownList", menuCommand, s_ThickGUIElementSize);

			//Create Template
			GameObject itemTemplate = AddButtonAsChild(dropDownListRoot);
			GameObject itemTemplateImage = AddImageAsChild(itemTemplate);
			itemTemplateImage.GetComponent<Transform>().SetSiblingIndex(0);

			//Create Main Button
			GameObject mainButton = AddButtonAsChild(dropDownListRoot);
			GameObject mainButtonImage = AddImageAsChild(mainButton);
			mainButtonImage.GetComponent<Transform>().SetSiblingIndex(0);

			//Create Overlay
			GameObject overlay = CreateUIObject("Overlay", dropDownListRoot);
			GameObject overlayScrollPanel = CreateUIObject("ScrollPanel", overlay);
			GameObject overlayScrollPanelItems = CreateUIObject("Items", overlayScrollPanel);
			GameObject overlayScrollPanelScrollBar = AddScrollbarAsChild(overlayScrollPanel);

			//Create Arrow Button
			GameObject arrowText = AddTextAsChild(dropDownListRoot);

			//Setup DropDownList
			var dropDownList = dropDownListRoot.AddComponent<DropDownList>();
			var cbbRT = dropDownListRoot.GetComponent<RectTransform>();

			//Setup Template
			itemTemplate.name = "ItemTemplate";
			var itemTemplateRT = itemTemplate.GetComponent<RectTransform>();
			itemTemplateRT.sizeDelta = cbbRT.sizeDelta;
			var itemTemplateButton = itemTemplate.GetComponent<Button>();
			itemTemplateButton.transition = Selectable.Transition.None;
			var itemTemplateLayoutElement = itemTemplate.AddComponent<LayoutElement>();
			itemTemplateLayoutElement.minHeight = cbbRT.rect.height;
			itemTemplate.SetActive(false);
			//Item Template Image
			var itemTemplateImageRT = itemTemplateImage.GetComponent<RectTransform>();
			itemTemplateImageRT.anchorMin = Vector2.zero;
			itemTemplateImageRT.anchorMax = new Vector2(0f, 1f);
			itemTemplateImageRT.pivot = new Vector2(0f, 1f);
			itemTemplateImageRT.sizeDelta = Vector2.one;
			itemTemplateImageRT.offsetMin = new Vector2(4f, 4f);
			itemTemplateImageRT.offsetMax = new Vector2(22f, -4f);
			itemTemplateImage.GetComponent<Image>().color = new Color(0, 0, 0, 0);

			//Setup Main Button
			mainButton.name = "MainButton";
			var mainButtonRT = mainButton.GetComponent<RectTransform>();
			mainButtonRT.anchorMin = Vector2.zero;
			mainButtonRT.anchorMax = Vector2.one;
			mainButtonRT.sizeDelta = Vector2.zero;
			Events.UnityEventTools.AddBoolPersistentListener(mainButton.GetComponent<Button>().onClick, new UnityEngine.Events.UnityAction<bool>(dropDownList.ToggleDropdownPanel), true);
			var mainButtonText = mainButton.GetComponentInChildren<Text>();
			mainButtonText.alignment = TextAnchor.MiddleLeft;
			mainButtonText.text = "Select Item...";
			var mainButtonTextRT = mainButtonText.GetComponent<RectTransform>();
			mainButtonTextRT.anchorMin = Vector2.zero;
			mainButtonTextRT.anchorMin = Vector2.zero;
			mainButtonTextRT.pivot = new Vector2(0f, 1f);
			mainButtonTextRT.offsetMin = new Vector2(10f, 0f);
			mainButtonTextRT.offsetMax = new Vector2(-4f, 0f);
			//Main Button Image
			var mainButtonImageRT = mainButtonImage.GetComponent<RectTransform>();
			mainButtonImageRT.anchorMin = Vector2.zero;
			mainButtonImageRT.anchorMax = new Vector2(0f, 1f);
			mainButtonImageRT.pivot = new Vector2(0f, 1f);
			mainButtonImageRT.sizeDelta = Vector2.one;
			mainButtonImageRT.offsetMin = new Vector2(4f, 4f);
			mainButtonImageRT.offsetMax = new Vector2(22f, -4f);
			mainButtonImageRT.GetComponent<Image>().color = new Color(1, 1, 1, 0);


			//Setup Overlay
			var overlayRT = overlay.GetComponent<RectTransform>();
			overlayRT.anchorMin = new Vector2(0f, 1f);
			overlayRT.anchorMax = new Vector2(0f, 1f);
			overlayRT.sizeDelta = new Vector2(0f, 1f);
			overlayRT.pivot = new Vector2(0f, 1f);
			overlay.AddComponent<Image>().color = new Color(0.243f, 0.871f, 0f, 0f);
			Events.UnityEventTools.AddBoolPersistentListener(overlay.AddComponent<Button>().onClick, new UnityEngine.Events.UnityAction<bool>(dropDownList.ToggleDropdownPanel), true);
			//Overlay Scroll Panel
			var overlayScrollPanelRT = overlayScrollPanel.GetComponent<RectTransform>();
			overlayScrollPanelRT.position += new Vector3(0, -cbbRT.sizeDelta.y, 0);
			overlayScrollPanelRT.anchorMin = new Vector2(0f, 1f);
			overlayScrollPanelRT.anchorMax = new Vector2(0f, 1f);
			overlayScrollPanelRT.sizeDelta = new Vector2(cbbRT.sizeDelta.x, cbbRT.sizeDelta.y * 3);
			overlayScrollPanelRT.pivot = new Vector2(0f, 1f);
			overlayScrollPanel.AddComponent<Image>();
			overlayScrollPanel.AddComponent<Mask>();
			var scrollRect = overlayScrollPanel.AddComponent<ScrollRect>();
			scrollRect.horizontal = false;
			scrollRect.movementType = ScrollRect.MovementType.Clamped;
			scrollRect.verticalScrollbar = overlayScrollPanelScrollBar.GetComponent<Scrollbar>();
			//Overlay Items list
			var overlayScrollPanelItemsRT = overlayScrollPanelItems.GetComponent<RectTransform>();
			overlayScrollPanelItemsRT.position += new Vector3(5, 0, 0);
			overlayScrollPanelItemsRT.anchorMin = new Vector2(0f, 1f);
			overlayScrollPanelItemsRT.anchorMax = new Vector2(0f, 1f);
			overlayScrollPanelItemsRT.sizeDelta = new Vector2(120f, 5f);
			overlayScrollPanelItemsRT.pivot = new Vector2(0f, 1f);
			scrollRect.content = overlayScrollPanelItemsRT;
			var overlayScrollPanelItemsVLG = overlayScrollPanelItems.AddComponent<VerticalLayoutGroup>();
			overlayScrollPanelItemsVLG.padding = new RectOffset(0, 0, 5, 0);
			var overlayScrollPanelItemsFitter = overlayScrollPanelItems.AddComponent<ContentSizeFitter>();
			overlayScrollPanelItemsFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
			//Overlay Scrollbar
			var overlayScrollPanelScrollbarRT = overlayScrollPanelScrollBar.GetComponent<RectTransform>();
			overlayScrollPanelScrollbarRT.anchorMin = new Vector2(1f, 0f);
			overlayScrollPanelScrollbarRT.anchorMax = Vector2.one;
			overlayScrollPanelScrollbarRT.sizeDelta = new Vector2(cbbRT.sizeDelta.y, 0f);
			overlayScrollPanelScrollbarRT.pivot = Vector2.one;
			overlayScrollPanelScrollbarRT.GetComponent<Scrollbar>().direction = Scrollbar.Direction.BottomToTop;
			overlayScrollPanelScrollBar.transform.GetChild(0).name = "SlidingArea";

			//Arrow Button
			arrowText.name = "Arrow";
			var arrowTextRT = arrowText.GetComponent<RectTransform>();
			arrowTextRT.anchorMin = new Vector2(1f, 0f);
			arrowTextRT.anchorMax = Vector2.one;
			arrowTextRT.sizeDelta = new Vector2(cbbRT.sizeDelta.y, cbbRT.sizeDelta.y);
			arrowTextRT.pivot = new Vector2(1f, 0.5f);
			var arrowTextComponent = arrowText.GetComponent<Text>();
			arrowTextComponent.text = "▼";
			arrowTextComponent.alignment = TextAnchor.MiddleCenter;
			var arrowTextCanvasGroup = arrowText.AddComponent<CanvasGroup>();
			arrowTextCanvasGroup.interactable = false;
			arrowTextCanvasGroup.blocksRaycasts = false;
			Selection.activeGameObject = dropDownListRoot;
		}
		#endregion

		#region RTS Selection box
		[MenuItem("GameObject/UI/Extensions/Selection Box", false)]
		static public void AddSelectionBox(MenuCommand menuCommand)
		{
			var go = CreateNewUI();
			go.name = "Selection Box";
			GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
			RectTransform rect = go.transform as RectTransform;
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.anchoredPosition = Vector2.zero;
			rect.sizeDelta = Vector2.zero;

			Image image = go.AddComponent<Image>();
			image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpriteResourcePath);
			image.type = Image.Type.Sliced;
			image.fillCenter = false;
			image.color = new Color(1f, 1f, 1f, 0.392f);


			SelectionBox selectableArea = go.AddComponent<SelectionBox>();
			selectableArea.selectionMask = rect;
			selectableArea.color = new Color(0.816f, 0.816f, 0.816f, 0.353f);


			GameObject childSelectableItem = CreateUIObject("Selectable", go);
			childSelectableItem.AddComponent<Image>();
			childSelectableItem.AddComponent<ExampleSelectable>();


			Selection.activeGameObject = go;
		}
		#endregion

		#region Bound Tooltip
		[MenuItem("GameObject/UI/Extensions/Bound Tooltip/Tooltip", false)]
		static public void AddBoundTooltip(MenuCommand menuCommand)
		{
			GameObject go = CreateUIElementRoot("Tooltip", menuCommand, s_ImageGUIElementSize);
			var tooltip = go.AddComponent<BoundTooltipTrigger>();
			tooltip.text = "This is my Tooltip Text";
			var boundTooltip = go.AddComponent<Image>();
			boundTooltip.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpriteResourcePath);

			// if there is no ToolTipItem add one...
			CreateToolTipItem(false, go);
			Selection.activeGameObject = go;
		}

		private static void CreateToolTipItem(bool select)
		{
			CreateToolTipItem(select, null);
		}

		private static void CreateToolTipItem(bool select, GameObject parent)
		{
			var btti = Object.FindObjectOfType<BoundTooltipItem>();
			if (btti == null)
			{
				var boundTooltipItem = CreateUIObject("ToolTipItem", parent.GetComponentInParent<Canvas>().gameObject);
				btti = boundTooltipItem.AddComponent<BoundTooltipItem>();
				var boundTooltipItemCanvasGroup = boundTooltipItem.AddComponent<CanvasGroup>();
				boundTooltipItemCanvasGroup.interactable = false;
				boundTooltipItemCanvasGroup.blocksRaycasts = false;
				var boundTooltipItemImage = boundTooltipItem.AddComponent<Image>();
				boundTooltipItemImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpriteResourcePath);
				var boundTooltipItemText = CreateUIObject("Text", boundTooltipItem);
				var boundTooltipItemTextRT = boundTooltipItemText.GetComponent<RectTransform>();
				boundTooltipItemTextRT.anchorMin = Vector2.zero;
				boundTooltipItemTextRT.anchorMax = Vector2.one;
				boundTooltipItemTextRT.sizeDelta = Vector2.one;
				var boundTooltipItemTextcomponent = boundTooltipItemText.AddComponent<Text>();
				boundTooltipItemTextcomponent.alignment = TextAnchor.MiddleCenter;
				Undo.RegisterCreatedObjectUndo(boundTooltipItem, "Create " + boundTooltipItem.name);
			}

			if (select && btti != null)
			{
				Selection.activeGameObject = btti.gameObject;
			}
		}

		#endregion

		#region Progress bar
		[MenuItem("GameObject/UI/Extensions/Progress Bar", false)]
		static public void AddSlider(MenuCommand menuCommand)
		{
			// Create GOs Hierarchy
			GameObject root = CreateUIElementRoot("Progress Bar", menuCommand, s_ThinGUIElementSize);

			GameObject background = CreateUIObject("Background", root);
			GameObject fillArea = CreateUIObject("Fill Area", root);
			GameObject fill = CreateUIObject("Fill", fillArea);

			// Background
			Image backgroundImage = background.AddComponent<Image>();
			backgroundImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpriteResourcePath);
			backgroundImage.type = Image.Type.Sliced;
			backgroundImage.color = s_DefaultSelectableColor;
			RectTransform backgroundRect = background.GetComponent<RectTransform>();
			backgroundRect.anchorMin = new Vector2(0, 0.25f);
			backgroundRect.anchorMax = new Vector2(1, 0.75f);
			backgroundRect.sizeDelta = new Vector2(0, 0);

			// Fill Area
			RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
			fillAreaRect.anchorMin = new Vector2(0, 0.25f);
			fillAreaRect.anchorMax = new Vector2(1, 0.75f);
			fillAreaRect.anchoredPosition = Vector2.zero;
			fillAreaRect.sizeDelta = Vector2.zero;

			// Fill
			Image fillImage = fill.AddComponent<Image>();
			fillImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
			fillImage.type = Image.Type.Sliced;
			fillImage.color = s_DefaultSelectableColor;

			RectTransform fillRect = fill.GetComponent<RectTransform>();
			fillRect.sizeDelta = Vector2.zero;

			// Setup slider component
			Slider slider = root.AddComponent<Slider>();
			slider.value = 0;
			slider.fillRect = fill.GetComponent<RectTransform>();
			slider.targetGraphic = fillImage;
			slider.direction = Slider.Direction.LeftToRight;
			SetDefaultColorTransitionValues(slider);
		}
		#endregion

		#region Primitives

		[MenuItem("GameObject/UI/Extensions/Primitives/UI Line Renderer", false)]
		static public void AddUILineRenderer(MenuCommand menuCommand)
		{
			GameObject go = CreateUIElementRoot("UI LineRenderer", menuCommand, s_ImageGUIElementSize);
			go.AddComponent<UILineRenderer>();
			Selection.activeGameObject = go;
		}

		[MenuItem("GameObject/UI/Extensions/Primitives/UI Line Texture Renderer", false)]
		static public void AddUILineTextureRenderer(MenuCommand menuCommand)
		{
			GameObject go = CreateUIElementRoot("UI LineTextureRenderer", menuCommand, s_ImageGUIElementSize);
			go.AddComponent<UILineTextureRenderer>();
			Selection.activeGameObject = go;
		}

		[MenuItem("GameObject/UI/Extensions/Primitives/UI Circle", false)]
		static public void AddUICircle(MenuCommand menuCommand)
		{
			GameObject go = CreateUIElementRoot("UI Circle", menuCommand, s_ImageGUIElementSize);
			go.AddComponent<UICircle>();
			Selection.activeGameObject = go;
		}

		[MenuItem("GameObject/UI/Extensions/Primitives/UI Diamond Graph", false)]
		static public void AddDiamondGraph(MenuCommand menuCommand)
		{
			GameObject go = CreateUIElementRoot("UI Diamond Graph", menuCommand, s_ImageGUIElementSize);
			go.AddComponent<DiamondGraph>();
			Selection.activeGameObject = go;
		}

		[MenuItem("GameObject/UI/Extensions/Primitives/UI Cut Corners", false)]
		static public void AddCutCorners(MenuCommand menuCommand)
		{
			GameObject go = CreateUIElementRoot("UI Cut Corners", menuCommand, s_ImageGUIElementSize);
			go.AddComponent<UICornerCut>();
			Selection.activeGameObject = go;
		}

		[MenuItem("GameObject/UI/Extensions/Primitives/UI Polygon", false)]
		static public void AddPolygon(MenuCommand menuCommand)
		{
			GameObject go = CreateUIElementRoot("UI Polygon", menuCommand, s_ImageGUIElementSize);
			go.AddComponent<UIPolygon>();
			Selection.activeGameObject = go;
		}

        [MenuItem("GameObject/UI/Extensions/Primitives/UI Grid Renderer", false)]
        static public void AddUIGridRenderer(MenuCommand menuCommand)
        {
            GameObject go = CreateUIElementRoot("UI GridRenderer", menuCommand, s_ImageGUIElementSize);
            go.AddComponent<UIGridRenderer>();
            Selection.activeGameObject = go;
        }

        #endregion

        #region Re-Orderable Lists

        [MenuItem("GameObject/UI/Extensions/Re-orderable Lists/Re-orderable Vertical Scroll Rect", false)]
		static public void AddReorderableScrollRectVertical(MenuCommand menuCommand)
		{
			GameObject reorderableScrollRoot = CreateUIElementRoot("Re-orderable Vertical ScrollRect", menuCommand, s_ThickGUIElementSize);

			GameObject childContent = CreateUIObject("List_Content", reorderableScrollRoot);

			GameObject Element01 = CreateUIObject("Element_01", childContent);
			GameObject Element02 = CreateUIObject("Element_02", childContent);
			GameObject Element03 = CreateUIObject("Element_03", childContent);


			// Set RectTransform to stretch
			RectTransform rectTransformScrollSnapRoot = reorderableScrollRoot.GetComponent<RectTransform>();
			rectTransformScrollSnapRoot.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchoredPosition = Vector2.zero;
			rectTransformScrollSnapRoot.sizeDelta = new Vector2(150f, 150f);


			Image image = reorderableScrollRoot.AddComponent<Image>();
			image.color = new Color(1f, 1f, 1f, 0.392f);

			ScrollRect sr = reorderableScrollRoot.AddComponent<ScrollRect>();
			sr.vertical = true;
			sr.horizontal = false;

			LayoutElement reorderableScrollRootLE = reorderableScrollRoot.AddComponent<LayoutElement>();
			reorderableScrollRootLE.preferredHeight = 300;

			reorderableScrollRoot.AddComponent<Mask>();
			ReorderableList reorderableScrollRootRL = reorderableScrollRoot.AddComponent<ReorderableList>();

			//Setup Content container
			RectTransform rectTransformContent = childContent.GetComponent<RectTransform>();
			rectTransformContent.anchorMin = Vector2.zero;
			rectTransformContent.anchorMax = new Vector2(1f, 1f);
			rectTransformContent.sizeDelta = Vector2.zero;
			VerticalLayoutGroup childContentVLG = childContent.AddComponent<VerticalLayoutGroup>();
			ContentSizeFitter childContentCSF = childContent.AddComponent<ContentSizeFitter>();
			childContentCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			sr.content = rectTransformContent;
			reorderableScrollRootRL.ContentLayout = childContentVLG;

			//Setup 1st Child
			Image Element01Image = Element01.AddComponent<Image>();
			Element01Image.color = Color.green;

			RectTransform rectTransformElement01 = Element01.GetComponent<RectTransform>();
			rectTransformElement01.anchorMin = new Vector2(0f, 0.5f);
			rectTransformElement01.anchorMax = new Vector2(0f, 0.5f);
			rectTransformElement01.pivot = new Vector2(0.5f, 0.5f);

			LayoutElement LEElement01 = Element01.AddComponent<LayoutElement>();
			LEElement01.minHeight = 50;

			//Setup 2nd Child
			Image Element02Image = Element02.AddComponent<Image>();
			Element02Image.color = Color.red;

			RectTransform rectTransformElement02 = Element02.GetComponent<RectTransform>();
			rectTransformElement02.anchorMin = new Vector2(0f, 0.5f);
			rectTransformElement02.anchorMax = new Vector2(0f, 0.5f);
			rectTransformElement02.pivot = new Vector2(0.5f, 0.5f);

			LayoutElement LEElement02 = Element02.AddComponent<LayoutElement>();
			LEElement02.minHeight = 50;

			//Setup 2nd Child
			Image Element03Image = Element03.AddComponent<Image>();
			Element03Image.color = Color.blue;

			RectTransform rectTransformElement03 = Element03.GetComponent<RectTransform>();
			rectTransformElement03.anchorMin = new Vector2(0f, 0.5f);
			rectTransformElement03.anchorMax = new Vector2(0f, 0.5f);
			rectTransformElement03.pivot = new Vector2(0.5f, 0.5f);

			LayoutElement LEElement03 = Element03.AddComponent<LayoutElement>();
			LEElement03.minHeight = 50;


			//Need to add example child components like in the Asset (SJ)
			Selection.activeGameObject = reorderableScrollRoot;
		}

		[MenuItem("GameObject/UI/Extensions/Re-orderable Lists/Re-orderable Horizontal Scroll Rect", false)]
		static public void AddReorderableScrollRectHorizontal(MenuCommand menuCommand)
		{
			GameObject reorderableScrollRoot = CreateUIElementRoot("Re-orderable Horizontal ScrollRect", menuCommand, s_ThickGUIElementSize);

			GameObject childContent = CreateUIObject("List_Content", reorderableScrollRoot);

			GameObject Element01 = CreateUIObject("Element_01", childContent);
			GameObject Element02 = CreateUIObject("Element_02", childContent);
			GameObject Element03 = CreateUIObject("Element_03", childContent);


			// Set RectTransform to stretch
			RectTransform rectTransformScrollSnapRoot = reorderableScrollRoot.GetComponent<RectTransform>();
			rectTransformScrollSnapRoot.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchoredPosition = Vector2.zero;
			rectTransformScrollSnapRoot.sizeDelta = new Vector2(150f, 150f);


			Image image = reorderableScrollRoot.AddComponent<Image>();
			image.color = new Color(1f, 1f, 1f, 0.392f);

			ScrollRect sr = reorderableScrollRoot.AddComponent<ScrollRect>();
			sr.vertical = false;
			sr.horizontal = true;

			LayoutElement reorderableScrollRootLE = reorderableScrollRoot.AddComponent<LayoutElement>();
			reorderableScrollRootLE.preferredHeight = 300;

			reorderableScrollRoot.AddComponent<Mask>();
			ReorderableList reorderableScrollRootRL = reorderableScrollRoot.AddComponent<ReorderableList>();

			//Setup Content container
			RectTransform rectTransformContent = childContent.GetComponent<RectTransform>();
			rectTransformContent.anchorMin = Vector2.zero;
			rectTransformContent.anchorMax = new Vector2(1f, 1f);
			rectTransformContent.sizeDelta = Vector2.zero;
			HorizontalLayoutGroup childContentHLG = childContent.AddComponent<HorizontalLayoutGroup>();
			ContentSizeFitter childContentCSF = childContent.AddComponent<ContentSizeFitter>();
			childContentCSF.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

			sr.content = rectTransformContent;
			reorderableScrollRootRL.ContentLayout = childContentHLG;

			//Setup 1st Child
			Image Element01Image = Element01.AddComponent<Image>();
			Element01Image.color = Color.green;

			RectTransform rectTransformElement01 = Element01.GetComponent<RectTransform>();
			rectTransformElement01.anchorMin = new Vector2(0f, 0.5f);
			rectTransformElement01.anchorMax = new Vector2(0f, 0.5f);
			rectTransformElement01.pivot = new Vector2(0.5f, 0.5f);

			LayoutElement LEElement01 = Element01.AddComponent<LayoutElement>();
			LEElement01.minWidth = 50;

			//Setup 2nd Child
			Image Element02Image = Element02.AddComponent<Image>();
			Element02Image.color = Color.red;

			RectTransform rectTransformElement02 = Element02.GetComponent<RectTransform>();
			rectTransformElement02.anchorMin = new Vector2(0f, 0.5f);
			rectTransformElement02.anchorMax = new Vector2(0f, 0.5f);
			rectTransformElement02.pivot = new Vector2(0.5f, 0.5f);

			LayoutElement LEElement02 = Element02.AddComponent<LayoutElement>();
			LEElement02.minWidth = 50;

			//Setup 2nd Child
			Image Element03Image = Element03.AddComponent<Image>();
			Element03Image.color = Color.blue;

			RectTransform rectTransformElement03 = Element03.GetComponent<RectTransform>();
			rectTransformElement03.anchorMin = new Vector2(0f, 0.5f);
			rectTransformElement03.anchorMax = new Vector2(0f, 0.5f);
			rectTransformElement03.pivot = new Vector2(0.5f, 0.5f);

			LayoutElement LEElement03 = Element03.AddComponent<LayoutElement>();
			LEElement03.minWidth = 50;


			//Need to add example child components like in the Asset (SJ)
			Selection.activeGameObject = reorderableScrollRoot;
		}

		[MenuItem("GameObject/UI/Extensions/Re-orderable Lists/Re-orderable Grid Scroll Rect", false)]
		static public void AddReorderableScrollRectGrid(MenuCommand menuCommand)
		{
			GameObject reorderableScrollRoot = CreateUIElementRoot("Re-orderable Grid ScrollRect", menuCommand, s_ThickGUIElementSize);

			GameObject childContent = CreateUIObject("List_Content", reorderableScrollRoot);

			GameObject Element01 = CreateUIObject("Element_01", childContent);
			GameObject Element02 = CreateUIObject("Element_02", childContent);
			GameObject Element03 = CreateUIObject("Element_03", childContent);


			// Set RectTransform to stretch
			RectTransform rectTransformScrollSnapRoot = reorderableScrollRoot.GetComponent<RectTransform>();
			rectTransformScrollSnapRoot.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchoredPosition = Vector2.zero;
			rectTransformScrollSnapRoot.sizeDelta = new Vector2(150f, 150f);


			Image image = reorderableScrollRoot.AddComponent<Image>();
			image.color = new Color(1f, 1f, 1f, 0.392f);

			ScrollRect sr = reorderableScrollRoot.AddComponent<ScrollRect>();
			sr.vertical = true;
			sr.horizontal = false;

			LayoutElement reorderableScrollRootLE = reorderableScrollRoot.AddComponent<LayoutElement>();
			reorderableScrollRootLE.preferredHeight = 300;

			reorderableScrollRoot.AddComponent<Mask>();
			ReorderableList reorderableScrollRootRL = reorderableScrollRoot.AddComponent<ReorderableList>();

			//Setup Content container
			RectTransform rectTransformContent = childContent.GetComponent<RectTransform>();
			rectTransformContent.anchorMin = Vector2.zero;
			rectTransformContent.anchorMax = new Vector2(1f, 1f);
			rectTransformContent.sizeDelta = Vector2.zero;
			GridLayoutGroup childContentGLG = childContent.AddComponent<GridLayoutGroup>();
			childContentGLG.cellSize = new Vector2(30, 30);
			childContentGLG.spacing = new Vector2(10, 10);
			ContentSizeFitter childContentCSF = childContent.AddComponent<ContentSizeFitter>();
			childContentCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			sr.content = rectTransformContent;
			reorderableScrollRootRL.ContentLayout = childContentGLG;

			//Setup 1st Child
			Image Element01Image = Element01.AddComponent<Image>();
			Element01Image.color = Color.green;

			RectTransform rectTransformElement01 = Element01.GetComponent<RectTransform>();
			rectTransformElement01.anchorMin = new Vector2(0f, 0.5f);
			rectTransformElement01.anchorMax = new Vector2(0f, 0.5f);
			rectTransformElement01.pivot = new Vector2(0.5f, 0.5f);

			LayoutElement LEElement01 = Element01.AddComponent<LayoutElement>();
			LEElement01.minHeight = 50;

			//Setup 2nd Child
			Image Element02Image = Element02.AddComponent<Image>();
			Element02Image.color = Color.red;

			RectTransform rectTransformElement02 = Element02.GetComponent<RectTransform>();
			rectTransformElement02.anchorMin = new Vector2(0f, 0.5f);
			rectTransformElement02.anchorMax = new Vector2(0f, 0.5f);
			rectTransformElement02.pivot = new Vector2(0.5f, 0.5f);

			LayoutElement LEElement02 = Element02.AddComponent<LayoutElement>();
			LEElement02.minHeight = 50;

			//Setup 2nd Child
			Image Element03Image = Element03.AddComponent<Image>();
			Element03Image.color = Color.blue;

			RectTransform rectTransformElement03 = Element03.GetComponent<RectTransform>();
			rectTransformElement03.anchorMin = new Vector2(0f, 0.5f);
			rectTransformElement03.anchorMax = new Vector2(0f, 0.5f);
			rectTransformElement03.pivot = new Vector2(0.5f, 0.5f);

			LayoutElement LEElement03 = Element03.AddComponent<LayoutElement>();
			LEElement03.minHeight = 50;


			//Need to add example child components like in the Asset (SJ)
			Selection.activeGameObject = reorderableScrollRoot;
		}

		[MenuItem("GameObject/UI/Extensions/Re-orderable Lists/Re-orderable Vertical List", false)]
		static public void AddReorderableVerticalList(MenuCommand menuCommand)
		{
			GameObject reorderableScrollRoot = CreateUIElementRoot("Re-orderable Vertical List", menuCommand, s_ThickGUIElementSize);

			GameObject childContent = CreateUIObject("List_Content", reorderableScrollRoot);

			GameObject Element01 = CreateUIObject("Element_01", childContent);
			GameObject Element02 = CreateUIObject("Element_02", childContent);
			GameObject Element03 = CreateUIObject("Element_03", childContent);


			// Set RectTransform to stretch
			RectTransform rectTransformScrollSnapRoot = reorderableScrollRoot.GetComponent<RectTransform>();
			rectTransformScrollSnapRoot.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchoredPosition = Vector2.zero;
			rectTransformScrollSnapRoot.sizeDelta = new Vector2(150f, 150f);

			Image image = reorderableScrollRoot.AddComponent<Image>();
			image.color = new Color(1f, 1f, 1f, 0.392f);

			LayoutElement reorderableScrollRootLE = reorderableScrollRoot.AddComponent<LayoutElement>();
			reorderableScrollRootLE.preferredHeight = 300;

			reorderableScrollRoot.AddComponent<Mask>();
			ReorderableList reorderableScrollRootRL = reorderableScrollRoot.AddComponent<ReorderableList>();

			//Setup Content container
			RectTransform rectTransformContent = childContent.GetComponent<RectTransform>();
			rectTransformContent.anchorMin = Vector2.zero;
			rectTransformContent.anchorMax = new Vector2(1f, 1f);
			rectTransformContent.sizeDelta = Vector2.zero;
			VerticalLayoutGroup childContentVLG = childContent.AddComponent<VerticalLayoutGroup>();
			ContentSizeFitter childContentCSF = childContent.AddComponent<ContentSizeFitter>();
			childContentCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			reorderableScrollRootRL.ContentLayout = childContentVLG;

			//Setup 1st Child
			Image Element01Image = Element01.AddComponent<Image>();
			Element01Image.color = Color.green;

			RectTransform rectTransformElement01 = Element01.GetComponent<RectTransform>();
			rectTransformElement01.anchorMin = new Vector2(0f, 0.5f);
			rectTransformElement01.anchorMax = new Vector2(0f, 0.5f);
			rectTransformElement01.pivot = new Vector2(0.5f, 0.5f);

			LayoutElement LEElement01 = Element01.AddComponent<LayoutElement>();
			LEElement01.minHeight = 50;

			//Setup 2nd Child
			Image Element02Image = Element02.AddComponent<Image>();
			Element02Image.color = Color.red;

			RectTransform rectTransformElement02 = Element02.GetComponent<RectTransform>();
			rectTransformElement02.anchorMin = new Vector2(0f, 0.5f);
			rectTransformElement02.anchorMax = new Vector2(0f, 0.5f);
			rectTransformElement02.pivot = new Vector2(0.5f, 0.5f);

			LayoutElement LEElement02 = Element02.AddComponent<LayoutElement>();
			LEElement02.minHeight = 50;

			//Setup 2nd Child
			Image Element03Image = Element03.AddComponent<Image>();
			Element03Image.color = Color.blue;

			RectTransform rectTransformElement03 = Element03.GetComponent<RectTransform>();
			rectTransformElement03.anchorMin = new Vector2(0f, 0.5f);
			rectTransformElement03.anchorMax = new Vector2(0f, 0.5f);
			rectTransformElement03.pivot = new Vector2(0.5f, 0.5f);

			LayoutElement LEElement03 = Element03.AddComponent<LayoutElement>();
			LEElement03.minHeight = 50;


			//Need to add example child components like in the Asset (SJ)
			Selection.activeGameObject = reorderableScrollRoot;
		}

		[MenuItem("GameObject/UI/Extensions/Re-orderable Lists/Re-orderable Horizontal List", false)]
		static public void AddReorderableHorizontalList(MenuCommand menuCommand)
		{
			GameObject reorderableScrollRoot = CreateUIElementRoot("Re-orderable Horizontal List", menuCommand, s_ThickGUIElementSize);

			GameObject childContent = CreateUIObject("List_Content", reorderableScrollRoot);

			GameObject Element01 = CreateUIObject("Element_01", childContent);
			GameObject Element02 = CreateUIObject("Element_02", childContent);
			GameObject Element03 = CreateUIObject("Element_03", childContent);


			// Set RectTransform to stretch
			RectTransform rectTransformScrollSnapRoot = reorderableScrollRoot.GetComponent<RectTransform>();
			rectTransformScrollSnapRoot.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchoredPosition = Vector2.zero;
			rectTransformScrollSnapRoot.sizeDelta = new Vector2(150f, 150f);


			Image image = reorderableScrollRoot.AddComponent<Image>();
			image.color = new Color(1f, 1f, 1f, 0.392f);

			LayoutElement reorderableScrollRootLE = reorderableScrollRoot.AddComponent<LayoutElement>();
			reorderableScrollRootLE.preferredHeight = 300;

			reorderableScrollRoot.AddComponent<Mask>();
			ReorderableList reorderableScrollRootRL = reorderableScrollRoot.AddComponent<ReorderableList>();

			//Setup Content container
			RectTransform rectTransformContent = childContent.GetComponent<RectTransform>();
			rectTransformContent.anchorMin = Vector2.zero;
			rectTransformContent.anchorMax = new Vector2(1f, 1f);
			rectTransformContent.sizeDelta = Vector2.zero;
			HorizontalLayoutGroup childContentHLG = childContent.AddComponent<HorizontalLayoutGroup>();
			ContentSizeFitter childContentCSF = childContent.AddComponent<ContentSizeFitter>();
			childContentCSF.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

			reorderableScrollRootRL.ContentLayout = childContentHLG;

			//Setup 1st Child
			Image Element01Image = Element01.AddComponent<Image>();
			Element01Image.color = Color.green;

			RectTransform rectTransformElement01 = Element01.GetComponent<RectTransform>();
			rectTransformElement01.anchorMin = new Vector2(0f, 0.5f);
			rectTransformElement01.anchorMax = new Vector2(0f, 0.5f);
			rectTransformElement01.pivot = new Vector2(0.5f, 0.5f);

			LayoutElement LEElement01 = Element01.AddComponent<LayoutElement>();
			LEElement01.minWidth = 50;

			//Setup 2nd Child
			Image Element02Image = Element02.AddComponent<Image>();
			Element02Image.color = Color.red;

			RectTransform rectTransformElement02 = Element02.GetComponent<RectTransform>();
			rectTransformElement02.anchorMin = new Vector2(0f, 0.5f);
			rectTransformElement02.anchorMax = new Vector2(0f, 0.5f);
			rectTransformElement02.pivot = new Vector2(0.5f, 0.5f);

			LayoutElement LEElement02 = Element02.AddComponent<LayoutElement>();
			LEElement02.minWidth = 50;

			//Setup 2nd Child
			Image Element03Image = Element03.AddComponent<Image>();
			Element03Image.color = Color.blue;

			RectTransform rectTransformElement03 = Element03.GetComponent<RectTransform>();
			rectTransformElement03.anchorMin = new Vector2(0f, 0.5f);
			rectTransformElement03.anchorMax = new Vector2(0f, 0.5f);
			rectTransformElement03.pivot = new Vector2(0.5f, 0.5f);

			LayoutElement LEElement03 = Element03.AddComponent<LayoutElement>();
			LEElement03.minWidth = 50;


			//Need to add example child components like in the Asset (SJ)
			Selection.activeGameObject = reorderableScrollRoot;
		}

		[MenuItem("GameObject/UI/Extensions/Re-orderable Lists/Re-orderable Grid", false)]
		static public void AddReorderableGrid(MenuCommand menuCommand)
		{
			GameObject reorderableScrollRoot = CreateUIElementRoot("Re-orderable Grid", menuCommand, s_ThickGUIElementSize);

			GameObject childContent = CreateUIObject("List_Content", reorderableScrollRoot);

			GameObject Element01 = CreateUIObject("Element_01", childContent);
			GameObject Element02 = CreateUIObject("Element_02", childContent);
			GameObject Element03 = CreateUIObject("Element_03", childContent);


			// Set RectTransform to stretch
			RectTransform rectTransformScrollSnapRoot = reorderableScrollRoot.GetComponent<RectTransform>();
			rectTransformScrollSnapRoot.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransformScrollSnapRoot.anchoredPosition = Vector2.zero;
			rectTransformScrollSnapRoot.sizeDelta = new Vector2(150f, 150f);


			Image image = reorderableScrollRoot.AddComponent<Image>();
			image.color = new Color(1f, 1f, 1f, 0.392f);

			LayoutElement reorderableScrollRootLE = reorderableScrollRoot.AddComponent<LayoutElement>();
			reorderableScrollRootLE.preferredHeight = 300;

			reorderableScrollRoot.AddComponent<Mask>();
			ReorderableList reorderableScrollRootRL = reorderableScrollRoot.AddComponent<ReorderableList>();

			//Setup Content container
			RectTransform rectTransformContent = childContent.GetComponent<RectTransform>();
			rectTransformContent.anchorMin = Vector2.zero;
			rectTransformContent.anchorMax = new Vector2(1f, 1f);
			rectTransformContent.sizeDelta = Vector2.zero;
			GridLayoutGroup childContentGLG = childContent.AddComponent<GridLayoutGroup>();
			childContentGLG.cellSize = new Vector2(30, 30);
			childContentGLG.spacing = new Vector2(10, 10);
			ContentSizeFitter childContentCSF = childContent.AddComponent<ContentSizeFitter>();
			childContentCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			reorderableScrollRootRL.ContentLayout = childContentGLG;

			//Setup 1st Child
			Image Element01Image = Element01.AddComponent<Image>();
			Element01Image.color = Color.green;

			RectTransform rectTransformElement01 = Element01.GetComponent<RectTransform>();
			rectTransformElement01.anchorMin = new Vector2(0f, 0.5f);
			rectTransformElement01.anchorMax = new Vector2(0f, 0.5f);
			rectTransformElement01.pivot = new Vector2(0.5f, 0.5f);

			LayoutElement LEElement01 = Element01.AddComponent<LayoutElement>();
			LEElement01.minHeight = 50;

			//Setup 2nd Child
			Image Element02Image = Element02.AddComponent<Image>();
			Element02Image.color = Color.red;

			RectTransform rectTransformElement02 = Element02.GetComponent<RectTransform>();
			rectTransformElement02.anchorMin = new Vector2(0f, 0.5f);
			rectTransformElement02.anchorMax = new Vector2(0f, 0.5f);
			rectTransformElement02.pivot = new Vector2(0.5f, 0.5f);

			LayoutElement LEElement02 = Element02.AddComponent<LayoutElement>();
			LEElement02.minHeight = 50;

			//Setup 2nd Child
			Image Element03Image = Element03.AddComponent<Image>();
			Element03Image.color = Color.blue;

			RectTransform rectTransformElement03 = Element03.GetComponent<RectTransform>();
			rectTransformElement03.anchorMin = new Vector2(0f, 0.5f);
			rectTransformElement03.anchorMax = new Vector2(0f, 0.5f);
			rectTransformElement03.pivot = new Vector2(0.5f, 0.5f);

			LayoutElement LEElement03 = Element03.AddComponent<LayoutElement>();
			LEElement03.minHeight = 50;


			//Need to add example child components like in the Asset (SJ)
			Selection.activeGameObject = reorderableScrollRoot;
		}


        #endregion

        #region Segmented Control
        [MenuItem("GameObject/UI/Extensions/Segmented Control", false)]
        static public void AddSegmentedControl(MenuCommand menuCommand)
        {
            GameObject go = CreateUIElementRoot("Segmented Control", menuCommand, s_ThinGUIElementSize);
            SegmentedControl control = go.AddComponent<SegmentedControl>();

            Color selectedColor = new Color(0f, 0.455f, 0.894f);
            control.selectedColor = selectedColor;

            var labels = new string[] { "This", "That", "Other" };
            for (int i = 0; i < 3; i++)
			{
                var button = AddButtonAsChild(go);
                button.name = "Segment " + (i + 1);
                var text = button.GetComponentInChildren<Text>();
                text.text = labels[i];
                text.color = selectedColor;
            }

            control.LayoutSegments();

            Selection.activeGameObject = go;
        }
        #endregion
		
        #region Stepper
        [MenuItem("GameObject/UI/Extensions/Stepper", false)]
        static public void AddStepper(MenuCommand menuCommand)
        {
            GameObject go = CreateUIElementRoot("Stepper", menuCommand, new Vector2(42, kThinHeight));
            Stepper control = go.AddComponent<Stepper>();

            var labels = new string[] { "−", "+" };
            for (int i = 0; i < 2; i++)
            {
                var button = AddButtonAsChild(go);
				button.gameObject.AddComponent<StepperSide>();
                button.name = i == 0 ? "Minus" : "Plus";
                var text = button.GetComponentInChildren<Text>();
                text.text = labels[i];
            }

            control.LayoutSides();

            Selection.activeGameObject = go;
        }
        #endregion

        #region UI Knob
        [MenuItem("GameObject/UI/Extensions/UI Knob", false)]
		static public void AddUIKnob(MenuCommand menuCommand)
		{
			GameObject go = CreateUIElementRoot("UI Knob", menuCommand, s_ImageGUIElementSize);
			go.AddComponent<Image>();
			go.AddComponent<UI_Knob>();
			Selection.activeGameObject = go;
		}
        #endregion

        #region TextPic
        [MenuItem("GameObject/UI/Extensions/TextPic", false)]
		static public void AddTextPic(MenuCommand menuCommand)
		{
			GameObject go = CreateUIElementRoot("TextPic", menuCommand, s_ImageGUIElementSize);
			go.AddComponent<TextPic>();
			Selection.activeGameObject = go;
		}
        #endregion

        #region BoxSlider
        [MenuItem("GameObject/UI/Extensions/Box Slider", false)]
        static public void AddBoxSlider(MenuCommand menuCommand)
        {

            GameObject uiboxSliderRoot = CreateUIElementRoot("Box Slider", menuCommand, s_ImageGUIElementSize);

            GameObject handleSlideArea = CreateUIObject("Handle Slide Area", uiboxSliderRoot);

            GameObject handle = CreateUIObject("Handle", handleSlideArea);

            // Set RectTransform to stretch
            SetAnchorsAndStretch(uiboxSliderRoot);
            Image backgroundImage = uiboxSliderRoot.AddComponent<Image>();
            backgroundImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpriteResourcePath);
            backgroundImage.type = Image.Type.Sliced;
            backgroundImage.fillCenter = false;
            backgroundImage.color = new Color(1f, 1f, 1f, 0.392f);

            RectTransform handleRect = SetAnchorsAndStretch(handle); 
            handleRect.sizeDelta = new Vector2(25, 25);
            Image handleImage = handle.AddComponent<Image>();
            handleImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kKnobPath);
            handleImage.type = Image.Type.Simple;
            handleImage.fillCenter = false;
            handleImage.color = new Color(1f, 1f, 1f, 0.392f);


            BoxSlider selectableArea = uiboxSliderRoot.AddComponent<BoxSlider>();
            selectableArea.HandleRect = handle.GetComponent<RectTransform>();
            selectableArea.ValueX = selectableArea.ValueY = 0.5f;

            Selection.activeGameObject = uiboxSliderRoot;
        }
        #endregion

        #region Non Drawing  Graphic options
        [MenuItem("GameObject/UI/Extensions/NonDrawingGraphic", false)]
        static public void AddNonDrawingGraphic(MenuCommand menuCommand)
        {
            GameObject go = CreateUIElementRoot("NonDrawing Graphic", menuCommand, s_ImageGUIElementSize);
            go.AddComponent<NonDrawingGraphic>();
            Selection.activeGameObject = go;
        }

        [MenuItem("GameObject/UI/Extensions/NonDrawingGraphicClickable", false)]
        static public void AddClickableNonDrawingGraphic(MenuCommand menuCommand)
        {
            GameObject go = CreateUIElementRoot("NonDrawing Graphic-Clickable", menuCommand, s_ImageGUIElementSize);
            go.AddComponent<NonDrawingGraphic>();
            go.AddComponent<UISelectableExtension>();
            Selection.activeGameObject = go;
        }
        #endregion

        #region Radial Slider
        [MenuItem("GameObject/UI/Extensions/RadialSlider", false)]
        static public void AddRadialSlider(MenuCommand menuCommand)
        {
            GameObject sliderRoot = CreateUIElementRoot("Radial Slider", menuCommand, s_ThickGUIElementSize);
            GameObject SliderControl = CreateUIObject("Slider", sliderRoot);

            Image image = sliderRoot.AddComponent<Image>();
            image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
            image.type = Image.Type.Simple;
            image.color = s_DefaultSelectableColor;

            RectTransform sliderRootRectTransform = sliderRoot.GetComponent<RectTransform>();
            sliderRootRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            sliderRootRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            sliderRootRectTransform.anchoredPosition = Vector2.zero;
            sliderRootRectTransform.sizeDelta = new Vector2(250f, 250f);

            Image slidrImage = SliderControl.AddComponent<Image>();
            slidrImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
            slidrImage.type = Image.Type.Filled;
            slidrImage.fillMethod = Image.FillMethod.Radial360;
            slidrImage.fillOrigin = 3;
            slidrImage.color = Color.red;
            slidrImage.fillAmount = 0;
            RadialSlider slider = SliderControl.AddComponent<RadialSlider>();
            slider.StartColor = Color.green;
            slider.EndColor = Color.red;

            RectTransform sliderRectTransform = SliderControl.GetComponent<RectTransform>();
            sliderRectTransform.anchorMin = Vector2.zero;
            sliderRectTransform.anchorMax = Vector2.one;
            sliderRectTransform.sizeDelta = Vector2.zero;

            Selection.activeGameObject = sliderRoot;
        }
        #endregion

        #region Menu Manager GO
        [MenuItem("GameObject/UI/Extensions/Menu Manager", false)]
        static public void AddMenuManager(MenuCommand menuCommand)
        {
            GameObject child = new GameObject("MenuManager");
            Undo.RegisterCreatedObjectUndo(child, "Create " + "MenuManager");
            child.AddComponent<MenuManager>();
            Selection.activeGameObject = child;
        }
        #endregion


        #endregion

        #region Helper Functions
        private static GameObject AddInputFieldAsChild(GameObject parent)
		{
			GameObject root = CreateUIObject("InputField", parent);

			GameObject childPlaceholder = CreateUIObject("Placeholder", root);
			GameObject childText = CreateUIObject("Text", root);

			Image image = root.AddComponent<Image>();
			image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kInputFieldBackgroundPath);
			image.type = Image.Type.Sliced;
			image.color = s_DefaultSelectableColor;

			InputField inputField = root.AddComponent<InputField>();
			SetDefaultColorTransitionValues(inputField);

			Text text = childText.AddComponent<Text>();
			text.text = "";
			text.supportRichText = false;
			SetDefaultTextValues(text);

			Text placeholder = childPlaceholder.AddComponent<Text>();
			placeholder.text = "Enter text...";
			placeholder.fontStyle = FontStyle.Italic;
			// Make placeholder color half as opaque as normal text color.
			Color placeholderColor = text.color;
			placeholderColor.a *= 0.5f;
			placeholder.color = placeholderColor;

			RectTransform textRectTransform = childText.GetComponent<RectTransform>();
			textRectTransform.anchorMin = Vector2.zero;
			textRectTransform.anchorMax = Vector2.one;
			textRectTransform.sizeDelta = Vector2.zero;
			textRectTransform.offsetMin = new Vector2(10, 6);
			textRectTransform.offsetMax = new Vector2(-10, -7);

			RectTransform placeholderRectTransform = childPlaceholder.GetComponent<RectTransform>();
			placeholderRectTransform.anchorMin = Vector2.zero;
			placeholderRectTransform.anchorMax = Vector2.one;
			placeholderRectTransform.sizeDelta = Vector2.zero;
			placeholderRectTransform.offsetMin = new Vector2(10, 6);
			placeholderRectTransform.offsetMax = new Vector2(-10, -7);

			inputField.textComponent = text;
			inputField.placeholder = placeholder;

			return root;
		}

		private static GameObject AddScrollbarAsChild(GameObject parent)
		{
			// Create GOs Hierarchy
			GameObject scrollbarRoot = CreateUIObject("Scrollbar", parent);

			GameObject sliderArea = CreateUIObject("Sliding Area", scrollbarRoot);
			GameObject handle = CreateUIObject("Handle", sliderArea);

			Image bgImage = scrollbarRoot.AddComponent<Image>();
			bgImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpriteResourcePath);
			bgImage.type = Image.Type.Sliced;
			bgImage.color = s_DefaultSelectableColor;

			Image handleImage = handle.AddComponent<Image>();
			handleImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
			handleImage.type = Image.Type.Sliced;
			handleImage.color = s_DefaultSelectableColor;

			RectTransform sliderAreaRect = sliderArea.GetComponent<RectTransform>();
			sliderAreaRect.sizeDelta = new Vector2(-20, -20);
			sliderAreaRect.anchorMin = Vector2.zero;
			sliderAreaRect.anchorMax = Vector2.one;

			RectTransform handleRect = handle.GetComponent<RectTransform>();
			handleRect.sizeDelta = new Vector2(20, 20);

			Scrollbar scrollbar = scrollbarRoot.AddComponent<Scrollbar>();
			scrollbar.handleRect = handleRect;
			scrollbar.targetGraphic = handleImage;
			SetDefaultColorTransitionValues(scrollbar);

			return scrollbarRoot;
		}

		private static GameObject AddTextAsChild(GameObject parent)
		{
			GameObject go = CreateUIObject("Text", parent);

			Text lbl = go.AddComponent<Text>();
			lbl.text = "New Text";
			SetDefaultTextValues(lbl);

			return go;
		}

		private static GameObject AddImageAsChild(GameObject parent)
		{
			GameObject go = CreateUIObject("Image", parent);

			go.AddComponent<Image>();

			return go;
		}

		private static GameObject AddButtonAsChild(GameObject parent)
		{
			GameObject buttonRoot = CreateUIObject("Button", parent);

			GameObject childText = new GameObject("Text");
			GameObjectUtility.SetParentAndAlign(childText, buttonRoot);

			Image image = buttonRoot.AddComponent<Image>();
			image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
			image.type = Image.Type.Sliced;
			image.color = s_DefaultSelectableColor;

			Button bt = buttonRoot.AddComponent<Button>();
			SetDefaultColorTransitionValues(bt);

			Text text = childText.AddComponent<Text>();
			text.text = "Button";
			text.alignment = TextAnchor.MiddleCenter;
			SetDefaultTextValues(text);

			RectTransform textRectTransform = childText.GetComponent<RectTransform>();
			textRectTransform.anchorMin = Vector2.zero;
			textRectTransform.anchorMax = Vector2.one;
			textRectTransform.sizeDelta = Vector2.zero;

			return buttonRoot;
		}

        private static RectTransform SetAnchorsAndStretch(GameObject root)
        {
            RectTransform rectTransformRoot = root.GetComponent<RectTransform>();
            rectTransformRoot.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransformRoot.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransformRoot.anchoredPosition = Vector2.zero;
            return rectTransformRoot;
        }

        #endregion

    }
}
