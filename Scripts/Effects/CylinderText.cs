/// adaption for cylindrical bending by herbst
/// Credit Breyer
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/#post-1777407

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(Text), typeof(RectTransform))]
    [AddComponentMenu("UI/Effects/Extensions/Cylinder Text")]
    public class CylinderText : BaseMeshEffect
    {
        public float radius = 360;
        private RectTransform rectTrans;


#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (rectTrans == null)
                rectTrans = GetComponent<RectTransform>();
        }
#endif
        protected override void Awake()
        {
            base.Awake();
            rectTrans = GetComponent<RectTransform>();
            OnRectTransformDimensionsChange();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            rectTrans = GetComponent<RectTransform>();
            OnRectTransformDimensionsChange();
        }
        public override void ModifyMesh(Mesh mesh)
        {
            if (!IsActive())
                return;
            Vector3[] verts = mesh.vertices;

            int count = verts.Length;
            if (!IsActive() || count == 0)
            {
                return;
            }
            for (int index = 0; index < count; index++)
            {
                var uiVertex = verts[index];

                // get x position
                var x = uiVertex.x;                

                // calculate bend based on pivot and radius
                uiVertex.z = -radius * Mathf.Cos(x / radius);
                uiVertex.x = radius * Mathf.Sin(x / radius);
                
                verts[index] = uiVertex;
            }
            mesh.vertices = verts;
        }
    }
}
