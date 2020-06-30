/// Credit jack.sydorenko, firagon
/// Sourced from - http://forum.unity3d.com/threads/new-ui-and-line-drawing.253772/
/// Updated/Refactored from - http://forum.unity3d.com/threads/new-ui-and-line-drawing.253772/#post-2528050

using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/UILineRendererList")]
    [RequireComponent(typeof(RectTransform))]
    public class UILineRendererList : UIPrimitiveBase
	{
		private enum SegmentType
		{
			Start,
            Middle,
            End,
            Full,
		}

		public enum JoinType
		{
			Bevel,
            Miter
		}

		public enum BezierType
		{
			None,
            Quick,
            Basic,
            Improved,
            Catenary,
        }

		private const float MIN_MITER_JOIN = 15 * Mathf.Deg2Rad;

		// A bevel 'nice' join displaces the vertices of the line segment instead of simply rendering a
		// quad to connect the endpoints. This improves the look of textured and transparent lines, since
		// there is no overlapping.
        private const float MIN_BEVEL_NICE_JOIN = 30 * Mathf.Deg2Rad;

		private static Vector2 UV_TOP_LEFT, UV_BOTTOM_LEFT, UV_TOP_CENTER_LEFT, UV_TOP_CENTER_RIGHT, UV_BOTTOM_CENTER_LEFT, UV_BOTTOM_CENTER_RIGHT, UV_TOP_RIGHT, UV_BOTTOM_RIGHT;
		private static Vector2[] startUvs, middleUvs, endUvs, fullUvs;

        [SerializeField, Tooltip("Points to draw lines between\n Can be improved using the Resolution Option")]
        internal List<Vector2> m_points;
        // [SerializeField, Tooltip("Segments to be drawn\n This is a list of arrays of points")]
		//internal List<Vector2[]> m_segments;

        [SerializeField, Tooltip("Thickness of the line")]
        internal float lineThickness = 2;
        [SerializeField, Tooltip("Use the relative bounds of the Rect Transform (0,0 -> 0,1) or screen space coordinates")]
        internal bool relativeSize;
        [SerializeField, Tooltip("Do the points identify a single line or split pairs of lines")]
        internal bool lineList;
        [SerializeField, Tooltip("Add end caps to each line\nMultiple caps when used with Line List")]
        internal bool lineCaps;
        [SerializeField, Tooltip("Resolution of the Bezier curve, different to line Resolution")]
        internal int bezierSegmentsPerCurve = 10;

        public float LineThickness
        {
            get { return lineThickness; }
            set { lineThickness = value; SetAllDirty(); }
        }

        public bool RelativeSize
        {
            get { return relativeSize; }
            set { relativeSize = value; SetAllDirty(); }
        }

        public bool LineList
        {
            get { return lineList; }
            set { lineList = value; SetAllDirty(); }
        }

        public bool LineCaps
        {
            get { return lineCaps; }
            set { lineCaps = value; SetAllDirty(); }
        }

        [Tooltip("The type of Join used between lines, Square/Mitre or Curved/Bevel")]
		public JoinType LineJoins = JoinType.Bevel;

        [Tooltip("Bezier method to apply to line, see docs for options\nCan't be used in conjunction with Resolution as Bezier already changes the resolution")]
        public BezierType BezierMode = BezierType.None;

        public int BezierSegmentsPerCurve
        {
            get { return bezierSegmentsPerCurve; }
            set { bezierSegmentsPerCurve = value; }
        }

        [HideInInspector]
        public bool drivenExternally = false;

        /// <summary>
        /// Points to be drawn in the line.
        /// </summary>
        /// <remarks>Don't add points to the list directly, use the add / remove functions</remarks>
        public List<Vector2> Points
        {
            get
            {
                return m_points;
            }

            set
            {
                if (m_points == value)
                    return;
                m_points = value;
                SetAllDirty();
            }
        }

        //      /// <summary>
        //      /// List of Segments to be drawn.
        //      /// </summary>
        //      public List<Vector2[]> Segments
        //{
        //	get
        //	{
        //		return m_segments;
        //	}

        //	set
        //	{
        //		m_segments = value;
        //		SetAllDirty();
        //	}
        //}


        public void AddPoint(Vector2 pointToAdd)
        {
            m_points.Add(pointToAdd);
            SetAllDirty();
        }

        public void RemovePoint(Vector2 pointToRemove)
        {
            m_points.Remove(pointToRemove);
            SetAllDirty();
        }

        public void ClearPoints()
        {
            m_points.Clear();
            SetAllDirty();
        }

        private void PopulateMesh(VertexHelper vh, List<Vector2> pointsToDraw)
		{
			//If Bezier is desired, pick the implementation
			if (BezierMode != BezierType.None && BezierMode != BezierType.Catenary && pointsToDraw.Count > 3) {
				BezierPath bezierPath = new BezierPath ();

				bezierPath.SetControlPoints (pointsToDraw);
				bezierPath.SegmentsPerCurve = bezierSegmentsPerCurve;
				List<Vector2> drawingPoints;
				switch (BezierMode) {
				case BezierType.Basic:
					drawingPoints = bezierPath.GetDrawingPoints0 ();
					break;
				case BezierType.Improved:
					drawingPoints = bezierPath.GetDrawingPoints1 ();
					break;
				default:
					drawingPoints = bezierPath.GetDrawingPoints2 ();
					break;
				}

				pointsToDraw = drawingPoints;
			}
			if (BezierMode == BezierType.Catenary && pointsToDraw.Count == 2) {
				CableCurve cable = new CableCurve (pointsToDraw);
				cable.slack = Resoloution;
				cable.steps = BezierSegmentsPerCurve;
                pointsToDraw.Clear();
				pointsToDraw.AddRange(cable.Points());
			}

			if (ImproveResolution != ResolutionMode.None) {
				pointsToDraw = IncreaseResolution (pointsToDraw);
			}

			// scale based on the size of the rect or use absolute, this is switchable
			var sizeX = !relativeSize ? 1 : rectTransform.rect.width;
			var sizeY = !relativeSize ? 1 : rectTransform.rect.height;
			var offsetX = -rectTransform.pivot.x * sizeX;
			var offsetY = -rectTransform.pivot.y * sizeY;

			// Generate the quads that make up the wide line
			var segments = new List<UIVertex[]> ();
			if (lineList) {
				for (var i = 1; i < pointsToDraw.Count; i += 2) {
					var start = pointsToDraw [i - 1];
					var end = pointsToDraw [i];
					start = new Vector2 (start.x * sizeX + offsetX, start.y * sizeY + offsetY);
					end = new Vector2 (end.x * sizeX + offsetX, end.y * sizeY + offsetY);

					if (lineCaps) {
						segments.Add (CreateLineCap (start, end, SegmentType.Start));
					}

					//segments.Add(CreateLineSegment(start, end, SegmentType.Full));
					segments.Add (CreateLineSegment (start, end, SegmentType.Middle));

					if (lineCaps) {
						segments.Add (CreateLineCap (start, end, SegmentType.End));
					}
				}
			} else {
				for (var i = 1; i < pointsToDraw.Count; i++) {
					var start = pointsToDraw [i - 1];
					var end = pointsToDraw [i];
					start = new Vector2 (start.x * sizeX + offsetX, start.y * sizeY + offsetY);
					end = new Vector2 (end.x * sizeX + offsetX, end.y * sizeY + offsetY);

					if (lineCaps && i == 1) {
						segments.Add (CreateLineCap (start, end, SegmentType.Start));
					}

					segments.Add (CreateLineSegment (start, end, SegmentType.Middle));
					//segments.Add(CreateLineSegment(start, end, SegmentType.Full));

					if (lineCaps && i == pointsToDraw.Count - 1) {
						segments.Add (CreateLineCap (start, end, SegmentType.End));
					}
				}
			}

			// Add the line segments to the vertex helper, creating any joins as needed
			for (var i = 0; i < segments.Count; i++) {
				if (!lineList && i < segments.Count - 1) {
					var vec1 = segments [i] [1].position - segments [i] [2].position;
					var vec2 = segments [i + 1] [2].position - segments [i + 1] [1].position;
					var angle = Vector2.Angle (vec1, vec2) * Mathf.Deg2Rad;

					// Positive sign means the line is turning in a 'clockwise' direction
					var sign = Mathf.Sign (Vector3.Cross (vec1.normalized, vec2.normalized).z);

					// Calculate the miter point
					var miterDistance = lineThickness / (2 * Mathf.Tan (angle / 2));
					var miterPointA = segments [i] [2].position - vec1.normalized * miterDistance * sign;
					var miterPointB = segments [i] [3].position + vec1.normalized * miterDistance * sign;

					var joinType = LineJoins;
					if (joinType == JoinType.Miter) {
						// Make sure we can make a miter join without too many artifacts.
						if (miterDistance < vec1.magnitude / 2 && miterDistance < vec2.magnitude / 2 && angle > MIN_MITER_JOIN) {
							segments [i] [2].position = miterPointA;
							segments [i] [3].position = miterPointB;
							segments [i + 1] [0].position = miterPointB;
							segments [i + 1] [1].position = miterPointA;
						} else {
							joinType = JoinType.Bevel;
						}
					}

					if (joinType == JoinType.Bevel) {
						if (miterDistance < vec1.magnitude / 2 && miterDistance < vec2.magnitude / 2 && angle > MIN_BEVEL_NICE_JOIN) {
							if (sign < 0) {
								segments [i] [2].position = miterPointA;
								segments [i + 1] [1].position = miterPointA;
							} else {
								segments [i] [3].position = miterPointB;
								segments [i + 1] [0].position = miterPointB;
							}
						}

						var join = new UIVertex[] { segments [i] [2], segments [i] [3], segments [i + 1] [0], segments [i + 1] [1] };
						vh.AddUIVertexQuad (join);
					}
				}

				vh.AddUIVertexQuad (segments [i]);
			}
			if (vh.currentVertCount > 64000) {
				Debug.LogError ("Max Verticies size is 64000, current mesh vertcies count is [" + vh.currentVertCount + "] - Cannot Draw");
				vh.Clear ();
				return;
			}

		}

        protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (m_points != null && m_points.Count > 0) {
				GeneratedUVs ();
				vh.Clear ();

				PopulateMesh (vh, m_points);

			}
			//else if (m_segments != null && m_segments.Count > 0) {
			//	GeneratedUVs ();
			//	vh.Clear ();

			//	for (int s = 0; s < m_segments.Count; s++) {
			//		Vector2[] pointsToDraw = m_segments [s];
			//		PopulateMesh (vh, pointsToDraw);
			//	}
			//} 
        }

		private UIVertex[] CreateLineCap(Vector2 start, Vector2 end, SegmentType type)
		{
			if (type == SegmentType.Start)
			{
				var capStart = start - ((end - start).normalized * lineThickness / 2);
				return CreateLineSegment(capStart, start, SegmentType.Start);
			}
			else if (type == SegmentType.End)
			{
				var capEnd = end + ((end - start).normalized * lineThickness / 2);
				return CreateLineSegment(end, capEnd, SegmentType.End);
			}

			Debug.LogError("Bad SegmentType passed in to CreateLineCap. Must be SegmentType.Start or SegmentType.End");
			return null;
		}

		private UIVertex[] CreateLineSegment(Vector2 start, Vector2 end, SegmentType type)
		{
			Vector2 offset = new Vector2((start.y - end.y), end.x - start.x).normalized * lineThickness / 2;

			var v1 = start - offset;
			var v2 = start + offset;
			var v3 = end + offset;
			var v4 = end - offset;
            //Return the VDO with the correct uvs
            switch (type)
            {
                case SegmentType.Start:
                    return SetVbo(new[] { v1, v2, v3, v4 }, startUvs);
                case SegmentType.End:
                    return SetVbo(new[] { v1, v2, v3, v4 }, endUvs);
                case SegmentType.Full:
                    return SetVbo(new[] { v1, v2, v3, v4 }, fullUvs);
                default:
                    return SetVbo(new[] { v1, v2, v3, v4 }, middleUvs);
            }
		}

        protected override void GeneratedUVs()
        {
            if (activeSprite != null)
            {
                var outer = Sprites.DataUtility.GetOuterUV(activeSprite);
                var inner = Sprites.DataUtility.GetInnerUV(activeSprite);
                UV_TOP_LEFT = new Vector2(outer.x, outer.y);
                UV_BOTTOM_LEFT = new Vector2(outer.x, outer.w);
                UV_TOP_CENTER_LEFT = new Vector2(inner.x, inner.y);
                UV_TOP_CENTER_RIGHT = new Vector2(inner.z, inner.y);
                UV_BOTTOM_CENTER_LEFT = new Vector2(inner.x, inner.w);
                UV_BOTTOM_CENTER_RIGHT = new Vector2(inner.z, inner.w);
                UV_TOP_RIGHT = new Vector2(outer.z, outer.y);
                UV_BOTTOM_RIGHT = new Vector2(outer.z, outer.w);
            }
            else
            {
                UV_TOP_LEFT = Vector2.zero;
                UV_BOTTOM_LEFT = new Vector2(0, 1);
                UV_TOP_CENTER_LEFT = new Vector2(0.5f, 0);
                UV_TOP_CENTER_RIGHT = new Vector2(0.5f, 0);
                UV_BOTTOM_CENTER_LEFT = new Vector2(0.5f, 1);
                UV_BOTTOM_CENTER_RIGHT = new Vector2(0.5f, 1);
                UV_TOP_RIGHT = new Vector2(1, 0);
                UV_BOTTOM_RIGHT = Vector2.one;
            }


            startUvs = new[] { UV_TOP_LEFT, UV_BOTTOM_LEFT, UV_BOTTOM_CENTER_LEFT, UV_TOP_CENTER_LEFT };
            middleUvs = new[] { UV_TOP_CENTER_LEFT, UV_BOTTOM_CENTER_LEFT, UV_BOTTOM_CENTER_RIGHT, UV_TOP_CENTER_RIGHT };
            endUvs = new[] { UV_TOP_CENTER_RIGHT, UV_BOTTOM_CENTER_RIGHT, UV_BOTTOM_RIGHT, UV_TOP_RIGHT };
            fullUvs = new[] { UV_TOP_LEFT, UV_BOTTOM_LEFT, UV_BOTTOM_RIGHT, UV_TOP_RIGHT };
        }

        protected override void ResolutionToNativeSize(float distance)
        {
            if (UseNativeSize)
            {
                m_Resolution = distance / (activeSprite.rect.width / pixelsPerUnit);
                lineThickness = activeSprite.rect.height / pixelsPerUnit;
            }
        }
    }
}