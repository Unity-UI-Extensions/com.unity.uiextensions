/// Credit zge
/// Sourced from - http://forum.unity3d.com/threads/draw-circles-or-primitives-on-the-new-ui-canvas.272488/#post-2293224

using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/UI Circle")]
    public class UICircle : Graphic
    {
        [SerializeField]
        Texture m_Texture;
        [Range(0, 100)]
        public int fillPercent = 100;
        public bool fill = true;
        public float thickness = 5;
        [Range(0, 360)]
        public int segments = 360;

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

        protected void SetVbo(List<UIVertex> vbo, Vector2[] vertices, Vector2[] uvs)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                var vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                vbo.Add(vert);
            }
        }

        protected override void OnFillVBO(List<UIVertex> vbo)
        {
            float outer = -rectTransform.pivot.x * rectTransform.rect.width;
            float inner = -rectTransform.pivot.x * rectTransform.rect.width + this.thickness;

            vbo.Clear();

            Vector2 prevX = Vector2.zero;
            Vector2 prevY = Vector2.zero;
            Vector2 uv0 = new Vector2(0, 0);
            Vector2 uv1 = new Vector2(0, 1);
            Vector2 uv2 = new Vector2(1, 1);
            Vector2 uv3 = new Vector2(1, 0);
            Vector2 pos0;
            Vector2 pos1;
            Vector2 pos2;
            Vector2 pos3;

            float f = (this.fillPercent / 100f);
            float degrees = 360f / segments;
            int fa = (int)((segments + 1) * f);


            for (int i = 0; i < fa; i++)
            {
                float rad = Mathf.Deg2Rad * (i * degrees);
                float c = Mathf.Cos(rad);
                float s = Mathf.Sin(rad);

                uv0 = new Vector2(0, 1);
                uv1 = new Vector2(1, 1);
                uv2 = new Vector2(1, 0);
                uv3 = new Vector2(0, 0);

                pos0 = prevX;
                pos1 = new Vector2(outer * c, outer * s);

                if (fill)
                {
                    pos2 = Vector2.zero;
                    pos3 = Vector2.zero;
                }
                else
                {
                    pos2 = new Vector2(inner * c, inner * s);
                    pos3 = prevY;
                }

                prevX = pos1;
                prevY = pos2;

                SetVbo(vbo, new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 });
            }
        }
    }
}