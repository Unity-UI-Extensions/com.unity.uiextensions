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
        private static Vector2 s_ThinGUIElementSize     = new Vector2(kWidth, kThinHeight);
        private static Vector2 s_ImageGUIElementSize    = new Vector2(100f, 100f);
        private static Color   s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
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

        [MenuItem("GameObject/UI/Canvas", false, 2009)]
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
        #endregion
        #endregion

        #region UI Extensions "Create" Menu items

        [MenuItem("GameObject/UI/Extensions/Horizontal Scroll Snap", false, 2000)]
        static public void AddHorizontalScrollSnap(MenuCommand menuCommand)
        {
            GameObject horizontalScrollSnapRoot = CreateUIElementRoot("Horizontal Scroll Snap", menuCommand, s_ThickGUIElementSize);

            // Set RectTransform to stretch
            RectTransform rectTransform = horizontalScrollSnapRoot.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;

            Image image = horizontalScrollSnapRoot.AddComponent<Image>();
            image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpriteResourcePath);
            image.type = Image.Type.Sliced;
            image.color = new Color(1f, 1f, 1f, 0.392f);

            ScrollRect sr = horizontalScrollSnapRoot.AddComponent<ScrollRect>();
            HorizontalScrollSnap HSS = horizontalScrollSnapRoot.AddComponent<HorizontalScrollSnap>();
            Selection.activeGameObject = horizontalScrollSnapRoot;
        }

        [MenuItem("GameObject/UI/Extensions/UI Button", false, 2001)]
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

        [MenuItem("GameObject/UI/Extensions/UI Flippable", false, 2003)]
        static public void AddUIFlippableImage(MenuCommand menuCommand)
        {
            GameObject go = CreateUIElementRoot("UI Flippable", menuCommand, s_ImageGUIElementSize);
            go.AddComponent<Image>();
            go.AddComponent<UIFlippable>();
            Selection.activeGameObject = go;
        }

        [MenuItem("GameObject/UI/Extensions/UI Window Base", false, 2004)]
        static public void AddUIWindowBase(MenuCommand menuCommand)
        {
            GameObject go = CreateUIElementRoot("UI Window Base", menuCommand, s_ThickGUIElementSize);
            go.AddComponent<UIWindowBase>();
            Selection.activeGameObject = go;
        }

        [MenuItem("GameObject/UI/Extensions/Accordion/Accordion Group", false, 2002)]
        static public void AddAccordionGroup(MenuCommand menuCommand)
        {
            GameObject go = CreateUIElementRoot("Accordion Group", menuCommand, s_ThickGUIElementSize);
            go.AddComponent<VerticalLayoutGroup>();
            go.AddComponent<ContentSizeFitter>();
            go.AddComponent<ToggleGroup>();
            go.AddComponent<Accordion>();
            Selection.activeGameObject = go;
        }

        [MenuItem("GameObject/UI/Extensions/Accordion/Accordion Element", false, 2002)]
        static public void AddAccordionElement(MenuCommand menuCommand)
        {
            GameObject go = CreateUIElementRoot("Accordion Element", menuCommand, s_ThickGUIElementSize);
            go.AddComponent<LayoutElement>();
            go.AddComponent<AccordionElement>();
            Selection.activeGameObject = go;

        }

        [MenuItem("GameObject/UI/Extensions/ComboBox", false, 2002)]
        static public void AddComboBox(MenuCommand menuCommand)
        {
            GameObject go = CreateUIElementRoot("ComboBox", menuCommand, s_ThickGUIElementSize);
            go.AddComponent<ComboBox>();
            Selection.activeGameObject = go;
        }

        [MenuItem("GameObject/UI/Extensions/Selection Box", false, 2009)]
        static public void AddSelectionBox(MenuCommand menuCommand)
        {
            var go = CreateNewUI();
            go.name = "Selection Box";
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            if (go.transform.parent as RectTransform)
            {
                RectTransform rect = go.transform as RectTransform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = Vector2.zero;
            }
            go.AddComponent<SelectionBox>();
            Selection.activeGameObject = go;
        }

        #endregion
    }
}
