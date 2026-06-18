///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.grid-raw-image

using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UI.Extensions
{
    // /!\ Important for serialization:
    // Serialization helper will rely on the name of the struct type.
    // In order to work, it must be BitArrayN where N is the capacity without suffix.

    /// <summary>
    /// Bit array of size 8.
    /// </summary>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{this.GetType().Name} {humanizedData}")]
    public struct BitArray8 : IBitArray
    {
        [SerializeField]
        byte data;

        /// <summary>Number of elements in the bit array.</summary>
        public uint Capacity => 8u;
        /// <summary>True if all bits are 0.</summary>
        public bool AllFalse => data == 0u;
        /// <summary>True if all bits are 1.</summary>
        public bool AllTrue => data == byte.MaxValue;
        /// <summary>Returns the bit array in a human readable form.</summary>
        public string HumanizedData => String.Format("{0, " + Capacity + "}", Convert.ToString(data, 2)).Replace(' ', '0');

        /// <summary>
        /// Returns the state of the bit at a specific index.
        /// </summary>
        /// <param name="index">Index of the bit.</param>
        /// <returns>State of the bit at the provided index.</returns>
        public bool this[uint index]
        {
            get => BitArrayUtilities.Get8(index, data);
            set => BitArrayUtilities.Set8(index, ref data, value);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="initValue">Initialization value.</param>
        public BitArray8(byte initValue) => data = initValue;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bitIndexTrue">List of indices where bits should be set to true.</param>
        public BitArray8(IEnumerable<uint> bitIndexTrue)
        {
            data = (byte)0u;
            if (bitIndexTrue == null)
                return;
            for (int index = bitIndexTrue.Count() - 1; index >= 0; --index)
            {
                uint bitIndex = bitIndexTrue.ElementAt(index);
                if (bitIndex >= Capacity) continue;
                data |= (byte)(1u << (int)bitIndex);
            }
        }

        /// <summary>
        /// Bit-wise Not operator
        /// </summary>
        /// <param name="a">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray8 operator ~(BitArray8 a) => new BitArray8((byte)~a.data);

        /// <summary>
        /// Bit-wise Or operator
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray8 operator |(BitArray8 a, BitArray8 b) => new BitArray8((byte)(a.data | b.data));

        /// <summary>
        /// Bit-wise And operator
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray8 operator &(BitArray8 a, BitArray8 b) => new BitArray8((byte)(a.data & b.data));

        /// <summary>
        /// Bit-wise And
        /// </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public IBitArray BitAnd(IBitArray other) => this & (BitArray8)other;

        /// <summary>
        /// Bit-wise Or
        /// </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public IBitArray BitOr(IBitArray other) => this | (BitArray8)other;

        /// <summary>
        /// Bit-wise Not
        /// </summary>
        /// <returns>The resulting bit array.</returns>
        public IBitArray BitNot() => ~this;

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if both bit arrays are equals.</returns>
        public static bool operator ==(BitArray8 a, BitArray8 b) => a.data == b.data;

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if the bit arrays are not equals.</returns>
        public static bool operator !=(BitArray8 a, BitArray8 b) => a.data != b.data;

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="obj">Bit array to compare to.</param>
        /// <returns>True if the provided bit array is equal to this..</returns>
        public override bool Equals(object obj) => obj is BitArray8 && ((BitArray8)obj).data == data;

        /// <summary>
        /// Get the hashcode of the bit array.
        /// </summary>
        /// <returns>Hashcode of the bit array.</returns>
        public override int GetHashCode() => 1768953197 + data.GetHashCode();
    }
}