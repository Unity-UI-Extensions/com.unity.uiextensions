///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.grid-raw-image

namespace UnityEngine.UI.Extensions
{
    public interface IBitArray
    {
        /// <summary>Number of elements in the bit array.</summary>
        uint Capacity { get; }
        /// <summary>True if all bits are 0.</summary>
        bool AllFalse { get; }
        /// <summary>True if all bits are 1.</summary>
        bool AllTrue { get; }
        /// <summary>
        /// Returns the state of the bit at a specific index.
        /// </summary>
        /// <param name="index">Index of the bit.</param>
        /// <returns>State of the bit at the provided index.</returns>
        bool this[uint index] { get; set; }
        /// <summary>Returns the bit array in a human readable form.</summary>
        string HumanizedData { get; }

        /// <summary>
        /// Bit-wise And operation.
        /// </summary>
        /// <param name="other">Bit array with which to the And operation.</param>
        /// <returns>The resulting bit array.</returns>
        IBitArray BitAnd(IBitArray other);

        /// <summary>
        /// Bit-wise Or operation.
        /// </summary>
        /// <param name="other">Bit array with which to the Or operation.</param>
        /// <returns>The resulting bit array.</returns>
        IBitArray BitOr(IBitArray other);

        /// <summary>
        /// Invert the bit array.
        /// </summary>
        /// <returns></returns>
        IBitArray BitNot();
    }
}