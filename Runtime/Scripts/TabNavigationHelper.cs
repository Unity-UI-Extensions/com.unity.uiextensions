/// Credit Melang 
/// Sourced from - http://forum.unity3d.com/members/melang.593409/
/// Updated omatase 10-18-14 - support for Shift + Tab as well
///                         - bug fix to prevent crash if no control selected
///                         - updated to support new semantics for EventSystem in later 4.6 builds
///                        - autoselect "firstSelectedGameObject" since it doesn't seem to work automatically
/// Updated 08-29-15 - On request of Issue #13 on repo, added a manual navigation order.

using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    public enum NavigationMode { Auto = 0, Manual = 1};
    [RequireComponent(typeof(EventSystem))]
    [AddComponentMenu("Event/Extensions/Tab Navigation Helper")]
    public class TabNavigationHelper : MonoBehaviour
    {
        private EventSystem _system;
        private Selectable StartingObject;
        private Selectable LastObject;
        [Tooltip("The path to take when user is tabbing through ui components.")]
        public Selectable[] NavigationPath;
        [Tooltip("Use the default Unity navigation system or a manual fixed order using Navigation Path")]
        public NavigationMode NavigationMode;
        [Tooltip("If True, this will loop the tab order from last to first automatically")]
        public bool CircularNavigation;

        void Start()
        {
            _system = GetComponent<EventSystem>();
            if (_system == null)
            {
                Debug.LogError("Needs to be attached to the Event System component in the scene");
            }
            if (NavigationMode == NavigationMode.Manual && NavigationPath.Length > 0)
            {
                StartingObject = NavigationPath[0].gameObject.GetComponent<Selectable>();
            }
            if (StartingObject == null && CircularNavigation)
            {
                SelectDefaultObject(out StartingObject); 
            }
        }

        public void Update()
        {
            Selectable next = null;
            if (LastObject == null && _system.currentSelectedGameObject != null)
            {
                //Find the last selectable object
                next = _system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                while (next != null)
                {
                    LastObject = next;
                    next = next.FindSelectableOnDown();
                }
            }

            if (UIExtensionsInputManager.GetKeyDown(KeyCode.Tab) && UIExtensionsInputManager.GetKey(KeyCode.LeftShift))
            {
                if (NavigationMode == NavigationMode.Manual && NavigationPath.Length > 0)
                {
                    for (var i = NavigationPath.Length - 1; i >= 0; i--)
                    {
                        if (_system.currentSelectedGameObject != NavigationPath[i].gameObject) continue;

                        next = i == 0 ? NavigationPath[NavigationPath.Length - 1] : NavigationPath[i - 1];

                        break;
                    }
                }
                else
                {
                    if (_system.currentSelectedGameObject != null)
                    {
                        next = _system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
                        if (next == null && CircularNavigation)
                        {
                            next = LastObject;
                        }
                    }
                    else
                    {
                        SelectDefaultObject(out next);
                    }
                }
            }
            else if (UIExtensionsInputManager.GetKeyDown(KeyCode.Tab))
            {
                if (NavigationMode == NavigationMode.Manual && NavigationPath.Length > 0)
                {
                    for (var i = 0; i < NavigationPath.Length; i++)
                    {
                        if (_system.currentSelectedGameObject != NavigationPath[i].gameObject) continue;

                        next = i == (NavigationPath.Length - 1) ? NavigationPath[0] : NavigationPath[i + 1];

                        break;
                    }
                }
                else
                {
                    if (_system.currentSelectedGameObject != null)
                    {
                        next = _system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                        if (next == null && CircularNavigation)
                        {
                            next = StartingObject;
                        }
                    }
                    else
                    {
                        SelectDefaultObject(out next);
                    }
                }
            }
            else if (_system.currentSelectedGameObject == null)
            {
                SelectDefaultObject(out next);
            }

            if (CircularNavigation && StartingObject == null)
            {
                StartingObject = next;
            }
            selectGameObject(next);
        }

        private void SelectDefaultObject(out Selectable next)
        {
            if (_system.firstSelectedGameObject)
            {
                next = _system.firstSelectedGameObject.GetComponent<Selectable>();
            }
            else
            {
                next = null;
            }
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