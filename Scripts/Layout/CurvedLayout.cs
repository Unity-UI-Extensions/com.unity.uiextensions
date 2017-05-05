/// <summary>
/// Curved Layout Group Created by Freezy - http://www.ElicitIce.com
/// Posted on Unity Forums http://forum.unity3d.com/threads/curved-layout.403985/
/// 
/// Free for any use and alteration, source code may not be sold without my permission.
/// If you make improvements on this script please share them with the community.
/// 
/// </summary>

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// TODO:
    /// - add automatic child sizing, like in the HorizontalOrVerticalLayoutGroup.cs
    /// - nicer anchor handling for initial child positions
    /// </summary>
    [AddComponentMenu("Layout/Extensions/Curved Layout")]
    public class CurvedLayout : LayoutGroup {
        public Vector3 CurveOffset;

        // Yes these two could be combined into a single vector
        // but this makes it easier to use?
        [Tooltip("axis along which to place the items, Normalized before use")]
        public Vector3 itemAxis;
        [Tooltip("size of each item along the Normalized axis")]
        public float itemSize;

        // the slope can be moved by altering this setting, it could be constrained to the 0-1 range, but other values are usefull for animations
        public float centerpoint = 0.5f;

        protected override void OnEnable() { base.OnEnable(); CalculateRadial(); }
        public override void SetLayoutHorizontal() {
        }
        public override void SetLayoutVertical() {
        }
        public override void CalculateLayoutInputVertical() {
            CalculateRadial();
        }
        public override void CalculateLayoutInputHorizontal() {
            CalculateRadial();
        }
#if UNITY_EDITOR
        protected override void OnValidate() {
            base.OnValidate();
            CalculateRadial();
        }
#endif

        void CalculateRadial() {
            m_Tracker.Clear();
            if (transform.childCount == 0)
                return;

            //one liner for figuring out the desired pivot (should be moved into a utility function)
            Vector2 pivot = new Vector2(((int)childAlignment % 3) * 0.5f, ((int)childAlignment / 3) * 0.5f);

            //this seems to work ok-ish
            Vector3 lastPos = new Vector3(
                GetStartOffset(0, GetTotalPreferredSize(0)),
                GetStartOffset(1, GetTotalPreferredSize(1)),
                0f
            );

            // 0 = first, 1 = last child
            float lerp = 0;
            //no need to catch divide by 0 as childCount > 0
            float step = 1f / transform.childCount;

            //normalize and create a distance between items
            var dist = itemAxis.normalized * itemSize;

            for (int i = 0; i < transform.childCount; i++) {
                RectTransform child = (RectTransform)transform.GetChild(i);
                if (child != null) {
                    //stop the user from altering certain values in the editor
                    m_Tracker.Add(this, child,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.Pivot);
                    Vector3 vPos = lastPos + dist;

                    child.localPosition = lastPos = vPos + (lerp - centerpoint) * CurveOffset;

                    child.pivot = pivot;
                    //child anchors are not yet calculated, each child should set it's own size for now
                    child.anchorMin = child.anchorMax = new Vector2(0.5f, 0.5f);
                    lerp += step;
                }
            }

        }
    }
}
