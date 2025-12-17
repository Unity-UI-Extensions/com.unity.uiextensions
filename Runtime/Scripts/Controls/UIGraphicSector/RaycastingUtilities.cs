///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.ui-graphic-sector

namespace UnityEngine.UI.Extensions
{
	public class RaycastingUtilities
	{
		public bool IsRaycastLocationValid(Sector sector, Vector2 screenPoint, Camera eventCamera)
		{
			RectTransform rectTransform = sector.rectTransform;
			Vector2 localPoint;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out localPoint))
				return false;

			Vector2 center = sector.Cache.CircleCenter;
			Vector2 pointerVector = localPoint - center;
			float inner = sector.Cache.InnerRadius;
			float outer = sector.Cache.OuterRadius;
			float angle = Vector2.SignedAngle(Vector2.right, pointerVector);
			Vector2 size = sector.Cache.TransformRect.size / 2;
			Vector2 vector = new Vector2(MathUtilities.Cos(angle * Mathf.Deg2Rad), MathUtilities.Sin(angle * Mathf.Deg2Rad));
			Vector2 innerVector = vector * size * inner;
			Vector2 outerVector = vector * size * outer;

			// Debug.DrawLine(screenPoint, rectTransform.TransformPoint(center), Color.black);
			// Debug.DrawLine(rectTransform.TransformPoint(center), rectTransform.TransformPoint(center) + (Vector3)innerVector, Color.black);
			// Debug.DrawLine(default, pointerVector, Color.black);

			if (pointerVector.sqrMagnitude < innerVector.sqrMagnitude || pointerVector.sqrMagnitude > outerVector.sqrMagnitude)
			{
				return false;
			}

			Vector2 startVector = sector.Cache.StartVector;
			Vector2 endVector = sector.Cache.EndVector;
			if (sector.Settings.Clockwise == SectorDirection.Clockwise)
			{
				(startVector, endVector) = (endVector, startVector);
			}
			if (sector.Cache.DeltaAngleAbs < 181)
			{
				if (IsLeft(center, center + startVector, localPoint))
				{
					return false;
				}
// #if UNITY_EDITOR
// 				if (Application.isPlaying)
// 				{
// 					Vector2 drawCenter = rectTransform.TransformPoint(center);
// 					Vector2 drawVector = rectTransform.TransformVector(endVector);
// 					//if (sector.Settings.Gizmo)
// 					{
// 						Debug.DrawLine(drawCenter, drawCenter + drawVector * 1000, Color.red);
// 					}
// 				}
// #endif

				if (!IsLeft(center, center + endVector, localPoint))
				{
					return false;
				}
			}
			return true;
		}

		static bool IsLeft(Vector2 a, Vector2 b, Vector2 c)
		{
			Vector2 vector = a - b;
			Vector2 originToPoint = c - b;
			return -vector.x * originToPoint.y + vector.y * originToPoint.x < 0;
		}
	}
}