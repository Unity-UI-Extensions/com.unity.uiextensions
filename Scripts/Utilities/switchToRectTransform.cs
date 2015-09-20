/// Credit Izitmee
/// Sourced from - http://forum.unity3d.com/threads/find-anchoredposition-of-a-recttransform-relative-to-another-recttransform.330560/#post-2300992
/// Updated by Brave Michael - http://forum.unity3d.com/threads/find-anchoredposition-of-a-recttransform-relative-to-another-recttransform.330560/#post-2300992

namespace UnityEngine.UI.Extensions
{
    public static class RectTransformExtension
    {
        /// <summary>
        /// Converts the anchoredPosition of the first RectTransform to the second RectTransform,
        /// taking into consideration offset, anchors and pivot, and returns the new anchoredPosition
        /// </summary>
        public static Vector2 switchToRectTransform(this RectTransform from, RectTransform to)
        {
            Vector2 localPoint;
            Vector2 fromPivotDerivedOffset = new Vector2(from.rect.width * from.pivot.x + from.rect.xMin, from.rect.height * from.pivot.y + from.rect.yMin);
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, from.position);
            screenP += fromPivotDerivedOffset;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out localPoint);
            Vector2 pivotDerivedOffset = new Vector2(to.rect.width * to.pivot.x + to.rect.xMin, to.rect.height * to.pivot.y + to.rect.yMin);
            return to.anchoredPosition + localPoint - pivotDerivedOffset;
        }
    }
}