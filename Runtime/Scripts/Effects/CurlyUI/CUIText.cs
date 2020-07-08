/// Credit Titinious (https://github.com/Titinious)
/// Sourced from - https://github.com/Titinious/CurlyUI

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Text))]
    [AddComponentMenu("UI/Effects/Extensions/Curly UI Text")]
    public class CUIText : CUIGraphic
    {
        public override void ReportSet()
        {
            if (uiGraphic == null)
                uiGraphic = GetComponent<Text>();

            base.ReportSet();
        }
    }
}