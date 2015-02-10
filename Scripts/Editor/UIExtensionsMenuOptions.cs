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
        private const float  kWidth       = 160f;
        private const float  kThickHeight = 30f;
        private const float  kThinHeight  = 20f;
        private const string kStandardSpritePath           = "UI/Skin/UISprite.psd";
        private const string kBackgroundSpriteResourcePath = "UI/Skin/Background.psd";
        private const string kInputFieldBackgroundPath     = "UI/Skin/InputFieldBackground.psd";
        private const string kKnobPath                     = "UI/Skin/Knob.psd";
        private const string kCheckmarkPath                = "UI/Skin/Checkmark.psd";

        private static Vector2 s_ThickGUIElementSize    = new Vector2(kWidth, kThickHeight);
        //private static Vector2 s_ThinGUIElementSize     = new Vector2(kWidth, kThinHeight);
        private static Vector2 s_ImageGUIElementSize    = new Vector2(100f, 100f);
        private static Color   s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
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

        [MenuItem("GameObject/UI/EventSystem", false, 2010)]
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
                eventSystem.AddComponent<TouchInputModule>();

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

        [MenuItem("GameObject/UI/Extensions/Horizontal Scroll Snap", false)]
        static public void AddHorizontalScrollSnap(MenuCommand menuCommand)
        {
            GameObject horizontalScrollSnapRoot = CreateUIElementRoot("Horizontal Scroll Snap", menuCommand, s_ThickGUIElementSize);

            GameObject childContent = new GameObject("Content");
            GameObjectUtility.SetParentAndAlign(childContent, horizontalScrollSnapRoot);

            GameObject childPage01 = new GameObject("Page_01");
            GameObjectUtility.SetParentAndAlign(childPage01, childContent);

            GameObject childText = new GameObject("Text");
            GameObjectUtility.SetParentAndAlign(childText, childPage01);

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
            horizontalScrollSnapRoot.AddComponent<HorizontalScrollSnap>();

            //Setup Content container
            RectTransform rectTransformContent = childContent.AddComponent<RectTransform>();
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
            rectTransformPage01.anchorMin = new Vector2(0f, 0.5f);
            rectTransformPage01.anchorMax = new Vector2(0f, 0.5f);
            //rectTransformPage01.anchoredPosition = Vector2.zero;
            //rectTransformPage01.sizeDelta = Vector2.zero;
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
            //rectTransformPage01Text.anchoredPosition = Vector2.zero;
            //rectTransformPage01Text.sizeDelta = Vector2.zero;
            rectTransformPage01Text.pivot = new Vector2(0.5f, 0.5f);


            //Need to add example child components like in the Asset (SJ)

            Selection.activeGameObject = horizontalScrollSnapRoot;
        }

        [MenuItem("GameObject/UI/Extensions/UI Button", false)]
        static public void AddUIButton(MenuCommand menuCommand)
        {
            GameObject uiButtonRoot = CreateUIElementRoot("UI Button", menuCommand, s_ThickGUIElementSize);
            GameObject childText = new GameObject("Text");
            GameObjectUtility.SetParentAndAlign(childText, uiButtonRoot);

            Image image = uiButtonRoot.AddComponent<Image>();
            image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
            image.type = Image.Type.Sliced;
            image.color = s_DefaultSelectableColor;

            UIButton bt = uiButtonRoot.AddComponent<UIButton>();
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

        [MenuItem("GameObject/UI/Extensions/UI Flippable", false)]
        static public void AddUIFlippableImage(MenuCommand menuCommand)
        {
            GameObject go = CreateUIElementRoot("UI Flippable", menuCommand, s_ImageGUIElementSize);
            go.AddComponent<Image>();
            go.AddComponent<UIFlippable>();
            Selection.activeGameObject = go;
        }

        [MenuItem("GameObject/UI/Extensions/UI Window Base", false)]
        static public void AddUIWindowBase(MenuCommand menuCommand)
        {
            GameObject go = CreateUIElementRoot("UI Window Base", menuCommand, s_ThickGUIElementSize);
            go.AddComponent<UIWindowBase>();
            go.AddComponent<Image>();
            Selection.activeGameObject = go;
        }

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
            Events.UnityEventTools.AddPersistentListener<string>(inputField.GetComponent<InputField>().onValueChange, new UnityEngine.Events.UnityAction<string>(comboBox.OnValueChanged));

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


            GameObject childSelectableItem = new GameObject("Selectable");
            GameObjectUtility.SetParentAndAlign(childSelectableItem, go);
            childSelectableItem.AddComponent<Image>();
            childSelectableItem.AddComponent<ExampleSelectable>();


            Selection.activeGameObject = go;
        }

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
        #endregion
    }
}
