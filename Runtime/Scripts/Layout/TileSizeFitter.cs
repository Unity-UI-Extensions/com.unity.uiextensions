/// Credit Ges
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-3#post-2280109

using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Layout/Extensions/Tile Size Fitter")]
    public class TileSizeFitter : UIBehaviour, ILayoutSelfController
    {
        [SerializeField]
        private Vector2 m_Border = Vector2.zero;
        public Vector2 Border { get { return m_Border; } set { if (SetPropertyUtility.SetStruct(ref m_Border, value)) SetDirty(); } }

        [SerializeField]
        private Vector2 m_TileSize = Vector2.zero;
        public Vector2 TileSize { get { return m_TileSize; } set { if (SetPropertyUtility.SetStruct(ref m_TileSize, value)) SetDirty(); } }

        [System.NonSerialized]
        private RectTransform m_Rect;
        private RectTransform rectTransform { get { if (m_Rect == null) m_Rect = GetComponent<RectTransform>(); return m_Rect; } }

        private DrivenRectTransformTracker m_Tracker;

        #region Unity Lifetime calls

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        #endregion

        protected override void OnRectTransformDimensionsChange()
        {
            UpdateRect();
        }

        private void UpdateRect()
        {
            if (!IsActive())
                return;

            m_Tracker.Clear();

            m_Tracker.Add(this, rectTransform,
                DrivenTransformProperties.Anchors |
                DrivenTransformProperties.AnchoredPosition);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;

            m_Tracker.Add(this, rectTransform,
                DrivenTransformProperties.SizeDeltaX |
                DrivenTransformProperties.SizeDeltaY);
            Vector2 sizeDelta = GetParentSize() - Border;
            if (TileSize.x > 0.001f)
                sizeDelta.x -= Mathf.Floor(sizeDelta.x / TileSize.x) * TileSize.x;
            else
                sizeDelta.x = 0;
            if (TileSize.y > 0.001f)
                sizeDelta.y -= Mathf.Floor(sizeDelta.y / TileSize.y) * TileSize.y;
            else
                sizeDelta.y = 0;
            rectTransform.sizeDelta = -sizeDelta;
        }

        private Vector2 GetParentSize()
        {
            RectTransform parent = rectTransform.parent as RectTransform;
            if (!parent)
                return Vector2.zero;
            return parent.rect.size;
        }

        public virtual void SetLayoutHorizontal() { }
        public virtual void SetLayoutVertical() { }

        protected void SetDirty()
        {
            if (!IsActive())
                return;

            UpdateRect();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            m_TileSize.x = Mathf.Clamp(m_TileSize.x, 0.001f, 1000f);
            m_TileSize.y = Mathf.Clamp(m_TileSize.y, 0.001f, 1000f);
            SetDirty();
        }
#endif
    }
}