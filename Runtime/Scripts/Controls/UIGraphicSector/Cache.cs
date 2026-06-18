///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.ui-graphic-sector

namespace UnityEngine.UI.Extensions
{
	public class Cache
	{
		public Sector RootSector;
		public Vector3 CircleCenter;
		public Vector3 SectorCenter;
		public Vector2 StartVector;
		public Vector2 MiddleVector;
		public Vector2 EndVector;
		public Color Color;
		public Color32 Color32;
		public Rect TransformRect;
		public RectTransform RootSectorTransform;
		public RectTransform Root;
		public Matrix4x4 Matrix4x4;
		public int childCount;

		public int SiblingIndex;
		public float InnerRadius;
		public float OuterRadius;
		public float HalfRadius;
		public float MinAngle;
		public float MiddleAngle;
		public float MaxAngle;
		public float DeltaAngle;
		public float DeltaAngleFinalAbs;
		public float DeltaAngleAbs;
		public float RectMinSize;
		public float ContentSize;
		public float DegreesOffset;
	}
}