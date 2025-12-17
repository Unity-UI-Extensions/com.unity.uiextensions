///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.ui-graphic-sector

using System;
using System.Collections;
using UnityEditor;

namespace UnityEngine.UI.Extensions
{
	[Serializable]
	public class Layout
	{
		public Anchor[] Anchors = Array.Empty<Anchor>();
		DrivenRectTransformTracker _tracker;
		Sector _sector;

		public void Update(Sector sector)
		{
			_sector ??= sector;
			if (_sector.Cache.Root)
			{
				int newCount = _sector.Cache.Root.childCount;
				if (newCount != _sector.Cache.childCount)
				{
					sector.SetAllDirty();
				}
			}
			if (_sector.Cache.RootSectorTransform)
			{
				int siblingIndex = _sector.Cache.RootSectorTransform.GetSiblingIndex();
				if (siblingIndex != _sector.Cache.SiblingIndex)
				{
					sector.SetAllDirty();
				}
			}
			foreach (Anchor anchor in Anchors)
			{
				anchor.Execute(_sector);
			}
		}

		public void SetLayoutHorizontal(Sector sector)
		{
			_sector ??= sector;
			foreach (Anchor anchor in Anchors)
			{
				anchor.Execute(_sector);
			}
			_tracker.Clear();
			DrivenTransformProperties flags = default;

			if (_sector.Settings.PivotToSector)
			{
				flags |= DrivenTransformProperties.Pivot;
				Vector2 pivot = ((Vector2)_sector.Cache.SectorCenter - _sector.Cache.TransformRect.min) / _sector.Cache.TransformRect.size;
				if (!float.IsNaN(pivot.x) && !float.IsNaN(pivot.y))
				{
					sector.rectTransform.pivot = pivot;
				}
			}
			if (flags != default)
			{
				_tracker.Add(sector, sector.rectTransform, flags);
			}

			foreach (Anchor anchor in Anchors)
			{
				anchor.Execute(_sector);
			}
		}

		public void SetLayoutVertical(Sector sector) => SetLayoutHorizontal(sector);

		[Serializable]
		public class Anchor
		{
			static readonly Vector2 HALF_VECTOR = Vector2.one * .5f;
			const float DEG2_RAD = Mathf.Deg2Rad;
			Sector _sector;
			public RectTransform rectTransform;
			public bool Resize;
			public Vector2 ResizeOffset;
			public bool PivotToOuterCorner;
			public Vector2 NormalizedAnchorAngleRadius = HALF_VECTOR;
			public Vector2 OffsetAnchorAngleRadius;
			public eRotation Rotation = eRotation.Root;

			DrivenRectTransformTracker _tracker;

			public void Execute(Sector sector)
			{
				_sector ??= sector;
				_tracker.Clear();
				if (!rectTransform) return;

				float radius = Mathf.LerpUnclamped(_sector.Cache.InnerRadius, _sector.Cache.OuterRadius, NormalizedAnchorAngleRadius.y) + OffsetAnchorAngleRadius.y;
				float angle = (Mathf.LerpUnclamped(_sector.Cache.MinAngle, _sector.Cache.MaxAngle, NormalizedAnchorAngleRadius.x) + OffsetAnchorAngleRadius.x) * DEG2_RAD;
				Vector2 position = new(MathUtilities.Cos(angle) * radius * _sector.Cache.TransformRect.width / 2, MathUtilities.Sin(angle) * radius * _sector.Cache.TransformRect.height / 2);
				if (!float.IsNaN(position.x) && !float.IsNaN(position.y))
				{
					rectTransform.anchoredPosition = position;
				}
				rectTransform.anchorMin = rectTransform.anchorMax = HALF_VECTOR;

				var flags = DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition;
				switch (Rotation)
				{
					case eRotation.Root:
						{
							flags |= DrivenTransformProperties.Rotation;
							if (_sector.Cache.Root)
							{
								rectTransform.rotation = _sector.Cache.Root.rotation;
							}
							break;
						}
					case eRotation.ToCenter:
						{
							flags |= DrivenTransformProperties.Rotation;
							rectTransform.localRotation = Quaternion.LookRotation(Vector3.forward, _sector.Cache.CircleCenter - rectTransform.localPosition);
							break;
						}
					default:
					case eRotation.None: break;
				}
				if (Resize)
				{
					flags |= DrivenTransformProperties.SizeDelta;

					if (!float.IsNaN(_sector.Cache.ContentSize))
					{
						rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _sector.Cache.ContentSize + ResizeOffset.x);
						rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _sector.Cache.ContentSize + ResizeOffset.y);
					}
				}
				if (PivotToOuterCorner)
				{
					flags |= DrivenTransformProperties.Pivot;

					Vector2 vector = _sector.Cache.MiddleVector;

					Vector2 pivot = new Vector2(.5f, .5f);
					pivot.x = vector.x > 0 ? 0 : 1;
					pivot.y = vector.y > 0 ? 0 : 1;
					rectTransform.pivot = pivot;
				}

				#if UNITY_EDITOR
				EditorUtility.SetDirty(rectTransform);
				#endif

				_tracker.Add(_sector, rectTransform, flags);
			}
		}

		public void SetDirty()
		{
			if (!_sector || !_sector.isActiveAndEnabled)
				return;

			if (!CanvasUpdateRegistry.IsRebuildingGraphics())
				_sector.SetVerticesDirty();

			if (!CanvasUpdateRegistry.IsRebuildingLayout())
				_sector.SetLayoutDirty();
			else
				_sector.StartCoroutine(DelayedSetDirty(_sector.rectTransform));
		}

		IEnumerator DelayedSetDirty(RectTransform rectTransform)
		{
			yield return null;
			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
		}

		public enum eRotation
		{
			None,
			Root,
			ToCenter,
		}
	}
}