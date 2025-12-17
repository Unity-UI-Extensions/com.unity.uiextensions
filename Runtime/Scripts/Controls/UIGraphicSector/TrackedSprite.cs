///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.ui-graphic-sector

using System;
using System.Collections.Generic;
using UnityEngine.U2D;

namespace UnityEngine.UI.Extensions
{
	[Serializable]
	public class TrackedSprite
	{
		MaskableGraphic _graphic;

		// To track textureless images, which will be rebuild if sprite atlas manager registered a Sprite Atlas that will give this image new texture
		static List<TrackedSprite> m_TrackedTextureless = new();
		static bool s_Initialized;

		[SerializeField] Sprite m_Sprite;
		// Whether this is being tracked for Atlas Binding.
		bool m_Tracked;
		public Sprite sprite
		{
			get { return m_Sprite; }
			set
			{
				if (m_Sprite != null)
				{
					if (m_Sprite != value)
					{
						m_Sprite = value;
						_graphic.SetAllDirty();
						TrackSprite();
					}
				}
				else if (value != null)
				{
					m_Sprite = value;
					_graphic.SetAllDirty();
					TrackSprite();
				}
			}
		}

		[NonSerialized] Sprite m_OverrideSprite;

		public Sprite overrideSprite
		{
			get => activeSprite;
			set
			{
				if (!SetClass(ref m_OverrideSprite, value)) return;
				_graphic.SetAllDirty();
				TrackSprite();
			}
		}

		public Sprite activeSprite => m_OverrideSprite != null ? m_OverrideSprite : sprite;

		public bool hasBorder
		{
			get
			{
				if (activeSprite != null)
				{
					Vector4 v = activeSprite.border;
					return v.sqrMagnitude > 0f;
				}
				return false;
			}
		}

		static void Rebuild(SpriteAtlas spriteAtlas)
		{
			for (var i = m_TrackedTextureless.Count - 1; i >= 0; i--)
			{
				var tracked = m_TrackedTextureless[i];
				if (null != tracked.activeSprite && spriteAtlas.CanBindTo(tracked.activeSprite))
				{
					tracked._graphic.SetAllDirty();
					m_TrackedTextureless.RemoveAt(i);
				}
			}
		}

		static void Track(TrackedSprite tracked)
		{
			if (!s_Initialized)
			{
				SpriteAtlasManager.atlasRegistered += Rebuild;
				s_Initialized = true;
			}

			m_TrackedTextureless.Add(tracked);
		}

		static void UnTrack(TrackedSprite tracked)
		{
			m_TrackedTextureless.Remove(tracked);
		}

		public void OnEnable(MaskableGraphic graphic)
		{
			_graphic = graphic;
			TrackSprite();
		}

		public void OnDisable()
		{
			if (m_Tracked) UnTrack(this);
		}

		void TrackSprite()
		{
			if (activeSprite == null || activeSprite.texture != null) return;
			Track(this);
			m_Tracked = true;
		}

		static bool SetClass<T>(ref T currentValue, T newValue) where T : class
		{
			if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
				return false;

			currentValue = newValue;
			return true;
		}
	}
}