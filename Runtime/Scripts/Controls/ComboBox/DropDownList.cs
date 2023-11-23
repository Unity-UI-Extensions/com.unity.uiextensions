///Credit perchik
///Sourced from - http://forum.unity3d.com/threads/receive-onclick-event-and-pass-it-on-to-lower-ui-elements.293642/

using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	/// <summary>
	///  Extension to the UI class which creates a dropdown list
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("UI/Extensions/ComboBox/Dropdown List")]
	public class DropDownList : MonoBehaviour
	{
		public Color disabledTextColor;
		public DropDownListItem SelectedItem { get; private set; } //outside world gets to get this, not set it

		[Header("Dropdown List Items")]
		public List<DropDownListItem> Items;

		[Header("Properties")]

		[SerializeField]
		private bool isActive = true;

		public bool OverrideHighlighted = true;

		//private bool isInitialized = false;
		private bool _isPanelActive = false;
		private bool _hasDrawnOnce = false;

		private DropDownListButton _mainButton;

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

		private List<DropDownListButton> _panelItems = new List<DropDownListButton>();

		private GameObject _itemTemplate;
		private bool _initialized;

		private string _defaultMainButtonCaption = null;
		private Color _defaultNormalColor;

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

		private int _selectedIndex = -1;

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
		private float dropdownOffset;

		[SerializeField]
		private bool _displayPanelAbove = false;

		public bool SelectFirstItemOnStart = false;

		[SerializeField]
		private int selectItemIndexOnStart = 0;
		private bool shouldSelectItemOnStart => SelectFirstItemOnStart || selectItemIndexOnStart > 0;

		[System.Serializable]
		public class SelectionChangedEvent : Events.UnityEvent<int> { }

		// fires when item is changed;
		[Header("Events")]
		public SelectionChangedEvent OnSelectionChanged;

		[System.Serializable]
		public class ControlDisabledEvent : Events.UnityEvent<bool> { }

		// fires when item changes between enabled and disabled;
		public ControlDisabledEvent OnControlDisabled;

		public void Start()
		{
			Initialize();
			if (shouldSelectItemOnStart && Items.Count > 0)
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
				_mainButton = new DropDownListButton(_rectTransform.Find("MainButton").gameObject);

				_defaultMainButtonCaption = _mainButton.txt.text;
				_defaultNormalColor = _mainButton.btn.colors.normalColor;

				_overlayRT = _rectTransform.Find("Overlay").GetComponent<RectTransform>();
				_overlayRT.gameObject.SetActive(false);
				_scrollPanelRT = _overlayRT.Find("ScrollPanel").GetComponent<RectTransform>();
				_scrollBarRT = _scrollPanelRT.Find("Scrollbar").GetComponent<RectTransform>();
				_slidingAreaRT = _scrollBarRT.Find("SlidingArea").GetComponent<RectTransform>();
				_scrollHandleRT = _slidingAreaRT.Find("Handle").GetComponent<RectTransform>();
				_itemsPanelRT = _scrollPanelRT.Find("Items").GetComponent<RectTransform>();
				//itemPanelLayout = itemsPanelRT.gameObject.GetComponent<LayoutGroup>();

				_canvas = GetComponentInParent<Canvas>();
				_canvasRT = _canvas.GetComponent<RectTransform>();

				_scrollRect = _scrollPanelRT.GetComponent<ScrollRect>();
				_scrollRect.scrollSensitivity = _rectTransform.sizeDelta.y / 2;
				_scrollRect.movementType = ScrollRect.MovementType.Clamped;
				_scrollRect.content = _itemsPanelRT;

				_itemTemplate = _rectTransform.Find("ItemTemplate").gameObject;
				_itemTemplate.SetActive(false);
			}
			catch (System.NullReferenceException ex)
			{
				Debug.LogException(ex);
				Debug.LogError("Something is setup incorrectly with the dropdownlist component causing a Null Reference Exception");
				success = false;
			}
			_initialized = true;

			RebuildPanel();
			RedrawPanel();
			return success;
		}

		/// <summary>
		/// Update the drop down selection to a specific index
		/// </summary>
		/// <param name="index"></param>
		public void SelectItemIndex(int index)
		{
			ToggleDropdownPanel();
			OnItemClicked(index);
		}

		// currently just using items in the list instead of being able to add to it.
		/// <summary>
		/// Rebuilds the list from a new collection.
		/// </summary>
		/// <remarks>
		/// NOTE, this will clear all existing items
		/// </remarks>
		/// <param name="list"></param>
		public void RefreshItems(params object[] list)
		{
			Items.Clear();
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
			RebuildPanel();
			RedrawPanel();
		}

		/// <summary>
		/// Adds an additional item to the drop down list (recommended)
		/// </summary>
		/// <param name="item">Item of type DropDownListItem</param>
		public void AddItem(DropDownListItem item)
		{
			Items.Add(item);
			RebuildPanel();
			RedrawPanel();
		}

		/// <summary>
		/// Adds an additional drop down list item using a string name
		/// </summary>
		/// <param name="item">Item of type String</param>
		public void AddItem(string item)
		{
			Items.Add(new DropDownListItem(caption: (string)item));
			RebuildPanel();
			RedrawPanel();
		}

		/// <summary>
		/// Adds an additional drop down list item using a sprite image
		/// </summary>
		/// <param name="item">Item of type UI Sprite</param>
		public void AddItem(Sprite item)
		{
			Items.Add(new DropDownListItem(image: (Sprite)item));
			RebuildPanel();
			RedrawPanel();
		}

		/// <summary>
		/// Removes an item from the drop down list (recommended)
		/// </summary>
		/// <param name="item">Item of type DropDownListItem</param>
		public void RemoveItem(DropDownListItem item)
		{
			Items.Remove(item);
			RebuildPanel();
			RedrawPanel();
		}

		/// <summary>
		/// Removes an item from the drop down list item using a string name
		/// </summary>
		/// <param name="item">Item of type String</param>
		public void RemoveItem(string item)
		{
			Items.Remove(new DropDownListItem(caption: (string)item));
			RebuildPanel();
			RedrawPanel();
		}

		/// <summary>
		/// Removes an item from the drop down list item using a sprite image
		/// </summary>
		/// <param name="item">Item of type UI Sprite</param>
		public void RemoveItem(Sprite item)
		{
			Items.Remove(new DropDownListItem(image: (Sprite)item));
			RebuildPanel();
			RedrawPanel();
		}

		public void ResetDropDown()
		{
			if (!_initialized)
			{
				return;
			}

			_mainButton.txt.text = _defaultMainButtonCaption;
			for (int i = 0; i < _itemsPanelRT.childCount; i++)
			{
				_panelItems[i].btnImg.color = _defaultNormalColor;
			}

			_selectedIndex = -1;
			_initialized = false;
			Initialize();
		}

		public void ResetItems()
		{
			Items.Clear();
			RebuildPanel();
			RedrawPanel();
		}

		/// <summary>
		/// Rebuilds the contents of the panel in response to items being added.
		/// </summary>
		private void RebuildPanel()
		{
			if (Items.Count == 0) return;

			if (!_initialized)
			{
				Start();
			}

			int indx = _panelItems.Count;
			while (_panelItems.Count < Items.Count)
			{
				GameObject newItem = Instantiate(_itemTemplate) as GameObject;
				newItem.name = "Item " + indx;
				newItem.transform.SetParent(_itemsPanelRT, false);

				_panelItems.Add(new DropDownListButton(newItem));
				indx++;
			}
			for (int i = 0; i < _panelItems.Count; i++)
			{
				if (i < Items.Count)
				{
					DropDownListItem item = Items[i];

					_panelItems[i].txt.text = item.Caption;
					if (item.IsDisabled) _panelItems[i].txt.color = disabledTextColor;

					if (_panelItems[i].btnImg != null) _panelItems[i].btnImg.sprite = null;//hide the button image
					_panelItems[i].img.sprite = item.Image;
					_panelItems[i].img.color = (item.Image == null) ? new Color(1, 1, 1, 0)
																	: item.IsDisabled ? new Color(1, 1, 1, .5f)
																					  : Color.white;
					int ii = i; //have to copy the variable for use in anonymous function
					_panelItems[i].btn.onClick.RemoveAllListeners();
					_panelItems[i].btn.onClick.AddListener(() =>
					{
						OnItemClicked(ii);
						if (item.OnSelect != null) item.OnSelect();
					});
				}
				_panelItems[i].gameobject.SetActive(i < Items.Count);// if we have more thanks in the panel than Items in the list hide them
			}
		}

		private void OnItemClicked(int indx)
		{
			//Debug.Log("item " + indx + " clicked");
			if (indx != _selectedIndex && OnSelectionChanged != null) OnSelectionChanged.Invoke(indx);

			_selectedIndex = indx;
			ToggleDropdownPanel();
			UpdateSelected();
		}

		private void UpdateSelected()
		{
			SelectedItem = (_selectedIndex > -1 && _selectedIndex < Items.Count) ? Items[_selectedIndex] : null;
			if (SelectedItem == null) return;

			bool hasImage = SelectedItem.Image != null;
			if (hasImage)
			{
				_mainButton.img.sprite = SelectedItem.Image;
				_mainButton.img.color = Color.white;
			}
			else
			{
				_mainButton.img.sprite = null;
			}

			_mainButton.txt.text = SelectedItem.Caption;

			//update selected index color
			if (OverrideHighlighted)
			{
				for (int i = 0; i < _itemsPanelRT.childCount; i++)
				{
					_panelItems[i].btnImg.color = (_selectedIndex == i) ? _mainButton.btn.colors.highlightedColor : new Color(0, 0, 0, 0);
				}
			}
		}

		private void RedrawPanel()
		{
			float scrollbarWidth = _panelItems.Count > ItemsToDisplay ? _scrollBarWidth : 0f;//hide the scrollbar if there's not enough items
			_scrollBarRT.gameObject.SetActive(_panelItems.Count > ItemsToDisplay);

			float dropdownHeight = _itemsToDisplay > 0 ? _rectTransform.sizeDelta.y * Mathf.Min(_itemsToDisplay, _panelItems.Count) : _rectTransform.sizeDelta.y * _panelItems.Count;
			dropdownHeight += dropdownOffset;

			if (!_hasDrawnOnce || _rectTransform.sizeDelta != _mainButton.rectTransform.sizeDelta)
			{
				_hasDrawnOnce = true;
				_mainButton.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _rectTransform.sizeDelta.x);
				_mainButton.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _rectTransform.sizeDelta.y);

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

		/// <summary>
		/// Toggle the drop down list if it is active
		/// </summary>
		/// <param name="directClick">Retained for backwards compatibility only.</param>
		[Obsolete("DirectClick Parameter is no longer required")]
		public void ToggleDropdownPanel(bool directClick = false)
		{
			ToggleDropdownPanel();
		}		

		/// <summary>
		/// Toggle the drop down list if it is active
		/// </summary>
		public void ToggleDropdownPanel()
		{
			if (!isActive)
			{
				return;
			}

			_overlayRT.transform.localScale = new Vector3(1, 1, 1);
			_scrollBarRT.transform.localScale = new Vector3(1, 1, 1);
			_isPanelActive = !_isPanelActive;
			_overlayRT.gameObject.SetActive(_isPanelActive);

			if (_isPanelActive)
			{
				transform.SetAsLastSibling();
			}
		}

		/// <summary>
		/// Hides the drop down panel if its visible at the moment
		/// </summary>
		public void HideDropDownPanel()
		{
			if (!_isPanelActive)
			{
				return;
			}
			
			ToggleDropdownPanel();
		}

		/// <summary>
		/// Updates the control and sets its active status, determines whether the dropdown will open ot not
		/// and takes care of the underlying button to follow the status.
		/// </summary>
		/// <param name="status"></param>
		public void SetActive(bool status)
		{
			if (status == isActive)
			{
				return;
			}
			isActive = status;
			OnControlDisabled?.Invoke(isActive);
			_mainButton.btn.enabled = isActive;
		}
	}
}