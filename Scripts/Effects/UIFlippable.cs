/// Credit ChoMPHi
/// Sourced from - http://forum.unity3d.com/threads/script-flippable-for-ui-graphics.291711/

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(RectTransform), typeof(Graphic)), DisallowMultipleComponent]
    [AddComponentMenu("UI/Effects/Extensions/Flippable")]
    public class UIFlippable : BaseMeshEffect
    {     
        [SerializeField] private bool m_Horizontal = false;
        [SerializeField] private bool m_Veritical = false;

#if UNITY_EDITOR
        protected override void Awake()
        {
            OnValidate();
        }
#endif

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UnityEngine.UI.UIFlippable"/> should be flipped horizontally.
        /// </summary>
        /// <value><c>true</c> if horizontal; otherwise, <c>false</c>.</value>
        public bool horizontal
        {
            get { return this.m_Horizontal; }
            set { this.m_Horizontal = value; }
        }
     
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UnityEngine.UI.UIFlippable"/> should be flipped vertically.
        /// </summary>
        /// <value><c>true</c> if vertical; otherwise, <c>false</c>.</value>
        public bool vertical
        {
            get { return this.m_Veritical; }
            set { this.m_Veritical = value; }
        }

        public override void ModifyMesh(VertexHelper verts)
        {
            RectTransform rt = this.transform as RectTransform;
         
            for (int i = 0; i < verts.currentVertCount; ++i)
            {
                UIVertex uiVertex = new UIVertex();
                verts.PopulateUIVertex(ref uiVertex,i);

                // Modify positions
                uiVertex.position = new Vector3(
                    (this.m_Horizontal ? (uiVertex.position.x + (rt.rect.center.x - uiVertex.position.x) * 2) : uiVertex.position.x),
                    (this.m_Veritical ?  (uiVertex.position.y + (rt.rect.center.y - uiVertex.position.y) * 2) : uiVertex.position.y),
                    uiVertex.position.z
                );

                // Apply
                verts.SetUIVertex(uiVertex, i);
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            var components = gameObject.GetComponents(typeof(BaseMeshEffect));
            foreach (var comp in components)
            {
                if (comp.GetType() != typeof(UIFlippable))
                {
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(this);
                }
                else break;
            }
            this.GetComponent<Graphic>().SetVerticesDirty();
            base.OnValidate();
        }
#endif
    }
}