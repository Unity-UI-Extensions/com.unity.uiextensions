/// Credit SimonDarksideJ
/// Sourced from: https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/issues/348/menu-manager-does-not-work-with-the-new


namespace UnityEngine.UI.Extensions
{
    public static class UIExtensionsInputManager
    {
        public static bool GetMouseButton(int button)
        {
            return Input.GetMouseButton(button);
        }

        public static bool GetMouseButtonDown(int button)
        {
            return Input.GetMouseButtonDown(button);
        }

        public static bool GetMouseButtonUp(int button)
        {
            return Input.GetMouseButtonUp(button);
        }

        public static bool GetButton(string input)
        {
            return Input.GetButton(input);
        }

        public static bool GetButtonDown(string input)
        {
            return Input.GetButtonDown(input);
        }

        public static bool GetButtonUp(string input)
        {
            return Input.GetButtonUp(input);
        }

        public static bool GetKey(KeyCode key)
        {
            return Input.GetKey(key);
        }

        public static bool GetKeyDown(KeyCode key)
        {
            return Input.GetKeyDown(key);
        }

        public static bool GetKeyUp(KeyCode key)
        {
            return Input.GetKeyUp(key);
        }

        public static float GetAxisRaw(string axis)
        {
            return Input.GetAxisRaw(axis);
        }

        public static Vector3 MousePosition
        {
            get
            {
                return Input.mousePosition;
            }
        }
    }
}
