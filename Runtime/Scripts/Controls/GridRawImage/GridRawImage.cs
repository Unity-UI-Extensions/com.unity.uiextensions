///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.grid-raw-image

using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(CanvasRenderer))]
	public class GridRawImage : MaskableGraphic, ILayoutSelfController, ICanvasRaycastFilter
	{
		static Vector2 UV0 = new Vector2(0, 0);
		static Vector2 UV1 = new Vector2(0, 1);
		static Vector2 UV2 = new Vector2(1, 1);
		static Vector2 UV3 = new Vector2(1, 0);

		public Texture texture;
		[SerializeField] GridRawImageResize _resize = GridRawImageResize.ResizeValidPositionsOnly;
		[SerializeField] Vector2 _cellSize = Vector2.one * 100;
		[SerializeField] float _extrude;
		[SerializeField] GridShape _shape = GridShape.Default;

		DrivenRectTransformTracker _tracker;

		static List<Vector2Int> _buffer = new List<Vector2Int>();
		Color32 color32;
		RectInt _validRect;

		/// <summary>
		/// Image's texture comes from the UnityEngine.Image.
		/// </summary>
		public override Texture mainTexture
		{
			get
			{
				if (texture == null)
				{
					if (material != null && material.mainTexture != null)
					{
						return material.mainTexture;
					}

					return s_WhiteTexture;
				}

				return texture;
			}
		}

		public void SetShape(GridShape shape)
		{
			_shape = shape;
			SetAllDirty();
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			color32 = color;

			Vector2 offset = rectTransform.pivot * _cellSize;
			switch (_resize)
			{
				case GridRawImageResize.Resize:
				{
					offset *= -_shape.Size;
					break;
				}

				case GridRawImageResize.ResizeValidPositionsOnly:
				{
					_validRect = _shape.GetValidRect();
					offset *= -_validRect.min;

					//offset = (_validRect.min - _validRect.size) * _cellSize;
					Debug.Log($"{_shape.Size} {_validRect.size} {_validRect.min}");
					break;
				}

				case GridRawImageResize.None:
				{
					offset *= -_shape.Size;
					break;
				}

				case GridRawImageResize.RectSizeToCount:
				{
					Vector2 newSize = rectTransform.rect.size / _cellSize;
					Vector2Int newSizeInt = Vector2Int.RoundToInt(newSize);
					if (newSizeInt != _shape.Size)
					{
						_shape.GetValidPositions(_buffer, invertY: true);
						_shape = new GridShape(newSizeInt, _buffer);
					}

					break;
				}

				default:
					throw new ArgumentOutOfRangeException();
			}

			_shape.GetValidPositions(_buffer);
			if (_buffer.Count > 0)
			{
				for (int i = 0; i < _buffer.Count; i++)
				{
					Vector2 position = _buffer[i] * _cellSize + offset;
					DrawPosition(vh, position, i);
				}
			}
		}

		private void DrawPosition(VertexHelper vh, Vector2 p, int i)
		{
			var corners = new Vector4(p.x - _extrude, p.y - _extrude, p.x + _cellSize.x + _extrude, p.y + _cellSize.y + _extrude);

			vh.AddVert(new Vector3(corners.x, corners.y), color32, UV0);
			vh.AddVert(new Vector3(corners.x, corners.w), color32, UV1);
			vh.AddVert(new Vector3(corners.z, corners.w), color32, UV2);
			vh.AddVert(new Vector3(corners.z, corners.y), color32, UV3);

			i *= 4;

			vh.AddTriangle(i + 0, i + 1, i + 2);
			vh.AddTriangle(i + 2, i + 3, i + 0);
		}

		public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out var local))
				return false;

			_shape.GetValidPositions(_buffer);

			if (_buffer.Count == 0) return false;

			for (int i = 0; i < _buffer.Count; i++)
			{
				Vector2 p = _buffer[i] * _cellSize;
				Vector4 corners = new Vector4(p.x - _extrude, p.y - _extrude, p.x + _cellSize.x + _extrude, p.y + _cellSize.y + _extrude);
				Rect rect = new Rect(corners.x, corners.y, corners.z - corners.x, corners.w - corners.y);

				if (rect.Contains(local))
				{
					return true;
				}
			}

			return false;
		}

		public void SetLayoutHorizontal()
		{
			if (_resize == GridRawImageResize.None)
			{
				_tracker.Clear();
				return;
			}

			_tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDelta);

			switch (_resize)
			{
				case GridRawImageResize.Resize:
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _shape.Size.x * _cellSize.x);
					break;
				}

				case GridRawImageResize.ResizeValidPositionsOnly:
				{
					_validRect = _shape.GetValidRect();
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (_validRect.size.x + 1) * _cellSize.x);
					break;
				}
			}
		}

		public void SetLayoutVertical()
		{
			if (_resize == GridRawImageResize.None)
			{
				_tracker.Clear();
				return;
			}

			switch (_resize)
			{
				case GridRawImageResize.Resize:
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _shape.Size.y * _cellSize.y);
					break;
				}

				case GridRawImageResize.ResizeValidPositionsOnly:
				{
					_validRect = _shape.GetValidRect();
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (_validRect.size.y + 1) * _cellSize.y);
					break;
				}
			}
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			SetAllDirty();
			base.OnValidate();
		}
#endif
	}
}