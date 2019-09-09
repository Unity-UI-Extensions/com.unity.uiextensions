/// Credit drobina, w34edrtfg, playemgames 
/// Sourced from - http://forum.unity3d.com/threads/sprite-icons-with-text-e-g-emoticons.265927/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions {
    // Image according to the label inside the name attribute to load, read from the Resources directory. The size of the image is controlled by the size property.

    // Use: Add Icon name and sprite to the icons list

    [AddComponentMenu("UI/Extensions/TextPic")]
       
    [ExecuteInEditMode] // Needed for culling images that are not used //
    public class TextPic : Text, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler, ISelectHandler {
		// Icon entry to replace text with
        [Serializable]
        public struct IconName {
            public string name;
            public Sprite sprite;
			public Vector2 offset;
			public Vector2 scale;
        }

		// Icons and text to replace
        public IconName[] inspectorIconList;

		[Tooltip("Global scaling factor for all images")]
        public float ImageScalingFactor = 1;

        // Write the name or hex value of the hyperlink color
        public string hyperlinkColor = "blue";

        // Offset image by x, y
        public Vector2 imageOffset = Vector2.zero;
        public bool isCreating_m_HrefInfos = true;

        [Serializable]
        public class HrefClickEvent : UnityEvent<string> { }

        [SerializeField]
        private HrefClickEvent m_OnHrefClick = new HrefClickEvent();

        /// <summary>
        /// Hyperlink Click Event
        /// </summary>
        public HrefClickEvent onHrefClick {
            get { return m_OnHrefClick; }
            set { m_OnHrefClick = value; }
        }

        /// <summary>
        /// Image Pool
        /// </summary>
        private readonly List<Image> m_ImagesPool = new List<Image>();
        private readonly List<GameObject> culled_ImagesPool = new List<GameObject>();

		// Used for check for culling images
        private bool clearImages = false;

		// Lock to ensure images get culled properly
		private Object thisLock = new Object();

        /// <summary>
        /// Vertex Index
        /// </summary>
        private readonly List<int> m_ImagesVertexIndex = new List<int>();

        /// <summary>
        /// Regular expression to replace 
        /// </summary>
        private static readonly Regex s_Regex =
            new Regex(@"<quad name=(.+?) size=(\d*\.?\d+%?) width=(\d*\.?\d+%?) />", RegexOptions.Singleline);

        /// <summary>
        /// Hyperlink Regular Expression
        /// </summary>
        private static readonly Regex s_HrefRegex =
            new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);

		// String to create quads
        private string fixedString;

		// Update the quad images when true
		private bool updateQuad = false;

        /// <summary>
        /// After parsing the final text
        /// </summary>
        private string m_OutputText;

        private Button button;

        // Used for custom selection as a variable for other scripts
        private bool selected = false;

        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        // Positions of images for icon placement
        private List<Vector2> positions = new List<Vector2>();
        
        // Little hack to support multiple hrefs with same name
        private string previousText = "";

        /// <summary>
        /// Hyperlinks Info
        /// </summary>
		[Serializable]
        public class HrefInfo {
            public int startIndex;

            public int endIndex;

            public string name;

            public readonly List<Rect> boxes = new List<Rect>();
        }

        /// <summary>
        /// Hyperlink List
        /// </summary>
        private readonly List<HrefInfo> m_HrefInfos = new List<HrefInfo>();

        /// <summary>
        /// Text Builder
        /// </summary>
        private static readonly StringBuilder s_TextBuilder = new StringBuilder();

		// Matches for quad tags
		private MatchCollection matches;

		// Matches for quad tags
		private MatchCollection href_matches;

		// Matches for removing characters
		private MatchCollection removeCharacters;

		// Index of current pic
		private int picIndex;

		// Index of current pic vertex
		private int vertIndex;

		/// <summary>
		/// Unity 2019.1.5 Fixes to text placement resulting from the removal of verts for spaces and non rendered characters
		/// </summary>

		// There is no directive for incremented versions so will have to hack together
		private bool usesNewRendering = false;

		#if UNITY_2019_1_OR_NEWER
        /// <summary>
        /// Regular expression to remove non rendered characters
        /// </summary>
        private static readonly Regex remove_Regex =
            new Regex(@"<b>|</b>|<i>|</i>|<size=.*?>|</size>|<color=.*?>|</color>|<material=.*?>|</material>|<quad name=(.+?) size=(\d*\.?\d+%?) width=(\d*\.?\d+%?) />|<a href=([^>\n\s]+)>|</a>|\s", RegexOptions.Singleline);

		// List of indexes that are compared against matches to remove quad tags
		List<int> indexes = new List<int>();

		// Characters to remove from string for finding the correct index for vertices for images
		private int charactersRemoved = 0;

		// Characters to remove from string for finding the correct start index for vertices for href bounds
		private int startCharactersRemoved = 0;

		// Characters to remove from string for finding the correct end index for vertices for href bounds
		private int endCharactersRemoved = 0;
		#endif

		// Count of current href
		private int count = 0;

		// Index of current href
		private int indexText = 0;

		// Original text temporary variable holder
		private string originalText;

		// Vertex we are modifying
		private UIVertex vert;

		// Local Point for Href
		private Vector2 lp;


		/// METHODS ///

		public void ResetIconList() {
            Reset_m_HrefInfos ();
			base.Start();
        }

        protected void UpdateQuadImage() {
#if UNITY_EDITOR && !UNITY_2018_3_OR_NEWER
            if (UnityEditor.PrefabUtility.GetPrefabType(this) == UnityEditor.PrefabType.Prefab) {
                return;
            }
#endif
            m_OutputText = GetOutputText();

			matches = s_Regex.Matches(m_OutputText);

			if (matches != null && matches.Count > 0) {
				for (int i = 0; i < matches.Count; i++) {
					m_ImagesPool.RemoveAll(image => image == null);

					if (m_ImagesPool.Count == 0) {
						GetComponentsInChildren<Image>(true, m_ImagesPool);
					}

					if (matches.Count > m_ImagesPool.Count) {
						DefaultControls.Resources resources = new DefaultControls.Resources();
						GameObject go = DefaultControls.CreateImage(resources);

						go.layer = gameObject.layer;

						RectTransform rt = go.transform as RectTransform;

						if (rt) {
							rt.SetParent(rectTransform);
							rt.anchoredPosition3D = Vector3.zero;
							rt.localRotation = Quaternion.identity;
							rt.localScale = Vector3.one;
						}

						m_ImagesPool.Add(go.GetComponent<Image>());
					}

					string spriteName = matches[i].Groups[1].Value;

					Image img = m_ImagesPool[i];

					Vector2 imgoffset = Vector2.zero;

					if (img.sprite == null || img.sprite.name != spriteName) {
						if (inspectorIconList != null && inspectorIconList.Length > 0) {
							for (int s = 0; s < inspectorIconList.Length; s++) {
								if (inspectorIconList[s].name == spriteName) {
									img.sprite = inspectorIconList[s].sprite;
									img.preserveAspect = true;
									img.rectTransform.sizeDelta = new Vector2(fontSize * ImageScalingFactor * inspectorIconList[s].scale.x, 
																				fontSize * ImageScalingFactor * inspectorIconList[s].scale.y);
									imgoffset = inspectorIconList[s].offset;

									break;
								}
							}
						}
					}

					img.enabled = true;

					if (positions.Count > 0 && i < positions.Count) {
						img.rectTransform.anchoredPosition = positions[i] += imgoffset;
					}
				}
			}
			else {
				// If there are no matches, remove the images from the pool
				for (int i = m_ImagesPool.Count - 1; i > 0; i--) {
					if (m_ImagesPool[i]) {
						if (!culled_ImagesPool.Contains(m_ImagesPool[i].gameObject)) {
							culled_ImagesPool.Add(m_ImagesPool[i].gameObject);
							m_ImagesPool.Remove(m_ImagesPool[i]);
						}
					}
				}
			}

			// Remove any images that are not being used
			for (int i = m_ImagesPool.Count - 1; i >= matches.Count; i--) {
				if (i >= 0 && m_ImagesPool.Count > 0) {
					if (m_ImagesPool[i]) {
						if (!culled_ImagesPool.Contains(m_ImagesPool[i].gameObject)) {
							culled_ImagesPool.Add(m_ImagesPool[i].gameObject);
							m_ImagesPool.Remove(m_ImagesPool[i]);
						}
					}
				}
			}

			// Clear the images when it is safe to do so
            if (culled_ImagesPool.Count > 0) {
                clearImages = true;
            }
        }

        // Reseting m_HrefInfos array if there is any change in text
        void Reset_m_HrefInfos () {
            previousText = text;

            m_HrefInfos.Clear();

            isCreating_m_HrefInfos = true;
        }

        /// <summary>
        /// Finally, the output text hyperlinks get parsed
        /// </summary>
        /// <returns></returns>
        protected string GetOutputText() {
            s_TextBuilder.Length = 0;
            
            indexText = 0;

            fixedString = this.text;

            if (inspectorIconList != null && inspectorIconList.Length > 0) {
                for (int i = 0; i < inspectorIconList.Length; i++) {
                    if (!string.IsNullOrEmpty(inspectorIconList[i].name)) {
                        fixedString = fixedString.Replace(inspectorIconList[i].name, 
														"<quad name=" + inspectorIconList[i].name + " size=" + fontSize + " width=1 />");
                    }
                }
            }

            count = 0;

			href_matches = s_HrefRegex.Matches(fixedString);

			if (href_matches != null && href_matches.Count > 0) {
				for (int i = 0; i < href_matches.Count; i++ ) {
					s_TextBuilder.Append(fixedString.Substring(indexText, href_matches[i].Index - indexText));
					s_TextBuilder.Append("<color=" + hyperlinkColor + ">");  // Hyperlink color

					var group = href_matches[i].Groups[1];

					if (isCreating_m_HrefInfos) {
						HrefInfo hrefInfo = new HrefInfo {
							// Hyperlinks in text starting index
							startIndex = (usesNewRendering ? s_TextBuilder.Length : s_TextBuilder.Length * 4), 
							endIndex = (usesNewRendering ? (s_TextBuilder.Length + href_matches[i].Groups[2].Length - 1) : (s_TextBuilder.Length + href_matches[i].Groups[2].Length - 1) * 4 + 3),
							name = group.Value
						};

						m_HrefInfos.Add(hrefInfo);
					}
					else {
						if (count <= m_HrefInfos.Count - 1) {
							// Hyperlinks in text starting index
							m_HrefInfos[count].startIndex = (usesNewRendering ? s_TextBuilder.Length : s_TextBuilder.Length * 4); 
							m_HrefInfos[count].endIndex = (usesNewRendering ? (s_TextBuilder.Length + href_matches[i].Groups[2].Length - 1) : (s_TextBuilder.Length + href_matches[i].Groups[2].Length - 1) * 4 + 3);

							count++;
						}
					}

					s_TextBuilder.Append(href_matches[i].Groups[2].Value);
					s_TextBuilder.Append("</color>");

					indexText = href_matches[i].Index + href_matches[i].Length;
				}
			}

            // we should create array only once or if there is any change in the text
            if (isCreating_m_HrefInfos)
                isCreating_m_HrefInfos = false;
                
            s_TextBuilder.Append(fixedString.Substring(indexText, fixedString.Length - indexText));

			m_OutputText = s_TextBuilder.ToString();

            m_ImagesVertexIndex.Clear();

			matches = s_Regex.Matches(m_OutputText);

			#if UNITY_2019_1_OR_NEWER
			href_matches = s_HrefRegex.Matches(m_OutputText);

			indexes.Clear();

			for (int r = 0; r < matches.Count; r++) { 
				indexes.Add(matches[r].Index);
			}
			#endif


			if (matches != null && matches.Count > 0) {
				for (int i = 0; i < matches.Count; i++) {
					picIndex = matches[i].Index;

					#if UNITY_2019_1_OR_NEWER
					if (usesNewRendering) {
						charactersRemoved = 0;

						removeCharacters = remove_Regex.Matches(m_OutputText);

						for (int r = 0; r < removeCharacters.Count; r++) { 
							if (removeCharacters[r].Index < picIndex && !indexes.Contains(removeCharacters[r].Index)) {
								charactersRemoved += removeCharacters[r].Length;
							}
						}

						for (int r = 0; r < i; r++) { 
							charactersRemoved += (matches[r].Length - 1);
						}

						picIndex -= charactersRemoved;
					}
					#endif

					vertIndex = picIndex * 4 + 3;

					m_ImagesVertexIndex.Add(vertIndex);
				}
			}

			#if UNITY_2019_1_OR_NEWER
			if (usesNewRendering) {
				if (m_HrefInfos != null && m_HrefInfos.Count > 0) {
					for (int i = 0; i < m_HrefInfos.Count; i++) {
						startCharactersRemoved = 0;
						endCharactersRemoved = 0;

						removeCharacters = remove_Regex.Matches(m_OutputText);

						for (int r = 0; r < removeCharacters.Count; r++) { 
							if (removeCharacters[r].Index < m_HrefInfos[i].startIndex && !indexes.Contains(removeCharacters[r].Index)) {
								startCharactersRemoved += removeCharacters[r].Length;
							}
							else if (removeCharacters[r].Index < m_HrefInfos[i].startIndex && indexes.Contains(removeCharacters[r].Index)) {
								startCharactersRemoved += removeCharacters[r].Length - 1;
							}

							if (removeCharacters[r].Index < m_HrefInfos[i].endIndex && !indexes.Contains(removeCharacters[r].Index)) {
								endCharactersRemoved += removeCharacters[r].Length;
							}
							else if (removeCharacters[r].Index < m_HrefInfos[i].endIndex && indexes.Contains(removeCharacters[r].Index)) {
								endCharactersRemoved += removeCharacters[r].Length - 1;
							}
						}

						m_HrefInfos[i].startIndex -= startCharactersRemoved;

						m_HrefInfos[i].startIndex = m_HrefInfos[i].startIndex * 4;

						m_HrefInfos[i].endIndex -= endCharactersRemoved;

						m_HrefInfos[i].endIndex = m_HrefInfos[i].endIndex * 4 + 3;
					}
				}
			}
			#endif

            return m_OutputText;
        }

		// Process href links to open them as a url, can override this function for custom functionality
		public virtual void OnHrefClick(string hrefName) {
			Application.OpenURL(hrefName);

			// Debug.Log(hrefName);
		}


		/// UNITY METHODS ///

        protected override void OnPopulateMesh(VertexHelper toFill) {
            originalText = m_Text;
            m_Text = GetOutputText();

            base.OnPopulateMesh(toFill);

			m_DisableFontTextureRebuiltCallback = true;

            m_Text = originalText;

            positions.Clear();

            vert = new UIVertex();

            for (int i = 0; i < m_ImagesVertexIndex.Count; i++) {
                int endIndex = m_ImagesVertexIndex[i];

				if (endIndex < toFill.currentVertCount) {
					toFill.PopulateUIVertex(ref vert, endIndex);

					positions.Add(new Vector2((vert.position.x + fontSize / 2), (vert.position.y + fontSize / 2)) + imageOffset);

					// Erase the lower left corner of the black specks
					toFill.PopulateUIVertex(ref vert, endIndex - 3);

					Vector3 pos = vert.position;

					for (int j = endIndex, m = endIndex - 3; j > m; j--) {
						toFill.PopulateUIVertex(ref vert, endIndex);
						vert.position = pos;
						toFill.SetUIVertex(vert, j);
					}
				}
            }

            // Hyperlinks surround processing box
            for (int h = 0; h < m_HrefInfos.Count; h++) {
                m_HrefInfos[h].boxes.Clear();

                if (m_HrefInfos[h].startIndex >= toFill.currentVertCount) {
                    continue;
                }

                // Hyperlink inside the text is added to surround the vertex index coordinate frame
                toFill.PopulateUIVertex(ref vert, m_HrefInfos[h].startIndex);

                Vector3 pos = vert.position;

                Bounds bounds = new Bounds(pos, Vector3.zero);

                for (int i = m_HrefInfos[h].startIndex, m = m_HrefInfos[h].endIndex; i < m; i++) {
                    if (i >= toFill.currentVertCount) {
                        break;
                    }

                    toFill.PopulateUIVertex(ref vert, i);

                    pos = vert.position;

					// Wrap re-add surround frame
                    if (pos.x < bounds.min.x)  {
                        m_HrefInfos[h].boxes.Add(new Rect(bounds.min, bounds.size));
                        bounds = new Bounds(pos, Vector3.zero);
                    }
                    else {
                        bounds.Encapsulate(pos); // Extended enclosed box
                    }
                }

                m_HrefInfos[h].boxes.Add(new Rect(bounds.min, bounds.size));
            }

			// Update the quad images
            updateQuad = true;

			m_DisableFontTextureRebuiltCallback = false;
        }

        /// <summary>
        /// Click event is detected whether to click a hyperlink text
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData) { 
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out lp);

            for (int h = 0; h < m_HrefInfos.Count; h++) {
                for (int i = 0; i < m_HrefInfos[h].boxes.Count; ++i) {
                    if (m_HrefInfos[h].boxes[i].Contains(lp)) {
                        m_OnHrefClick.Invoke(m_HrefInfos[h].name);
                        return;
                    }
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {
            //do your stuff when highlighted
            selected = true;

            if (m_ImagesPool.Count >= 1) {
                for (int i = 0; i < m_ImagesPool.Count; i++) {
                    if (button != null && button.isActiveAndEnabled) {
                        m_ImagesPool[i].color = button.colors.highlightedColor;
                    }
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            //do your stuff when highlighted
            selected = false;

            if (m_ImagesPool.Count >= 1) {
                for (int i = 0; i < m_ImagesPool.Count; i++) {
                    if (button != null && button.isActiveAndEnabled) {
                        m_ImagesPool[i].color = button.colors.normalColor;
                    }
                    else {
                        m_ImagesPool[i].color = color;
                    }
                }
            }
        }

        public void OnSelect(BaseEventData eventData) {
            //do your stuff when selected
            selected = true;

            if (m_ImagesPool.Count >= 1) {
                for (int i = 0; i < m_ImagesPool.Count; i++) {
                    if (button != null && button.isActiveAndEnabled) {
                        m_ImagesPool[i].color = button.colors.highlightedColor;
                    }
                }
            }
        }

        public void OnDeselect(BaseEventData eventData) {
            //do your stuff when selected
            selected = false;

            if (m_ImagesPool.Count >= 1) {
                for (int i = 0; i < m_ImagesPool.Count; i++) {
                    if (button != null && button.isActiveAndEnabled) {
                        m_ImagesPool[i].color = button.colors.normalColor;
                    }
                }
            }
        }


        public override void SetVerticesDirty() {
            base.SetVerticesDirty();

			// Update the quad images
            updateQuad = true;
        }

#if UNITY_EDITOR
        protected override void OnValidate() {
            base.OnValidate();

			// Update the quad images
            updateQuad = true;
            
            if (inspectorIconList != null) {
                for (int i = 0; i < inspectorIconList.Length; i++) {
                    if (inspectorIconList[i].scale == Vector2.zero) {
                        inspectorIconList[i].scale = Vector2.one;
                    }
                }
            }
        }
#endif

		protected override void OnEnable() {
			#if UNITY_2019_1_OR_NEWER
			// Here is the hack to see if Unity is using the new rendering system for text
			usesNewRendering = false;

			if (Application.unityVersion.StartsWith("2019.1.")) { 
				if (!Char.IsDigit(Application.unityVersion[8])) {
					int number = Convert.ToInt32(Application.unityVersion[7].ToString());

					if (number > 4) {
						usesNewRendering = true;
					}
				}
				else {
					usesNewRendering = true;
				}
			}
			else {
				usesNewRendering = true;
			}
			#endif

			base.OnEnable();

			supportRichText = true;
			alignByGeometry = true;

			// Enable images on TextPic disable
            if (m_ImagesPool.Count >= 1) {
                for (int i = 0; i < m_ImagesPool.Count; i++) {
                    if(m_ImagesPool[i] != null) {
						m_ImagesPool[i].enabled = true;
					}
                }
            }

			// Update the quads on re-enable
			updateQuad = true;

			this.onHrefClick.AddListener(OnHrefClick);
		}

		protected override void OnDisable() {
			base.OnDisable();

			// Disable images on TextPic disable
            if (m_ImagesPool.Count >= 1) {
                for (int i = 0; i < m_ImagesPool.Count; i++) {
                    if (m_ImagesPool[i] != null) {
						m_ImagesPool[i].enabled = false;
					}
                }
            }

			this.onHrefClick.RemoveListener(OnHrefClick);
		}

        new void Start() {
            button = GetComponent<Button>();
            ResetIconList();
        }

        void LateUpdate() {
			// Reset the hrefs if text is changed
            if (previousText != text) {
                Reset_m_HrefInfos();

				// Update the quad on text change
				updateQuad = true;
			}

			// Need to lock to remove images properly
			lock (thisLock) {
				// Can only update the images when it is not in a rebuild, this prevents the error
				if (updateQuad) {
					UpdateQuadImage();
					updateQuad = false;
				}

				// Destroy any images that are not in use
				if (clearImages) {
					for (int i = 0; i < culled_ImagesPool.Count; i++) {
						DestroyImmediate(culled_ImagesPool[i]);
					}

					culled_ImagesPool.Clear();

					clearImages = false;
				}
			}
        }
    }
}