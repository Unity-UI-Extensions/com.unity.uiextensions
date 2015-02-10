///Credit Martin Sharkbomb
///Sourced from - http://forum.unity3d.com/threads/tooltips.264395/#post-1957075

using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Bound Tooltip/Tooltip Trigger")]
    public class BoundTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        public string text;

        public void OnPointerEnter(PointerEventData eventData)
        {
            StartHover(new Vector3(eventData.position.x, eventData.position.y, 0f));
        }
        public void OnSelect(BaseEventData eventData)
        {
            StartHover(transform.position);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            StopHover();
        }
        public void OnDeselect(BaseEventData eventData)
        {
            StopHover();
        }

        void StartHover(Vector3 position)
        {
            BoundTooltipItem.Instance.ShowTooltip(text, position);
        }
        void StopHover()
        {
            BoundTooltipItem.Instance.HideTooltip();
        }
    }
}
