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
        /// <summary>
        /// Image Pool
        /// </summary>
        private readonly List<Image> m_ImagesPool = new List<Image>();
        private readonly List<GameObject> culled_ImagesPool = new List<GameObject>();
        private bool clearImages = false;
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

        private string fixedString;        [SerializeField]

        [Tooltip("Allow click events to be received by parents, (default) blocks")]

        private bool m_ClickParents;

		// Update the quad images when true
		private bool updateQuad = false;

        public bool AllowClickParents {
            get { return m_ClickParents; }
            set { m_ClickParents = value; }
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

			for (int i = 0; i < inspectorIconList.Length; i++) {
                if (inspectorIconList[i].scale == Vector2.zero) {
                    inspectorIconList[i].scale = Vector2.one;
                }
            }
        }
#endif

        /// <summary>
        /// After parsing the final text
        /// </summary>
        private string m_OutputText;

        [System.Serializable]
        public struct IconName {
            public string name;
            public Sprite sprite;
			public Vector2 offset;
			public Vector2 scale;
        }

        public IconName[] inspectorIconList;

		[Tooltip("Global scaling factor for all images")]
        public float ImageScalingFactor = 1;

        // Write the name or hex value of the hyperlink color
        public string hyperlinkColor = "blue";

        // Offset image by x, y
        [SerializeField]
        public Vector2 imageOffset = Vector2.zero;

        private Button button;

        private List<Vector2> positions = new List<Vector2>();
        
        /**
        * Little hack to support multiple hrefs with same name
        */
        private string previousText = "";
        public bool isCreating_m_HrefInfos = true;

        new void Start() {
            button = GetComponent<Button>();
            ResetIconList();
        }

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

			MatchCollection matches = s_Regex.Matches(m_OutputText);

			if (matches != null && matches.Count > 0) {
				for (int i = 0; i < matches.Count; i++) {
					m_ImagesPool.RemoveAll(image => image == null);

					if (m_ImagesPool.Count == 0) {
						GetComponentsInChildren<Image>(true, m_ImagesPool);
					}

					if (matches.Count > m_ImagesPool.Count) {
						var resources = new DefaultControls.Resources();
						var go = DefaultControls.CreateImage(resources);
						go.layer = gameObject.layer;
						var rt = go.transform as RectTransform;

						if (rt) {
							rt.SetParent(rectTransform);
							rt.anchoredPosition3D = Vector3.zero;
							rt.localRotation = Quaternion.identity;
							rt.localScale = Vector3.one;
						}

						m_ImagesPool.Add(go.GetComponent<Image>());
					}

					var spriteName = matches[i].Groups[1].Value;

					var img = m_ImagesPool[i];

					Vector2 imgoffset = Vector2.zero;

					if (img.sprite == null || img.sprite.name != spriteName) {
						if (inspectorIconList != null && inspectorIconList.Length > 0) {
							foreach (IconName icon in inspectorIconList) {
								if (icon.name == spriteName) {
									img.sprite = icon.sprite;
									img.preserveAspect = true;
									img.rectTransform.sizeDelta = new Vector2(fontSize * ImageScalingFactor * icon.scale.x, fontSize * ImageScalingFactor * icon.scale.y);
									imgoffset = icon.offset;
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

        protected override void OnPopulateMesh(VertexHelper toFill) {
            var orignText = m_Text;
            m_Text = GetOutputText();
            base.OnPopulateMesh(toFill);
            m_Text = orignText;
            positions.Clear();

            UIVertex vert = new UIVertex();

            for (var i = 0; i < m_ImagesVertexIndex.Count; i++) {
                var endIndex = m_ImagesVertexIndex[i];

				if (endIndex < toFill.currentVertCount) {
					toFill.PopulateUIVertex(ref vert, endIndex);
					positions.Add(new Vector2((vert.position.x + fontSize / 2), (vert.position.y + fontSize / 2)) + imageOffset);

					// Erase the lower left corner of the black specks
					toFill.PopulateUIVertex(ref vert, endIndex - 3);
					var pos = vert.position;

					for (int j = endIndex, m = endIndex - 3; j > m; j--) {
						toFill.PopulateUIVertex(ref vert, endIndex);
						vert.position = pos;
						toFill.SetUIVertex(vert, j);
					}
				}
            }

            // Hyperlinks surround processing box
            foreach (var hrefInfo in m_HrefInfos) {
                hrefInfo.boxes.Clear();

                if (hrefInfo.startIndex >= toFill.currentVertCount) {
                    continue;
                }

                // Hyperlink inside the text is added to surround the vertex index coordinate frame
                toFill.PopulateUIVertex(ref vert, hrefInfo.startIndex);
                var pos = vert.position;
                var bounds = new Bounds(pos, Vector3.zero);

                for (int i = hrefInfo.startIndex, m = hrefInfo.endIndex; i < m; i++) {
                    if (i >= toFill.currentVertCount) {
                        break;
                    }

                    toFill.PopulateUIVertex(ref vert, i);
                    pos = vert.position;

					// Wrap re-add surround frame
                    if (pos.x < bounds.min.x)  {
                        hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
                        bounds = new Bounds(pos, Vector3.zero);
                    }
                    else {
                        bounds.Encapsulate(pos); // Extended enclosed box
                    }
                }

                hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
            }

			// Update the quad images
            updateQuad = true;
        }

        /// <summary>
        /// Hyperlink List
        /// </summary>
        private readonly List<HrefInfo> m_HrefInfos = new List<HrefInfo>();

        /// <summary>
        /// Text Builder
        /// </summary>
        private static readonly StringBuilder s_TextBuilder = new StringBuilder();

        /// <summary>
        /// Hyperlink Regular Expression
        /// </summary>
        private static readonly Regex s_HrefRegex =
            new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);

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
        /// Finally, the output text hyperlinks get parsed
        /// </summary>
        /// <returns></returns>
        protected string GetOutputText() {
            s_TextBuilder.Length = 0;
            
            var indexText = 0;
            fixedString = this.text;

            if (inspectorIconList != null && inspectorIconList.Length > 0) {
                foreach (IconName icon in inspectorIconList) {
                    if (!string.IsNullOrEmpty(icon.name)) {
                        fixedString = fixedString.Replace(icon.name, "<quad name=" + icon.name + " size=" + fontSize + " width=1 />");
                    }
                }
            }

            int count = 0;

            foreach (Match match in s_HrefRegex.Matches(fixedString)) {
                s_TextBuilder.Append(fixedString.Substring(indexText, match.Index - indexText));
                s_TextBuilder.Append("<color=" + hyperlinkColor + ">");  // Hyperlink color

                var group = match.Groups[1];

                if (isCreating_m_HrefInfos) {
                    var hrefInfo = new HrefInfo

                    {
                        startIndex = s_TextBuilder.Length * 4, // Hyperlinks in text starting vertex indices
                        endIndex = (s_TextBuilder.Length + match.Groups[2].Length - 1) * 4 + 3,
                        name = group.Value
                    };

                    m_HrefInfos.Add(hrefInfo);
                }
				else {
                    if(m_HrefInfos.Count > 0) {
                        m_HrefInfos[count].startIndex = s_TextBuilder.Length * 4; // Hyperlinks in text starting vertex indices;
                        m_HrefInfos[count].endIndex = (s_TextBuilder.Length + match.Groups[2].Length - 1) * 4 + 3;
                        count++;
                    }
                }

                s_TextBuilder.Append(match.Groups[2].Value);
                s_TextBuilder.Append("</color>");
                indexText = match.Index + match.Length;
            }

            // we should create array only once or if there is any change in the text
            if (isCreating_m_HrefInfos)
                isCreating_m_HrefInfos = false;
                
            s_TextBuilder.Append(fixedString.Substring(indexText, fixedString.Length - indexText));

			m_OutputText = s_TextBuilder.ToString();

            m_ImagesVertexIndex.Clear();

			MatchCollection matches = s_Regex.Matches(m_OutputText);

			if (matches != null && matches.Count > 0) {
				foreach (Match match in matches) {
					var picIndex = match.Index;
					var endIndex = picIndex * 4 + 3;
					m_ImagesVertexIndex.Add(endIndex);
				}
			}

            return m_OutputText;
        }

        /// <summary>
        /// Click event is detected whether to click a hyperlink text
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData) {
            Vector2 lp;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out lp);

            foreach (var hrefInfo in m_HrefInfos) {
                var boxes = hrefInfo.boxes;
                for (var i = 0; i < boxes.Count; ++i) {
                    if (boxes[i].Contains(lp)) {
                        m_OnHrefClick.Invoke(hrefInfo.name);
                        return;
                    }
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {

            if (m_ImagesPool.Count >= 1) {
                foreach (Image img in m_ImagesPool) {
                    if (button != null && button.isActiveAndEnabled) {
                        img.color = button.colors.highlightedColor;
                    }
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData) {

            if (m_ImagesPool.Count >= 1) {
                foreach (Image img in m_ImagesPool) {
                    if (button != null && button.isActiveAndEnabled) {
                        img.color = button.colors.normalColor;
                    }
                    else {
                        img.color = color;
                    }
                }
            }
        }

        public void OnSelect(BaseEventData eventData) {

            if (m_ImagesPool.Count >= 1) {
                foreach (Image img in m_ImagesPool) {
                    if (button != null && button.isActiveAndEnabled) {
                        img.color = button.colors.highlightedColor;
                    }
                }
            }
        }

        public void OnDeselect(BaseEventData eventData) {

            if (m_ImagesPool.Count >= 1) {
                foreach (Image img in m_ImagesPool) {
                    if (button != null && button.isActiveAndEnabled) {
                        img.color = button.colors.normalColor;
                    }
                }
            }
        }

        /// <summary>
        /// Hyperlinks Info
        /// </summary>
        private class HrefInfo {
            public int startIndex;

            public int endIndex;

            public string name;

            public readonly List<Rect> boxes = new List<Rect>();
        }
    
        void LateUpdate() {
			// Reset the hrefs if text is changed
            if( previousText != text) {
                Reset_m_HrefInfos ();

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
        
        // Reseting m_HrefInfos array if there is any change in text
        void Reset_m_HrefInfos () {
            previousText = text;
            m_HrefInfos.Clear();
            isCreating_m_HrefInfos = true;
        }

		protected override void OnEnable() {
			base.OnEnable();

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
		}
    }
}