///Credit judah4
///Sourced from - http://forum.unity3d.com/threads/color-picker.267043/

using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    public class HsvBoxSelector : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        public HSVPicker picker;

        void PlaceCursor(PointerEventData eventData)
        {

            var pos = new Vector2(eventData.position.x - picker.hsvImage.rectTransform.position.x, picker.hsvImage.rectTransform.rect.height * picker.hsvImage.transform.lossyScale.y - (picker.hsvImage.rectTransform.position.y - eventData.position.y));
            // Debug.Log(pos);
            pos.x /= picker.hsvImage.rectTransform.rect.width * picker.hsvImage.transform.lossyScale.x;
            pos.y /= picker.hsvImage.rectTransform.rect.height * picker.hsvImage.transform.lossyScale.y;

            pos.x = Mathf.Clamp(pos.x, 0, .9999f);  //1 is the same as 0
            pos.y = Mathf.Clamp(pos.y, 0, .9999f);

            //Debug.Log(pos);
            picker.MoveCursor(pos.x, pos.y);
        }


        public void OnDrag(PointerEventData eventData)
        {
            PlaceCursor(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            PlaceCursor(eventData);
        }
    }
}