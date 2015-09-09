/// Credit Breyer
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/#post-1777407

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(Text), typeof(RectTransform))]
    [AddComponentMenu("UI/Effects/Extensions/Curved Text")]
    public class CurvedText : BaseMeshEffect
    {
        public AnimationCurve curveForText = AnimationCurve.Linear(0, 0, 1, 10);
        public float curveMultiplier = 1;
        private RectTransform rectTrans;


#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (curveForText[0].time != 0)
            {
                var tmpRect = curveForText[0];
                tmpRect.time = 0;
                curveForText.MoveKey(0, tmpRect);
            }
            if (rectTrans == null)
                rectTrans = GetComponent<RectTransform>();
            if (curveForText[curveForText.length - 1].time != rectTrans.rect.width)
                OnRectTransformDimensionsChange();
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
            for (int index = 0; index < verts.Length; index++)
            {
                var uiVertex = verts[index];
                //Debug.Log ();
                uiVertex.y += curveForText.Evaluate(rectTrans.rect.width * rectTrans.pivot.x + uiVertex.x) * curveMultiplier;
                verts[index] = uiVertex;
            }
            mesh.vertices = verts;
        }
        protected override void OnRectTransformDimensionsChange()
        {
            var tmpRect = curveForText[curveForText.length - 1];
            tmpRect.time = rectTrans.rect.width;
            curveForText.MoveKey(curveForText.length - 1, tmpRect);
        }
    }
}
