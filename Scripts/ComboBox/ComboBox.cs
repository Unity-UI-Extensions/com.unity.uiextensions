///Credit perchik
///Sourced from - http://forum.unity3d.com/threads/receive-onclick-event-and-pass-it-on-to-lower-ui-elements.293642/

using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Extensions/ComboBox")]
    public class ComboBox : MonoBehaviour
    {
        #region declarations
        #region private members

        private bool _isActive = false; //is the drop down panel active

        private Button comboBtn;
        private Image comboBtnImg;
        private Text comboBtnText;

        private Button overlayBtn;

        private GridLayoutGroup itemLayout;


        private float _scrollbarWidth = 20.0f;

        private int scrollOffset; //offset of the selected item

        private int _itemsToDisplay = 4; //how many items to show in the dropdown panel

        private bool _hideFirstItem = false; //lets us hide the prompt after something is chosen

        private int _selectedIndex = 0;

        private List<ComboBoxItem> _items; //conceptual items in the list

        private bool _interactable = true;

        private Canvas _canvas;

        #region private rect transforms
        /// <remarks> All of these have to be properties so that the editor script can access them</remarks>

        private RectTransform _overlay; //overlayRT is a screensized box to handle clicks *not* on the button. (although this might have to change with multiple things on the screen.
        private RectTransform overlayRT
        {
            get
            {
                if (_overlay == null)
                {
                    _overlay = rectTransform.FindChild("Overlay").GetComponent<RectTransform>();
                    overlayBtn = _overlay.gameObject.GetComponent<Button>();
                }
                return _overlay;
            }
            set
            {
                _overlay = value;
            }
        }

        private RectTransform _rectTransform;
        private RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                {
                    _rectTransform = GetComponent<RectTransform>();
                }
                return _rectTransform;
            }
            set { _rectTransform = value; }
        }

        private RectTransform _comboBtnRT;
        private RectTransform comboBtnRT
        {
            get
            {
                if (_comboBtnRT == null)
                {
                    _comboBtnRT = rectTransform.FindChild("ComboButton").GetComponent<RectTransform>();
                    comboBtn = _comboBtnRT.GetComponent<Button>();
                    comboBtnImg = _comboBtnRT.FindChild("Image").GetComponent<Image>();
                    comboBtnText = _comboBtnRT.FindChild("Text").GetComponent<Text>();
                }
                return _comboBtnRT;
            }
            set
            {
                _comboBtnRT = value;
            }
        }

        private GameObject _scrollPanel;
        private GameObject scrollPanel
        {
            get
            {
                if (_scrollPanel == null)
                    _scrollPanel = overlayRT.FindChild("ScrollPanel").gameObject;
                return _scrollPanel;
            }
            set
            {
                _scrollPanel = value;
            }
        }

        private RectTransform _scrollPanelRT;
        private RectTransform scrollPanelRT
        {
            get
            {
                if (_scrollPanelRT == null)
                    _scrollPanelRT = scrollPanel.GetComponent<RectTransform>();
                return _scrollPanelRT;
            }
            set
            {
                _scrollPanelRT = value;
            }
        }

        private RectTransform _itemsRT;
        private RectTransform itemsRT
        {
            get
            {
                if (_itemsRT == null)
                {
                    _itemsRT = scrollPanelRT.Find("Items").GetComponent<RectTransform>();
                    itemLayout = _itemsRT.gameObject.GetComponent<GridLayoutGroup>();
                }
                return _itemsRT;
            }
            set
            {
                _itemsRT = value;
            }
        }

        private RectTransform _scrollbarRT;
        private RectTransform scrollbarRT
        {
            get
            {
                if (_scrollbarRT == null)
                    _scrollbarRT = scrollPanelRT.Find("Scrollbar").GetComponent<RectTransform>();
                return _scrollbarRT;
            }
            set
            {
                _scrollbarRT = value;
            }
        }

        private RectTransform _slidingAreaRT;
        private RectTransform slidingAreaRT
        {
            get
            {
                if (_slidingAreaRT == null)
                    _slidingAreaRT = scrollbarRT.Find("SlidingArea").GetComponent<RectTransform>();
                return _slidingAreaRT;
            }
            set
            {
                _slidingAreaRT = value;
            }
        }

        private RectTransform _scrollHandleRT;
        private RectTransform scrollHandleRT
        {
            get
            {
                if (_scrollHandleRT == null)
                    _scrollHandleRT = slidingAreaRT.Find("Handle").GetComponent<RectTransform>();
                return _scrollHandleRT;
            }
            set
            {
                _scrollHandleRT = value;
            }
        }


        #endregion

        #endregion
        #region public accessors

        public string HeaderOption = "";
        public Color32 disabledTextColor = new Color32(174, 174, 174, 255);
        public bool Interactable
        {
            get
            {
                return _interactable;
            }
            set
            {
                _interactable = value;

                comboBtn.interactable = _interactable;
                if (comboBtnImg.sprite != null)
                {
                    comboBtnImg.color = _interactable ?
                            comboBtn.colors.normalColor :
                            comboBtn.colors.disabledColor;
                }
                else
                {
                    comboBtnImg.color = new Color(1, 1, 1, 0); //transparent
                }
                if (!Application.isPlaying)//stop it from messing up in the editor
                    return;
                if (!_interactable && _isActive)
                    ToggleComboBox(false);
            }
        }

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (_selectedIndex == value)
                    return;
                if (value > -1 && value < Items.Count)
                {
                    _selectedIndex = value;
                    RefreshSelected();
                }
            }
        }


        public List<ComboBoxItem> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<ComboBoxItem>();
                }

                return _items;
            }
            set
            {
                _items = value;
                Refresh();
            }
        }

        public bool HideFirstItem
        {
            get
            {
                return _hideFirstItem;
            }
            set
            {
                if (value)
                    scrollOffset--;
                else
                    scrollOffset++;
                _hideFirstItem = value;
                Refresh();
            }
        }

        public int ItemsToDisplay
        {
            get
            {
                return _itemsToDisplay;
            }
            set
            {
                if (_itemsToDisplay == value)
                    return;
                _itemsToDisplay = value;
                Refresh();
            }
        }

        public System.Action<int> OnSelectionChanged;//fires  when selection is changed.

        #endregion


        #endregion

        #region public methods

        /// <summary>
        /// Update the main button with the selected item's parameters
        /// </summary>
        public void RefreshSelected()
        {
            //get the selected item
            ComboBoxItem item = (SelectedIndex > -1 && SelectedIndex < Items.Count) ? Items[SelectedIndex] : null;
            if (item == null) return;

            bool hasImage = (item.Image != null);
            comboBtnImg.sprite = hasImage ? item.Image : null;
            comboBtnImg.color = !hasImage ? new Color(1, 1, 1, 0)//transparent if there's no image.
                                          : !Interactable ? new Color(1, 1, 1, .5f) //semitransparent if the combobox is disabled
                                                          : Color.white;  //fully opaque if it has an image and the combobox is enabled
            UpdateComboBoxText(comboBtnRT, hasImage);
            comboBtnText.text = item.Caption;

            comboBtn.onClick.RemoveAllListeners();
            comboBtn.onClick.AddListener(() =>
            {
                ToggleComboBox(true);
            });
            if (!Application.isPlaying) return; //if it was running in editor we stop here.

            for (int i = 0; i < itemsRT.childCount; i++)
            {
                Image tempImg = itemsRT.GetChild(i).GetComponent<Image>();
                tempImg.color = (SelectedIndex == (i + (HideFirstItem ? 1 : 0))) ? comboBtn.colors.highlightedColor : comboBtn.colors.normalColor;
            }
        }

        /// <summary>
        /// what happens when an item in the list is selected
        /// </summary>
        /// <param name="index"></param>
        public void OnItemClicked(int index)
        {
            Debug.Log("item " + index + " was clicked");
            bool selectionChanged = (index != SelectedIndex);
            SelectedIndex = index;
            ToggleComboBox(true);
            if (selectionChanged && OnSelectionChanged != null) OnSelectionChanged(index);
        }

        /// <summary>
        /// Add items to the dropdown list. Accepts any object of type ComboBoxItem, String, or Image
        /// </summary>
        /// <param name="list"></param>
        public void AddItems(params object[] list)
        {
            List<ComboBoxItem> cbItems = new List<ComboBoxItem>();
            foreach (var obj in list)
            {
                if (obj is ComboBoxItem)
                {
                    cbItems.Add((ComboBoxItem)obj);
                }
                else if (obj is string)
                {
                    cbItems.Add(new ComboBoxItem(caption: (string)obj));
                }
                else if (obj is Sprite)
                {
                    cbItems.Add(new ComboBoxItem(image: (Sprite)obj));
                }
                else
                {
                    throw new System.Exception("Only ComboBoxItems, Strings, and Sprite types are allowed");
                }
            }
            Items.AddRange(cbItems);
            Items = Items.Distinct().ToList();//remove any duplicates

        }

        public void ClearItems()
        {
            Items.Clear();
        }

        /// <summary>
        /// Redraw the graphics (in response to something changing)
        /// </summary>
        public void UpdateGraphics()
        {
            //center the handle in the scrollbar
            float scrollbarWidth = Items.Count - (HideFirstItem ? 1 : 0) > ItemsToDisplay ? _scrollbarWidth : 0.0f;
            scrollHandleRT.offsetMin = -scrollbarWidth / 2 * Vector2.one;
            scrollHandleRT.offsetMax = scrollbarWidth / 2 * Vector2.one;

            if (rectTransform.sizeDelta != comboBtnRT.sizeDelta)
            {
                comboBtnRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.sizeDelta.x);
                comboBtnRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.sizeDelta.y);
                comboBtnText.rectTransform.offsetMax = new Vector2(4, 0);

                scrollPanelRT.SetParent(transform, true);
                scrollPanelRT.anchoredPosition = new Vector2(0, -comboBtnRT.sizeDelta.y);


                //make hte overlay fill the whole screen
                overlayRT.SetParent(_canvas.transform, false);
                overlayRT.offsetMax = Vector2.zero;
                overlayRT.offsetMin = Vector2.zero;

                //reattach to correct parents, maintining global position
                overlayRT.SetParent(transform, true);
                scrollPanelRT.SetParent(overlayRT, true);

                scrollPanel.GetComponent<ScrollRect>().scrollSensitivity = comboBtnRT.sizeDelta.y;

                UpdateComboBoxText(comboBtnRT, Items[SelectedIndex].Image != null);
                Refresh();
            }
        }

        /// <summary>
        /// toggle the drop down list
        /// </summary>
        /// <param name="directClick">whether it was toggled by directly clicking on </param>
        public void ToggleComboBox(bool directClick)
        {
            if (HeaderOption != "") HideFirstItem = true;
            _isActive = !_isActive;
            // Debug.Log("toggling combo box tp "+ _isActive);
            overlayRT.gameObject.SetActive(_isActive);
            if (_isActive)
            {
                transform.SetAsLastSibling();
                FixScrollOffset();
            }
            else if (directClick)
            {
                scrollOffset = Mathf.RoundToInt(_itemsRT.anchoredPosition.y / rectTransform.sizeDelta.y);
            }
        }


        #endregion

        #region private methods

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            Initialize();
        }

        /// <summary>
        /// Initialize the control
        /// </summary>
        private void Initialize()
        {
            overlayRT.gameObject.SetActive(false);
            overlayBtn.onClick.AddListener(() => { ToggleComboBox(false); });

            if (HeaderOption != "") AddItems(HeaderOption);

            //float dropdownHeight = comboBtnRT.sizeDelta.y * Mathf.Min(ItemsToDisplay, Items.Length - (HideFirstItem ? 1 : 0));

            //scrollPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
            //scrollPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, comboBtnRT.sizeDelta.x);

            ScrollRect scrollPanelScrollRect = scrollPanel.GetComponent<ScrollRect>();
            scrollPanelScrollRect.scrollSensitivity = comboBtnRT.sizeDelta.y;
            scrollPanelScrollRect.content = itemsRT;

            itemLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            itemLayout.constraintCount = 1;

            //float scrollbarWidth = Items.Length - (HideFirstItem ? 1 : 0) > _itemsToDisplay ? _scrollbarWidth : 0.0f;
            //itemsRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollPanelRT.sizeDelta.x - scrollbarWidth);

            //itemLayout.cellSize = new Vector2(comboBtnRT.sizeDelta.x - scrollbarWidth, comboBtnRT.sizeDelta.y);
            //itemLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            //itemLayout.constraintCount = 1;

            //scrollbarRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollbarWidth);
            //scrollbarRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);

            //slidingAreaRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            //slidingAreaRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight - scrollbarRT.sizeDelta.x);

            //scrollHandleRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollbarWidth);
            //scrollHandleRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scrollbarWidth);

            Interactable = Interactable; //call the logic in the getter.

            Refresh();
        }


        /// <summary>
        /// Redraw the component, with realigning.
        /// </summary>
        public void Refresh()
        {
            //        Debug.Log("Refreshing");

            int itemsLength = Items.Count - (HideFirstItem ? 1 : 0);
            if (itemsLength < 1)
                return;

            float dropdownHeight = comboBtnRT.sizeDelta.y * Mathf.Min(_itemsToDisplay, itemsLength);
            float scrollbarWidth = itemsLength > ItemsToDisplay ? _scrollbarWidth : 0.0f;


            scrollPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
            scrollPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, comboBtnRT.sizeDelta.x);

            itemsRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollPanelRT.sizeDelta.x - scrollbarWidth);

            itemLayout.cellSize = new Vector2(comboBtnRT.sizeDelta.x - scrollbarWidth, comboBtnRT.sizeDelta.y);

            scrollbarRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollbarWidth);
            scrollbarRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);

            slidingAreaRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            slidingAreaRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight - scrollbarRT.sizeDelta.x);

            scrollHandleRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollbarWidth);
            scrollHandleRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scrollbarWidth);

            for (int i = itemsRT.childCount - 1; i >= 0; i--)//delete in reverse to avoid having to re-allocate memory on each delete. (ie if I deleted child 0, everythign would get shifted forward in the array)
            {
                DestroyImmediate(itemsRT.GetChild(0).gameObject);
            }

            for (int i = (HideFirstItem ? 1 : 0); i < Items.Count; i++) //for each element to be shown in the dropdown list
            {
                ComboBoxItem item = Items[i];
                item.OnUpdate = Refresh;

                Transform itemTfm = Instantiate(comboBtnRT) as Transform;//copy the top level combo box 
                itemTfm.name += " " + i;
                itemTfm.SetParent(itemsRT, false);
                itemTfm.GetComponent<Image>().sprite = null; //hide the original background image (so that the dropdown box shows)

                Text itemText = itemTfm.Find("Text").GetComponent<Text>();
                itemText.text = item.Caption;
                if (item.IsDisabled) itemText.color = disabledTextColor;

                Image itemImg = itemTfm.Find("Image").GetComponent<Image>();
                itemImg.sprite = item.Image;
                itemImg.color = (item.Image == null) ? new Color(1, 1, 1, 0)
                                                   : item.IsDisabled ? new Color(1, 1, 1, .5f)
                                                                     : Color.white;
                Button itemBtn = itemTfm.GetComponent<Button>();
                itemBtn.interactable = !item.IsDisabled;

                int indx = i;
                itemBtn.onClick.RemoveAllListeners();
                itemBtn.onClick.AddListener(() =>
                {
                    OnItemClicked(indx);
                    if (item.OnSelect != null) item.OnSelect();
                }
                );

            }

            RefreshSelected();
            UpdateComboBoxItems();
            UpdateGraphics();
            FixScrollOffset();


        }

        /// <summary>
        /// adjusts all of the items in the dropdown list to account for any images
        /// </summary>
        private void UpdateComboBoxItems()
        {
            //decide if any item in the list has images
            bool includeImages = false;
            foreach (ComboBoxItem item in Items)
            {
                if (item.Image != null)
                {
                    includeImages = true; break;
                }
            }

            //either align all of the text 10 units from the side, or 8+image width.
            foreach (Transform child in itemsRT)
            {
                UpdateComboBoxText(child, includeImages);
            }
        }


        private void UpdateComboBoxText(Transform child, bool includeImages)
        {
            child.Find("Text").GetComponent<RectTransform>().offsetMin = Vector2.right * (includeImages ? comboBtnImg.rectTransform.rect.width + 8.0f : 10.0f);
        }


        private void FixScrollOffset()
        {
            int selectedIndex = SelectedIndex + (HideFirstItem ? 1 : 0);
            if (selectedIndex < scrollOffset)
                scrollOffset = selectedIndex;
            else
                if (selectedIndex > scrollOffset + ItemsToDisplay - 1)
                    scrollOffset = selectedIndex - ItemsToDisplay + 1;


            int itemsCount = Items.Count - (HideFirstItem ? 1 : 0);
            if (scrollOffset > itemsCount - ItemsToDisplay)
                scrollOffset = itemsCount - ItemsToDisplay;
            if (scrollOffset < 0)
                scrollOffset = 0;

            _itemsRT.anchoredPosition = new Vector2(0.0f, scrollOffset * rectTransform.sizeDelta.y);
        }

        #endregion

    }
}
