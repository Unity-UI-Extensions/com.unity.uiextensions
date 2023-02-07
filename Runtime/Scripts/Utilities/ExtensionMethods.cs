using System;

namespace UnityEngine.UI.Extensions
{
    public static class ExtensionMethods
    {
        public static T GetOrAddComponent<T>(this GameObject child) where T : Component
        {
            T result = child.GetComponent<T>();
            if (result == null)
            {
                result = child.AddComponent<T>();
            }
            return result;
        }

        public static bool IsPrefab(this GameObject gameObject)
        {
            if (gameObject == null)
            {
                throw new ArgumentNullException(nameof(gameObject));
            }

            return
                !gameObject.scene.IsValid() &&
                !gameObject.scene.isLoaded &&
                gameObject.GetInstanceID() >= 0 &&
                // I noticed that ones with IDs under 0 were objects I didn't recognize
                !gameObject.hideFlags.HasFlag(HideFlags.HideInHierarchy);
                    // I don't care about GameObjects *inside* prefabs, just the overall prefab.
        }

        /// <summary>
        /// Generic clamp method to limt a value between a range of values
        /// </summary>
        /// <typeparam name="T"><see cref="IComparable"/> data type</typeparam>
        /// <param name="value">Value to clamp</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns></returns>
        public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
            {
                value = min;
            }
            if (value.CompareTo(max) > 0)
            {
                value =  max;
            }

            return value;
        }
    }
}
