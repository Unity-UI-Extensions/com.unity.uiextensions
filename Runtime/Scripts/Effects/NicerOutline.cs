/// Credit Melang, Lee Hui
/// Sourced from - http://forum.unity3d.com/members/melang.593409/
/// GC Alloc fix - https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/pull-requests/130
/// NOT supported in Unity 2022

#if !UNITY_2022_1_OR_NEWER
using System.Collections.Generic;
namespace UnityEngine.UI.Extensions
{
    //An outline that looks a bit nicer than the default one. It has less "holes" in the outline by drawing more copies of the effect
    [AddComponentMenu("UI/Effects/Extensions/Nicer Outline")]
	public class NicerOutline : BaseMeshEffect
	{
		[SerializeField]
		private Color m_EffectColor = new Color (0f, 0f, 0f, 0.5f);

		[SerializeField]
		private Vector2 m_EffectDistance = new Vector2 (1f, -1f);

		[SerializeField]
		private bool m_UseGraphicAlpha = true;

		private List < UIVertex > m_Verts = new List<UIVertex>();

		//
		// Properties
		//
		public Color effectColor
		{
			get
			{
				return this.m_EffectColor;
			}
			set
			{
				this.m_EffectColor = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty ();
				}
			}
		}

		public Vector2 effectDistance
		{
			get
			{
				return this.m_EffectDistance;
			}
			set
			{
				if (value.x > 600f)
				{
					value.x = 600f;
				}
				if (value.x < -600f)
				{
					value.x = -600f;
				}
				if (value.y > 600f)
				{
					value.y = 600f;
				}
				if (value.y < -600f)
				{
					value.y = -600f;
				}
				if (this.m_EffectDistance == value)
				{
					return;
				}
				this.m_EffectDistance = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty ();
				}
			}
		}

		public bool useGraphicAlpha
		{
			get
			{
				return this.m_UseGraphicAlpha;
			}
			set
			{
				this.m_UseGraphicAlpha = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty ();
				}
			}
		}
		
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!this.IsActive ())
			{
				return;
			}

	        m_Verts.Clear();
            vh.GetUIVertexStream(m_Verts);

            Text foundtext = GetComponent<Text>();

			float best_fit_adjustment = 1f;

			if (foundtext && foundtext.resizeTextForBestFit)
			{
				best_fit_adjustment = (float)foundtext.cachedTextGenerator.fontSizeUsedForBestFit / (foundtext.resizeTextMaxSize-1); //max size seems to be exclusive

			}

			float distanceX = this.effectDistance.x * best_fit_adjustment;
			float distanceY = this.effectDistance.y * best_fit_adjustment;

			vh.Clear();

			int start = 0;

			// Apply Outline
			start += this.ApplyOutlineNoGC(m_Verts, this.effectColor, distanceX, distanceY, vh, start);
			start += this.ApplyOutlineNoGC(m_Verts, this.effectColor, distanceX, -distanceY, vh, start);
			start += this.ApplyOutlineNoGC(m_Verts, this.effectColor, -distanceX, distanceY, vh, start);
			start += this.ApplyOutlineNoGC(m_Verts, this.effectColor, -distanceX, -distanceY, vh, start);
			start += this.ApplyOutlineNoGC(m_Verts, this.effectColor, distanceX, 0, vh, start);
			start += this.ApplyOutlineNoGC(m_Verts, this.effectColor, -distanceX, 0, vh, start);
			start += this.ApplyOutlineNoGC(m_Verts, this.effectColor, 0, distanceY, vh, start);
			start += this.ApplyOutlineNoGC(m_Verts, this.effectColor, 0, -distanceY, vh, start);

			// Apply self Text stuff
			start += ApplyText(m_Verts, vh, start);
        }
        
		private int ApplyOutlineNoGC(List<UIVertex> verts, Color color, float x, float y, VertexHelper vh, int startIndex)
		{
			int length = verts.Count;
			for (int i = 0; i < length; ++i)
			{
				UIVertex vt = verts[i];

				Vector3 v = vt.position;
				v.x += x;
				v.y += y;
				vt.position = v;
				var newColor = color;
				if (m_UseGraphicAlpha)
					newColor.a = (byte)((newColor.a * verts[i].color.a) / 255);
				vt.color = newColor;

				// Tips: Since two triangles share same two vertices, in theory vertices can reduce to 4 / 6
				// But VertexHelper.FillMesh forbid, so leave it be.
				
				vh.AddVert(vt);
			}

			int triangleCount = length / 3;
			for(int i=0; i<triangleCount; ++i)
			{
				int start = startIndex + 3 * i;
				vh.AddTriangle(start + 0, start + 1, start + 2);
			}

			return length;
		}
		
		private int ApplyText(List<UIVertex> verts, VertexHelper vh, int startIndex)
		{
			int length = verts.Count;

			for (int i = 0; i < length; ++i)
			{
				vh.AddVert(verts[i]);
			}

			int triangleCount = length / 3;
			for (int i = 0; i < triangleCount; ++i)
			{
				int start = startIndex + 3 * i;
				vh.AddTriangle(start + 0, start + 1, start + 2);
			}

			return length;
		}


#if UNITY_EDITOR
		protected override void OnValidate ()
		{
			this.effectDistance = this.m_EffectDistance;
			base.OnValidate ();
		}
#endif
	}
}
#endif