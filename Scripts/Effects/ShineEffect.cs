/// Credit ömer faruk sayılır
/// Sourced from - https://bitbucket.org/snippets/Lordinarius/nrn4L

namespace UnityEngine.UI.Extensions
{
    public class ShineEffect : MaskableGraphic
    {

        [SerializeField]
        float yoffset = -1;

        public float Yoffset
        {
            get
            {
                return yoffset;
            }
            set
            {
                SetVerticesDirty();
                yoffset = value;
            }
        }

        [SerializeField]
        float width = 1;

        public float Width
        {
            get
            {
                return width;
            }
            set
            {
                SetAllDirty();
                width = value;
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            var r = GetPixelAdjustedRect();
            var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);
            float dif = (v.w - v.y) * 2;
            Color32 color32 = color;
            vh.Clear();

            color32.a = (byte)0;
            vh.AddVert(new Vector3(v.x - 50, width * v.y + yoffset * dif), color32, new Vector2(0f, 0f));
            vh.AddVert(new Vector3(v.z + 50, width * v.y + yoffset * dif), color32, new Vector2(1f, 0f));

            color32.a = (byte)(color.a * 255);
            vh.AddVert(new Vector3(v.x - 50, width * (v.y / 4) + yoffset * dif), color32, new Vector2(0f, 1f));
            vh.AddVert(new Vector3(v.z + 50, width * (v.y / 4) + yoffset * dif), color32, new Vector2(1f, 1f));

            color32.a = (byte)(color.a * 255);
            vh.AddVert(new Vector3(v.x - 50, width * (v.w / 4) + yoffset * dif), color32, new Vector2(0f, 1f));
            vh.AddVert(new Vector3(v.z + 50, width * (v.w / 4) + yoffset * dif), color32, new Vector2(1f, 1f));
            color32.a = (byte)(color.a * 255);

            color32.a = (byte)0;
            vh.AddVert(new Vector3(v.x - 50, width * v.w + yoffset * dif), color32, new Vector2(0f, 1f));
            vh.AddVert(new Vector3(v.z + 50, width * v.w + yoffset * dif), color32, new Vector2(1f, 1f));

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 1);

            vh.AddTriangle(2, 3, 4);
            vh.AddTriangle(4, 5, 3);

            vh.AddTriangle(4, 5, 6);
            vh.AddTriangle(6, 7, 5);
        }

        public void Triangulate(VertexHelper vh)
        {
            int triangleCount = vh.currentVertCount - 2;
            Debug.Log(triangleCount);
            for (int i = 0; i <= triangleCount / 2 + 1; i += 2)
            {
                vh.AddTriangle(i, i + 1, i + 2);
                vh.AddTriangle(i + 2, i + 3, i + 1);
            }
        }

#if UNITY_EDITOR
        public override void OnRebuildRequested()
        {
            base.OnRebuildRequested();
        }
#endif

    }
}