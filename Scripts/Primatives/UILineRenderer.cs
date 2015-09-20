/// Credit jack.sydorenko 
/// Sourced from - http://forum.unity3d.com/threads/new-ui-and-line-drawing.253772/

using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
 	[AddComponentMenu("UI/Extensions/Primitives/UILineRenderer")]
   public class UILineRenderer : MaskableGraphic
    {
        [SerializeField]
        Texture m_Texture;
        [SerializeField]
        Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

        public float LineThickness = 2;
        public bool UseMargins;
        public Vector2 Margin;
        public Vector2[] Points;
        public bool relativeSize;

        public override Texture mainTexture
        {
            get
            {
                return m_Texture == null ? s_WhiteTexture : m_Texture;
            }
        }

        /// <summary>
        /// Texture to be used.
        /// </summary>
        public Texture texture
        {
            get
            {
                return m_Texture;
            }
            set
            {
                if (m_Texture == value)
                    return;

                m_Texture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

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

        protected override void OnPopulateMesh(Mesh toFill)
        {
            // requires sets of quads
            if (Points == null || Points.Length < 2)
                Points = new[] { new Vector2(0, 0), new Vector2(1, 1) };
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
            // build a new set of points taking into account the cap sizes. 
            // would be cool to support corners too, but that might be a bit tough :)
            var pointList = new List<Vector2>();
            pointList.Add(Points[0]);
            var capPoint = Points[0] + (Points[1] - Points[0]).normalized * capSize;
            pointList.Add(capPoint);

            // should bail before the last point to add another cap point
            for (int i = 1; i < Points.Length - 1; i++)
            {
                pointList.Add(Points[i]);
            }
            capPoint = Points[Points.Length - 1] - (Points[Points.Length - 1] - Points[Points.Length - 2]).normalized * capSize;
            pointList.Add(capPoint);
            pointList.Add(Points[Points.Length - 1]);

            var TempPoints = pointList.ToArray();
            if (UseMargins)
            {
                sizeX -= Margin.x;
                sizeY -= Margin.y;
                offsetX += Margin.x / 2f;
                offsetY += Margin.y / 2f;
            }

            toFill.Clear();
            var vbo = new VertexHelper(toFill);

            Vector2 prevV1 = Vector2.zero;
            Vector2 prevV2 = Vector2.zero;

            for (int i = 1; i < TempPoints.Length; i++)
            {
                var prev = TempPoints[i - 1];
                var cur = TempPoints[i];
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
                    vbo.AddUIVertexQuad(SetVbo(new[] { prevV1, prevV2, v1, v2 }, uvs));

                if (i == 1)
                    uvs = new[] { uvTopLeft, uvBottomLeft, uvBottomCenter, uvTopCenter };
                else if (i == TempPoints.Length - 1)
                    uvs = new[] { uvTopCenter, uvBottomCenter, uvBottomRight, uvTopRight };

                vbo.AddUIVertexQuad(SetVbo(new[] { v1, v2, v3, v4 }, uvs));


                prevV1 = v3;
                prevV2 = v4;
            }

            if (vbo.currentVertCount > 3)
            {
                vbo.FillMesh(toFill);
            }
        }

        protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
        {
            UIVertex[] vbo = new UIVertex[4];
                for (int i = 0; i < vertices.Length; i++)
                {
                    var vert = UIVertex.simpleVert;
                    vert.color = color;
                    vert.position = vertices[i];
                    vert.uv0 = uvs[i];
                    vbo[i] = vert;
                }
            return vbo;
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