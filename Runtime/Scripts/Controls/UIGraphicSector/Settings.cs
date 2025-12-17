///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.ui-graphic-sector


using System;

namespace UnityEngine.UI.Extensions
{
	[Serializable]
	public class Settings
	{
		const string T0 = "<b>Current Transform</b> = trasnform.parent.childCount used to calculate Sector angle\n<b>Circle Always</b> = full circle always\n<b>Parent Offset Transform</b> get number in ParentOffsetTransform to use it's parent.childCount to calculate sector angle";
		const string T1 = "<b>degrees</b>. Total outer angle to include all sectors with sibling Transforms";
		const string T2 = "<b>degrees</b>. Rotate Angle";
		const string T3 = "<b>degrees</b>. Create expanding triangle Margin between neibhour Sectors with sibling Transforms";
		const string T4 = "Required for <b>LocalRescaleDelta</b>\nRecalculate pivot of this RectTransform, to fit in Sector center";
		const string T5 = "Require and force <b>PivotToSector</b>!\nscales down or up Sector graphic, to it's local center. Create straight line Margin in abstract 'pixels' between neibhour Sectors with sibling Transforms";
		const string T6 = "<b>Affects Performance!</b> % Percents % Geometry Resolution, how many quads-per-degree would be generated";
		const string T7 = "Work only if <b>Type = Sliced</b> and Sprite has border in import settings\n% Percents % \n-- more radius border \n++ more degrees border";
		const string T8 = "% Percents % from RectTransform.size";
		const string T9 = "Work only if <b>Type = Sliced</b> or Tiled. Adjust Sprite scale";
		const string TA = "Work only if <b>Type = Sliced<b>. Skip center vertices";
		const string TB = "Work Same way as UI.Image.type\nSimple - stretch sprite to Sector along outer sides\nSliced - 9-slice sprite according to its Border in import settings\nTiled - Tiles sprite at RectTransform-spaced UV";
		const string TC = "Required for <b>CloneRootSectorOuter</b>. -1 = full circle always\n0 = use this.trasnform.parent.childCount to calculate Sector angle to draw\n1 and higher = this.transform.parent.parent.childCount used";
		const string TD = "Require ParentOffsetTransform > 0\nClone some settings from Sector, found at chosen parent";
		const string TE = "Sprite used to draw";
		const string TF = "<b>Affects Performance!</b>\nUnity's Gradient is slow. Remap vertex colors by radius, total degree or degree inside Sector";

		[Tooltip(T0)] public ShapeSource ShapeSource;
		[Tooltip(TD)] public bool CloneParentSectorSettings;
		[Tooltip(TC)] public int ParentOffsetTransform;
		//[Header("Cloned from root")]
		public SectorDirection Clockwise = SectorDirection.Clockwise;
		[Tooltip(T1), Range(0f, 360f)] public float DegreesTotal = 360;
		[Tooltip(T2), Range(-360f, 360f)] public float DegreesOffset;
		[Tooltip(T3), Range(-360f, 360f)] public float DegreesGap;
		[Tooltip(T4)] public bool PivotToSector;
		[Tooltip(T5)] public float LocalRescaleDelta;

		[Header("Other")]
		[Tooltip(T6), Range(1, 100)] public float GeometryResolution = 20;
		[Tooltip(T7), Range(-98f, 98f)] public float SpriteBorderBalance = 85;
		[Tooltip(T8)] public float Radius1 = 50;
		[Tooltip(T8)] public float Radius2 = 100;
		[Tooltip(T9)] public float PixelsPerUnitMultiplier = 1;
		[Tooltip(TA)] public bool FillCenter = true;
		[Tooltip(TB)] public ImageFillType Type = ImageFillType.Sliced;
		[Tooltip(TE)] public TrackedSprite Sprite = new();
		[Tooltip(TF)] public SectorGradient[] Gradients = Array.Empty<SectorGradient>();
		// public eMinMax FillOrigin;
		// public eFillMethod FillMethod;
	}
}