///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.ui-graphic-sector

using System.Runtime.CompilerServices;

namespace UnityEngine.UI.Extensions
{
	public class MathUtilities
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Sin(float x)//x in radians
		{
			float sinn;
			if (x < -3.14159265f)
				x += 6.28318531f;
			else if (x > 3.14159265f)
				x -= 6.28318531f;

			if (x < 0)
			{
				sinn = 1.27323954f * x + 0.405284735f * x * x;

				if (sinn < 0)
					sinn = 0.225f * (sinn * -sinn - sinn) + sinn;
				else
					sinn = 0.225f * (sinn * sinn - sinn) + sinn;
				return sinn;
			}
			else
			{
				sinn = 1.27323954f * x - 0.405284735f * x * x;

				if (sinn < 0)
					sinn = 0.225f * (sinn * -sinn - sinn) + sinn;
				else
					sinn = 0.225f * (sinn * sinn - sinn) + sinn;
				return sinn;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Cos(float x)//x in radians
		{
			return Sin(x + 1.5707963f);
		}
	}
}