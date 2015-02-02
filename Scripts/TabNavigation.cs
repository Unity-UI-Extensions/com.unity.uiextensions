/// Credit Melang 
/// Sourced from - http://forum.unity3d.com/members/melang.593409/
/// Updated omatase 10-18-14 - support for Shift + Tab as well
///                         - bug fix to prevent crash if no control selected
///                         - updated to support new semantics for EventSystem in later 4.6 builds
///                        - autoselect "firstSelectedGameObject" since it doesn't seem to work automatically

using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(EventSystem))]
    [AddComponentMenu("Event/Extensions/Tab Navigation Helper")]
    public class TabNavigationHelper : MonoBehaviour
    {
        private EventSystem _system;

        void Start()
        {
            _system = GetComponent<EventSystem>();
            if (_system == null)
            {
                Debug.LogError("Needs to be attached to the Event System component in the scene");
            }
        }

        public void Update()
        {
            Selectable next = null;

            if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
            {
                if (_system.currentSelectedGameObject != null)
                {
                    next = _system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
                }
                else
                {
                    next = _system.firstSelectedGameObject.GetComponent<Selectable>();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (_system.currentSelectedGameObject != null)
                {
                    next = _system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                }
                else
                {
                    next = _system.firstSelectedGameObject.GetComponent<Selectable>();
                }
            }
            else if (_system.currentSelectedGameObject == null)
            {
                next = _system.firstSelectedGameObject.GetComponent<Selectable>();
            }

            selectGameObject(next);
        }

        private void selectGameObject(Selectable selectable)
        {
            if (selectable != null)
            {
                InputField inputfield = selectable.GetComponent<InputField>();
                if (inputfield != null) inputfield.OnPointerClick(new PointerEventData(_system));  //if it's an input field, also set the text caret

                _system.SetSelectedGameObject(selectable.gameObject, new BaseEventData(_system));
            }
        }
    }
}