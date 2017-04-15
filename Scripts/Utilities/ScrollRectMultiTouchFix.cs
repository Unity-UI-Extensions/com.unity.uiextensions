/// Credit Erdener Gonenc - @PixelEnvision
/*USAGE: Simply use that instead of the regular ScrollRect */

using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu ("UI/Extensions/ScrollRectMultiTouchFix")]
	public class ScrollRectMultiTouchFix : ScrollRect
	{

		private int pid = -100;

		/// <summary>
		/// Begin drag event
		/// </summary>
		public override void OnBeginDrag (UnityEngine.EventSystems.PointerEventData eventData)
		{
			pid = eventData.pointerId;
			base.OnBeginDrag (eventData);
		}

		/// <summary>
		/// Drag event
		/// </summary>
		public override void OnDrag (UnityEngine.EventSystems.PointerEventData eventData)
		{
			if (pid == eventData.pointerId)
				base.OnDrag (eventData);
		}

		/// <summary>
		/// End drag event
		/// </summary>
		public override void OnEndDrag (UnityEngine.EventSystems.PointerEventData eventData)
		{
				pid = -100;
				base.OnEndDrag (eventData);
		}

	}
}