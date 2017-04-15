///Credit perchik
///Sourced from - http://forum.unity3d.com/threads/receive-onclick-event-and-pass-it-on-to-lower-ui-elements.293642/

using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Extensions/AutoComplete ComboBox")]
    public class AutoCompleteComboBox : MonoBehaviour
    {
        public Color disabledTextColor;
        public DropDownListItem SelectedItem { get; private set; } //outside world gets to get this, not set it

        public List<string> AvailableOptions;

        //private bool isInitialized = false;
        private bool _isPanelActive = false;
        private bool _hasDrawnOnce = false;

        private InputField _mainInput;
        private RectTransform _inputRT;

		private Button _arrow_Button;

        private RectTransform _rectTransform;

        private RectTransform _overlayRT;
        private RectTransform _scrollPanelRT;
        private RectTransform _scrollBarRT;
        private RectTransform _slidingAreaRT;
        //   private RectTransform scrollHandleRT;
        private RectTransform _itemsPanelRT;
        private Canvas _canvas;
        private RectTransform _canvasRT;

        private ScrollRect _scrollRect;

        private List<string> _panelItems; //items that will get shown in the dropdown
        private List<string> _prunedPanelItems; //items that used to show in the dropdown

        private Dictionary<string, GameObject> panelObjects;
        
        private GameObject itemTemplate;

        public string Text { get; private set; }

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

        //    private int scrollOffset; //offset of the selected item
        //    private int _selectedIndex = 0;

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

		public bool interactible 
		{
			get { return _mainInput.interactable || _arrow_Button.interactable;	}
			private set {
				_mainInput.interactable = value;
				_arrow_Button.interactable = value;
				if (!value && _isPanelActive) {
					ToggleDropdownPanel (false);
				}
			}
		}

		[SerializeField]
		//I couldn't come up with a better name
		private bool _technicallyInteractible = true;
		public bool TechnicallyInteractible
		{ 
			get { return _technicallyInteractible; }
			set 
			{
				_technicallyInteractible = value;
				interactible = _technicallyInteractible && (AvailableOptions.Count > 0 || _remainInteractableIfEmpty);
			}
		}

		[SerializeField]
		private bool _remainInteractableIfEmpty = true;
		public bool RemainInteractableIfEmpty
		{ 
			get { return _remainInteractableIfEmpty; }
			set 
			{
				_remainInteractableIfEmpty = value;
				interactible = _technicallyInteractible && (AvailableOptions.Count > 0 || _remainInteractableIfEmpty);
			}
		}

		public bool SelectFirstItemOnStart = false;

		[SerializeField]
		private bool _ChangeInputTextColorBasedOnMatchingItems = false;
		public bool ChangeInputTextColorBasedOnMatchingItems{
			get { return _remainInteractableIfEmpty; }
			set 
			{
				_ChangeInputTextColorBasedOnMatchingItems = value;
				if (_ChangeInputTextColorBasedOnMatchingItems) {
					SetInputTextColor ();
				}
			}
		}

		//TODO design as foldout for Inspector
		public Color ValidSelectionTextColor = Color.green;
		public Color MatchingItemsRemainingTextColor = Color.black;
		public Color NoItemsRemainingTextColor = Color.red;

		private bool _selectionIsValid = false;

		[System.Serializable]
		public class SelectionChangedEvent :  UnityEngine.Events.UnityEvent<string, bool> {
		}

		[System.Serializable]
		public class SelectinTextChangedEvent :  UnityEngine.Events.UnityEvent<string> {
		}

		[System.Serializable]
		public class SelectionValidityChangedEvent :  UnityEngine.Events.UnityEvent<bool> {
		}

		// fires when input text is changed;
		public SelectinTextChangedEvent OnSelectinTextChanged;
		// fires when when an Item gets selected / deselected (including when items are added/removed once this is possible)
		public SelectionValidityChangedEvent OnSelectionValidityChanged;
		// fires in both cases
		public SelectionChangedEvent OnSelectionChanged;



        public void Awake()
        {
            Initialize();
        }
		public void Start()
		{
			if (SelectFirstItemOnStart && AvailableOptions.Count > 0) {
				ToggleDropdownPanel (false);
				OnItemClicked (AvailableOptions [0]);
			}
		}

        private bool Initialize()
        {
            bool success = true;
            try
            {
                _rectTransform = GetComponent<RectTransform>();
                _inputRT = _rectTransform.FindChild("InputField").GetComponent<RectTransform>();
                _mainInput = _inputRT.GetComponent<InputField>();

				_arrow_Button = _rectTransform.FindChild ("ArrowBtn").GetComponent<Button> ();

                _overlayRT = _rectTransform.FindChild("Overlay").GetComponent<RectTransform>();
                _overlayRT.gameObject.SetActive(false);


                _scrollPanelRT = _overlayRT.FindChild("ScrollPanel").GetComponent<RectTransform>();
                _scrollBarRT = _scrollPanelRT.FindChild("Scrollbar").GetComponent<RectTransform>();
                _slidingAreaRT = _scrollBarRT.FindChild("SlidingArea").GetComponent<RectTransform>();
                //  scrollHandleRT = slidingAreaRT.FindChild("Handle").GetComponent<RectTransform>();
                _itemsPanelRT = _scrollPanelRT.FindChild("Items").GetComponent<RectTransform>();
                //itemPanelLayout = itemsPanelRT.gameObject.GetComponent<LayoutGroup>();

                _canvas = GetComponentInParent<Canvas>();
                _canvasRT = _canvas.GetComponent<RectTransform>();

                _scrollRect = _scrollPanelRT.GetComponent<ScrollRect>();
                _scrollRect.scrollSensitivity = _rectTransform.sizeDelta.y / 2;
                _scrollRect.movementType = ScrollRect.MovementType.Clamped;
                _scrollRect.content = _itemsPanelRT;

                itemTemplate = _rectTransform.FindChild("ItemTemplate").gameObject;
                itemTemplate.SetActive(false);
            }
            catch (System.NullReferenceException ex)
            {
                Debug.LogException(ex);
                Debug.LogError("Something is setup incorrectly with the dropdownlist component causing a Null Refernece Exception");
                success = false;
            }
            panelObjects = new Dictionary<string, GameObject>();

            _prunedPanelItems = new List<string>();
            _panelItems = AvailableOptions.ToList();

            RebuildPanel();
            //RedrawPanel(); - causes an initialisation failure in U5
            return success;
        }

        /* currently just using items in the list instead of being able to add to it.
        public void AddItems(params object[] list)
        {
            List<DropDownListItem> ddItems = new List<DropDownListItem>();
            foreach (var obj in list)
            {
                if (obj is DropDownListItem)
                {
                    ddItems.Add((DropDownListItem)obj);
                }
                else if (obj is string)
                {
                    ddItems.Add(new DropDownListItem(caption: (string)obj));
                }
                else if (obj is Sprite)
                {
                    ddItems.Add(new DropDownListItem(image: (Sprite)obj));
                }
                else
                {
                    throw new System.Exception("Only ComboBoxItems, Strings, and Sprite types are allowed");
                }
            }
            Items.AddRange(ddItems);
            Items = Items.Distinct().ToList();//remove any duplicates
            RebuildPanel();
        }
        */

        /// <summary>
        /// Rebuilds the contents of the panel in response to items being added.
        /// </summary>
        private void RebuildPanel()
        {
            //panel starts with all options
            _panelItems.Clear();
            foreach (string option in AvailableOptions)
            {
                _panelItems.Add(option.ToLower());
            }
            _panelItems.Sort();

            _prunedPanelItems.Clear();
            List<GameObject> itemObjs = new List<GameObject>(panelObjects.Values);
            panelObjects.Clear();

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
                    itemObjs[i].transform.FindChild("Text").GetComponent<Text>().text = _panelItems[i]; //set the text value

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
			interactible = _technicallyInteractible && (AvailableOptions.Count > 0 || _remainInteractableIfEmpty);
			SetInputTextColor ();
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

        //private void UpdateSelected()
        //{
        //    SelectedItem = (_selectedIndex > -1 && _selectedIndex < Items.Count) ? Items[_selectedIndex] : null;
        //    if (SelectedItem == null) return;

        //    bool hasImage = SelectedItem.Image != null;
        //    if (hasImage)
        //    {
        //        mainButton.img.sprite = SelectedItem.Image;
        //        mainButton.img.color = Color.white;

        //        //if (Interactable) mainButton.img.color = Color.white;
        //        //else mainButton.img.color = new Color(1, 1, 1, .5f);
        //    }
        //    else
        //    {
        //        mainButton.img.sprite = null;
        //    }

        //    mainButton.txt.text = SelectedItem.Caption;

        //    //update selected index color
        //    for (int i = 0; i < itemsPanelRT.childCount; i++)
        //    {
        //        panelItems[i].btnImg.color = (_selectedIndex == i) ? mainButton.btn.colors.highlightedColor : new Color(0, 0, 0, 0);
        //    }
        //}


        private void RedrawPanel()
        {
            float scrollbarWidth = _panelItems.Count > ItemsToDisplay ? _scrollBarWidth : 0f;//hide the scrollbar if there's not enough items
            _scrollBarRT.gameObject.SetActive(_panelItems.Count > ItemsToDisplay);
            if (!_hasDrawnOnce || _rectTransform.sizeDelta != _inputRT.sizeDelta)
            {
                _hasDrawnOnce = true;
                _inputRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _rectTransform.sizeDelta.x);
                _inputRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _rectTransform.sizeDelta.y);

                _scrollPanelRT.SetParent(transform, true);//break the scroll panel from the overlay
                _scrollPanelRT.anchoredPosition = new Vector2(0, -_rectTransform.sizeDelta.y); //anchor it to the bottom of the button

                //make the overlay fill the screen
                _overlayRT.SetParent(_canvas.transform, false); //attach it to top level object
                _overlayRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _canvasRT.sizeDelta.x);
                _overlayRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _canvasRT.sizeDelta.y);

                _overlayRT.SetParent(transform, true);//reattach to this object
                _scrollPanelRT.SetParent(_overlayRT, true); //reattach the scrollpanel to the overlay
            }

            if (_panelItems.Count < 1) return;

            float dropdownHeight = _rectTransform.sizeDelta.y * Mathf.Min(_itemsToDisplay, _panelItems.Count);

            _scrollPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
            _scrollPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _rectTransform.sizeDelta.x);

            _itemsPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _scrollPanelRT.sizeDelta.x - scrollbarWidth - 5);
            _itemsPanelRT.anchoredPosition = new Vector2(5, 0);

            _scrollBarRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollbarWidth);
            _scrollBarRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);

            _slidingAreaRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            _slidingAreaRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight - _scrollBarRT.sizeDelta.x);
        }

        public void OnValueChanged(string currText)
        {
            Text = currText;
            PruneItems(currText);
            RedrawPanel();
            //Debug.Log("value changed to: " + currText);

            if (_panelItems.Count == 0)
            {
                _isPanelActive = true;//this makes it get turned off
                ToggleDropdownPanel(false);
            }
            else if (!_isPanelActive)
            {
                ToggleDropdownPanel(false);
            }

			bool validity_changed = (_panelItems.Contains (Text) == _selectionIsValid);
			_selectionIsValid = _panelItems.Contains (Text);
			OnSelectionChanged.Invoke (Text, _selectionIsValid);
			OnSelectinTextChanged.Invoke (Text);
			if(validity_changed){
				OnSelectionValidityChanged.Invoke (_selectionIsValid);
			}

			SetInputTextColor ();
        }

		private void SetInputTextColor(){
			if (ChangeInputTextColorBasedOnMatchingItems) {
				if (_selectionIsValid) {
					_mainInput.textComponent.color = ValidSelectionTextColor;
				} else if (_panelItems.Count > 0) {
					_mainInput.textComponent.color = MatchingItemsRemainingTextColor;
				} else {
					_mainInput.textComponent.color = NoItemsRemainingTextColor;
				}
			}
		}



        /// <summary>
        /// Toggle the drop down list
        /// </summary>
        /// <param name="directClick"> whether an item was directly clicked on</param>
        public void ToggleDropdownPanel(bool directClick)
        {
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

        private void PruneItems(string currText)
        {
            List<string> notToPrune = _panelItems.Where(x => x.ToLower().Contains(currText.ToLower())).ToList();
            List<string> toPrune = _panelItems.Except(notToPrune).ToList();
            foreach (string key in toPrune)
            {
                //            Debug.Log("pruning key " + key);
                panelObjects[key].SetActive(false);
                _panelItems.Remove(key);
                _prunedPanelItems.Add(key);
            }

            List<string> toAddBack = _prunedPanelItems.Where(x => x.ToLower().Contains(currText)).ToList();
            foreach (string key in toAddBack)
            {
                //            Debug.Log("adding back key " + key);
                panelObjects[key].SetActive(true);
                _panelItems.Add(key);
                _prunedPanelItems.Remove(key);
            }
        }
    }
}
