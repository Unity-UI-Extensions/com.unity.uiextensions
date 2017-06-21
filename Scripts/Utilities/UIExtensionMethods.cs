/// Credit Simon (darkside) Jackson
/// Sourced from - My head
namespace UnityEngine.UI.Extensions
{
    public static class UIExtensionMethods
    {
        public static Canvas GetParentCanvas(this RectTransform rt)
        {
            RectTransform parent = rt;
            Canvas parentCanvas = rt.GetComponent<Canvas>();

            int SearchIndex = 0;
            while (parentCanvas == null || SearchIndex > 50)
            {
                parentCanvas = rt.GetComponentInParent<Canvas>();
                if (parentCanvas == null)
                {
                    parent = parent.parent.GetComponent<RectTransform>();
                    SearchIndex++;
                }
            }
            return parentCanvas;
        }

    }
}
