///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.ui-graphic-sector

using System;
using System.Collections;
using UnityEditor;

namespace UnityEngine.UI.Extensions
{
	public class Rendering
	{
		const float SEGMENT_MIN_ANGLE = 3;
		const float DEG2_RAD = Mathf.Deg2Rad;
		static readonly Vector4 DEFAULT_UV = new Vector4(0, 0, 1, 1);
		static readonly float SQRT_2 = Mathf.Sqrt(2);
		static readonly UIVertexPool _pool = new(100);
		/// left,center horizontal, right 
		static readonly int[] s_segmentsPerBorder = new int[3];
		static readonly float[] s_radius = new float[2];

		[NonSerialized] ArcRenderValues values;
		/// normalized uv outside all Sprite
		[NonSerialized] Vector4 outerUV;

		/// normalized uv inside Sprite-border
		[NonSerialized] Vector4 innerUV;

		/// X=left, Y=bottom, Z=right, W=top | pixel-metric printed in Sprite Editor
		[NonSerialized] Vector4 spriteBorder;

		[NonSerialized] float multipliedPixelsPerUnit;
		[NonSerialized] VertexHelper _vh;
		[NonSerialized] RaycastingUtilities _raycasting;
		[NonSerialized] Sector _sector;
		[NonSerialized] Settings Settings;
		[NonSerialized] Cache Cache;
		[NonSerialized] Vector4 _spriteUV;
		[NonSerialized] bool hasGradients;
		[NonSerialized] SharedSettings _sharedSettings;

		public void OnPopulateMesh(VertexHelper vh, Sector sector, RaycastingUtilities raycasting)
		{
			vh.Clear();
			_vh = vh;
			_sector = sector;
			_raycasting = raycasting;
			Settings = _sector.Settings;

			FillCache();

			if (_sector.Cache.Color32.a != 0)
			{
				Render();
			}
		}

		void FillCache()
		{
			Cache = _sector.Cache;
			Cache.RootSectorTransform = (RectTransform)_sector.GetRootSector();

			CloneRootSector();
			Cache.Root = (RectTransform)Cache.RootSectorTransform?.parent;
			Cache.childCount = 1;
			Cache.SiblingIndex = 0;

			if (Settings.ShapeSource != ShapeSource.CircleAlways)
			{
				if (Cache.Root)
				{
					Cache.childCount = Cache.Root.childCount;
				}
				if (Cache.RootSectorTransform)
				{
					Cache.SiblingIndex = Cache.RootSectorTransform.GetSiblingIndex();
				}
			}

			Cache.InnerRadius = Mathf.Min(Settings.Radius1, Settings.Radius2) / 100f;
			Cache.OuterRadius = Mathf.Max(Settings.Radius1, Settings.Radius2) / 100f;
			Cache.HalfRadius = Mathf.LerpUnclamped(Cache.OuterRadius, Cache.InnerRadius, .5f);
			Cache.TransformRect = _sector.rectTransform.rect;

			// if (Settings.PivotToSector)
			// {
			// 	Cache.TransformRect.size *= 2;
			// 	Cache.TransformRect.min -= Cache.TransformRect.size / 2;
			// }

			Rect rect = Cache.TransformRect;

			Cache.CircleCenter = rect.center;

			float centerRadius = Mathf.Lerp(Cache.OuterRadius, Cache.InnerRadius, .5f);
			float radiusX = rect.width / 2 * centerRadius;
			float radiusY = rect.height / 2 * centerRadius;
			int clockwise = (int)Settings.Clockwise;
			Cache.DegreesOffset = Settings.DegreesOffset * clockwise;
			float offset = Cache.DegreesOffset;
			float degreeStep = Cache.DeltaAngle = Settings.DegreesTotal / Cache.childCount * clockwise;
			Cache.DeltaAngleAbs = Mathf.Abs(degreeStep);
			float currentDegree = offset + degreeStep * Cache.SiblingIndex;

			Cache.MinAngle = currentDegree + Settings.DegreesGap * clockwise;
			Cache.MaxAngle = currentDegree + degreeStep - Settings.DegreesGap * clockwise;
			Cache.MiddleAngle = Mathf.LerpUnclamped(Cache.MinAngle, Cache.MaxAngle, .5f);

			Cache.DeltaAngleFinalAbs = Mathf.Abs(Cache.MaxAngle - Cache.MinAngle);

			Vector3 vector = Cache.MiddleVector = new Vector2(MathUtilities.Cos(Cache.MiddleAngle * DEG2_RAD) * radiusX, MathUtilities.Sin(Cache.MiddleAngle * DEG2_RAD) * radiusY);
			Cache.SectorCenter = Cache.CircleCenter + vector;

			Cache.Matrix4x4 = Matrix4x4.TRS(default, Quaternion.identity, Vector3.one * (Cache.OuterRadius - Settings.LocalRescaleDelta));
			Cache.Color = _sector.color;
			Cache.Color32 = _sector.color;
			Cache.StartVector = new Vector2(MathUtilities.Cos(Cache.MinAngle * DEG2_RAD) * radiusX, MathUtilities.Sin(Cache.MinAngle * DEG2_RAD) * radiusY);
			Cache.EndVector = new Vector2(MathUtilities.Cos(Cache.MaxAngle * DEG2_RAD) * radiusX, MathUtilities.Sin(Cache.MaxAngle * DEG2_RAD) * radiusY);
			hasGradients = Settings.Gradients.Length > 0;

			Cache.RectMinSize = Mathf.Min(Cache.TransformRect.width, Cache.TransformRect.height);
			float size = (_sector.Cache.OuterRadius - _sector.Cache.InnerRadius) * Cache.RectMinSize / 2;
			float size2 = Mathf.Abs(Cache.DeltaAngleFinalAbs * DEG2_RAD * Cache.HalfRadius * Cache.RectMinSize * Mathf.PI / 8);
			Cache.ContentSize = Mathf.Min(size, size2) - Settings.LocalRescaleDelta * Cache.OuterRadius * Cache.RectMinSize / 2 * SQRT_2;
		}

		public void Update(Sector sector)
		{
			Settings = sector.Settings;
			if (!Settings.CloneParentSectorSettings) return;

			_sector = sector;
			Cache = sector.Cache;
			CloneRootSector();
		}

		void CloneRootSector()
		{
			if (!Settings.CloneParentSectorSettings) return;
			if (!Cache.RootSectorTransform) return;
			Cache.RootSector = Cache.RootSectorTransform.GetComponent<Sector>();
			Sector root = Cache.RootSector;
			if (!root) return;

			SharedSettings newSettings = _sharedSettings;

			_sharedSettings.PivotToSector = Settings.PivotToSector;
			_sharedSettings.AngleMargin = Settings.DegreesGap;
			_sharedSettings.PixelsMargin = Settings.LocalRescaleDelta;
			_sharedSettings.AngleMax = Settings.DegreesTotal;
			_sharedSettings.AngleOffset = Settings.DegreesOffset;
			_sharedSettings.Clockwise = Settings.Clockwise;

			newSettings.PivotToSector = root.Settings.PivotToSector;
			newSettings.AngleMargin = root.Settings.DegreesGap;
			newSettings.PixelsMargin = root.Settings.LocalRescaleDelta;
			newSettings.AngleMax = root.Settings.DegreesTotal;
			newSettings.AngleOffset = root.Settings.DegreesOffset;
			newSettings.Clockwise = root.Settings.Clockwise;

			if (newSettings != _sharedSettings)
			{
				Settings.PivotToSector = root.Settings.PivotToSector;
				Settings.DegreesGap = root.Settings.DegreesGap;
				Settings.LocalRescaleDelta = root.Settings.LocalRescaleDelta;
				Settings.DegreesTotal = root.Settings.DegreesTotal;
				Settings.DegreesOffset = root.Settings.DegreesOffset;
				Settings.Clockwise = root.Settings.Clockwise;

				SetDirty();
			}
		}

		void Render()
		{
			switch (Settings.Type)
			{
				case ImageFillType.Sliced:
					{
						bool isSliceAvailable = Settings.Sprite.sprite && Settings.Sprite.sprite.border != default || !Settings.FillCenter;

						if (isSliceAvailable)
						{
							RenderSliced();
						}
						else
						{
							RenderSimple();
						}

						break;
					}

				case ImageFillType.Simple:
					{
						RenderSimple();
						break;
					}
				case ImageFillType.Tiled:
					{
						RenderTiled();
						break;
					}
				// case eType.Filled:
				// 	{
				// 		RenderFilled();
				// 		break;
				// 	}
			}
			_pool.ReleaseAll();
		}

		void RenderSimple()
		{
			_spriteUV = Settings.Sprite.activeSprite != null ? UnityEngine.Sprites.DataUtility.GetOuterUV(Settings.Sprite.activeSprite) : DEFAULT_UV;
			multipliedPixelsPerUnit = 1;
			values = new ArcRenderValues
			{
				uvMin = new Vector2(_spriteUV.x, _spriteUV.y),
				uvMax = new Vector2(_spriteUV.z, _spriteUV.w),
				segmentCount = Mathf.CeilToInt(360 / SEGMENT_MIN_ANGLE * (Settings.GeometryResolution / 100f) * (Cache.DeltaAngleAbs / 360)),
				angle1 = Cache.MinAngle,
				angle2 = Cache.MaxAngle,
				radius1 = Cache.InnerRadius,
				radius2 = Cache.OuterRadius,
			};
			values.uvDelta = (values.uvMax - values.uvMin) / values.segmentCount;
			RenderArc(ref values);
		}

		void RenderTiled()
		{
			_spriteUV = (Settings.Sprite.activeSprite != null) ? UnityEngine.Sprites.DataUtility.GetOuterUV(Settings.Sprite.activeSprite) : DEFAULT_UV;
			multipliedPixelsPerUnit = _sector.multipliedPixelsPerUnit;
			values = new ArcRenderValues
			{
				uvMin = new Vector2(_spriteUV.x, _spriteUV.y),
				uvMax = new Vector2(_spriteUV.z, _spriteUV.w),
				segmentCount = Mathf.CeilToInt(360 / SEGMENT_MIN_ANGLE * (Settings.GeometryResolution / 100f) * (Cache.DeltaAngleAbs / Settings.DegreesTotal)),
				angle1 = Cache.MinAngle,
				angle2 = Cache.MaxAngle,
				radius1 = Cache.InnerRadius,
				radius2 = Cache.OuterRadius,
			};
			values.uvDelta = (values.uvMax - values.uvMin) / values.segmentCount;
			RenderArc(ref values);
		}

		void RenderSliced()
		{
			if (!_sector.Settings.Sprite.hasBorder)
			{
				RenderSimple();
				return;
			}
			multipliedPixelsPerUnit = _sector.multipliedPixelsPerUnit;

			Sprite activeSprite = _sector.Settings.Sprite.activeSprite;

			outerUV = UnityEngine.Sprites.DataUtility.GetOuterUV(activeSprite);
			innerUV = UnityEngine.Sprites.DataUtility.GetInnerUV(activeSprite);

			/// X=left, Y=bottom, Z=right, W=top | pixel-metric printed in Sprite Editor
			spriteBorder = activeSprite.border;
			float widthFactor = activeSprite.rect.width * multipliedPixelsPerUnit * (1 + Settings.SpriteBorderBalance / 100f) * Cache.DeltaAngleAbs / 360;
			float heightFactor = activeSprite.rect.height * multipliedPixelsPerUnit * (1 - Settings.SpriteBorderBalance / 100f) * (Cache.OuterRadius - Cache.InnerRadius);
			spriteBorder.x /= widthFactor;
			spriteBorder.z /= widthFactor;
			spriteBorder.y /= heightFactor;
			spriteBorder.w /= heightFactor;

			// sprite border applied per-quad, to simplify math
			/// left,center horizontal, right 
			int segmentCount = Mathf.CeilToInt(360 / SEGMENT_MIN_ANGLE * (Settings.GeometryResolution / 100f) * (Cache.DeltaAngleAbs / 360));
			s_segmentsPerBorder[0] = Mathf.Clamp(Mathf.CeilToInt(spriteBorder.x * segmentCount), 0, segmentCount);
			s_segmentsPerBorder[2] = Mathf.Clamp(Mathf.CeilToInt(spriteBorder.z * segmentCount), 0, segmentCount);
			s_segmentsPerBorder[1] = Mathf.Clamp(segmentCount - s_segmentsPerBorder[0] - s_segmentsPerBorder[2], 1, segmentCount);
			s_radius[0] = Mathf.Lerp(Cache.InnerRadius, Cache.OuterRadius, spriteBorder.y);
			s_radius[1] = Mathf.Lerp(Cache.InnerRadius, Cache.OuterRadius, 1 - spriteBorder.w);

			// bot left

			//values = new ArcRenderValues();
			values.uvMin = new Vector2(outerUV.x, outerUV.y);
			values.uvMax = new Vector2(innerUV.x, innerUV.y);
			values.segmentCount = s_segmentsPerBorder[0];
			values.angle1 = Cache.MinAngle;
			values.angle2 = Cache.MinAngle + Cache.DeltaAngle * spriteBorder.x;
			values.radius1 = Cache.InnerRadius;
			values.radius2 = s_radius[0];
			values.uvDelta = (values.uvMax - values.uvMin) / values.segmentCount;
			RenderArc(ref values);

			// left

			//values = new ArcRenderValues();
			values.uvMin = new Vector2(outerUV.x, innerUV.y);
			values.uvMax = new Vector2(innerUV.x, innerUV.w);
			values.segmentCount = s_segmentsPerBorder[0];
			values.angle1 = Cache.MinAngle;
			values.angle2 = Cache.MinAngle + Cache.DeltaAngle * spriteBorder.x;
			values.radius1 = s_radius[0];
			values.radius2 = s_radius[1];
			values.uvDelta = (values.uvMax - values.uvMin) / values.segmentCount;
			RenderArc(ref values);

			//  left top

			//values = new ArcRenderValues();
			values.uvMin = new Vector2(outerUV.x, innerUV.w);
			values.uvMax = new Vector2(innerUV.x, outerUV.w);
			values.segmentCount = s_segmentsPerBorder[0];
			values.angle1 = Cache.MinAngle;
			values.angle2 = Cache.MinAngle + Cache.DeltaAngle * spriteBorder.x;
			values.radius1 = s_radius[1];
			values.radius2 = Cache.OuterRadius;
			values.uvDelta = (values.uvMax - values.uvMin) / values.segmentCount;
			RenderArc(ref values);

			// bot

			//values = new ArcRenderValues();
			values.uvMin = new Vector2(innerUV.x, outerUV.y);
			values.uvMax = new Vector2(innerUV.z, innerUV.y);
			values.segmentCount = s_segmentsPerBorder[1];
			values.angle1 = Cache.MinAngle + Cache.DeltaAngle * spriteBorder.x;
			values.angle2 = Cache.MaxAngle - Cache.DeltaAngle * spriteBorder.z;
			values.radius1 = Cache.InnerRadius;
			values.radius2 = s_radius[0];
			values.uvDelta = (values.uvMax - values.uvMin) / values.segmentCount;
			RenderArc(ref values);

			// center
			if (Settings.FillCenter)
			{
				//values = new ArcRenderValues();
				values.uvMin = new Vector2(innerUV.x, innerUV.y);
				values.uvMax = new Vector2(innerUV.z, innerUV.w);
				values.segmentCount = s_segmentsPerBorder[1];
				values.angle1 = Cache.MinAngle + Cache.DeltaAngle * spriteBorder.x;
				values.angle2 = Cache.MaxAngle - Cache.DeltaAngle * spriteBorder.z;
				values.radius1 = s_radius[0];
				values.radius2 = s_radius[1];
				values.uvDelta = (values.uvMax - values.uvMin) / values.segmentCount;
				RenderArc(ref values);
			}

			// top

			//values = new ArcRenderValues();
			values.uvMin = new Vector2(innerUV.x, innerUV.w);
			values.uvMax = new Vector2(innerUV.z, outerUV.w);
			values.segmentCount = s_segmentsPerBorder[1];
			values.angle1 = Cache.MinAngle + Cache.DeltaAngle * spriteBorder.x;
			values.angle2 = Cache.MaxAngle - Cache.DeltaAngle * spriteBorder.z;
			values.radius1 = s_radius[1];
			values.radius2 = Cache.OuterRadius;
			values.uvDelta = (values.uvMax - values.uvMin) / values.segmentCount;
			RenderArc(ref values);

			// right bot 

			//values = new ArcRenderValues();
			values.uvMin = new Vector2(innerUV.x, outerUV.y);
			values.uvMax = new Vector2(outerUV.x, innerUV.y);
			values.segmentCount = s_segmentsPerBorder[2];
			values.angle1 = Cache.MaxAngle - Cache.DeltaAngle * spriteBorder.z;
			values.angle2 = Cache.MaxAngle;
			values.radius1 = Cache.InnerRadius;
			values.radius2 = s_radius[0];
			values.uvDelta = (values.uvMax - values.uvMin) / values.segmentCount;
			RenderArc(ref values);

			// right

			//values = new ArcRenderValues();
			values.uvMin = new Vector2(innerUV.x, innerUV.y);
			values.uvMax = new Vector2(outerUV.x, innerUV.w);
			values.segmentCount = s_segmentsPerBorder[2];
			values.angle1 = Cache.MaxAngle - Cache.DeltaAngle * spriteBorder.z;
			values.angle2 = Cache.MaxAngle;
			values.radius1 = s_radius[0];
			values.radius2 = s_radius[1];
			values.uvDelta = (values.uvMax - values.uvMin) / values.segmentCount;
			RenderArc(ref values);

			//  right top

			//values = new ArcRenderValues();
			values.uvMin = new Vector2(innerUV.x, innerUV.w);
			values.uvMax = new Vector2(outerUV.x, outerUV.w);
			values.segmentCount = s_segmentsPerBorder[2];
			values.angle1 = Cache.MaxAngle - Cache.DeltaAngle * spriteBorder.z;
			values.angle2 = Cache.MaxAngle;
			values.radius1 = s_radius[1];
			values.radius2 = Cache.OuterRadius;
			values.uvDelta = (values.uvMax - values.uvMin) / values.segmentCount;
			RenderArc(ref values);
		}

		void RenderArc(ref ArcRenderValues arc)
		{
			QuadRenderValues q = default;
			Rect rect = Cache.TransformRect;
			Vector2 uvMin = arc.uvMin;

			arc.uvDelta = (arc.uvMax - uvMin) / arc.segmentCount;
			arc.uvDeltaGradient = 1f / arc.segmentCount;
			q.uvGradient = new(0, arc.uvDeltaGradient);
			Vector2 uvMax = arc.uvMin + arc.uvDelta;
			uvMax.y = arc.uvMax.y;
			q.uvSprite = new Vector4(uvMin.x, uvMin.y, uvMax.x, uvMax.y);

			float angle1 = arc.angle1;
			float angleDelta = (arc.angle2 - arc.angle1) / arc.segmentCount;
			float angle2 = angle1 + angleDelta;

			float radius1X = arc.radius1 * rect.width / 2;
			float radius1Y = arc.radius1 * rect.height / 2;
			float radius2X = arc.radius2 * rect.width / 2;
			float radius2Y = arc.radius2 * rect.height / 2;
			q.leftBot = new Vector3(MathUtilities.Cos(angle1 * DEG2_RAD) * radius1X, MathUtilities.Sin(angle1 * DEG2_RAD) * radius1Y) + Cache.CircleCenter;
			q.leftTop = new Vector3(MathUtilities.Cos(angle1 * DEG2_RAD) * radius2X, MathUtilities.Sin(angle1 * DEG2_RAD) * radius2Y) + Cache.CircleCenter;
			q.rightTop = new Vector3(MathUtilities.Cos(angle2 * DEG2_RAD) * radius2X, MathUtilities.Sin(angle2 * DEG2_RAD) * radius2Y) + Cache.CircleCenter;
			q.rightBot = new Vector3(MathUtilities.Cos(angle2 * DEG2_RAD) * radius1X, MathUtilities.Sin(angle2 * DEG2_RAD) * radius1Y) + Cache.CircleCenter;
			q.angles = new(angle1, angle2);
			q.radius = new(0, 1);
			for (int i = 0; i < arc.segmentCount; i++)
			{
				q.leftBot.Set(MathUtilities.Cos(angle1 * DEG2_RAD) * radius1X + Cache.CircleCenter.x, MathUtilities.Sin(angle1 * DEG2_RAD) * radius1Y + Cache.CircleCenter.y, 0);
				q.leftTop.Set(MathUtilities.Cos(angle1 * DEG2_RAD) * radius2X + Cache.CircleCenter.x, MathUtilities.Sin(angle1 * DEG2_RAD) * radius2Y + Cache.CircleCenter.y, 0);
				q.rightTop.Set(MathUtilities.Cos(angle2 * DEG2_RAD) * radius2X + Cache.CircleCenter.x, MathUtilities.Sin(angle2 * DEG2_RAD) * radius2Y + Cache.CircleCenter.y, 0);
				q.rightBot.Set(MathUtilities.Cos(angle2 * DEG2_RAD) * radius1X + Cache.CircleCenter.x, MathUtilities.Sin(angle2 * DEG2_RAD) * radius1Y + Cache.CircleCenter.y, 0);

				if (Settings.LocalRescaleDelta != 0)
				{
					q.leftBot = Cache.Matrix4x4.MultiplyPoint(q.leftBot);
					q.leftTop = Cache.Matrix4x4.MultiplyPoint(q.leftTop);
					q.rightTop = Cache.Matrix4x4.MultiplyPoint(q.rightTop);
					q.rightBot = Cache.Matrix4x4.MultiplyPoint(q.rightBot);
				}

				RenderQuad(ref arc, ref q);

				angle1 += angleDelta;
				angle2 += angleDelta;
				q.uvSprite.x += arc.uvDelta.x;
				q.uvSprite.z += arc.uvDelta.x;
				q.uvGradient.x += arc.uvDeltaGradient;
				q.uvGradient.y += arc.uvDeltaGradient;
				q.angles.Set(angle1, angle2);
			}
		}

		void RenderFilled()
		{
		}
		
		void RenderQuad(ref ArcRenderValues arc, ref QuadRenderValues q)
		{
			UIVertex[] vertices = _pool.Get();

			vertices[0].position.Set(q.leftBot.x, q.leftBot.y, q.leftBot.z);
			vertices[1].position.Set(q.rightBot.x, q.rightBot.y, q.rightBot.z);
			vertices[2].position.Set(q.rightTop.x, q.rightTop.y, q.rightTop.z);
			vertices[3].position.Set(q.leftTop.x, q.leftTop.y, q.leftTop.z);
			Vector2 size = Cache.TransformRect.size;
			Vector2 sizeMultiplied = size * multipliedPixelsPerUnit;

			switch (Settings.Type)
			{
				case ImageFillType.Tiled:
					{
						vertices[0].uv0.Set(q.leftBot.x / sizeMultiplied.x, q.leftBot.y / sizeMultiplied.y, 0, 0);
						vertices[1].uv0.Set(q.rightBot.x / sizeMultiplied.x, q.rightBot.y / sizeMultiplied.y, 0, 0);
						vertices[2].uv0.Set(q.rightTop.x / sizeMultiplied.x, q.rightTop.y / sizeMultiplied.y, 0, 0);
						vertices[3].uv0.Set(q.leftTop.x / sizeMultiplied.x, q.leftTop.y / sizeMultiplied.y, 0, 0);
						break;
					}
				default:
					{
						vertices[0].uv0.Set(q.uvSprite.x, q.uvSprite.y, 0, 0);
						vertices[1].uv0.Set(q.uvSprite.z, q.uvSprite.y, 0, 0);
						vertices[2].uv0.Set(q.uvSprite.z, q.uvSprite.w, 0, 0);
						vertices[3].uv0.Set(q.uvSprite.x, q.uvSprite.w, 0, 0);

						break;
					}
			}
			if (hasGradients)
			{
				Vector2 min = Cache.TransformRect.min + (Vector2)Cache.CircleCenter;
				for (int v = 0; v < 4; v++)
				{
					Color color = Cache.Color;
					for (int g = 0; g < Settings.Gradients.Length; g++)
					{
						SectorGradient gr = Settings.Gradients[g];
						{
							color = gr.Apply(color, vertices[v], Cache.TransformRect, q, arc, Settings, v > 1 ? 1 : 0, v is 1 or 2 ? 1 : 0);
						}
					}
					vertices[v].color = color;
				}
			}
			else
			{
				vertices[0].color = Cache.Color32;
				vertices[1].color = Cache.Color32;
				vertices[2].color = Cache.Color32;
				vertices[3].color = Cache.Color32;
			}

			_vh.AddUIVertexQuad(vertices);
		}

		void SetDirty()
		{
			if (!_sector || !_sector.isActiveAndEnabled)
				return;
#if UNITY_EDITOR
			EditorUtility.SetDirty(_sector);
#endif
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
	}
}