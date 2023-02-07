///Credit perchik
///Sourced from - http://forum.unity3d.com/threads/receive-onclick-event-and-pass-it-on-to-lower-ui-elements.293642/

using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UI.Extensions
{
    public enum AutoCompleteSearchType
    {
        ArraySort,
        Linq
    }

    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Extensions/ComboBox/AutoComplete ComboBox")]
    public class AutoCompleteComboBox : MonoBehaviour
    {
        public DropDownListItem SelectedItem { get; private set; } //outside world gets to get this, not set it

        /// <summary>
        /// Contains the included items. To add and remove items to/from this list, use the <see cref="AddItem(string)"/>,
        /// <see cref="RemoveItem(string)"/> and <see cref="SetAvailableOptions(List{string})"/> methods as these also execute
        /// the required methods to update to the current collection.
        /// </summary>
        [Header("AutoComplete Box Items")]
        public List<string> AvailableOptions;

        private bool _isPanelActive = false;
        private bool _hasDrawnOnce = false;

        private InputField _mainInput;
        private RectTransform _inputRT;

        private RectTransform _rectTransform;

        private RectTransform _overlayRT;
        private RectTransform _scrollPanelRT;
        private RectTransform _scrollBarRT;
        private RectTransform _slidingAreaRT;
        private RectTransform _scrollHandleRT;
        private RectTransform _itemsPanelRT;
        private Canvas _canvas;
        private RectTransform _canvasRT;

        private ScrollRect _scrollRect;

        private List<string> _panelItems; //items that will get shown in the drop-down
        private List<string> _prunedPanelItems; //items that used to show in the drop-down

        private Dictionary<string, GameObject> panelObjects;

        private GameObject itemTemplate;
        private bool _initialized;

        public string Text { get; private set; }

        [Header("Properties")]
        [SerializeField]
        private bool isActive = true;

        [SerializeField]
        private float _scrollBarWidth = 20.0f;
        public float ScrollBarWidth
        {
            get { return _scrollBarWidth; }
            set
            {
                _scrollBarWidth = value;
                RedrawPanel();
            }
        }

        [SerializeField]
        private int _itemsToDisplay;
        public int ItemsToDisplay
        {
            get { return _itemsToDisplay; }
            set
            {
                _itemsToDisplay = value;
                RedrawPanel();
            }
        }

        [SerializeField]
        [Tooltip("Change input text color based on matching items")]
        private bool _ChangeInputTextColorBasedOnMatchingItems = false;
        public bool InputColorMatching
        {
            get { return _ChangeInputTextColorBasedOnMatchingItems; }
            set
            {
                _ChangeInputTextColorBasedOnMatchingItems = value;
                if (_ChangeInputTextColorBasedOnMatchingItems)
                {
                    SetInputTextColor();
                }
            }
        }

        public float DropdownOffset = 10f;

        public Color ValidSelectionTextColor = Color.green;
        public Color MatchingItemsRemainingTextColor = Color.black;
        public Color NoItemsRemainingTextColor = Color.red;

        public AutoCompleteSearchType autocompleteSearchType = AutoCompleteSearchType.Linq;

        [SerializeField]
        private float dropdownOffset;

        [SerializeField]
        private bool _displayPanelAbove = false;

        public bool SelectFirstItemOnStart = false;

        [SerializeField]
        private int selectItemIndexOnStart = 0;

        private bool shouldSelectItemOnStart => SelectFirstItemOnStart || selectItemIndexOnStart > 0;

        private bool _selectionIsValid = false;

        [System.Serializable]
        public class SelectionChangedEvent : Events.UnityEvent<string, bool> { }

        [System.Serializable]
        public class SelectionTextChangedEvent : Events.UnityEvent<string> { }

        [System.Serializable]
        public class SelectionValidityChangedEvent : Events.UnityEvent<bool> { }

        [System.Serializable]
        public class ControlDisabledEvent : Events.UnityEvent<bool> { }

        // fires when input text is changed;
        [Header("Events")]
        public SelectionTextChangedEvent OnSelectionTextChanged;
        // fires when an Item gets selected / deselected (including when items are added/removed once this is possible)
        public SelectionValidityChangedEvent OnSelectionValidityChanged;
        // fires in both cases
        public SelectionChangedEvent OnSelectionChanged;
        // fires when item is changed;
        public ControlDisabledEvent OnControlDisabled;

        public void Awake()
        {
            Initialize();
        }

        public void Start()
        {
            if (shouldSelectItemOnStart && AvailableOptions.Count > 0)
            {
                SelectItemIndex(SelectFirstItemOnStart ? 0 : selectItemIndexOnStart);
            }
            RedrawPanel();
        }

        private bool Initialize()
        {
            if (_initialized) return true;

            bool success = true;
            try
            {
                _rectTransform = GetComponent<RectTransform>();
                _inputRT = _rectTransform.Find("InputField").GetComponent<RectTransform>();
                _mainInput = _inputRT.GetComponent<InputField>();

                _overlayRT = _rectTransform.Find("Overlay").GetComponent<RectTransform>();
                _overlayRT.gameObject.SetActive(false);


                _scrollPanelRT = _overlayRT.Find("ScrollPanel").GetComponent<RectTransform>();
                _scrollBarRT = _scrollPanelRT.Find("Scrollbar").GetComponent<RectTransform>();
                _slidingAreaRT = _scrollBarRT.Find("SlidingArea").GetComponent<RectTransform>();
                _scrollHandleRT = _slidingAreaRT.Find("Handle").GetComponent<RectTransform>();
                _itemsPanelRT = _scrollPanelRT.Find("Items").GetComponent<RectTransform>();

                _canvas = GetComponentInParent<Canvas>();
                _canvasRT = _canvas.GetComponent<RectTransform>();

                _scrollRect = _scrollPanelRT.GetComponent<ScrollRect>();
                _scrollRect.scrollSensitivity = _rectTransform.sizeDelta.y / 2;
                _scrollRect.movementType = ScrollRect.MovementType.Clamped;
                _scrollRect.content = _itemsPanelRT;

                itemTemplate = _rectTransform.Find("ItemTemplate").gameObject;
                itemTemplate.SetActive(false);
            }
            catch (System.NullReferenceException ex)
            {
                Debug.LogException(ex);
                Debug.LogError("Something is setup incorrectly with the dropdownlist component causing a Null Reference Exception");
                success = false;
            }
            panelObjects = new Dictionary<string, GameObject>();

            _prunedPanelItems = new List<string>();
            _panelItems = new List<string>();

            _initialized = true;

            RebuildPanel();
            return success;
        }

        /// <summary>
        /// Adds the item to <see cref="this.AvailableOptions"/> if it is not a duplicate and rebuilds the panel.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public void AddItem(string item)
        {
            if (!this.AvailableOptions.Contains(item))
            {
                this.AvailableOptions.Add(item);
                this.RebuildPanel();
            }
            else
            {
                Debug.LogWarning($"{nameof(AutoCompleteComboBox)}.{nameof(AddItem)}: items may only exists once. '{item}' can not be added.");
            }
        }

        /// <summary>
        /// Removes the item from <see cref="this.AvailableOptions"/> and rebuilds the panel.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        public void RemoveItem(string item)
        {
            if (this.AvailableOptions.Contains(item))
            {
                this.AvailableOptions.Remove(item);
                this.RebuildPanel();
            }
        }


        /// <summary>
        /// Update the drop down selection to a specific index
        /// </summary>
        /// <param name="index"></param>
        public void SelectItemIndex(int index)
        {
            ToggleDropdownPanel(false);
            OnItemClicked(AvailableOptions[index]);
        }

        /// <summary>
        /// Sets the given items as new content for the comboBox. Previous entries will be cleared.
        /// </summary>
        /// <param name="newOptions">New entries.</param>
        public void SetAvailableOptions(List<string> newOptions)
        {
            var uniqueOptions = newOptions.Distinct().ToArray();
            SetAvailableOptions(uniqueOptions);
        }

        /// <summary>
        /// Sets the given items as new content for the comboBox. Previous entries will be cleared.
        /// </summary>
        /// <param name="newOptions">New entries.</param>
        public void SetAvailableOptions(string[] newOptions)
        {
            var uniqueOptions = newOptions.Distinct().ToList();
            if (newOptions.Length != uniqueOptions.Count)
            {
                Debug.LogWarning($"{nameof(AutoCompleteComboBox)}.{nameof(SetAvailableOptions)}: items may only exists once. {newOptions.Length - uniqueOptions.Count} duplicates.");
            }

            this.AvailableOptions.Clear();

            for (int i = 0; i < newOptions.Length; i++)
            {
                this.AvailableOptions.Add(newOptions[i]);
            }

            this.RebuildPanel();
            this.RedrawPanel();
        }

        public void ResetItems()
        {
            AvailableOptions.Clear();
            RebuildPanel();
            RedrawPanel();
        }

        /// <summary>
        /// Rebuilds the contents of the panel in response to items being added.
        /// </summary>
        private void RebuildPanel()
        {
            if (!_initialized)
            {
                Start();
            }

            if (_isPanelActive) ToggleDropdownPanel();

            //panel starts with all options
            _panelItems.Clear();
            _prunedPanelItems.Clear();
            panelObjects.Clear();

            //clear Autocomplete children in scene
            foreach (Transform child in _itemsPanelRT.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (string option in AvailableOptions)
            {
                _panelItems.Add(option.ToLower());
            }

            List<GameObject> itemObjs = new List<GameObject>(panelObjects.Values);

            int indx = 0;
            while (itemObjs.Count < AvailableOptions.Count)
            {
                GameObject newItem = Instantiate(itemTemplate) as GameObject;
                newItem.name = "Item " + indx;
                newItem.transform.SetParent(_itemsPanelRT, false);
                itemObjs.Add(newItem);
                indx++;
            }

            for (int i = 0; i < itemObjs.Count; i++)
            {
                itemObjs[i].SetActive(i <= AvailableOptions.Count);
                if (i < AvailableOptions.Count)
                {
                    itemObjs[i].name = "Item " + i + " " + _panelItems[i];
#if UNITY_2022_1_OR_NEWER
                    itemObjs[i].transform.Find("Text").GetComponent<TMPro.TMP_Text>().text = AvailableOptions[i]; //set the text value
#else
                    itemObjs[i].transform.Find("Text").GetComponent<Text>().text = AvailableOptions[i]; //set the text value
#endif
                    Button itemBtn = itemObjs[i].GetComponent<Button>();
                    itemBtn.onClick.RemoveAllListeners();
                    string textOfItem = _panelItems[i]; //has to be copied for anonymous function or it gets garbage collected away
                    itemBtn.onClick.AddListener(() =>
                    {
                        OnItemClicked(textOfItem);
                    });
                    panelObjects[_panelItems[i]] = itemObjs[i];
                }
            }
            SetInputTextColor();
        }

        /// <summary>
        /// what happens when an item in the list is selected
        /// </summary>
        /// <param name="item"></param>
        private void OnItemClicked(string item)
        {
            //Debug.Log("item " + item + " clicked");
            Text = item;
            _mainInput.text = Text;
            ToggleDropdownPanel(true);
        }

        private void RedrawPanel()
        {
            float scrollbarWidth = _panelItems.Count > ItemsToDisplay ? _scrollBarWidth : 0f;//hide the scrollbar if there's not enough items
            _scrollBarRT.gameObject.SetActive(_panelItems.Count > ItemsToDisplay);

            float dropdownHeight = _itemsToDisplay > 0 ? _rectTransform.sizeDelta.y * Mathf.Min(_itemsToDisplay, _panelItems.Count) : _rectTransform.sizeDelta.y * _panelItems.Count;
            dropdownHeight += dropdownOffset;

            if (!_hasDrawnOnce || _rectTransform.sizeDelta != _inputRT.sizeDelta)
            {
                _hasDrawnOnce = true;
                _inputRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _rectTransform.sizeDelta.x);
                _inputRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _rectTransform.sizeDelta.y);

                var itemsRemaining = _panelItems.Count - ItemsToDisplay;
                itemsRemaining = itemsRemaining < 0 ? 0 : itemsRemaining;

                _scrollPanelRT.SetParent(transform, true);
                _scrollPanelRT.anchoredPosition = _displayPanelAbove ?
                    new Vector2(0, dropdownOffset + dropdownHeight) :
                    new Vector2(0, -(dropdownOffset + _rectTransform.sizeDelta.y));

                //make the overlay fill the screen
                _overlayRT.SetParent(_canvas.transform, false);
                _overlayRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _canvasRT.sizeDelta.x);
                _overlayRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _canvasRT.sizeDelta.y);

                _overlayRT.SetParent(transform, true);
                _scrollPanelRT.SetParent(_overlayRT, true);
            }

            if (_panelItems.Count < 1) return;

            _scrollPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
            _scrollPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _rectTransform.sizeDelta.x);

            _itemsPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _scrollPanelRT.sizeDelta.x - scrollbarWidth - 5);
            _itemsPanelRT.anchoredPosition = new Vector2(5, 0);

            _scrollBarRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollbarWidth);
            _scrollBarRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
            if (scrollbarWidth == 0) _scrollHandleRT.gameObject.SetActive(false); else _scrollHandleRT.gameObject.SetActive(true);

            _slidingAreaRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            _slidingAreaRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight - _scrollBarRT.sizeDelta.x);
        }

        public void OnValueChanged(string currText)
        {
            Text = currText;
            PruneItems(currText);
            RedrawPanel();

            if (_panelItems.Count == 0)
            {
                _isPanelActive = true;//this makes it get turned off
                ToggleDropdownPanel(false);
            }
            else if (!_isPanelActive)
            {
                ToggleDropdownPanel(false);
            }

            bool validity_changed = (_panelItems.Contains(Text) != _selectionIsValid);
            _selectionIsValid = _panelItems.Contains(Text);
            OnSelectionChanged.Invoke(Text, _selectionIsValid);
            OnSelectionTextChanged.Invoke(Text);
            if (validity_changed)
            {
                OnSelectionValidityChanged.Invoke(_selectionIsValid);
            }

            SetInputTextColor();
        }

        private void SetInputTextColor()
        {
            if (InputColorMatching)
            {
                if (_selectionIsValid)
                {
                    _mainInput.textComponent.color = ValidSelectionTextColor;
                }
                else if (_panelItems.Count > 0)
                {
                    _mainInput.textComponent.color = MatchingItemsRemainingTextColor;
                }
                else
                {
                    _mainInput.textComponent.color = NoItemsRemainingTextColor;
                }
            }
        }

        /// <summary>
        /// Toggle the drop down list
        /// </summary>
        /// <param name="directClick"> whether an item was directly clicked on</param>
        public void ToggleDropdownPanel(bool directClick = false)
        {
            if (!isActive) return;

            _isPanelActive = !_isPanelActive;

            _overlayRT.gameObject.SetActive(_isPanelActive);
            if (_isPanelActive)
            {
                transform.SetAsLastSibling();
            }
            else if (directClick)
            {
                // scrollOffset = Mathf.RoundToInt(itemsPanelRT.anchoredPosition.y / _rectTransform.sizeDelta.y); 
            }
        }


        /// <summary>
        /// Updates the control and sets its active status, determines whether the dropdown will open ot not
        /// </summary>
        /// <param name="status"></param>
        public void SetActive(bool status)
        {
            if (status != isActive)
            {
                OnControlDisabled?.Invoke(status);
            }
            isActive = status;
        }

        private void PruneItems(string currText)
        {
            if (autocompleteSearchType == AutoCompleteSearchType.Linq)
            {
                PruneItemsLinq(currText);
            }
            else
            {
                PruneItemsArray(currText);
            }
        }

        private void PruneItemsLinq(string currText)
        {
            currText = currText.ToLower();
            var toPrune = _panelItems.Where(x => !x.Contains(currText)).ToArray();
            foreach (string key in toPrune)
            {
                panelObjects[key].SetActive(false);
                _panelItems.Remove(key);
                _prunedPanelItems.Add(key);
            }

            var toAddBack = _prunedPanelItems.Where(x => x.Contains(currText)).ToArray();
            foreach (string key in toAddBack)
            {
                panelObjects[key].SetActive(true);
                _panelItems.Add(key);
                _prunedPanelItems.Remove(key);
            }
        }

        //Updated to not use Linq
        private void PruneItemsArray(string currText)
        {
            string _currText = currText.ToLower();

            for (int i = _panelItems.Count - 1; i >= 0; i--)
            {
                string _item = _panelItems[i];
                if (!_item.Contains(_currText))
                {
                    panelObjects[_panelItems[i]].SetActive(false);
                    _panelItems.RemoveAt(i);
                    _prunedPanelItems.Add(_item);
                }
            }
            for (int i = _prunedPanelItems.Count - 1; i >= 0; i--)
            {
                string _item = _prunedPanelItems[i];
                if (_item.Contains(_currText))
                {
                    panelObjects[_prunedPanelItems[i]].SetActive(true);
                    _prunedPanelItems.RemoveAt(i);
                    _panelItems.Add(_item);
                }
            }
        }
    }
}