/// Credit jonbro5556 
/// Based on original LineRender script by jack.sydorenko 
/// Sourced from - http://forum.unity3d.com/threads/new-ui-and-line-drawing.253772/

using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/UILineTextureRenderer")]
    public class UILineTextureRenderer : UIPrimitiveBase
    {
        [SerializeField]
        Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);
        [SerializeField]
        private Vector2[] m_points;

        public float LineThickness = 2;
        public bool UseMargins;
        public Vector2 Margin;
        public bool relativeSize;

        /// <summary>
        /// UV rectangle used by the texture.
        /// </summary>
        public Rect uvRect
        {
            get
            {
                return m_UVRect;
            }
            set
            {
                if (m_UVRect == value)
                    return;
                m_UVRect = value;
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// Points to be drawn in the line.
        /// </summary>
        public Vector2[] Points
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

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            // requires sets of quads
            if (m_points == null || m_points.Length < 2)
                m_points = new[] { new Vector2(0, 0), new Vector2(1, 1) };
            var capSize = 24;
            var sizeX = rectTransform.rect.width;
            var sizeY = rectTransform.rect.height;
            var offsetX = -rectTransform.pivot.x * rectTransform.rect.width;
            var offsetY = -rectTransform.pivot.y * rectTransform.rect.height;

            // don't want to scale based on the size of the rect, so this is switchable now
            if (!relativeSize)
            {
                sizeX = 1;
                sizeY = 1;
            }
            // build a new set of m_points taking into account the cap sizes. 
            // would be cool to support corners too, but that might be a bit tough :)
            var pointList = new List<Vector2>();
            pointList.Add(m_points[0]);
            var capPoint = m_points[0] + (m_points[1] - m_points[0]).normalized * capSize;
            pointList.Add(capPoint);

            // should bail before the last point to add another cap point
            for (int i = 1; i < m_points.Length - 1; i++)
            {
                pointList.Add(m_points[i]);
            }
            capPoint = m_points[m_points.Length - 1] - (m_points[m_points.Length - 1] - m_points[m_points.Length - 2]).normalized * capSize;
            pointList.Add(capPoint);
            pointList.Add(m_points[m_points.Length - 1]);

            var Tempm_points = pointList.ToArray();
            if (UseMargins)
            {
                sizeX -= Margin.x;
                sizeY -= Margin.y;
                offsetX += Margin.x / 2f;
                offsetY += Margin.y / 2f;
            }

            vh.Clear();

            Vector2 prevV1 = Vector2.zero;
            Vector2 prevV2 = Vector2.zero;

            for (int i = 1; i < Tempm_points.Length; i++)
            {
                var prev = Tempm_points[i - 1];
                var cur = Tempm_points[i];
                prev = new Vector2(prev.x * sizeX + offsetX, prev.y * sizeY + offsetY);
                cur = new Vector2(cur.x * sizeX + offsetX, cur.y * sizeY + offsetY);

                float angle = Mathf.Atan2(cur.y - prev.y, cur.x - prev.x) * 180f / Mathf.PI;

                var v1 = prev + new Vector2(0, -LineThickness / 2);
                var v2 = prev + new Vector2(0, +LineThickness / 2);
                var v3 = cur + new Vector2(0, +LineThickness / 2);
                var v4 = cur + new Vector2(0, -LineThickness / 2);

                v1 = RotatePointAroundPivot(v1, prev, new Vector3(0, 0, angle));
                v2 = RotatePointAroundPivot(v2, prev, new Vector3(0, 0, angle));
                v3 = RotatePointAroundPivot(v3, cur, new Vector3(0, 0, angle));
                v4 = RotatePointAroundPivot(v4, cur, new Vector3(0, 0, angle));

                Vector2 uvTopLeft = Vector2.zero;
                Vector2 uvBottomLeft = new Vector2(0, 1);

                Vector2 uvTopCenter = new Vector2(0.5f, 0);
                Vector2 uvBottomCenter = new Vector2(0.5f, 1);

                Vector2 uvTopRight = new Vector2(1, 0);
                Vector2 uvBottomRight = new Vector2(1, 1);

                Vector2[] uvs = new[] { uvTopCenter, uvBottomCenter, uvBottomCenter, uvTopCenter };

                if (i > 1)
                    vh.AddUIVertexQuad(SetVbo(new[] { prevV1, prevV2, v1, v2 }, uvs));

                if (i == 1)
                    uvs = new[] { uvTopLeft, uvBottomLeft, uvBottomCenter, uvTopCenter };
                else if (i == Tempm_points.Length - 1)
                    uvs = new[] { uvTopCenter, uvBottomCenter, uvBottomRight, uvTopRight };

                vh.AddUIVertexQuad(SetVbo(new[] { v1, v2, v3, v4 }, uvs));


                prevV1 = v3;
                prevV2 = v4;
            }
        }

        public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            Vector3 dir = point - pivot; // get point direction relative to pivot
            dir = Quaternion.Euler(angles) * dir; // rotate it
            point = dir + pivot; // calculate rotated point
            return point; // return it
        }
    }
}