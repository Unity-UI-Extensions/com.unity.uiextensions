/// Credit koohddang
/// Sourced from - http://forum.unity3d.com/threads/onfillvbo-to-onpopulatemesh-help.353977/#post-2299311

using System;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/Diamond Graph")]
    public class DiamondGraph : UIPrimitiveBase
    {
        [SerializeField]
        private float m_a = 1;
        [SerializeField]
        private float m_b = 1;
        [SerializeField]
        private float m_c = 1;
        [SerializeField]
        private float m_d = 1;


        public float A
        {
            get { return m_a; }
            set { m_a = value; }
        }

        public float B
        {
            get { return m_b; }
            set { m_b = value; }
        }

        public float C
        {
            get { return m_c; }
            set { m_c = value; }
        }

        public float D
        {
            get { return m_d; }
            set { m_d = value; }
        }


        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            float wHalf = rectTransform.rect.width / 2;
            //float hHalf = rectTransform.rect.height / 2;
            m_a = Math.Min(1, Math.Max(0, m_a));
            m_b = Math.Min(1, Math.Max(0, m_b));
            m_c = Math.Min(1, Math.Max(0, m_c));
            m_d = Math.Min(1, Math.Max(0, m_d));

            Color32 color32 = color;
            vh.AddVert(new Vector3(-wHalf * m_a, 0), color32, new Vector2(0f, 0f));
            vh.AddVert(new Vector3(0, wHalf * m_b), color32, new Vector2(0f, 1f));
            vh.AddVert(new Vector3(wHalf * m_c, 0), color32, new Vector2(1f, 1f));
            vh.AddVert(new Vector3(0, -wHalf * m_d), color32, new Vector2(1f, 0f));

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }
    }
}