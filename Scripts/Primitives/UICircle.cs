/// Credit zge, jeremie sellam
/// Sourced from - http://forum.unity3d.com/threads/draw-circles-or-primitives-on-the-new-ui-canvas.272488/#post-2293224
/// Updated from - https://bitbucket.org/ddreaper/unity-ui-extensions/issues/65/a-better-uicircle

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/UI Circle")]
    public class UICircle : UIPrimitiveBase
    {
        [Tooltip("The circular fill percentage of the primitive, affected by FixedToSegments")]
        [Range(0, 100)]
        public int fillPercent = 100;
        [Tooltip("Should the primitive fill draw by segments or absolute percentage")]
        public bool FixedToSegments = false;
        [Tooltip("Draw the primitive filled or as a line")]
        public bool fill = true;
        [Tooltip("If not filled, the thickness of the primitive line")]
        public float thickness = 5;
        [Tooltip("The number of segments to draw the primitive, more segments = smoother primitive")]
        [Range(0, 360)]
        public int segments = 360;

        void Update()
        {
            this.thickness = (float)Mathf.Clamp(this.thickness, 0, rectTransform.rect.width / 2);
        }
     
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            float outer = -rectTransform.pivot.x * rectTransform.rect.width;
            float inner = -rectTransform.pivot.x * rectTransform.rect.width + this.thickness;
     
            vh.Clear();
     
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

            if (FixedToSegments)
            {
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

                    StepThroughPointsAndFill(outer, inner, ref prevX, ref prevY, out pos0, out pos1, out pos2, out pos3, c, s);

                    vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
                }
            }
            else
            {
                float tw = rectTransform.rect.width;
                float th = rectTransform.rect.height;

                float angleByStep = (fillPercent / 100f * (Mathf.PI * 2f)) / segments;
                float currentAngle = 0f;
                for (int i = 0; i < segments + 1; i++)
                {

                    float c = Mathf.Cos(currentAngle);
                    float s = Mathf.Sin(currentAngle);

                    StepThroughPointsAndFill(outer, inner, ref prevX, ref prevY, out pos0, out pos1, out pos2, out pos3, c, s);

                    uv0 = new Vector2(pos0.x / tw + 0.5f, pos0.y / th + 0.5f);
                    uv1 = new Vector2(pos1.x / tw + 0.5f, pos1.y / th + 0.5f);
                    uv2 = new Vector2(pos2.x / tw + 0.5f, pos2.y / th + 0.5f);
                    uv3 = new Vector2(pos3.x / tw + 0.5f, pos3.y / th + 0.5f);

                    vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));

                    currentAngle += angleByStep;
                }
            }
        }

        private void StepThroughPointsAndFill(float outer, float inner, ref Vector2 prevX, ref Vector2 prevY, out Vector2 pos0, out Vector2 pos1, out Vector2 pos2, out Vector2 pos3, float c, float s)
        {
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
        }

    }
}