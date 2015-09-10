/// Credit Ges 
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-2062320

using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Image is a textured element in the UI hierarchy.
    /// </summary>

    [AddComponentMenu("UI/Extensions/Image Extended")]
    public class ImageExtended : MaskableGraphic, ISerializationCallbackReceiver, ILayoutElement, ICanvasRaycastFilter
    {
        public enum Type
        {
            Simple,
            Sliced,
            Tiled,
            Filled
        }

        public enum FillMethod
        {
            Horizontal,
            Vertical,
            Radial90,
            Radial180,
            Radial360,
        }

        public enum OriginHorizontal
        {
            Left,
            Right,
        }

        public enum OriginVertical
        {
            Bottom,
            Top,
        }

        public enum Origin90
        {
            BottomLeft,
            TopLeft,
            TopRight,
            BottomRight,
        }

        public enum Origin180
        {
            Bottom,
            Left,
            Top,
            Right,
        }

        public enum Origin360
        {
            Bottom,
            Right,
            Top,
            Left,
        }

        public enum Rotate
        {
            Rotate0,
            Rotate90,
            Rotate180,
            Rotate270,
        }

        [FormerlySerializedAs("m_Frame")]
        [SerializeField]
        private Sprite m_Sprite;
        public Sprite sprite { get { return m_Sprite; } set { if (SetPropertyUtility.SetClass(ref m_Sprite, value)) SetAllDirty(); } }

        [NonSerialized]
        private Sprite m_OverrideSprite;
        public Sprite overrideSprite { get { return m_OverrideSprite == null ? sprite : m_OverrideSprite; } set { if (SetPropertyUtility.SetClass(ref m_OverrideSprite, value)) SetAllDirty(); } }

        /// How the Image is drawn.
        [SerializeField]
        private Type m_Type = Type.Simple;
        public Type type { get { return m_Type; } set { if (SetPropertyUtility.SetStruct(ref m_Type, value)) SetVerticesDirty(); } }

        [SerializeField]
        private bool m_PreserveAspect = false;
        public bool preserveAspect { get { return m_PreserveAspect; } set { if (SetPropertyUtility.SetStruct(ref m_PreserveAspect, value)) SetVerticesDirty(); } }

        [SerializeField]
        private bool m_FillCenter = true;
        public bool fillCenter { get { return m_FillCenter; } set { if (SetPropertyUtility.SetStruct(ref m_FillCenter, value)) SetVerticesDirty(); } }

        /// Filling method for filled sprites.
        [SerializeField]
        private FillMethod m_FillMethod = FillMethod.Radial360;
        public FillMethod fillMethod { get { return m_FillMethod; } set { if (SetPropertyUtility.SetStruct(ref m_FillMethod, value)) { SetVerticesDirty(); m_FillOrigin = 0; } } }

        /// Amount of the Image shown. 0-1 range with 0 being nothing shown, and 1 being the full Image.
        [Range(0, 1)]
        [SerializeField]
        private float m_FillAmount = 1.0f;
        public float fillAmount { get { return m_FillAmount; } set { if (SetPropertyUtility.SetStruct(ref m_FillAmount, Mathf.Clamp01(value))) SetVerticesDirty(); } }

        /// Whether the Image should be filled clockwise (true) or counter-clockwise (false).
        [SerializeField]
        private bool m_FillClockwise = true;
        public bool fillClockwise { get { return m_FillClockwise; } set { if (SetPropertyUtility.SetStruct(ref m_FillClockwise, value)) SetVerticesDirty(); } }

        /// Controls the origin point of the Fill process. Value means different things with each fill method.
        [SerializeField]
        private int m_FillOrigin;
        public int fillOrigin { get { return m_FillOrigin; } set { if (SetPropertyUtility.SetStruct(ref m_FillOrigin, value)) SetVerticesDirty(); } }

        [SerializeField]
        private Rotate m_Rotate = Rotate.Rotate0;
        public Rotate rotate { get { return m_Rotate; } set { if (SetPropertyUtility.SetStruct(ref m_Rotate, value)) SetVerticesDirty(); } }

        // Not serialized until we support read-enabled sprites better.
        private float m_EventAlphaThreshold = 1;
        public float eventAlphaThreshold { get { return m_EventAlphaThreshold; } set { m_EventAlphaThreshold = value; } }

        protected ImageExtended()
        { }

        /// <summary>
        /// Image's texture comes from the UnityEngine.Image.
        /// </summary>
        public override Texture mainTexture
        {
            get
            {
                return overrideSprite == null ? s_WhiteTexture : overrideSprite.texture;
            }
        }

        /// <summary>
        /// Whether the Image has a border to work with.
        /// </summary>

        public bool hasBorder
        {
            get
            {
                if (overrideSprite != null)
                {
                    Vector4 v = overrideSprite.border;
                    return v.sqrMagnitude > 0f;
                }
                return false;
            }
        }

        public float pixelsPerUnit
        {
            get
            {
                float spritePixelsPerUnit = 100;
                if (sprite)
                    spritePixelsPerUnit = sprite.pixelsPerUnit;

                float referencePixelsPerUnit = 100;
                if (canvas)
                    referencePixelsPerUnit = canvas.referencePixelsPerUnit;

                return spritePixelsPerUnit / referencePixelsPerUnit;
            }
        }

        public virtual void OnBeforeSerialize() { }

        public virtual void OnAfterDeserialize()
        {
            if (m_FillOrigin < 0)
                m_FillOrigin = 0;
            else if (m_FillMethod == FillMethod.Horizontal && m_FillOrigin > 1)
                m_FillOrigin = 0;
            else if (m_FillMethod == FillMethod.Vertical && m_FillOrigin > 1)
                m_FillOrigin = 0;
            else if (m_FillOrigin > 3)
                m_FillOrigin = 0;

            m_FillAmount = Mathf.Clamp(m_FillAmount, 0f, 1f);
        }

        /// Image's dimensions used for drawing. X = left, Y = bottom, Z = right, W = top.
        private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
        {
            var padding = overrideSprite == null ? Vector4.zero : Sprites.DataUtility.GetPadding(overrideSprite);
            var size = overrideSprite == null ? Vector2.zero : new Vector2(overrideSprite.rect.width, overrideSprite.rect.height);

            Rect r = GetPixelAdjustedRect();
            // Debug.Log(string.Format("r:{2}, size:{0}, padding:{1}", size, padding, r));

            int spriteW = Mathf.RoundToInt(size.x);
            int spriteH = Mathf.RoundToInt(size.y);

            var v = overrideSprite == null ? new Vector4(0, 0, 1, 1) :
                new Vector4(
                    padding.x / spriteW,
                    padding.y / spriteH,
                    (spriteW - padding.z) / spriteW,
                    (spriteH - padding.w) / spriteH);

            if (shouldPreserveAspect && size.sqrMagnitude > 0.0f)
            {
                var spriteRatio = size.x / size.y;
                var rectRatio = r.width / r.height;

                if (spriteRatio > rectRatio)
                {
                    var oldHeight = r.height;
                    r.height = r.width * (1.0f / spriteRatio);
                    r.y += (oldHeight - r.height) * rectTransform.pivot.y;
                }
                else
                {
                    var oldWidth = r.width;
                    r.width = r.height * spriteRatio;
                    r.x += (oldWidth - r.width) * rectTransform.pivot.x;
                }
            }

            v = new Vector4(
                    r.x + r.width * v.x,
                    r.y + r.height * v.y,
                    r.x + r.width * v.z,
                    r.y + r.height * v.w
                    );

            return v;
        }

        public override void SetNativeSize()
        {
            if (overrideSprite != null)
            {
                float w = overrideSprite.rect.width / pixelsPerUnit;
                float h = overrideSprite.rect.height / pixelsPerUnit;
                rectTransform.anchorMax = rectTransform.anchorMin;
                rectTransform.sizeDelta = new Vector2(w, h);
                SetAllDirty();
            }
        }

        /// <summary>
        /// Update the UI renderer mesh.
        /// </summary>

        protected override void OnPopulateMesh(Mesh toFill)
        {
            List<UIVertex> vbo = new List<UIVertex>();
            using (var helper = new VertexHelper(toFill))
            {
                helper.GetUIVertexStream(vbo);
            }

            switch (type)
            {
                case Type.Simple:
                    GenerateSimpleSprite(vbo, m_PreserveAspect);
                    break;
                case Type.Sliced:
                    GenerateSlicedSprite(vbo);
                    break;
                case Type.Tiled:
                    GenerateTiledSprite(vbo);
                    break;
                case Type.Filled:
                    GenerateFilledSprite(vbo, m_PreserveAspect);
                    break;
            }

            using (var helper = new VertexHelper())
            {
                helper.AddUIVertexTriangleStream(vbo);
                helper.FillMesh(toFill);
            }
        }

        #region Various fill functions
        /// <summary>
        /// Generate vertices for a simple Image.
        /// </summary>

        void GenerateSimpleSprite(List<UIVertex> vbo, bool preserveAspect)
        {
            var vert = UIVertex.simpleVert;
            vert.color = color;

            Vector4 v = GetDrawingDimensions(preserveAspect);
            var uv = (overrideSprite != null) ? Sprites.DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;

            AddQuad(vbo, vert,
                new Vector2(v.x, v.y), new Vector2(v.z, v.w),
                new Vector2(uv.x, uv.y), new Vector2(uv.z, uv.w));
        }

        /// <summary>
        /// Generate vertices for a 9-sliced Image.
        /// </summary>

        static readonly Vector2[] s_VertScratch = new Vector2[4];
        static readonly Vector2[] s_UVScratch = new Vector2[4];
        void GenerateSlicedSprite(List<UIVertex> vbo)
        {
            if (!hasBorder)
            {
                GenerateSimpleSprite(vbo, false);
                return;
            }

            Vector4 outer, inner, padding, border;

            if (overrideSprite != null)
            {
                outer = Sprites.DataUtility.GetOuterUV(overrideSprite);
                inner = Sprites.DataUtility.GetInnerUV(overrideSprite);
                padding = Sprites.DataUtility.GetPadding(overrideSprite);
                border = overrideSprite.border;
            }
            else
            {
                outer = Vector4.zero;
                inner = Vector4.zero;
                padding = Vector4.zero;
                border = Vector4.zero;
            }

            Rect rect = GetPixelAdjustedRect();
            border = GetAdjustedBorders(border / pixelsPerUnit, rect);
            padding = padding / pixelsPerUnit;

            int offset = 4 - (int)rotate;
            for (int i = 0; i < 4; ++i)
            {
                s_VertScratch[(4 - i / 2) % 4][i % 2] = padding[(i + offset) % 4];
                s_VertScratch[1 + i / 2][i % 2] = border[(i + offset) % 4];
            }
            for (int i = 2; i < 4; ++i)
            {
                s_VertScratch[i].x = rect.width - s_VertScratch[i].x;
                s_VertScratch[i].y = rect.height - s_VertScratch[i].y;
            }
            for (int i = 0; i < 4; ++i)
            {
                s_VertScratch[i].x += rect.x;
                s_VertScratch[i].y += rect.y;
            }

            s_UVScratch[0] = new Vector2(outer.x, outer.y);
            s_UVScratch[1] = new Vector2(inner.x, inner.y);
            s_UVScratch[2] = new Vector2(inner.z, inner.w);
            s_UVScratch[3] = new Vector2(outer.z, outer.w);

            var uiv = UIVertex.simpleVert;
            uiv.color = color;
            for (int x = 0; x < 3; ++x)
            {
                int x2 = x + 1;

                for (int y = 0; y < 3; ++y)
                {
                    if (!m_FillCenter && x == 1 && y == 1)
                        continue;

                    int y2 = y + 1;

                    int vx1 = x, vy1 = y;
                    int vx2 = x2, vy2 = y2;
                    for (int i = 0; i < (int)rotate; ++i)
                    {
                        int t1 = 4 - vy1 - 1;
                        vy1 = vx1; vx1 = t1;
                        int t2 = 4 - vy2 - 1;
                        vy2 = vx2; vx2 = t2;
                    }
                    int ux1 = x, uy1 = y;
                    int ux2 = x2, uy2 = y2;
                    if ((int)rotate >= 2)
                    {
                        ux1 = x2; ux2 = x;
                    }
                    if (((int)rotate + 1) % 4 >= 2)
                    {
                        uy1 = y2; uy2 = y;
                    }
                    if (Mathf.Abs(s_VertScratch[vx1].x - s_VertScratch[vx2].x) < Mathf.Epsilon)
                        continue;
                    if (Mathf.Abs(s_VertScratch[vy1].y - s_VertScratch[vy2].y) < Mathf.Epsilon)
                        continue;
                    AddQuad(vbo, uiv,
                        new Vector2(s_VertScratch[vx1].x, s_VertScratch[vy1].y),
                        new Vector2(s_VertScratch[vx2].x, s_VertScratch[vy2].y),
                        new Vector2(s_UVScratch[ux1].x, s_UVScratch[uy1].y),
                        new Vector2(s_UVScratch[ux2].x, s_UVScratch[uy2].y));
                }
            }
        }

        /// <summary>
        /// Generate vertices for a tiled Image.
        /// </summary>

        static readonly Vector2[] s_UVTiled = new Vector2[2];
        void GenerateTiledSprite(List<UIVertex> vbo)
        {
            Vector4 outer, inner, border;
            Vector2 spriteSize;

            if (overrideSprite != null)
            {
                outer = Sprites.DataUtility.GetOuterUV(overrideSprite);
                inner = Sprites.DataUtility.GetInnerUV(overrideSprite);
                border = overrideSprite.border;
                spriteSize = overrideSprite.rect.size;
            }
            else
            {
                outer = Vector4.zero;
                inner = Vector4.zero;
                border = Vector4.zero;
                spriteSize = Vector2.one * 100;
            }

            Rect rect = GetPixelAdjustedRect();
            float tileWidth = (spriteSize.x - border.x - border.z) / pixelsPerUnit;
            float tileHeight = (spriteSize.y - border.y - border.w) / pixelsPerUnit;
            border = GetAdjustedBorders(border / pixelsPerUnit, rect);

            int offset = 4 - (int)rotate;
            int rx = (0 + offset) % 4, ry = (1 + offset) % 4, rz = (2 + offset) % 4, rw = (3 + offset) % 4;

            var v = UIVertex.simpleVert;
            v.color = color;

            // Min to max max range for tiled region in coordinates relative to lower left corner.
            float xMin = border[rx];
            float xMax = rect.width - border[rz];
            float yMin = border[ry];
            float yMax = rect.height - border[rw];

            // Safety check. Useful so Unity doesn't run out of memory if the sprites are too small.
            // Max tiles are 100 x 100.
            if ((xMax - xMin) > tileWidth * 100 || (yMax - yMin) > tileHeight * 100)
            {
                tileWidth = (xMax - xMin) / 100;
                tileHeight = (yMax - yMin) / 100;
            }
            if ((int)rotate % 2 == 1)
            {
                float t = tileWidth;
                tileWidth = tileHeight;
                tileHeight = t;
            }

            if (m_FillCenter)
            {
                for (float y1 = yMin; y1 < yMax; y1 += tileHeight)
                {
                    s_UVTiled[0] = new Vector2(inner.x, inner.y);
                    s_UVTiled[1] = new Vector2(inner.z, inner.w);
                    float y2 = y1 + tileHeight;
                    if (y2 > yMax)
                    {
                        int k1 = 1 - (int)rotate / 2, k2 = 1 - (int)rotate % 2;
                        s_UVTiled[k1][k2] = s_UVTiled[1 - k1][k2] + (s_UVTiled[k1][k2] - s_UVTiled[1 - k1][k2]) * (yMax - y1) / (y2 - y1);
                        y2 = yMax;
                    }
                    for (float x1 = xMin; x1 < xMax; x1 += tileWidth)
                    {
                        float x2 = x1 + tileWidth;
                        if (x2 > xMax)
                        {
                            int k1 = ((int)rotate + 3) % 4 / 2, k2 = (int)rotate % 2;
                            s_UVTiled[k1][k2] = s_UVTiled[1 - k1][k2] + (s_UVTiled[k1][k2] - s_UVTiled[1 - k1][k2]) * (xMax - x1) / (x2 - x1);
                            x2 = xMax;
                        }
                        AddQuad(vbo, v, new Vector2(x1, y1) + rect.position, new Vector2(x2, y2) + rect.position, s_UVTiled[0], s_UVTiled[1]);
                    }
                }
            }

            if (!hasBorder)
                return;

            // Bottom and top tiled border
            for (int i = 0; i < 2; ++i)
            {
                float y1 = i == 0 ? 0 : yMax;
                float y2 = i == 0 ? yMin : rect.height;
                if (Mathf.Abs(y1 - y2) < Mathf.Epsilon)
                    continue;

                s_UVTiled[0] = GetRotatedUV(inner, 0, i == 0 ? outer : inner, i == 0 ? 1 : 3);
                s_UVTiled[1] = GetRotatedUV(inner, 2, i == 0 ? inner : outer, i == 0 ? 1 : 3);
                RotatePairUV(s_UVTiled);
                for (float x1 = xMin; x1 < xMax; x1 += tileWidth)
                {
                    float x2 = x1 + tileWidth;
                    if (x2 > xMax)
                    {
                        int k1 = ((int)rotate + 3) % 4 / 2, k2 = (int)rotate % 2;
                        s_UVTiled[k1][k2] = s_UVTiled[1 - k1][k2] + (s_UVTiled[k1][k2] - s_UVTiled[1 - k1][k2]) * (xMax - x1) / (x2 - x1);
                        x2 = xMax;
                    }
                    AddQuad(vbo, v,
                        new Vector2(x1, y1) + rect.position,
                        new Vector2(x2, y2) + rect.position,
                        s_UVTiled[0], s_UVTiled[1]);
                }
            }

            // Left and right tiled border
            for (int i = 0; i < 2; ++i)
            {
                float x1 = i == 0 ? 0 : xMax;
                float x2 = i == 0 ? xMin : rect.width;
                if (Mathf.Abs(x1 - x2) < Mathf.Epsilon)
                    continue;

                s_UVTiled[0] = GetRotatedUV(i == 0 ? outer : inner, i == 0 ? 0 : 2, inner, 1);
                s_UVTiled[1] = GetRotatedUV(i == 0 ? inner : outer, i == 0 ? 0 : 2, inner, 3);
                RotatePairUV(s_UVTiled);
                for (float y1 = yMin; y1 < yMax; y1 += tileHeight)
                {
                    float y2 = y1 + tileHeight;
                    if (y2 > yMax)
                    {
                        int k1 = 1 - (int)rotate / 2, k2 = 1 - (int)rotate % 2;
                        s_UVTiled[k1][k2] = s_UVTiled[1 - k1][k2] + (s_UVTiled[k1][k2] - s_UVTiled[1 - k1][k2]) * (yMax - y1) / (y2 - y1);
                        y2 = yMax;
                    }
                    AddQuad(vbo, v,
                        new Vector2(x1, y1) + rect.position,
                        new Vector2(x2, y2) + rect.position,
                        s_UVTiled[0], s_UVTiled[1]);
                }
            }

            // Corners
            if (Mathf.Abs(border[rx]) > Mathf.Epsilon &&
                Mathf.Abs(border[ry]) > Mathf.Epsilon)
            {
                s_UVTiled[0] = GetRotatedUV(outer, 0, outer, 1);
                s_UVTiled[1] = GetRotatedUV(inner, 0, inner, 1);
                RotatePairUV(s_UVTiled);
                AddQuad(vbo, v,
                    new Vector2(0, 0) + rect.position,
                    new Vector2(xMin, yMin) + rect.position,
                        s_UVTiled[0], s_UVTiled[1]);
            }
            if (Mathf.Abs(border[rz]) > Mathf.Epsilon &&
                Mathf.Abs(border[ry]) > Mathf.Epsilon)
            {
                s_UVTiled[0] = GetRotatedUV(inner, 2, outer, 1);
                s_UVTiled[1] = GetRotatedUV(outer, 2, inner, 1);
                RotatePairUV(s_UVTiled);
                AddQuad(vbo, v,
                    new Vector2(xMax, 0) + rect.position,
                    new Vector2(rect.width, yMin) + rect.position,
                        s_UVTiled[0], s_UVTiled[1]);
            }
            if (Mathf.Abs(border[rx]) > Mathf.Epsilon &&
                Mathf.Abs(border[rw]) > Mathf.Epsilon)
            {
                s_UVTiled[0] = GetRotatedUV(outer, 0, inner, 3);
                s_UVTiled[1] = GetRotatedUV(inner, 0, outer, 3);
                RotatePairUV(s_UVTiled);
                AddQuad(vbo, v,
                    new Vector2(0, yMax) + rect.position,
                    new Vector2(xMin, rect.height) + rect.position,
                        s_UVTiled[0], s_UVTiled[1]);
            }
            if (Mathf.Abs(border[rz]) > Mathf.Epsilon &&
                Mathf.Abs(border[rw]) > Mathf.Epsilon)
            {
                s_UVTiled[0] = GetRotatedUV(inner, 2, inner, 3);
                s_UVTiled[1] = GetRotatedUV(outer, 2, outer, 3);
                RotatePairUV(s_UVTiled);
                AddQuad(vbo, v,
                    new Vector2(xMax, yMax) + rect.position,
                    new Vector2(rect.width, rect.height) + rect.position,
                        s_UVTiled[0], s_UVTiled[1]);
            }
        }

        Vector2 GetRotatedUV(Vector4 sX, int iX, Vector4 sY, int iY)
        {
            for (int i = 0; i < (int)rotate; i++)
            {
                Vector4 tS = sX;
                sX = sY; sY = tS;
                int tI = (iX + 3) % 4;
                iX = iY - 1; iY = tI;
            }
            return new Vector2(sX[iX], sY[iY]);
        }

        void RotatePairUV(Vector2[] uv)
        {
            if ((int)rotate / 2 == 1)
            {
                float t = uv[0].x;
                uv[0].x = uv[1].x;
                uv[1].x = t;
            }
            if (((int)rotate + 1) / 2 == 1)
            {
                float t = uv[0].y;
                uv[0].y = uv[1].y;
                uv[1].y = t;
            }
        }

        static readonly Vector3[] s_VertQuad = new Vector3[4];
        static readonly Vector2[] s_UVQuad = new Vector2[4];
        void AddQuad(List<UIVertex> vbo, UIVertex v, Vector2 posMin, Vector2 posMax, Vector2 uvMin, Vector2 uvMax)
        {
            s_VertQuad[0] = new Vector3(posMin.x, posMin.y, 0);
            s_VertQuad[1] = new Vector3(posMin.x, posMax.y, 0);
            s_VertQuad[2] = new Vector3(posMax.x, posMax.y, 0);
            s_VertQuad[3] = new Vector3(posMax.x, posMin.y, 0);
            s_UVQuad[0] = new Vector2(uvMin.x, uvMin.y);
            s_UVQuad[1] = new Vector2(uvMin.x, uvMax.y);
            s_UVQuad[2] = new Vector2(uvMax.x, uvMax.y);
            s_UVQuad[3] = new Vector2(uvMax.x, uvMin.y);

            int offset = (int)rotate;
            for (int i = 0; i < 4; i++)
            {
                v.position = s_VertQuad[i];
                v.uv0 = s_UVQuad[(i + offset) % 4];
                vbo.Add(v);
            }
        }

        Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
        {
            for (int axis = 0; axis <= 1; axis++)
            {
                // If the rect is smaller than the combined borders, then there's not room for the borders at their normal size.
                // In order to avoid artefacts with overlapping borders, we scale the borders down to fit.
                float combinedBorders = border[axis] + border[axis + 2];
                float rectSize = rect.size[(axis + (int)rotate % 2) % 2];
                if (rectSize < combinedBorders && combinedBorders != 0)
                {
                    float borderScaleRatio = rectSize / combinedBorders;
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }
            }
            return border;
        }

        /// <summary>
        /// Generate vertices for a filled Image.
        /// </summary>

        static readonly Vector2[] s_Xy = new Vector2[4];
        static readonly Vector2[] s_Uv = new Vector2[4];
        void GenerateFilledSprite(List<UIVertex> vbo, bool preserveAspect)
        {
            if (m_FillAmount < 0.001f)
                return;

            Vector4 v = GetDrawingDimensions(preserveAspect);
            Vector4 outer = overrideSprite != null ? Sprites.DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
            UIVertex uiv = UIVertex.simpleVert;
            uiv.color = color;

            int offset = 4 - (int)rotate;
            int rx = (0 + offset) % 4, ry = (1 + offset) % 4, rz = (2 + offset) % 4, rw = (3 + offset) % 4;

            // Horizontal and vertical filled sprites are simple -- just end the Image prematurely
            if (m_FillMethod == FillMethod.Horizontal || m_FillMethod == FillMethod.Vertical)
            {
                if (fillMethod == FillMethod.Horizontal)
                {
                    float fill = (outer[rz] - outer[rx]) * m_FillAmount;

                    if (m_FillOrigin == 1)
                    {
                        v.x = v.z - (v.z - v.x) * m_FillAmount;
                        outer[rx] = outer[rz] - fill;
                    }
                    else
                    {
                        v.z = v.x + (v.z - v.x) * m_FillAmount;
                        outer[rz] = outer[rx] + fill;
                    }
                }
                else if (fillMethod == FillMethod.Vertical)
                {
                    float fill = (outer[rw] - outer[ry]) * m_FillAmount;

                    if (m_FillOrigin == 1)
                    {
                        v.y = v.w - (v.w - v.y) * m_FillAmount;
                        outer[ry] = outer[rw] - fill;
                    }
                    else
                    {
                        v.w = v.y + (v.w - v.y) * m_FillAmount;
                        outer[rw] = outer[ry] + fill;
                    }
                }
            }

            s_Xy[0] = new Vector2(v.x, v.y);
            s_Xy[1] = new Vector2(v.x, v.w);
            s_Xy[2] = new Vector2(v.z, v.w);
            s_Xy[3] = new Vector2(v.z, v.y);

            s_Uv[(0 + offset) % 4] = new Vector2(outer.x, outer.y);
            s_Uv[(1 + offset) % 4] = new Vector2(outer.x, outer.w);
            s_Uv[(2 + offset) % 4] = new Vector2(outer.z, outer.w);
            s_Uv[(3 + offset) % 4] = new Vector2(outer.z, outer.y);

            if (m_FillAmount < 1f)
            {
                float tx0 = outer.x;
                float ty0 = outer.y;
                float tx1 = outer.z;
                float ty1 = outer.w;
                if (fillMethod == FillMethod.Radial90)
                {
                    if (RadialCut(s_Xy, s_Uv, m_FillAmount, m_FillClockwise, m_FillOrigin))
                    {
                        for (int i = 0; i < 4; ++i)
                        {
                            uiv.position = s_Xy[i];
                            uiv.uv0 = s_Uv[i];
                            vbo.Add(uiv);
                        }
                    }
                    return;
                }

                if (fillMethod == FillMethod.Radial180)
                {
                    for (int side = 0; side < 2; ++side)
                    {
                        float fx0, fx1, fy0, fy1;
                        int even = m_FillOrigin > 1 ? 1 : 0;

                        if (m_FillOrigin == 0 || m_FillOrigin == 2)
                        {
                            fy0 = 0f;
                            fy1 = 1f;
                            if (side == even) { fx0 = 0f; fx1 = 0.5f; }
                            else { fx0 = 0.5f; fx1 = 1f; }
                        }
                        else
                        {
                            fx0 = 0f;
                            fx1 = 1f;
                            if (side == even) { fy0 = 0.5f; fy1 = 1f; }
                            else { fy0 = 0f; fy1 = 0.5f; }
                        }

                        s_Xy[0].x = Mathf.Lerp(v.x, v.z, fx0);
                        s_Xy[1].x = s_Xy[0].x;
                        s_Xy[2].x = Mathf.Lerp(v.x, v.z, fx1);
                        s_Xy[3].x = s_Xy[2].x;

                        s_Xy[0].y = Mathf.Lerp(v.y, v.w, fy0);
                        s_Xy[1].y = Mathf.Lerp(v.y, v.w, fy1);
                        s_Xy[2].y = s_Xy[1].y;
                        s_Xy[3].y = s_Xy[0].y;

                        s_Uv[0].x = Mathf.Lerp(tx0, tx1, fx0);
                        s_Uv[1].x = s_Uv[0].x;
                        s_Uv[2].x = Mathf.Lerp(tx0, tx1, fx1);
                        s_Uv[3].x = s_Uv[2].x;

                        s_Uv[0].y = Mathf.Lerp(ty0, ty1, fy0);
                        s_Uv[1].y = Mathf.Lerp(ty0, ty1, fy1);
                        s_Uv[2].y = s_Uv[1].y;
                        s_Uv[3].y = s_Uv[0].y;

                        float val = m_FillClockwise ? fillAmount * 2f - side : m_FillAmount * 2f - (1 - side);

                        if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(val), m_FillClockwise, ((side + m_FillOrigin + 3) % 4)))
                        {
                            for (int i = 0; i < 4; ++i)
                            {
                                uiv.position = s_Xy[i];
                                uiv.uv0 = s_Uv[i];
                                vbo.Add(uiv);
                            }
                        }
                    }
                    return;
                }

                if (fillMethod == FillMethod.Radial360)
                {
                    for (int corner = 0; corner < 4; ++corner)
                    {
                        float fx0, fx1, fy0, fy1;

                        if (corner < 2) { fx0 = 0f; fx1 = 0.5f; }
                        else { fx0 = 0.5f; fx1 = 1f; }

                        if (corner == 0 || corner == 3) { fy0 = 0f; fy1 = 0.5f; }
                        else { fy0 = 0.5f; fy1 = 1f; }

                        s_Xy[0].x = Mathf.Lerp(v.x, v.z, fx0);
                        s_Xy[1].x = s_Xy[0].x;
                        s_Xy[2].x = Mathf.Lerp(v.x, v.z, fx1);
                        s_Xy[3].x = s_Xy[2].x;

                        s_Xy[0].y = Mathf.Lerp(v.y, v.w, fy0);
                        s_Xy[1].y = Mathf.Lerp(v.y, v.w, fy1);
                        s_Xy[2].y = s_Xy[1].y;
                        s_Xy[3].y = s_Xy[0].y;

                        s_Uv[0].x = Mathf.Lerp(tx0, tx1, fx0);
                        s_Uv[1].x = s_Uv[0].x;
                        s_Uv[2].x = Mathf.Lerp(tx0, tx1, fx1);
                        s_Uv[3].x = s_Uv[2].x;

                        s_Uv[0].y = Mathf.Lerp(ty0, ty1, fy0);
                        s_Uv[1].y = Mathf.Lerp(ty0, ty1, fy1);
                        s_Uv[2].y = s_Uv[1].y;
                        s_Uv[3].y = s_Uv[0].y;

                        float val = m_FillClockwise ?
                            m_FillAmount * 4f - ((corner + m_FillOrigin) % 4) :
                            m_FillAmount * 4f - (3 - ((corner + m_FillOrigin) % 4));

                        if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(val), m_FillClockwise, ((corner + 2) % 4)))
                        {
                            for (int i = 0; i < 4; ++i)
                            {
                                uiv.position = s_Xy[i];
                                uiv.uv0 = s_Uv[i];
                                vbo.Add(uiv);
                            }
                        }
                    }
                    return;
                }
            }

            // Fill the buffer with the quad for the Image
            for (int i = 0; i < 4; ++i)
            {
                uiv.position = s_Xy[i];
                uiv.uv0 = s_Uv[i];
                vbo.Add(uiv);
            }
        }

        /// <summary>
        /// Adjust the specified quad, making it be radially filled instead.
        /// </summary>

        static bool RadialCut(Vector2[] xy, Vector2[] uv, float fill, bool invert, int corner)
        {
            // Nothing to fill
            if (fill < 0.001f) return false;

            // Even corners invert the fill direction
            if ((corner & 1) == 1) invert = !invert;

            // Nothing to adjust
            if (!invert && fill > 0.999f) return true;

            // Convert 0-1 value into 0 to 90 degrees angle in radians
            float angle = Mathf.Clamp01(fill);
            if (invert) angle = 1f - angle;
            angle *= 90f * Mathf.Deg2Rad;

            // Calculate the effective X and Y factors
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            RadialCut(xy, cos, sin, invert, corner);
            RadialCut(uv, cos, sin, invert, corner);
            return true;
        }

        /// <summary>
        /// Adjust the specified quad, making it be radially filled instead.
        /// </summary>

        static void RadialCut(Vector2[] xy, float cos, float sin, bool invert, int corner)
        {
            int i0 = corner;
            int i1 = ((corner + 1) % 4);
            int i2 = ((corner + 2) % 4);
            int i3 = ((corner + 3) % 4);

            if ((corner & 1) == 1)
            {
                if (sin > cos)
                {
                    cos /= sin;
                    sin = 1f;

                    if (invert)
                    {
                        xy[i1].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
                        xy[i2].x = xy[i1].x;
                    }
                }
                else if (cos > sin)
                {
                    sin /= cos;
                    cos = 1f;

                    if (!invert)
                    {
                        xy[i2].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
                        xy[i3].y = xy[i2].y;
                    }
                }
                else
                {
                    cos = 1f;
                    sin = 1f;
                }

                if (!invert) xy[i3].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
                else xy[i1].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
            }
            else
            {
                if (cos > sin)
                {
                    sin /= cos;
                    cos = 1f;

                    if (!invert)
                    {
                        xy[i1].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
                        xy[i2].y = xy[i1].y;
                    }
                }
                else if (sin > cos)
                {
                    cos /= sin;
                    sin = 1f;

                    if (invert)
                    {
                        xy[i2].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
                        xy[i3].x = xy[i2].x;
                    }
                }
                else
                {
                    cos = 1f;
                    sin = 1f;
                }

                if (invert) xy[i3].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
                else xy[i1].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
            }
        }

        #endregion

        public virtual void CalculateLayoutInputHorizontal() { }
        public virtual void CalculateLayoutInputVertical() { }

        public virtual float minWidth { get { return 0; } }

        public virtual float preferredWidth
        {
            get
            {
                if (overrideSprite == null)
                    return 0;
                if (type == Type.Sliced || type == Type.Tiled)
                    return Sprites.DataUtility.GetMinSize(overrideSprite).x / pixelsPerUnit;
                return overrideSprite.rect.size.x / pixelsPerUnit;
            }
        }

        public virtual float flexibleWidth { get { return -1; } }

        public virtual float minHeight { get { return 0; } }

        public virtual float preferredHeight
        {
            get
            {
                if (overrideSprite == null)
                    return 0;
                if (type == Type.Sliced || type == Type.Tiled)
                    return Sprites.DataUtility.GetMinSize(overrideSprite).y / pixelsPerUnit;
                return overrideSprite.rect.size.y / pixelsPerUnit;
            }
        }

        public virtual float flexibleHeight { get { return -1; } }

        public virtual int layoutPriority { get { return 0; } }

        public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (m_EventAlphaThreshold >= 1)
                return true;

            Sprite sprite = overrideSprite;
            if (sprite == null)
                return true;

            Vector2 local;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out local);

            Rect rect = GetPixelAdjustedRect();

            // Convert to have lower left corner as reference point.
            local.x += rectTransform.pivot.x * rect.width;
            local.y += rectTransform.pivot.y * rect.height;

            local = MapCoordinate(local, rect);

            // Normalize local coordinates.
            Rect spriteRect = sprite.textureRect;
            Vector2 normalized = new Vector2(local.x / spriteRect.width, local.y / spriteRect.height);

            // Convert to texture space.
            float x = Mathf.Lerp(spriteRect.x, spriteRect.xMax, normalized.x) / sprite.texture.width;
            float y = Mathf.Lerp(spriteRect.y, spriteRect.yMax, normalized.y) / sprite.texture.height;

            try
            {
                return sprite.texture.GetPixelBilinear(x, y).a >= m_EventAlphaThreshold;
            }
            catch (UnityException e)
            {
                Debug.LogError("Using clickAlphaThreshold lower than 1 on Image whose sprite texture cannot be read. " + e.Message + " Also make sure to disable sprite packing for this sprite.", this);
                return true;
            }
        }

        private Vector2 MapCoordinate(Vector2 local, Rect rect)
        {
            Rect spriteRect = sprite.rect;
            if (type == Type.Simple || type == Type.Filled)
                return new Vector2(local.x * spriteRect.width / rect.width, local.y * spriteRect.height / rect.height);

            Vector4 border = sprite.border;
            Vector4 adjustedBorder = GetAdjustedBorders(border / pixelsPerUnit, rect);

            for (int i = 0; i < 2; i++)
            {
                if (local[i] <= adjustedBorder[i])
                    continue;

                if (rect.size[i] - local[i] <= adjustedBorder[i + 2])
                {
                    local[i] -= (rect.size[i] - spriteRect.size[i]);
                    continue;
                }

                if (type == Type.Sliced)
                {
                    float lerp = Mathf.InverseLerp(adjustedBorder[i], rect.size[i] - adjustedBorder[i + 2], local[i]);
                    local[i] = Mathf.Lerp(border[i], spriteRect.size[i] - border[i + 2], lerp);
                    continue;
                }
                else
                {
                    local[i] -= adjustedBorder[i];
                    local[i] = Mathf.Repeat(local[i], spriteRect.size[i] - border[i] - border[i + 2]);
                    local[i] += border[i];
                    continue;
                }
            }

            return local;
        }
    }
}