///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.grid-raw-image

using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;

namespace UnityEngine.UI.Extensions
{
	[Serializable]
	public struct GridShape : ISerializationCallbackReceiver
	{
		public static GridShape Rectangle2X2 = new(new Vector2Int(2, 2));
		public static GridShape Default = Rectangle2X2;
		static List<Vector2Int> _buffer = new();

		/// <summary>
		/// expected string format: <br/><br/>
		/// 00110 <br/>
		/// 01000 <br/>
		/// 00001
		/// </summary>
		public static void FromString(ref GridShape gridShape, string text)
		{
			string[] lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

			gridShape._size = new Vector2Int(lines[0].Length, lines.Length);

			for (int y = 0; y < lines.Length; y++)
			{
				string line = lines[y];
				int length = line.Length;
				for (int x = 0; x < length; x++)
				{
					gridShape._bitArray[gridShape.IndexFromPosition(x, y, true)] = line[x] == '1';
				}
			}
		}

		/// <summary>
		/// expected string format: <br/><br/>
		/// 00110 <br/>
		/// 01000 <br/>
		/// 00001
		/// </summary>
		public static GridShape FromString(string text)
		{
			GridShape gridShape = new();
			FromString(ref gridShape, text);
			return gridShape;
		}

		[SerializeField] private Vector2Int _size;
		[SerializeField] private bool _readable;
		[SerializeField, HideInInspector] private string _data;
		[SerializeField, HideInInspector] private BitArray256 _bitArray;
		public BitArray256 Values => _bitArray;
		public Vector2Int Size => _size;
		public int Width => _size.x;
		public int Height => _size.y;
		public int Length => _size.x * _size.y;
		public Vector2 GetSpritePivot() => -GetOffsetTransformPositionOffset() / (Vector2)_size;

		public GridShape(int x, int y, IEnumerable<Vector2Int> skippedPositions = null, bool readable = false) : this(skippedPositions, readable) => _size = new(x, y);
		public GridShape(Vector2Int size, IEnumerable<Vector2Int> skippedPositions = null, bool readable = false) : this(skippedPositions, readable) => _size = size;

		public GridShape(IEnumerable<Vector2Int> skippedPositions = null, bool readable = false)
		{
			_bitArray = new BitArray256(ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue);
			_readable = readable;
			_size = default;
			_data = null;

			uint IndexFromPositionLocal(Vector2Int position, Vector2Int newSize) =>
				(uint)(position.y * newSize.x + position.x);

			if (skippedPositions != null)
			{
				foreach (Vector2Int skippedPosition in skippedPositions)
				{
					_bitArray[IndexFromPositionLocal(skippedPosition, _size)] = false;
				}
			}

        #if UNITY_EDITOR
			_previewValues = null;
			OnInspectorInit();
        #endif
		}

		public GridShape Extrude(int x = 1, int y = 1)
		{
			GridShape newShape = this;
			GetValidPositions(_buffer);
			foreach (Vector2Int point in _buffer)
			{
				Vector2Int p = point;
				Vector2Int p2 = point;
				for (int i = 0; i < x; i++)
				{
					p.x++;
					p2.x--;
					newShape[p] = true;
					newShape[p2] = true;
				}

				p = point;
				p2 = point;
				for (int i = 0; i < y; i++)
				{
					p.y++;
					p2.y--;
					newShape[p] = true;
					newShape[p2] = true;
				}
			}
			return newShape;
		}

		public static bool operator ==(GridShape left, GridShape right) => left.Equals(right);
		public static bool operator !=(GridShape left, GridShape right) => !left.Equals(right);

		public override bool Equals(object obj) => obj is GridShape other && _size.Equals(other._size) && _bitArray.Equals(other._bitArray);
		public override int GetHashCode() => HashCode.Combine(_size, _bitArray);

		public uint IndexFromPosition(int x, int y, bool invertY = false) => (uint)(InvertY(y, invertY) * _size.x + x);
		private uint IndexFromPosition(uint x, uint y, bool invertY = false) => InvertY(y, invertY) * (uint)_size.x + x;
		public uint IndexFromPosition(Vector2Int position, bool invertY = false) => InvertY(position.y, invertY) * (uint)_size.x + (uint)position.x;
		private Vector2Int PositionFromIndex(uint index) => new((int)index % _size.x, (int)(index / _size.x));
		private void SetDefault() => this = Default;

		public bool Contains(int x, int y)
		{
			if (x < 0 || y < 0 || x >= _size.x || y >= _size.y) return false;

			return _bitArray[IndexFromPosition(x, y)];
		}

		public bool Contains(Vector2Int position)
		{
			if (position.x < 0 || position.y < 0 || position.x >= _size.x || position.y >= _size.y) return false;

			return _bitArray[IndexFromPosition(position)];
		}

		private Vector3 GetOffsetTransformPositionOffset() => new(-.5f, -.5f, 0);

		public Vector3 FromGridPosition(Vector2Int gridPosition, bool halfOffset = false)
		{
			return halfOffset ? GetOffsetTransformPositionOffset() + (Vector3)(Vector2)gridPosition : (Vector2)gridPosition;
		}

		public RectInt GetValidRect()
		{
			var rect = new RectInt();

			Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
			Vector2Int max = new Vector2Int(int.MinValue, int.MinValue);
			bool foundAny = false;
			for (uint i = 0; i < Length; i++)
			{
				if (_bitArray[i])
				{
					Vector2Int position = PositionFromIndex(i);
					min = Vector2Int.Min(position, min);
					max = Vector2Int.Max(position, max);
					foundAny = true;
				}
			}

			if (!foundAny) return default;
			rect.min = min;
			rect.max = max;
			return rect;
		}

		public void GetValidPositions(List<Vector2Int> buffer, bool invertY = false)
		{
			buffer.Clear();
			for (uint y = 0; y < _size.y; y++)
			{
				for (uint x = 0; x < _size.x; x++)
				{
					if (_bitArray[IndexFromPosition(x, y)])
					{
						buffer.Add(new Vector2Int((int)x, (int)InvertY(y, invertY)));
					}
				}
			}
		}

		private uint InvertY(int y, bool invert) => invert ? (uint)(_size.y - y - 1) : (uint)y;
		private uint InvertY(uint y, bool invert) => invert ? (uint)_size.y - y - 1 : y;

		/// <summary>
		/// result string format: <br/><br/>
		/// 00110 <br/>
		/// 01000 <br/>
		/// 00001
		/// </summary>
		public override string ToString()
		{
			var builder = new StringBuilder(_size.x * (_size.y + 1));
			char[] line = new char[_size.x + 1];
			line[^1] = '\n';
			int index = 0;

			for (int y = 0; y < _size.y; y++)
			{
				for (int x = 0; x < _size.x; x++)
				{
					line[x] = _bitArray[IndexFromPosition(x, y, true)] ? '1' : '0';
				}

				builder.Insert(index, line);
				index += line.Length;
			}

			return builder.ToString();
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if (_readable) _data = ToString();
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (_readable) FromString(_data);
			_data = null;
		}

		public bool this[Vector2Int coord]
		{
			get => Contains(coord);
			set => _bitArray[IndexFromPosition(coord.x, coord.y)] = value;
		}
		public bool this[int x, int y]
		{
			get => Contains(x, y);
			set => _bitArray[IndexFromPosition(x, y)] = value;
		}
		public bool this[uint i]
		{
			get => _bitArray[i];
			set => _bitArray[i] = value;
		}
		public bool this[uint x, uint y]
		{
			get => Contains((int)x, (int)y);
			set => _bitArray[IndexFromPosition(x, y)] = value;
		}

    #region Editor
#if UNITY_EDITOR
		(Vector2Int, uint)[,] _previewValues;

		public void OnDrawGizmos(Transform transform, Color color, bool wire = false, Vector2? cellSize = null, Vector3 offset = default)
		{
			GetValidPositions(_buffer);
			Gizmos.color = color;
			Gizmos.matrix = transform.localToWorldMatrix;
			Vector3 size = cellSize.HasValue ? cellSize.Value : Vector2.one * .95f;
			foreach (Vector2Int gridPosition in _buffer)
			{
				Vector3 position = FromGridPosition(gridPosition);

				position += offset;
				position *= (Vector2)size;
				if (wire)
				{
					Gizmos.DrawWireCube(position, size);
				}
				else
				{
					Gizmos.DrawCube(position, size);
				}
			}
		}

		private void OnInspectorInit()
		{
			_previewValues = new (Vector2Int, uint)[_size.x, _size.y];

			for (int y = 0; y < _size.y; y++)
			{
				for (int x = 0; x < _size.x; x++)
				{
					Vector2Int position = new(x, (int)InvertY(y, true));
					uint index = IndexFromPosition(position);
					_previewValues[x, y] = new(position, index);
				}
			}
		}

		private (Vector2Int, uint) Draw(Rect rect, (Vector2Int, uint) value)
		{
			uint index = value.Item2;
			if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition)) _bitArray[index] = !_bitArray[index];
			if (_bitArray[index]) EditorGUI.DrawRect(rect, Color.green);
			return value;
		}
#endif
    #endregion
	}
}