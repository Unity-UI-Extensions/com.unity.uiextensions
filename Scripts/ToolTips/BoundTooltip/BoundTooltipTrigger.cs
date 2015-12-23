///Credit Martin Nerurkar // www.martin.nerurkar.de // www.sharkbombs.com
///Sourced from - http://www.sharkbombs.com/2015/02/10/tooltips-with-the-new-unity-ui-ugui/

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
