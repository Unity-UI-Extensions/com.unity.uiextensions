/// Credit Anonymous donation
/// Sourced from - https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/issues/120/horizontal-scroll-snap-scroll-bar-fix
/// Updated by ddreaper - Made extension support all types of scroll snap

using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    public class ScrollSnapScrollbarHelper : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {

        internal IScrollSnap ss;

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnScrollBarDown();
        }

        public void OnDrag(PointerEventData eventData)
        {
            ss.CurrentPage();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnScrollBarUp();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnScrollBarDown();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnScrollBarUp();
        }

        void OnScrollBarDown()
        {
            if (ss != null)
            {
                ss.SetLerp(false);
                ss.StartScreenChange();
            }
        }

        void OnScrollBarUp()
        {
            ss.SetLerp(true);
            ss.ChangePage(ss.CurrentPage());
        }
    }
}