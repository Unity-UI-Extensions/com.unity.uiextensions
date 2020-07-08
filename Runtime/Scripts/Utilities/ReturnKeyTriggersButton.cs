/// Credit Melang 
/// Sourced from - http://forum.unity3d.com/members/melang.593409/
/// Updated SimonDarksideJ - reworked to 4.6.1 standards

using UnityEngine.EventSystems;
namespace UnityEngine.UI
{
    [RequireComponent(typeof(InputField))]
    [AddComponentMenu("UI/Extensions/Return Key Trigger")]
    public class ReturnKeyTriggersButton : MonoBehaviour, ISubmitHandler
    {
        private EventSystem _system;

        public Button button;
        private bool highlight = true;
        public float highlightDuration = 0.2f;
        
        void Start()
        {
            _system = EventSystem.current;
        }

        void RemoveHighlight()
        {
            button.OnPointerExit(new PointerEventData(_system));
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (highlight) button.OnPointerEnter(new PointerEventData(_system));
            button.OnPointerClick(new PointerEventData(_system));

            if (highlight) Invoke("RemoveHighlight", highlightDuration);
        }
    }
}
