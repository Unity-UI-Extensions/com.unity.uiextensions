/// adaption for cylindrical bending by herbst
/// Credit Breyer
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/#post-1777407

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(Text), typeof(RectTransform))]
    [AddComponentMenu("UI/Effects/Extensions/Cylinder Text")]
    public class CylinderText : BaseMeshEffect
    {
        public float radius;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
        }
#endif
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
        public override void ModifyMesh(VertexHelper vh)
        {
            if (! IsActive()) return;

            int count = vh.currentVertCount;
            if (!IsActive() || count == 0)
            {
                return;
            }
            for (int index = 0; index < vh.currentVertCount; index++)
            {
                UIVertex uiVertex = new UIVertex();
                vh.PopulateUIVertex(ref uiVertex, index);

                // get x position
                var x = uiVertex.position.x;                

                // calculate bend based on pivot and radius
                uiVertex.position.z = -radius * Mathf.Cos(x / radius);
                uiVertex.position.x = radius * Mathf.Sin(x / radius);
                
                vh.SetUIVertex(uiVertex, index);
            }
        }
    }
}
