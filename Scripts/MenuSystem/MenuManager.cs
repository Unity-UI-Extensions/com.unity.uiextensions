/// Credit Adam Kapos (Nezz) - http://www.songarc.net
/// Sourced from - https://github.com/YousicianGit/UnityMenuSystem
/// Updated by SimonDarksideJ - Refactored to be a more generic component

using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Menu Manager")]
    [DisallowMultipleComponent]
    public class MenuManager : MonoBehaviour
    {
        public Menu[] MenuScreens;
        public int StartScreen = 0;
        private Stack<Menu> menuStack = new Stack<Menu>();

        public static MenuManager Instance { get; set; }

        private void Awake()
        {
            Instance = this;
            if (MenuScreens.Length > 0 + StartScreen)
            {
                CreateInstance(MenuScreens[StartScreen].name);
                OpenMenu(MenuScreens[StartScreen]);
            }
            else
            {
                Debug.LogError("Not enough Menu Screens configured");
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void CreateInstance<T>() where T : Menu
        {
            var prefab = GetPrefab<T>();

            Instantiate(prefab, transform);
        }

        public void CreateInstance(string MenuName) 
        {
            var prefab = GetPrefab(MenuName);

            Instantiate(prefab, transform);
        }

        public void OpenMenu(Menu instance)
        {
            // De-activate top menu
            if (menuStack.Count > 0)
            {
                if (instance.DisableMenusUnderneath)
                {
                    foreach (var menu in menuStack)
                    {
                        menu.gameObject.SetActive(false);

                        if (menu.DisableMenusUnderneath)
                            break;
                    }
                }

                var topCanvas = instance.GetComponent<Canvas>();
                var previousCanvas = menuStack.Peek().GetComponent<Canvas>();
                topCanvas.sortingOrder = previousCanvas.sortingOrder + 1;
            }

            menuStack.Push(instance);
        }

        private GameObject GetPrefab(string PrefabName)
        {
            for (int i = 0; i < MenuScreens.Length; i++)
            {
                if (MenuScreens[i].name == PrefabName)
                {
                    return MenuScreens[i].gameObject;
                }
            }
            throw new MissingReferenceException("Prefab not found for " + PrefabName);
        }

        private T GetPrefab<T>() where T : Menu
        {
            // Get prefab dynamically, based on public fields set from Unity
            // You can use private fields with SerializeField attribute too
            var fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var field in fields)
            {
                var prefab = field.GetValue(this) as T;
                if (prefab != null)
                {
                    return prefab;
                }
            }

            throw new MissingReferenceException("Prefab not found for type " + typeof(T));
        }

        public void CloseMenu(Menu menu)
        {
            if (menuStack.Count == 0)
            {
                Debug.LogErrorFormat(menu, "{0} cannot be closed because menu stack is empty", menu.GetType());
                return;
            }

            if (menuStack.Peek() != menu)
            {
                Debug.LogErrorFormat(menu, "{0} cannot be closed because it is not on top of stack", menu.GetType());
                return;
            }

            CloseTopMenu();
        }

        public void CloseTopMenu()
        {
            var instance = menuStack.Pop();

            if (instance.DestroyWhenClosed)
                Destroy(instance.gameObject);
            else
                instance.gameObject.SetActive(false);

            // Re-activate top menu
            // If a re-activated menu is an overlay we need to activate the menu under it
            foreach (var menu in menuStack)
            {
                menu.gameObject.SetActive(true);

                if (menu.DisableMenusUnderneath)
                    break;
            }
        }

        private void Update()
        {
            // On Android the back button is sent as Esc
            if (Input.GetKeyDown(KeyCode.Escape) && menuStack.Count > 0)
            {
                menuStack.Peek().OnBackPressed();
            }
        }
    }

}