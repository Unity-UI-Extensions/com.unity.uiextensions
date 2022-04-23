/// Credit Simon (simonDarksideJ) Jackson
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
                    if (parent.parent == null)
                    {
                        return null;
                    }
                    parent = parent.parent.GetComponent<RectTransform>();
                    SearchIndex++;
                }
            }
            return parentCanvas;
        }

        public static Vector2 TransformInputBasedOnCanvasType(this Vector2 input, Canvas canvas)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return canvas.GetEventCamera().ScreenToWorldPoint(input);
            }
            else
            {
                return input;
            }
        }

        public static Vector3 TransformInputBasedOnCanvasType(this Vector2 input, RectTransform rt)
        {
            var canvas = rt.GetParentCanvas();
            if (input == Vector2.zero || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return input;
            }
            else
            {
                // Needs work :S
                Vector2 movePos;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rt,
                    input, canvas.GetEventCamera(),
                    out movePos);

                Vector3 output = canvas.transform.TransformPoint(movePos);
                return output;
            }
        }

        public static Camera GetEventCamera(this Canvas input)
        {
            return input.worldCamera == null ? Camera.main : input.worldCamera;

        }
    }
}
