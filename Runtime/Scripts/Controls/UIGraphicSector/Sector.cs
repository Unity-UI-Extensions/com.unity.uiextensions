///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.ui-graphic-sector

using System;
using Unity.Profiling;

namespace UnityEngine.UI.Extensions
{
	/// <summary>
	///9-slice graphic that draws an oval sector inside:<br />
	///- 1 script<br />
	///- no custom shaders<br />
	///- input is caught correctly by sector<br />
	///- SpriteBorder support<br />
	///- SpriteAtlas support<br />
	///- pixelPerUnitMultiplier support<br />
	///- there are anchors for other transforms, including resizing to squeeze content into a sector<br />
	///- UV-by radius and tile by RectTransform<br />
	///- free pivot, or generated in the center of the sector<br />
	///- gradients<br />
	///- nested sectors can stick to the parent<br />
	///- offset by angle<br />
	///- pixel offset<br />
	///- performance-wise it seems fast<br /><br />
	///The only negative is that when the number of sectors in the circle changes, it is not immediately rebuilt. I haven't found the reason yet<br />
	/// </summary>
	[RequireComponent(typeof(CanvasRenderer)), ExecuteAlways]
	public class Sector : MaskableGraphic, ICanvasRaycastFilter, ILayoutSelfController
	{
		static ProfilerMarker MarkerEnable = new("Sector.OnEnable");
		static ProfilerMarker MarkerDisable = new("Sector.OnDisable");
		static ProfilerMarker MarkerRender = new("Sector.OnPopulateMesh");
		static ProfilerMarker MarkerLayoutSelf = new("Sector.LayoutSelf");
		static ProfilerMarker MarkerLayout = new("Sector.Layout");

		[SerializeField] public bool Logs;
		[field: SerializeField] public Settings Settings { get; private set; } = new();
		[field: NonSerialized] public Cache Cache { get; private set; } = new();
		[NonSerialized] RaycastingUtilities _raycasting = new();
		[NonSerialized] Rendering _rendering = new();
		public Layout Layout = new();
		float m_CachedReferencePixelsPerUnit;

		public override Material material
		{
			get
			{
				if (m_Material != null)
					return m_Material;
#if UNITY_EDITOR
				if (Application.isPlaying && Settings.Sprite.activeSprite && Settings.Sprite.activeSprite.associatedAlphaSplitTexture != null)
					return Image.defaultETC1GraphicMaterial;
#else
                if (Settings.Sprite.activeSprite && Settings.Sprite.activeSprite.associatedAlphaSplitTexture != null)
                    return Image.defaultETC1GraphicMaterial;
#endif

				return defaultMaterial;
			}

			set
			{
				base.material = value;
			}
		}

		public override Texture mainTexture
		{
			get
			{
				if (Settings.Sprite.activeSprite == null)
				{
					if (material != null && material.mainTexture != null)
					{
						return material.mainTexture;
					}
					return s_WhiteTexture;
				}

				return Settings.Sprite.activeSprite.texture;
			}
		}

		public float pixelsPerUnit
		{
			get
			{
				float spritePixelsPerUnit = 100;
				if (Settings.Sprite.activeSprite)
					spritePixelsPerUnit = Settings.Sprite.activeSprite.pixelsPerUnit;

				if (canvas)
					m_CachedReferencePixelsPerUnit = canvas.referencePixelsPerUnit;

				return spritePixelsPerUnit / m_CachedReferencePixelsPerUnit;
			}
		}

		public float multipliedPixelsPerUnit => pixelsPerUnit * Settings.PixelsPerUnitMultiplier;

		protected Sector() => useLegacyMeshGeneration = false;

		protected override void OnEnable()
		{
			MarkerEnable.Begin(this);
			Settings.Sprite.OnEnable(this);
			base.OnEnable();
			Layout.SetLayoutHorizontal(this);
			MarkerEnable.End();
		}

		protected override void OnDisable()
		{
			MarkerDisable.Begin(this);
			Settings.Sprite.OnDisable();
			base.OnDisable();
			MarkerDisable.End();
		}

		void Update()
		{
			_rendering.Update(this);
			MarkerLayout.Begin(this);
			Layout.Update(this);
			MarkerLayout.End();
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			MarkerRender.Begin(this);
			_rendering.OnPopulateMesh(vh, this, _raycasting);
			MarkerRender.End();
		}

		void ILayoutController.SetLayoutHorizontal()
		{
			MarkerLayoutSelf.Begin(this);
			Layout.SetLayoutHorizontal(this);
			MarkerLayoutSelf.End();
		}

		void ILayoutController.SetLayoutVertical()
		{
			MarkerLayoutSelf.Begin(this);
			Layout.SetLayoutHorizontal(this);
			MarkerLayoutSelf.End();
		}

		protected override void OnTransformParentChanged()
		{
			Layout.SetLayoutHorizontal(this);
			base.OnTransformParentChanged();
		}

		protected override void OnRectTransformDimensionsChange()
		{
			Layout.SetLayoutHorizontal(this);
			base.OnRectTransformDimensionsChange();
		}

		bool ICanvasRaycastFilter.IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			return _raycasting.IsRaycastLocationValid(this, screenPoint, eventCamera);
		}

		public Transform GetRootSector()
		{
			if (Settings.ShapeSource == ShapeSource.CircleAlways) return null;
			if (Settings.ShapeSource == ShapeSource.CurrentTransform) return transform;
			Transform current = transform;
			Transform previous = current.parent;

			for (int i = 0; i < Settings.ParentOffsetTransform; i++)
			{
				Transform parent = current.parent;
				if (!parent) return previous;
				previous = current;
				current = parent;
			}
			return current;
		}

		#region Editor
#if UNITY_EDITOR
		protected override void OnValidate()
		{
			Settings.ParentOffsetTransform = Mathf.Clamp(Settings.ParentOffsetTransform, 1, GetTransformDepth());
			Settings.PixelsPerUnitMultiplier = Mathf.Max(Settings.PixelsPerUnitMultiplier, .001f);
			if (Settings.LocalRescaleDelta != 0)
			{
				//Settings.PivotToSector = true;
			}
			base.OnValidate();
		}

		int GetTransformDepth()
		{
			Transform parent = transform.parent;
			int transformDepth = 0;
			while (parent != null)
			{
				parent = parent.parent;
				transformDepth++;
			}
			return transformDepth;
		}
#endif
		#endregion
	}
}