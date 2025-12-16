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
	/// Bit array of size 256.
	/// </summary>
	[Serializable]
	[System.Diagnostics.DebuggerDisplay("{this.GetType().Name} {humanizedData}")]
	public struct BitArray256 : IBitArray
	{
		[SerializeField]
		ulong data1;
		[SerializeField]
		ulong data2;
		[SerializeField]
		ulong data3;
		[SerializeField]
		ulong data4;

		/// <summary>Number of elements in the bit array.</summary>
		public readonly uint Capacity => 256u;
		/// <summary>True if all bits are 0.</summary>
		public readonly bool AllFalse => data1 == 0uL && data2 == 0uL && data3 == 0uL && data4 == 0uL;
		/// <summary>True if all bits are 1.</summary>
		public readonly bool AllTrue => data1 == ulong.MaxValue && data2 == ulong.MaxValue && data3 == ulong.MaxValue && data4 == ulong.MaxValue;
		/// <summary>Returns the bit array in a human readable form.</summary>
		public readonly string HumanizedData =>
			System.Text.RegularExpressions.Regex.Replace(String.Format("{0, " + 64u + "}", Convert.ToString((long)data4, 2)).Replace(' ', '0'), ".{8}", "$0.")
			+ System.Text.RegularExpressions.Regex.Replace(String.Format("{0, " + 64u + "}", Convert.ToString((long)data3, 2)).Replace(' ', '0'), ".{8}", "$0.")
			+ System.Text.RegularExpressions.Regex.Replace(String.Format("{0, " + 64u + "}", Convert.ToString((long)data2, 2)).Replace(' ', '0'), ".{8}", "$0.")
			+ System.Text.RegularExpressions.Regex.Replace(String.Format("{0, " + 64u + "}", Convert.ToString((long)data1, 2)).Replace(' ', '0'), ".{8}", "$0.").TrimEnd('.');

		/// <summary>
		/// Returns the state of the bit at a specific index.
		/// </summary>
		/// <param name="index">Index of the bit.</param>
		/// <returns>State of the bit at the provided index.</returns>
		public bool this[uint index]
		{
			get => BitArrayUtilities.Get256(index, data1, data2, data3, data4);
			set => BitArrayUtilities.Set256(index, ref data1, ref data2, ref data3, ref data4, value);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="initValue1">Initialization value 1.</param>
		/// <param name="initValue2">Initialization value 2.</param>
		/// <param name="initValue3">Initialization value 3.</param>
		/// <param name="initValue4">Initialization value 4.</param>
		public BitArray256(ulong initValue1, ulong initValue2, ulong initValue3, ulong initValue4)
		{
			data1 = initValue1;
			data2 = initValue2;
			data3 = initValue3;
			data4 = initValue4;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="bitIndexTrue">List of indices where bits should be set to true.</param>
		public BitArray256(IEnumerable<uint> bitIndexTrue)
		{
			data1 = data2 = data3 = data4 = 0uL;
			if (bitIndexTrue == null)
				return;
			for (int index = bitIndexTrue.Count() - 1; index >= 0; --index)
			{
				uint bitIndex = bitIndexTrue.ElementAt(index);
				if (bitIndex < 64u)
					data1 |= 1uL << (int)bitIndex;
				else if (bitIndex < 128u)
					data2 |= 1uL << (int)(bitIndex - 64u);
				else if (bitIndex < 192u)
					data3 |= 1uL << (int)(bitIndex - 128u);
				else if (bitIndex < Capacity)
					data4 |= 1uL << (int)(bitIndex - 192u);
			}
		}

		/// <summary>
		/// Bit-wise Not operator
		/// </summary>
		/// <param name="a">Bit array with which to do the operation.</param>
		/// <returns>The resulting bit array.</returns>
		public static BitArray256 operator ~(BitArray256 a) => new BitArray256(~a.data1, ~a.data2, ~a.data3, ~a.data4);

		/// <summary>
		/// Bit-wise Or operator
		/// </summary>
		/// <param name="a">First bit array.</param>
		/// <param name="b">Second bit array.</param>
		/// <returns>The resulting bit array.</returns>
		public static BitArray256 operator |(BitArray256 a, BitArray256 b) => new BitArray256(a.data1 | b.data1, a.data2 | b.data2, a.data3 | b.data3, a.data4 | b.data4);

		/// <summary>
		/// Bit-wise And operator
		/// </summary>
		/// <param name="a">First bit array.</param>
		/// <param name="b">Second bit array.</param>
		/// <returns>The resulting bit array.</returns>
		public static BitArray256 operator &(BitArray256 a, BitArray256 b) => new BitArray256(a.data1 & b.data1, a.data2 & b.data2, a.data3 & b.data3, a.data4 & b.data4);

		/// <summary>
		/// Bit-wise And
		/// </summary>
		/// <param name="other">Bit array with which to do the operation.</param>
		/// <returns>The resulting bit array.</returns>
		public IBitArray BitAnd(IBitArray other) => this & (BitArray256)other;

		/// <summary>
		/// Bit-wise Or
		/// </summary>
		/// <param name="other">Bit array with which to do the operation.</param>
		/// <returns>The resulting bit array.</returns>
		public IBitArray BitOr(IBitArray other) => this | (BitArray256)other;

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
		public static bool operator ==(BitArray256 a, BitArray256 b) => a.data1 == b.data1 && a.data2 == b.data2 && a.data3 == b.data3 && a.data4 == b.data4;

		/// <summary>
		/// Inequality operator.
		/// </summary>
		/// <param name="a">First bit array.</param>
		/// <param name="b">Second bit array.</param>
		/// <returns>True if the bit arrays are not equals.</returns>
		public static bool operator !=(BitArray256 a, BitArray256 b) => a.data1 != b.data1 || a.data2 != b.data2 || a.data3 != b.data3 || a.data4 != b.data4;

		/// <summary>
		/// Equality operator.
		/// </summary>
		/// <param name="obj">Bit array to compare to.</param>
		/// <returns>True if the provided bit array is equal to this..</returns>
		public override bool Equals(object obj) =>
			(obj is BitArray256)
			&& data1.Equals(((BitArray256)obj).data1)
			&& data2.Equals(((BitArray256)obj).data2)
			&& data3.Equals(((BitArray256)obj).data3)
			&& data4.Equals(((BitArray256)obj).data4);

		/// <summary>
		/// Get the hashcode of the bit array.
		/// </summary>
		/// <returns>Hashcode of the bit array.</returns>
		public override int GetHashCode()
		{
			var hashCode = 1870826326;
			hashCode = hashCode * -1521134295 + data1.GetHashCode();
			hashCode = hashCode * -1521134295 + data2.GetHashCode();
			hashCode = hashCode * -1521134295 + data3.GetHashCode();
			hashCode = hashCode * -1521134295 + data4.GetHashCode();
			return hashCode;
		}
	}
}