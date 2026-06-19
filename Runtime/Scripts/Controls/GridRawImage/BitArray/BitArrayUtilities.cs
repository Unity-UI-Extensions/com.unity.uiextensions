///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.grid-raw-image

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Bit array utility class.
    /// </summary>
    public static class BitArrayUtilities
    {
        //written here to not duplicate the serialized accessor and runtime accessor

        /// <summary>
        /// Get a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data">Bit array data.</param>
        /// <returns>The value of the bit at the specific index.</returns>
        public static bool Get8(uint index, byte data) => (data & (1u << (int)index)) != 0u;

        /// <summary>
        /// Get a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data">Bit array data.</param>
        /// <returns>The value of the bit at the specific index.</returns>
        public static bool Get16(uint index, ushort data) => (data & (1u << (int)index)) != 0u;

        /// <summary>
        /// Get a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data">Bit array data.</param>
        /// <returns>The value of the bit at the specific index.</returns>
        public static bool Get32(uint index, uint data) => (data & (1u << (int)index)) != 0u;

        /// <summary>
        /// Get a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data">Bit array data.</param>
        /// <returns>The value of the bit at the specific index.</returns>
        public static bool Get64(uint index, ulong data) => (data & (1uL << (int)index)) != 0uL;

        /// <summary>
        /// Get a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data1">Bit array data 1.</param>
        /// <param name="data2">Bit array data 2.</param>
        /// <returns>The value of the bit at the specific index.</returns>
        public static bool Get128(uint index, ulong data1, ulong data2)
            => index < 64u
                ? (data1 & (1uL << (int)index)) != 0uL
                : (data2 & (1uL << (int)(index - 64u))) != 0uL;

        /// <summary>
        /// Get a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data1">Bit array data 1.</param>
        /// <param name="data2">Bit array data 2.</param>
        /// <param name="data3">Bit array data 3.</param>
        /// <param name="data4">Bit array data 4.</param>
        /// <returns>The value of the bit at the specific index.</returns>
        public static bool Get256(uint index, ulong data1, ulong data2, ulong data3, ulong data4)
            => index < 128u ? index < 64u
                    ? (data1 & (1uL << (int)index)) != 0uL
                    : (data2 & (1uL << (int)(index - 64u))) != 0uL
                : index < 192u ? (data3 & (1uL << (int)(index - 128u))) != 0uL
                : (data4 & (1uL << (int)(index - 192u))) != 0uL;

        /// <summary>
        /// Set a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data">Bit array data.</param>
        /// <param name="value">Value to set the bit to.</param>
        public static void Set8(uint index, ref byte data, bool value) => data = (byte)(value ? (data | (1u << (int)index)) : (data & ~(1u << (int)index)));

        /// <summary>
        /// Set a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data">Bit array data.</param>
        /// <param name="value">Value to set the bit to.</param>
        public static void Set16(uint index, ref ushort data, bool value) => data = (ushort)(value ? (data | (1u << (int)index)) : (data & ~(1u << (int)index)));

        /// <summary>
        /// Set a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data">Bit array data.</param>
        /// <param name="value">Value to set the bit to.</param>
        public static void Set32(uint index, ref uint data, bool value) => data = (value ? (data | (1u << (int)index)) : (data & ~(1u << (int)index)));

        /// <summary>
        /// Set a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data">Bit array data.</param>
        /// <param name="value">Value to set the bit to.</param>
        public static void Set64(uint index, ref ulong data, bool value) => data = (value ? (data | (1uL << (int)index)) : (data & ~(1uL << (int)index)));

        /// <summary>
        /// Set a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data1">Bit array data 1.</param>
        /// <param name="data2">Bit array data 2.</param>
        /// <param name="value">Value to set the bit to.</param>
        public static void Set128(uint index, ref ulong data1, ref ulong data2, bool value)

        {
            if (index < 64u)
                data1 = (value ? (data1 | (1uL << (int)index)) : (data1 & ~(1uL << (int)index)));
            else
                data2 = (value ? (data2 | (1uL << (int)(index - 64u))) : (data2 & ~(1uL << (int)(index - 64u))));
        }

        /// <summary>
        /// Set a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data1">Bit array data 1.</param>
        /// <param name="data2">Bit array data 2.</param>
        /// <param name="data3">Bit array data 3.</param>
        /// <param name="data4">Bit array data 4.</param>
        /// <param name="value">Value to set the bit to.</param>
        public static void Set256(uint index, ref ulong data1, ref ulong data2, ref ulong data3, ref ulong data4, bool value)
        {
            if (index < 64u)
                data1 = (value ? (data1 | (1uL << (int)index)) : (data1 & ~(1uL << (int)index)));
            else if (index < 128u)
                data2 = (value ? (data2 | (1uL << (int)(index - 64u))) : (data2 & ~(1uL << (int)(index - 64u))));
            else if (index < 192u)
                data3 = (value ? (data3 | (1uL << (int)(index - 64u))) : (data3 & ~(1uL << (int)(index - 128u))));
            else
                data4 = (value ? (data4 | (1uL << (int)(index - 64u))) : (data4 & ~(1uL << (int)(index - 192u))));
        }
    }
}