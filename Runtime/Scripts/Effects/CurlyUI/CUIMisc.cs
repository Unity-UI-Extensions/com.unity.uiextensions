/// Credit Titinious (https://github.com/Titinious)
/// Sourced from - https://github.com/Titinious/CurlyUI

namespace UnityEngine.UI.Extensions
{
    [System.Serializable]
    public struct Vector3_Array2D
    {
        [SerializeField]
        public Vector3[] array;

        public Vector3 this[int _idx]
        {
            get
            {
                return array[_idx];
            }
            set
            {
                array[_idx] = value;
            }
        }
    }
}