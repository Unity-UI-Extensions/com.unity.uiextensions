/// Credit SimonDarksideJ
/// Sourced from: https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/issues/348/menu-manager-does-not-work-with-the-new

using System;
using System.Collections.Generic;

#if (UNITY_2019 || UNITY_2020) && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

namespace UnityEngine.UI.Extensions
{
    public static class UIExtensionsInputManager
    {
#if (UNITY_2019 || UNITY_2020) && !ENABLE_LEGACY_INPUT_MANAGER
        private static bool[] mouseButtons = new bool[3] { false, false, false };
        private static Dictionary<KeyCode, bool> keys = new Dictionary<KeyCode, bool>();
        private static Dictionary<String, bool> buttons = new Dictionary<String, bool>();
#endif

        public static bool GetMouseButton(int button)
        {
#if (UNITY_2019 || UNITY_2020) && !ENABLE_LEGACY_INPUT_MANAGER
            if (Mouse.current == null)
            {
                return false;
            }

            return Mouse.current.leftButton.isPressed;
#else
            return Input.GetMouseButton(button);
#endif
        }

        public static bool GetMouseButtonDown(int button)
        {
#if (UNITY_2019 || UNITY_2020) && !ENABLE_LEGACY_INPUT_MANAGER
            if (Mouse.current == null)
            {
                return false;
            }

            if (Mouse.current.leftButton.isPressed)
            {
                if (!mouseButtons[button])
                {
                    mouseButtons[button] = true;
                    return true;
                }
            }
            return false;
#else
            return Input.GetMouseButtonDown(button);
#endif
        }

        public static bool GetMouseButtonUp(int button)
        {
#if (UNITY_2019 || UNITY_2020) && !ENABLE_LEGACY_INPUT_MANAGER
            if (Mouse.current == null)
            {
                return false;
            }

            if (mouseButtons[button] && !Mouse.current.leftButton.isPressed)
            {
                mouseButtons[button] = false;
                return true;
            }
            return false;
#else
            return Input.GetMouseButtonUp(button);
#endif
        }

        public static bool GetButton(string input)
        {
#if (UNITY_2019 || UNITY_2020) && !ENABLE_LEGACY_INPUT_MANAGER
            ButtonControl buttonPressed = GetButtonControlFromString(input);

            if (!buttons.ContainsKey(input))
            {
                buttons.Add(input, false);
            }

            return buttonPressed != null ? buttonPressed.isPressed : false;
#else
            return Input.GetButton(input);
#endif
        }

#if (UNITY_2019 || UNITY_2020) && !ENABLE_LEGACY_INPUT_MANAGER
        private static ButtonControl GetButtonControlFromString(string input)
        {
            if (Gamepad.current == null)
            {
                return null;
            }

            switch (input)
            {
                case "Submit":
                    return Gamepad.current.aButton;
                case "Cancel":
                    return Gamepad.current.bButton;
                default:
                    return null;
            }
        }
#endif

        public static bool GetButtonDown(string input)
        {
#if (UNITY_2019 || UNITY_2020) && !ENABLE_LEGACY_INPUT_MANAGER
            ButtonControl buttonPressed = GetButtonControlFromString(input);

            if (buttonPressed.isPressed)
            {
                if (!buttons.ContainsKey(input))
                {
                    buttons.Add(input, false);
                }

                if (!buttons[input])
                {
                    buttons[input] = true;
                    return true;
                }
            }
            else
            {
                buttons[input] = false;
            }
            return false;
#else
            return Input.GetButtonDown(input);
#endif
        }

        public static bool GetButtonUp(string input)
        {
#if (UNITY_2019 || UNITY_2020) && !ENABLE_LEGACY_INPUT_MANAGER
            ButtonControl buttonPressed = GetButtonControlFromString(input);

            if (buttons[input] && !buttonPressed.isPressed)
            {
                buttons[input] = false;
                return true;
            }
            return false;
#else
            return Input.GetButtonUp(input);
#endif
        }

        public static bool GetKey(KeyCode key)
        {
#if (UNITY_2019 || UNITY_2020) && !ENABLE_LEGACY_INPUT_MANAGER
            KeyControl keyPressed = GetKeyControlFromKeyCode(key);
            if (!keys.ContainsKey(key))
            {
                keys.Add(key, false);
            }

            return keyPressed != null ? keyPressed.isPressed : false;
#else
            return Input.GetKey(key);
#endif
        }

#if (UNITY_2019 || UNITY_2020) && !ENABLE_LEGACY_INPUT_MANAGER
        private static KeyControl GetKeyControlFromKeyCode(KeyCode key)
        {
            if (Keyboard.current == null)
            {
                return null;
            }

            switch (key)
            {
                case KeyCode.Escape:
                    return Keyboard.current.escapeKey;
                case KeyCode.KeypadEnter:
                    return Keyboard.current.numpadEnterKey;
                case KeyCode.UpArrow:
                    return Keyboard.current.upArrowKey;
                case KeyCode.DownArrow:
                    return Keyboard.current.downArrowKey;
                case KeyCode.RightArrow:
                    return Keyboard.current.rightArrowKey;
                case KeyCode.LeftArrow:
                    return Keyboard.current.leftArrowKey;
                case KeyCode.LeftShift:
                    return Keyboard.current.leftShiftKey;
                case KeyCode.Tab:
                    return Keyboard.current.tabKey;
                default:
                    return null;
            }
        }
#endif

        public static bool GetKeyDown(KeyCode key)
        {
#if (UNITY_2019 || UNITY_2020) && !ENABLE_LEGACY_INPUT_MANAGER
            KeyControl keyPressed = GetKeyControlFromKeyCode(key);
            if (keyPressed.isPressed)
            {
                if (!keys.ContainsKey(key))
                {
                    keys.Add(key, false);
                }

                if (!keys[key])
                {
                    keys[key] = true;
                    return true;
                }
            }
            else
            {
                keys[key] = false;
            }
            return false;
#else
            return Input.GetKeyDown(key);
#endif
        }

        public static bool GetKeyUp(KeyCode key)
        {
#if (UNITY_2019 || UNITY_2020) && !ENABLE_LEGACY_INPUT_MANAGER
            KeyControl keyPressed = GetKeyControlFromKeyCode(key);
            if (keys[key] && !keyPressed.isPressed)
            {
                keys[key] = false;
                return true;
            }
            return false;
#else
            return Input.GetKeyUp(key);
#endif
        }

        public static float GetAxisRaw(string axis)
        {
#if (UNITY_2019 || UNITY_2020) && !ENABLE_LEGACY_INPUT_MANAGER
            if (Gamepad.current == null)
            {
                return 0f;
            }

            switch (axis)
            {
                case "Horizontal":
                    return Gamepad.current.leftStick.x.ReadValue();
                case "Vertical":
                    return Gamepad.current.leftStick.y.ReadValue();

            }
            return 0f;
#else
            return Input.GetAxisRaw(axis);
#endif
        }

        public static Vector3 MousePosition
        {
            get
            {
#if (UNITY_2019 || UNITY_2020) && !ENABLE_LEGACY_INPUT_MANAGER
                return Mouse.current.position.ReadValue();
#else
                return Input.mousePosition;
#endif
            }
        }

        public static Vector3 MouseScrollDelta
        {
            get
            {
#if (UNITY_2019 || UNITY_2020) && !ENABLE_LEGACY_INPUT_MANAGER
                return Mouse.current.position.ReadValue();
#else
                return Input.mouseScrollDelta;
#endif
            }
        }
    }
}
