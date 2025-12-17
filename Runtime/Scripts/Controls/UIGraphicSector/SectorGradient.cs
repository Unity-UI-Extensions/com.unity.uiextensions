///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.ui-graphic-sector

using System;
using UnityGradient = UnityEngine.Gradient;

namespace UnityEngine.UI.Extensions
{
	[Serializable]
	public class SectorGradient
	{
		public static UnityGradient WHITE => new()
		{
			alphaKeys = new[] { new GradientAlphaKey(0, 255), },
			colorKeys = new[] { new GradientColorKey(Color.white, 0), }
		};
		public UnityGradient Gradient = WHITE;
		public ColorOperation Operation;
		public FillMethod UV;
		public AxisDirection RectDirection;

		public SectorGradient()
		{
			Gradient = WHITE;
		}

		public Color Apply(Color currentColor, UIVertex vertex, Rect rect, QuadRenderValues q, ArcRenderValues arc, Settings settings, int isOuterRadius, int isLastAngle)
		{
			int d = (int)RectDirection;

			Color color = UV switch
			{
				FillMethod.Radius => Gradient.Evaluate(q.radius[isOuterRadius]),
				FillMethod.Rect => Gradient.Evaluate((vertex.position[d] - rect.min[d]) / rect.size[d]),
				FillMethod.Degree => Gradient.Evaluate(q.angles[isLastAngle] / settings.DegreesTotal),
				FillMethod.DegreeSector => Gradient.Evaluate(q.uvGradient[isLastAngle]),
				_ => Color.white,
			};

			for (int i = 0; i < 4; i++)
			{
				switch (Operation)
				{
					case ColorOperation.Multiply:
						{
							currentColor[i] *= color[i];
							break;
						}
					case ColorOperation.Add:
						{
							currentColor[i] += color[i];
							break;
						}
					case ColorOperation.Override:
						{
							currentColor[i] = color[i];
							break;
						}
					case ColorOperation.Skip: { break; }
				}
			}
			//return isLastAngle == 1 ? Color.white : Color.black;
			return currentColor;
		}
	}
}