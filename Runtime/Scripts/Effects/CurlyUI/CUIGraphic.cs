/// Credit Titinious (https://github.com/Titinious)
/// Sourced from - https://github.com/Titinious/CurlyUI

using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Graphic))]
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/Effects/Extensions/Curly UI Graphic")]
    public class CUIGraphic : BaseMeshEffect
    {
        // Describing the properties that are shared by all objects of this class
        #region Nature

        readonly public static int bottomCurveIdx = 0;
        readonly public static int topCurveIdx = 1;

        #endregion

        /// <summary>
        /// Describing the properties of this object.
        /// </summary>
        #region Description

        [Tooltip("Set true to make the curve/morph to work. Set false to quickly see the original UI.")]
        [SerializeField]
        protected bool isCurved = true;
        public bool IsCurved
        {
            get
            {
                return isCurved;
            }
        }

        [Tooltip("Set true to dynamically change the curve according to the dynamic change of the UI layout")]
        [SerializeField]
        protected bool isLockWithRatio = true;
        public bool IsLockWithRatio
        {
            get
            {
                return isLockWithRatio;
            }
        }

        [Tooltip("Pick a higher resolution to improve the quality of the curved graphic.")]
        [SerializeField]
        [Range(0.01f, 30.0f)]
        protected float resolution = 5.0f;

        #endregion

        /// <summary>
        /// Reference to other objects that are needed by this object.
        /// </summary>
        #region Links

        protected RectTransform rectTrans;
        public RectTransform RectTrans
        {
            get
            {
                return rectTrans;
            }
        }

        [Tooltip("Put in the Graphic you want to curve/morph here.")]
        [SerializeField]
        protected Graphic uiGraphic;
        public Graphic UIGraphic
        {
            get
            {
                return uiGraphic;
            }
        }
        [Tooltip("Put in the reference Graphic that will be used to tune the bezier curves. Think button image and text.")]
        [SerializeField]
        protected CUIGraphic refCUIGraphic;
        public CUIGraphic RefCUIGraphic
        {
            get
            {
                return refCUIGraphic;
            }
        }

        [Tooltip("Do not touch this unless you are sure what you are doing. The curves are (re)generated automatically.")]
        [SerializeField]
        protected CUIBezierCurve[] refCurves;
        public CUIBezierCurve[] RefCurves
        {
            get
            {
                return refCurves;
            }
        }

        [HideInInspector]
        [SerializeField]
        protected Vector3_Array2D[] refCurvesControlRatioPoints;
        public Vector3_Array2D[] RefCurvesControlRatioPoints
        {
            get
            {
                return refCurvesControlRatioPoints;
            }
        }

#if UNITY_EDITOR

        public CUIBezierCurve[] EDITOR_RefCurves
        {
            set
            {
                refCurves = value;
            }
        }

        public Vector3_Array2D[] EDITOR_RefCurvesControlRatioPoints
        {
            set
            {
                refCurvesControlRatioPoints = value;
            }
        }

#endif

        #endregion

        // Methods that are used often.
        #region Reuse

        protected List<UIVertex> reuse_quads = new List<UIVertex>();

        #endregion

        #region Action

        protected void solveDoubleEquationWithVector(float _x_1, float _y_1, float _x_2, float _y_2, Vector3 _constant_1, Vector3 _contant_2, out Vector3 _x, out Vector3 _y)
        {
            Vector3 f;
            float g;

            if (Mathf.Abs(_x_1) > Mathf.Abs(_x_2))
            {
                f = _constant_1 * _x_2 / _x_1;
                g = _y_1 * _x_2 / _x_1;
                _y = (_contant_2 - f) / (_y_2 - g);
                if (_x_2 != 0)
                    _x = (f - g * _y) / _x_2;
                else
                    _x = (_constant_1 - _y_1 * _y) / _x_1;
            }
            else
            {
                f = _contant_2 * _x_1 / _x_2;
                g = _y_2 * _x_1 / _x_2;
                _x = (_constant_1 - f) / (_y_1 - g);
                if (_x_1 != 0)
                    _y = (f - g * _x) / _x_1;
                else
                    _y = (_contant_2 - _y_2 * _x) / _x_2;
            }
        }


        protected UIVertex uiVertexLerp(UIVertex _a, UIVertex _b, float _time)
        {
            UIVertex tmpUIVertex = new UIVertex();

            tmpUIVertex.position = Vector3.Lerp(_a.position, _b.position, _time);
            tmpUIVertex.normal = Vector3.Lerp(_a.normal, _b.normal, _time);
            tmpUIVertex.tangent = Vector3.Lerp(_a.tangent, _b.tangent, _time);
            tmpUIVertex.uv0 = Vector2.Lerp(_a.uv0, _b.uv0, _time);
            tmpUIVertex.uv1 = Vector2.Lerp(_a.uv1, _b.uv1, _time);
            tmpUIVertex.color = Color.Lerp(_a.color, _b.color, _time);

            return tmpUIVertex;
        }

        /// <summary>
        /// Bilinear Interpolation
        /// </summary>
        protected UIVertex uiVertexBerp(UIVertex v_bottomLeft, UIVertex v_topLeft, UIVertex v_topRight, UIVertex v_bottomRight, float _xTime, float _yTime)
        {
            UIVertex topX = uiVertexLerp(v_topLeft, v_topRight, _xTime);
            UIVertex bottomX = uiVertexLerp(v_bottomLeft, v_bottomRight, _xTime);
            return uiVertexLerp(bottomX, topX, _yTime);
        }

        protected void tessellateQuad(List<UIVertex> _quads, int _thisQuadIdx)
        {
            UIVertex v_bottomLeft = _quads[_thisQuadIdx];
            UIVertex v_topLeft = _quads[_thisQuadIdx + 1];
            UIVertex v_topRight = _quads[_thisQuadIdx + 2];
            UIVertex v_bottomRight = _quads[_thisQuadIdx + 3];

            float quadSize = 100.0f / resolution;

            int heightQuadEdgeNum = Mathf.Max(1, Mathf.CeilToInt((v_topLeft.position - v_bottomLeft.position).magnitude / quadSize));
            int widthQuadEdgeNum = Mathf.Max(1, Mathf.CeilToInt((v_topRight.position - v_topLeft.position).magnitude / quadSize));

            int quadIdx = 0;

            for (int x = 0; x < widthQuadEdgeNum; x++)
            {
                for (int y = 0; y < heightQuadEdgeNum; y++, quadIdx++)
                {
                    _quads.Add(new UIVertex());
                    _quads.Add(new UIVertex());
                    _quads.Add(new UIVertex());
                    _quads.Add(new UIVertex());

                    float xRatio = (float)x / widthQuadEdgeNum;
                    float yRatio = (float)y / heightQuadEdgeNum;
                    float xPlusOneRatio = (float)(x + 1) / widthQuadEdgeNum;
                    float yPlusOneRatio = (float)(y + 1) / heightQuadEdgeNum;

                    _quads[_quads.Count - 4] = uiVertexBerp(v_bottomLeft, v_topLeft, v_topRight, v_bottomRight, xRatio, yRatio);
                    _quads[_quads.Count - 3] = uiVertexBerp(v_bottomLeft, v_topLeft, v_topRight, v_bottomRight, xRatio, yPlusOneRatio);
                    _quads[_quads.Count - 2] = uiVertexBerp(v_bottomLeft, v_topLeft, v_topRight, v_bottomRight, xPlusOneRatio, yPlusOneRatio);
                    _quads[_quads.Count - 1] = uiVertexBerp(v_bottomLeft, v_topLeft, v_topRight, v_bottomRight, xPlusOneRatio, yRatio);

                }
            }
        }

        protected void tessellateGraphic(List<UIVertex> _verts)
        {
            for (int v = 0; v < _verts.Count; v += 6)
            {
                reuse_quads.Add(_verts[v]); // bottom left
                reuse_quads.Add(_verts[v + 1]); // top left
                reuse_quads.Add(_verts[v + 2]); // top right
                                                // verts[3] is redundant, top right
                reuse_quads.Add(_verts[v + 4]); // bottom right
                                                // verts[5] is redundant, bottom left
            }

            int oriQuadNum = reuse_quads.Count / 4;
            for (int q = 0; q < oriQuadNum; q++)
            {
                tessellateQuad(reuse_quads, q * 4);
            }

            // remove original quads
            reuse_quads.RemoveRange(0, oriQuadNum * 4);

            _verts.Clear();

            // process new quads and turn them into triangles
            for (int q = 0; q < reuse_quads.Count; q += 4)
            {
                _verts.Add(reuse_quads[q]);
                _verts.Add(reuse_quads[q + 1]);
                _verts.Add(reuse_quads[q + 2]);
                _verts.Add(reuse_quads[q + 2]);
                _verts.Add(reuse_quads[q + 3]);
                _verts.Add(reuse_quads[q]);
            }

            reuse_quads.Clear();
        }

        #endregion

        // Events are for handling reoccurring function calls that react to the changes of the environment.
        #region Events

        protected override void OnRectTransformDimensionsChange()
        {
            if (isLockWithRatio)
            {
                UpdateCurveControlPointPositions();
            }
        }

        public void Refresh() { Invoke(nameof(Refreshx), 0.3f); }

        private void Refreshx()
        {
            ReportSet();

            // we use local position as the true value. Ratio position follows it, so it should be updated when refresh

            for (int c = 0; c < refCurves.Length; c++)
            {

                CUIBezierCurve curve = refCurves[c];

                if (curve.ControlPoints != null)
                {

                    Vector3[] controlPoints = curve.ControlPoints;

                    for (int p = 0; p < CUIBezierCurve.CubicBezierCurvePtNum; p++)
                    {

#if UNITY_EDITOR
                        Undo.RecordObject(this, "Move Point");
#endif

                        Vector3 ratioPoint = controlPoints[p];

                        ratioPoint.x = (ratioPoint.x + rectTrans.rect.width * rectTrans.pivot.x) / rectTrans.rect.width;
                        ratioPoint.y = (ratioPoint.y + rectTrans.rect.height * rectTrans.pivot.y) / rectTrans.rect.height;

                        refCurvesControlRatioPoints[c][p] = ratioPoint;
                    }
                }
            }

            //uiText.SetAllDirty();
            // need this to refresh the UI text, SetAllDirty does not seem to work for all cases
            if (uiGraphic != null)
            {
                uiGraphic.enabled = false;
                uiGraphic.enabled = true;
            }
        }

        #endregion

        // Methods that change the behaviour of the object.
        #region Flash-Phase

        protected override void Awake()
        {
            base.Awake();
            OnRectTransformDimensionsChange();

        }
        protected override void OnEnable()
        {
            base.OnEnable();
            OnRectTransformDimensionsChange();

        }

        #endregion

        #region Configurations

        /// <summary>
        /// Check, prepare and set everything needed. 
        /// </summary>
        public virtual void ReportSet()
        {

            if (rectTrans == null)
                rectTrans = GetComponent<RectTransform>();

            if (refCurves == null)
                refCurves = new CUIBezierCurve[2];

            bool isCurvesReady = true;

            for (int c = 0; c < 2; c++)
            {
                isCurvesReady = isCurvesReady & refCurves[c] != null;
            }

            isCurvesReady = isCurvesReady & refCurves.Length == 2;

            if (!isCurvesReady)
            {
                CUIBezierCurve[] curves = refCurves;

                for (int c = 0; c < 2; c++)
                {
                    if (refCurves[c] == null)
                    {
                        GameObject go = new GameObject();
                        go.transform.SetParent(transform);
                        go.transform.localPosition = Vector3.zero;
                        go.transform.localEulerAngles = Vector3.zero;

                        if (c == 0)
                        {
                            go.name = "BottomRefCurve";
                        }
                        else
                        {
                            go.name = "TopRefCurve";
                        }

                        curves[c] = go.AddComponent<CUIBezierCurve>();

                    }
                    else
                    {
                        curves[c] = refCurves[c];
                    }
                    curves[c].ReportSet();
                }

                refCurves = curves;
            }

            if (refCurvesControlRatioPoints == null)
            {
                refCurvesControlRatioPoints = new Vector3_Array2D[refCurves.Length];

                for (int c = 0; c < refCurves.Length; c++)
                {
                    {
                        refCurvesControlRatioPoints[c].array = new Vector3[refCurves[c].ControlPoints.Length];
                    }
                }

                FixTextToRectTrans();
                Refresh();
            }

            for (int c = 0; c < 2; c++)
            {
                refCurves[c].OnRefresh = Refresh;
            }
        }

        public void FixTextToRectTrans()
        {
            for (int c = 0; c < refCurves.Length; c++)
            {
                CUIBezierCurve curve = refCurves[c];

                for (int p = 0; p < CUIBezierCurve.CubicBezierCurvePtNum; p++)
                {
                    if (curve.ControlPoints != null)
                    {
                        Vector3[] controlPoints = curve.ControlPoints;

                        if (c == 0)
                        {
                            controlPoints[p].y = -rectTrans.rect.height * rectTrans.pivot.y;
                        }
                        else
                        {
                            controlPoints[p].y = rectTrans.rect.height - rectTrans.rect.height * rectTrans.pivot.y;
                        }

                        controlPoints[p].x = rectTrans.rect.width * p / (CUIBezierCurve.CubicBezierCurvePtNum - 1);
                        controlPoints[p].x -= rectTrans.rect.width * rectTrans.pivot.x;

                        controlPoints[p].z = 0;
                    }
                }
            }
        }

        public void ReferenceCUIForBCurves()
        {
            // compute the position ratio of this rect transform in perspective of reference rect transform

            Vector3 posDeltaBetweenBottomLeftCorner = rectTrans.localPosition;// Difference between pivot

            posDeltaBetweenBottomLeftCorner.x += -rectTrans.rect.width * rectTrans.pivot.x + (refCUIGraphic.rectTrans.rect.width * refCUIGraphic.rectTrans.pivot.x);
            posDeltaBetweenBottomLeftCorner.y += -rectTrans.rect.height * rectTrans.pivot.y + (refCUIGraphic.rectTrans.rect.height * refCUIGraphic.rectTrans.pivot.y);
            //posDeltaBetweenBottomLeftCorner.z = rectTrans.localPosition.z;

            Vector3 bottomLeftPosRatio = new Vector3(posDeltaBetweenBottomLeftCorner.x / refCUIGraphic.RectTrans.rect.width, posDeltaBetweenBottomLeftCorner.y / refCUIGraphic.RectTrans.rect.height, posDeltaBetweenBottomLeftCorner.z);
            Vector3 topRightPosRatio = new Vector3((posDeltaBetweenBottomLeftCorner.x + rectTrans.rect.width) / refCUIGraphic.RectTrans.rect.width, (posDeltaBetweenBottomLeftCorner.y + rectTrans.rect.height) / refCUIGraphic.RectTrans.rect.height, posDeltaBetweenBottomLeftCorner.z);

            refCurves[0].ControlPoints[0] = refCUIGraphic.GetBCurveSandwichSpacePoint(bottomLeftPosRatio.x, bottomLeftPosRatio.y) - rectTrans.localPosition;
            refCurves[0].ControlPoints[3] = refCUIGraphic.GetBCurveSandwichSpacePoint(topRightPosRatio.x, bottomLeftPosRatio.y) - rectTrans.localPosition;

            refCurves[1].ControlPoints[0] = refCUIGraphic.GetBCurveSandwichSpacePoint(bottomLeftPosRatio.x, topRightPosRatio.y) - rectTrans.localPosition;
            refCurves[1].ControlPoints[3] = refCUIGraphic.GetBCurveSandwichSpacePoint(topRightPosRatio.x, topRightPosRatio.y) - rectTrans.localPosition;

            // use two sample points from the reference curves to find the second and third controls points for this curves
            for (int c = 0; c < refCurves.Length; c++)
            {
                CUIBezierCurve curve = refCurves[c];

                float yTime = c == 0 ? bottomLeftPosRatio.y : topRightPosRatio.y;

                Vector3 leftPoint = refCUIGraphic.GetBCurveSandwichSpacePoint(bottomLeftPosRatio.x, yTime);
                Vector3 rightPoint = refCUIGraphic.GetBCurveSandwichSpacePoint(topRightPosRatio.x, yTime);

                float quarter = 0.25f,
                threeQuarter = 0.75f;

                Vector3 quarterPoint = refCUIGraphic.GetBCurveSandwichSpacePoint((bottomLeftPosRatio.x * 0.75f + topRightPosRatio.x * 0.25f) / 1.0f, yTime);
                Vector3 threeQuaterPoint = refCUIGraphic.GetBCurveSandwichSpacePoint((bottomLeftPosRatio.x * 0.25f + topRightPosRatio.x * 0.75f) / 1.0f, yTime);

                float x_1 = 3 * threeQuarter * threeQuarter * quarter, // (1 - t)(1 - t)t
                    y_1 = 3 * threeQuarter * quarter * quarter,
                  x_2 = 3 * quarter * quarter * threeQuarter,
                  y_2 = 3 * quarter * threeQuarter * threeQuarter;

                Vector3 contant_1 = quarterPoint - Mathf.Pow(threeQuarter, 3) * leftPoint - Mathf.Pow(quarter, 3) * rightPoint,
                contant_2 = threeQuaterPoint - Mathf.Pow(quarter, 3) * leftPoint - Mathf.Pow(threeQuarter, 3) * rightPoint,
                p1,
                p2;

                solveDoubleEquationWithVector(x_1, y_1, x_2, y_2, contant_1, contant_2, out p1, out p2);

                curve.ControlPoints[1] = p1 - rectTrans.localPosition;
                curve.ControlPoints[2] = p2 - rectTrans.localPosition;

            }
            // use tangent and start and end time to derive control point 2 and 3
        }

        public override void ModifyMesh(Mesh _mesh)
        {

            if (!IsActive())
                return;

            using (VertexHelper vh = new VertexHelper(_mesh))
            {
                ModifyMesh(vh);
                vh.FillMesh(_mesh);
            }

        }

        public override void ModifyMesh(VertexHelper _vh)
        {

            if (!IsActive())
                return;

            List<UIVertex> vertexList = new List<UIVertex>();
            _vh.GetUIVertexStream(vertexList);

            modifyVertices(vertexList);

            _vh.Clear();
            _vh.AddUIVertexTriangleStream(vertexList);
        }

        protected virtual void modifyVertices(List<UIVertex> _verts)
        {
            if (!IsActive())
                return;

            tessellateGraphic(_verts);

            if (!isCurved)
            {
                return;
            }

            for (int index = 0; index < _verts.Count; index++)
            {
                var uiVertex = _verts[index];

                // finding the horizontal ratio position (0.0 - 1.0) of a vertex
                float horRatio = (uiVertex.position.x + rectTrans.rect.width * rectTrans.pivot.x) / rectTrans.rect.width;
                float verRatio = (uiVertex.position.y + rectTrans.rect.height * rectTrans.pivot.y) / rectTrans.rect.height;

                //Vector3 pos = Vector3.Lerp(refCurves[0].GetPoint(horRatio), refCurves[1].GetPoint(horRatio), verRatio);
                Vector3 pos = GetBCurveSandwichSpacePoint(horRatio, verRatio);

                uiVertex.position.x = pos.x;
                uiVertex.position.y = pos.y;
                uiVertex.position.z = pos.z;

                _verts[index] = uiVertex;
            }
        }

        public void UpdateCurveControlPointPositions()
        {
            ReportSet();

            for (int c = 0; c < refCurves.Length; c++)
            {
                CUIBezierCurve curve = refCurves[c];

#if UNITY_EDITOR
                Undo.RecordObject(curve, "Move Rect");
#endif

                for (int p = 0; p < refCurves[c].ControlPoints.Length; p++)
                {

                    Vector3 newPt = refCurvesControlRatioPoints[c][p];

                    newPt.x = newPt.x * rectTrans.rect.width - rectTrans.rect.width * rectTrans.pivot.x;
                    newPt.y = newPt.y * rectTrans.rect.height - rectTrans.rect.height * rectTrans.pivot.y;

                    curve.ControlPoints[p] = newPt;

                }
            }
        }

        #endregion

        // Methods that serves other objects 
        #region Services

        public Vector3 GetBCurveSandwichSpacePoint(float _xTime, float _yTime)
        {
            //return Vector3.Lerp(refCurves[0].GetPoint(_xTime), refCurves[1].GetPoint(_xTime), _yTime);
            return refCurves[0].GetPoint(_xTime) * (1 - _yTime) + refCurves[1].GetPoint(_xTime) * _yTime; // use a custom made lerp so that the value is not clamped between 0 and 1
        }

        public Vector3 GetBCurveSandwichSpaceTangent(float _xTime, float _yTime)
        {
            return refCurves[0].GetTangent(_xTime) * (1 - _yTime) + refCurves[1].GetTangent(_xTime) * _yTime;
        }

        #endregion

    }
}