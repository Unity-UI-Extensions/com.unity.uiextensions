using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    public enum ResolutionMode
    {
        None,
        PerSegment,
        PerLine
    }

    public class UIPrimitiveBase : MaskableGraphic, ILayoutElement, ICanvasRaycastFilter
    {

        [SerializeField]
        private Sprite m_Sprite;
        public Sprite sprite { get { return m_Sprite; } set { if (SetPropertyUtility.SetClass(ref m_Sprite, value)) SetAllDirty(); } }

        [NonSerialized]
        private Sprite m_OverrideSprite;
        public Sprite overrideSprite { get { return m_OverrideSprite == null ? sprite : m_OverrideSprite; } set { if (SetPropertyUtility.SetClass(ref m_OverrideSprite, value)) SetAllDirty(); } }

        // Not serialized until we support read-enabled sprites better.
        internal float m_EventAlphaThreshold = 1;
        public float eventAlphaThreshold { get { return m_EventAlphaThreshold; } set { m_EventAlphaThreshold = value; } }

        [SerializeField]
        private ResolutionMode m_improveResolution;
        public ResolutionMode ImproveResolution { get { return m_improveResolution; } set { m_improveResolution = value; SetAllDirty(); } }

        [SerializeField]
        private float m_Resolution;
        public float Resoloution { get { return m_Resolution; } set { m_Resolution = value; SetAllDirty(); } }



        /// <summary>
        /// Image's texture comes from the UnityEngine.Image.
        /// </summary>
        public override Texture mainTexture
        {
            get
            {
                if (overrideSprite == null)
                {
                    if (material != null && material.mainTexture != null)
                    {
                        return material.mainTexture;
                    }
                    return s_WhiteTexture;
                }

                return overrideSprite.texture;
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

        protected Vector2[] IncreaseResolution(Vector2[] input)
        {
            var outputList = new List<Vector2>();

            switch (ImproveResolution)
            {
                case ResolutionMode.PerLine:
                    float totalDistance = 0, increments = 0;
                    for (int i = 0; i < input.Length - 1; i++)
                    {
                        totalDistance += Vector2.Distance(input[i], input[i + 1]);
                    }
                    increments = totalDistance / m_Resolution;
                    var incrementCount = 0;
                    for (int i = 0; i < input.Length - 1; i++)
                    {
                        var p1 = input[i];
                        outputList.Add(p1);
                        var p2 = input[i + 1];
                        var segmentDistance = Vector2.Distance(p1, p2) / increments;
                        var incrementTime = 1f / segmentDistance;
                        for (int j=0; j < segmentDistance; j++)
                        {
                            outputList.Add(Vector2.Lerp(p1, (Vector2)p2, j * incrementTime));
                            incrementCount++;
                        }
                        outputList.Add(p2);
                    }
                    break;
                case ResolutionMode.PerSegment:
                    for (int i = 0; i < input.Length - 1; i++)
                    {
                        var p1 = input[i];
                        outputList.Add(p1);
                        var p2 = input[i + 1];
                        increments = 1f / m_Resolution;
                        for (Single j = 1; j < m_Resolution; j++)
                        {
                            outputList.Add(Vector2.Lerp(p1, (Vector2)p2, increments * j));
                        }
                        outputList.Add(p2);
                    }
                    break;
            }
            return outputList.ToArray();
        }

        #region ILayoutElement Interface

        public virtual void CalculateLayoutInputHorizontal() { }
        public virtual void CalculateLayoutInputVertical() { }

        public virtual float minWidth { get { return 0; } }

        public virtual float preferredWidth
        {
            get
            {
                if (overrideSprite == null)
                    return 0;
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
                return overrideSprite.rect.size.y / pixelsPerUnit;
            }
        }

        public virtual float flexibleHeight { get { return -1; } }

        public virtual int layoutPriority { get { return 0; } }

        #endregion

        #region ICanvasRaycastFilter Interface
        public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            // add test for line check
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

            //test local coord with Mesh

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

        /// <summary>
        /// Return image adjusted position
        /// **Copied from Unity's Image component for now and simplified for UI Extensions primatives
        /// </summary>
        /// <param name="local"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        private Vector2 MapCoordinate(Vector2 local, Rect rect)
        {
            Rect spriteRect = sprite.rect;
            //if (type == Type.Simple || type == Type.Filled)
                return new Vector2(local.x * spriteRect.width / rect.width, local.y * spriteRect.height / rect.height);

            //Vector4 border = sprite.border;
            //Vector4 adjustedBorder = GetAdjustedBorders(border / pixelsPerUnit, rect);

            //for (int i = 0; i < 2; i++)
            //{
            //    if (local[i] <= adjustedBorder[i])
            //        continue;

            //    if (rect.size[i] - local[i] <= adjustedBorder[i + 2])
            //    {
            //        local[i] -= (rect.size[i] - spriteRect.size[i]);
            //        continue;
            //    }

            //    if (type == Type.Sliced)
            //    {
            //        float lerp = Mathf.InverseLerp(adjustedBorder[i], rect.size[i] - adjustedBorder[i + 2], local[i]);
            //        local[i] = Mathf.Lerp(border[i], spriteRect.size[i] - border[i + 2], lerp);
            //        continue;
            //    }
            //    else
            //    {
            //        local[i] -= adjustedBorder[i];
            //        local[i] = Mathf.Repeat(local[i], spriteRect.size[i] - border[i] - border[i + 2]);
            //        local[i] += border[i];
            //        continue;
            //    }
            //}

            //return local;
        }

        Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
        {
            for (int axis = 0; axis <= 1; axis++)
            {
                // If the rect is smaller than the combined borders, then there's not room for the borders at their normal size.
                // In order to avoid artefacts with overlapping borders, we scale the borders down to fit.
                float combinedBorders = border[axis] + border[axis + 2];
                if (rect.size[axis] < combinedBorders && combinedBorders != 0)
                {
                    float borderScaleRatio = rect.size[axis] / combinedBorders;
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }
            }
            return border;
        }

        #endregion

        #region onEnable
        protected override void OnEnable()
        {
            SetAllDirty();
        }
        #endregion

    }
}
