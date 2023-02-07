/// Credit Steve Westhoff, jack.sydorenko, firagon
/// Sourced from - https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/issues/324
/// Refactored and updated for performance from UILineRenderer by Steve Westhoff

using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Primitives/UILineRendererFIFO")]
	[RequireComponent(typeof(RectTransform))]
	public class UILineRendererFIFO : UIPrimitiveBase
	{
        private static readonly Vector2[] middleUvs = new[] { new Vector2(0.5f, 0), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 0) };
        private List<Vector2> addedPoints = new List<Vector2>();
        private bool needsResize;

        [SerializeField, Tooltip("Thickness of the line")]
        private float lineThickness = 1;

		[SerializeField, Tooltip("Points to draw lines between\n Can be improved using the Resolution Option")]
		private List<Vector2> points = new List<Vector2>();

		[SerializeField, Tooltip("Segments to be drawn\n This is a list of arrays of points")]
		private List<UIVertex[]> segments = new List<UIVertex[]>();

        /// <summary>
		/// Thickness of the line
		/// </summary>
		public float LineThickness
        {
            get { return lineThickness; }
            set { lineThickness = value; SetAllDirty(); }
        }

        /// <summary>
        /// Points to be drawn in the line.
        /// </summary>
        /// <remarks>Don't add points to the list directly, use the add / remove functions</remarks>
        public List<Vector2> Points
        {
            get
            {
                return points;
            }

            set
            {
                if (points == value)
                    return;
                points = value;
                SetAllDirty();
            }
        }

        /// <summary>
        /// Adds to head
        /// </summary>
        /// <param name="point"></param>
        public void AddPoint(Vector2 point) {
			points.Add(point);
			addedPoints.Add(point);
		}

		/// <summary>
		/// Removes from tail (FIFO)
		/// </summary>
		public void RemovePoint() {
			points.RemoveAt(0);
			needsResize = true;
		}

        /// <summary>
		/// Clear all the points from the LineRenderer
		/// </summary>
		public void ClearPoints()
        {
            segments.Clear();
            points.Clear();
            addedPoints.Clear();
            needsResize = false;
        }

        public void Resize() {
			needsResize = true;
		}

		protected override void OnPopulateMesh(VertexHelper vertexHelper) {
			vertexHelper.Clear();
			if(needsResize) {
				needsResize = false;
				segments.Clear();
				addedPoints = new List<Vector2>(points);
			}
			int count = addedPoints.Count;
			if(count > 1) {
				PopulateMesh(addedPoints, vertexHelper);
				if(count % 2 == 0) {
					addedPoints.Clear();
				} else {
					Vector2 extraPoint = addedPoints[count - 1];
					addedPoints.Clear();
					addedPoints.Add(extraPoint);
				}
			}
		}

		void PopulateMesh(List<Vector2> pointsToDraw, VertexHelper vertexHelper) {
			if(ImproveResolution != ResolutionMode.None) {
				pointsToDraw = IncreaseResolution(pointsToDraw);
			}
			float sizeX = rectTransform.rect.width;
			float sizeY = rectTransform.rect.height;
			float offsetX = -rectTransform.pivot.x * sizeX;
			float offsetY = -rectTransform.pivot.y * sizeY;
			for(int i = 1; i < pointsToDraw.Count; i += 2) {
				Vector2 start = pointsToDraw[i - 1];
				Vector2 end = pointsToDraw[i];
				start = new Vector2(start.x * sizeX + offsetX, start.y * sizeY + offsetY);
				end = new Vector2(end.x * sizeX + offsetX, end.y * sizeY + offsetY);
				UIVertex[] segment = CreateLineSegment(start, end, segments.Count > 1 ? segments[segments.Count - 2] : null);
				segments.Add(segment);
			}
			for(int i = 0; i < segments.Count; i++) {
				vertexHelper.AddUIVertexQuad(segments[i]);
			}
			if(vertexHelper.currentVertCount > 64000) {
				Debug.LogError("Max Verticies size is 64000, current mesh vertcies count is [" + vertexHelper.currentVertCount + "] - Cannot Draw");
				vertexHelper.Clear();
			}
		}

		UIVertex[] CreateLineSegment(Vector2 start, Vector2 end, UIVertex[] previousVert = null) {
			Vector2 offset = new Vector2(start.y - end.y, end.x - start.x).normalized * lineThickness * 0.5f;
			Vector2 v1;
			Vector2 v2;
			if(previousVert != null) {
				v1 = new Vector2(previousVert[3].position.x, previousVert[3].position.y);
				v2 = new Vector2(previousVert[2].position.x, previousVert[2].position.y);
			} else {
				v1 = start - offset;
				v2 = start + offset;
			}
			return SetVbo(new[] { v1, v2, end + offset, end - offset }, middleUvs);
		}
	}
}