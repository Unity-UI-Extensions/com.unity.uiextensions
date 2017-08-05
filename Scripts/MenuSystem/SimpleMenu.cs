/// Credit Adam Kapos (Nezz) - http://www.songarc.net
/// Sourced from - https://github.com/YousicianGit/UnityMenuSystem
/// Updated by SimonDarksideJ - Refactored to be a more generic component

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// A base menu class that implements parameterless Show and Hide methods
    /// </summary>
    public abstract class SimpleMenu<T> : Menu<T> where T : SimpleMenu<T>
    {
        public static void Show()
        {
            Open();
        }

        public static void Hide()
        {
            Close();
        }
    }
}