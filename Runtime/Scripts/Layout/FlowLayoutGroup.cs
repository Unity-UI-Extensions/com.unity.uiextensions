/// Credit Simie
/// Sourced from - http://forum.unity3d.com/threads/flowlayoutgroup.296709/
/// Example http://forum.unity3d.com/threads/flowlayoutgroup.296709/
/// Update by Martin Sharkbomb - http://forum.unity3d.com/threads/flowlayoutgroup.296709/#post-1977028
/// Last item alignment fix by Vicente Russo - https://bitbucket.org/SimonDarksideJ/unity-ui-extensions/issues/22/flow-layout-group-align
/// Vertical Flow by Ramon Molossi 

using System.Collections.Generic;
using System.Text;

namespace UnityEngine.UI.Extensions
{
	/// <summary>
	/// Layout Group controller that arranges children in bars, fitting as many on a line until total size exceeds parent bounds
	/// </summary>
	[AddComponentMenu("Layout/Extensions/Flow Layout Group")]
	public class FlowLayoutGroup : LayoutGroup
	{
		public enum Axis { Horizontal = 0, Vertical = 1 }

        private float _layoutHeight;
        private float _layoutWidth;

        public float SpacingX = 0f;
		public float SpacingY = 0f;
		public bool ExpandHorizontalSpacing = false;
		public bool ChildForceExpandWidth = false;
		public bool ChildForceExpandHeight = false;
		public bool invertOrder = false;

		[SerializeField]
		protected Axis m_StartAxis = Axis.Horizontal;

		public Axis StartAxis { get { return m_StartAxis; } set { SetProperty(ref m_StartAxis, value); } }

		public override void CalculateLayoutInputHorizontal()
		{
			if (StartAxis == Axis.Horizontal)
			{
				base.CalculateLayoutInputHorizontal();
				var minWidth = GetGreatestMinimumChildWidth() + padding.left + padding.right;
				SetLayoutInputForAxis(minWidth, -1, -1, 0);
			}
			else
			{
				_layoutWidth = SetLayout(0, true);
			}

		}

		public override void SetLayoutHorizontal()
		{
			SetLayout(0, false);
		}

		public override void SetLayoutVertical()
		{
			SetLayout(1, false);
		}

		public override void CalculateLayoutInputVertical()
		{
			if (StartAxis == Axis.Horizontal)
			{
				_layoutHeight = SetLayout(1, true);
			}
			else
			{
				base.CalculateLayoutInputHorizontal();
				var minHeight = GetGreatestMinimumChildHeigth() + padding.bottom + padding.top;
				SetLayoutInputForAxis(minHeight, -1, -1, 1);
			}
		}

		protected bool IsCenterAlign
		{
			get => childAlignment == TextAnchor.LowerCenter || childAlignment == TextAnchor.MiddleCenter || childAlignment == TextAnchor.UpperCenter;
		}

		protected bool IsRightAlign
		{
			get => childAlignment == TextAnchor.LowerRight || childAlignment == TextAnchor.MiddleRight || childAlignment == TextAnchor.UpperRight;
		}

		protected bool IsMiddleAlign
		{
			get => childAlignment == TextAnchor.MiddleLeft || childAlignment == TextAnchor.MiddleRight || childAlignment == TextAnchor.MiddleCenter;
		}

		protected bool IsLowerAlign
		{
			get => childAlignment == TextAnchor.LowerLeft || childAlignment == TextAnchor.LowerRight || childAlignment == TextAnchor.LowerCenter;
		}

		/// <summary>
		/// Holds the rects that will make up the current bar being processed
		/// </summary>
		private readonly IList<RectTransform> _itemList = new List<RectTransform>();

		/// <summary>
		/// Main layout method
		/// </summary>
		/// <param name="width">Width to calculate the layout with</param>
		/// <param name="axis">0 for horizontal axis, 1 for vertical</param>
		/// <param name="layoutInput">If true, sets the layout input for the axis. If false, sets child position for axis</param>
		public float SetLayout(int axis, bool layoutInput)
		{
			//container height and width
			var groupHeight = rectTransform.rect.height;
			var groupWidth = rectTransform.rect.width;

			float spacingBetweenBars = 0;
			float spacingBetweenElements = 0;
			float offset = 0;
			float counterOffset = 0;
			float groupSize = 0;
			float workingSize = 0;
			if (StartAxis == Axis.Horizontal)
			{
				groupSize = groupHeight;
				workingSize = groupWidth - padding.left - padding.right;
				if (IsLowerAlign)
				{
					offset = (float)padding.bottom;
					counterOffset = (float)padding.top;
				}
				else
				{
					offset = (float)padding.top;
					counterOffset = (float)padding.bottom;
				}
				spacingBetweenBars = SpacingY;
				spacingBetweenElements = SpacingX;
			}
			else if (StartAxis == Axis.Vertical)
			{
				groupSize = groupWidth;
				workingSize = groupHeight - padding.top - padding.bottom;
				if (IsRightAlign)
				{
					offset = (float)padding.right;
					counterOffset = (float)padding.left;
				}
				else
				{
					offset = (float)padding.left;
					counterOffset = (float)padding.right;
				}
				spacingBetweenBars = SpacingX;
				spacingBetweenElements = SpacingY;
			}

			var currentBarSize = 0f;
			var currentBarSpace = 0f;

			for (var i = 0; i < rectChildren.Count; i++)
			{
				int index = i;
				var child = rectChildren[index];
				float childSize = 0;
				float childOtherSize = 0;
				//get height and width of elements.
				if (StartAxis == Axis.Horizontal)
				{
					if (invertOrder)
					{
						index = IsLowerAlign ? rectChildren.Count - 1 - i : i;
					}
					child = rectChildren[index];
					childSize = LayoutUtility.GetPreferredSize(child, 0);
					childSize = Mathf.Min(childSize, workingSize);
					childOtherSize = LayoutUtility.GetPreferredSize(child, 1);
				}
				else if (StartAxis == Axis.Vertical)
				{
					if (invertOrder)
					{
						index = IsRightAlign ? rectChildren.Count - 1 - i : i;
					}
					child = rectChildren[index];
					childSize = LayoutUtility.GetPreferredSize(child, 1);
					childSize = Mathf.Min(childSize, workingSize);
					childOtherSize = LayoutUtility.GetPreferredSize(child, 0);
				}

				// If adding this element would exceed the bounds of the container,
				// go to a new bar after processing the current bar
				if (currentBarSize + childSize > workingSize)
				{
					currentBarSize -= spacingBetweenElements;

					// Process current bar elements positioning
					if (!layoutInput)
					{
						if (StartAxis == Axis.Horizontal)
						{
							float newOffset = CalculateRowVerticalOffset(groupSize, offset, currentBarSpace);
							LayoutRow(_itemList, currentBarSize, currentBarSpace, workingSize, padding.left, newOffset, axis);
						}
						else if (StartAxis == Axis.Vertical)
						{
							float newOffset = CalculateColHorizontalOffset(groupSize, offset, currentBarSpace);
							LayoutCol(_itemList, currentBarSpace, currentBarSize, workingSize, newOffset, padding.top, axis);
						}
					}

					// Clear existing bar
					_itemList.Clear();

					// Add the current bar space to total barSpace accumulator, and reset to 0 for the next row
					offset += currentBarSpace;
					offset += spacingBetweenBars;

					currentBarSpace = 0;
					currentBarSize = 0;
				}

				currentBarSize += childSize;
				_itemList.Add(child);

				// We need the largest element height to determine the starting position of the next line
				currentBarSpace = childOtherSize > currentBarSpace ? childOtherSize : currentBarSpace;

				currentBarSize += spacingBetweenElements;
			}

			// Layout the final bar
			if (!layoutInput)
			{
				if (StartAxis == Axis.Horizontal)
				{
					float newOffset = CalculateRowVerticalOffset(groupHeight, offset, currentBarSpace);
					currentBarSize -= spacingBetweenElements;
					LayoutRow(_itemList, currentBarSize, currentBarSpace, workingSize, padding.left, newOffset, axis);
				}
				else if (StartAxis == Axis.Vertical)
				{
					float newOffset = CalculateColHorizontalOffset(groupWidth, offset, currentBarSpace);
					currentBarSize -= spacingBetweenElements;
					LayoutCol(_itemList, currentBarSpace, currentBarSize, workingSize, newOffset, padding.top, axis);
				}
			}

			_itemList.Clear();

			// Add the last bar space to the barSpace accumulator
			offset += currentBarSpace;
			offset += counterOffset;

			if (layoutInput)
			{
				SetLayoutInputForAxis(offset, offset, -1, axis);
			}
			return offset;
		}

		private float CalculateRowVerticalOffset(float groupHeight, float yOffset, float currentRowHeight)
		{
			if (IsLowerAlign)
			{
				return groupHeight - yOffset - currentRowHeight;
			}
			else if (IsMiddleAlign)
			{
				return groupHeight * 0.5f - _layoutHeight * 0.5f + yOffset;
			}
			return yOffset;
		}

		private float CalculateColHorizontalOffset(float groupWidth, float xOffset, float currentColWidth)
		{
			if (IsRightAlign)
			{
				return groupWidth - xOffset - currentColWidth;
			}
			else if (IsCenterAlign)
			{
				return groupWidth * 0.5f - _layoutWidth * 0.5f + xOffset;
			}
			return xOffset;
		}

		protected void LayoutRow(IList<RectTransform> contents, float rowWidth, float rowHeight, float maxWidth, float xOffset, float yOffset, int axis)
		{
			var xPos = xOffset;

			if (!ChildForceExpandWidth && IsCenterAlign)
			{
				xPos += (maxWidth - rowWidth) * 0.5f;
			}
			else if (!ChildForceExpandWidth && IsRightAlign)
			{
				xPos += (maxWidth - rowWidth);
			}

			var extraWidth = 0f;
			var extraSpacing = 0f;

			if (ChildForceExpandWidth)
			{
				extraWidth = (maxWidth - rowWidth) / contents.Count;
			}
			else if (ExpandHorizontalSpacing)
			{
				extraSpacing = (maxWidth - rowWidth) / (contents.Count - 1);
				if (contents.Count > 1)
				{
					if (IsCenterAlign)
					{
						xPos -= extraSpacing * 0.5f * (contents.Count - 1);
					}
					else if (IsRightAlign)
					{
						xPos -= extraSpacing * (contents.Count - 1);
					}
				}
			}

			for (var j = 0; j < contents.Count; j++)
			{
				var index = IsLowerAlign ? contents.Count - 1 - j : j;

				var rowChild = contents[index];

				var rowChildWidth = LayoutUtility.GetPreferredSize(rowChild, 0) + extraWidth;
				var rowChildHeight = LayoutUtility.GetPreferredSize(rowChild, 1);

				if (ChildForceExpandHeight)
				{
					rowChildHeight = rowHeight;
				}

				rowChildWidth = Mathf.Min(rowChildWidth, maxWidth);
				var yPos = yOffset;

				if (IsMiddleAlign)
				{
					yPos += (rowHeight - rowChildHeight) * 0.5f;
				}
				else if (IsLowerAlign)
				{
					yPos += (rowHeight - rowChildHeight);
				}

				if (ExpandHorizontalSpacing && j > 0)
				{
					xPos += extraSpacing;
				}

				if (axis == 0)
				{
					SetChildAlongAxis(rowChild, 0, xPos, rowChildWidth);
				}
				else
				{
					SetChildAlongAxis(rowChild, 1, yPos, rowChildHeight);
				}

				// Don't do horizontal spacing for the last one
				if (j < contents.Count - 1)
				{
					xPos += rowChildWidth + SpacingX;
				}
			}
		}

		protected void LayoutCol(IList<RectTransform> contents, float colWidth, float colHeight, float maxHeight, float xOffset, float yOffset, int axis)
		{
			var yPos = yOffset;

			if (!ChildForceExpandHeight && IsMiddleAlign)
			{
				yPos += (maxHeight - colHeight) * 0.5f;
			}
			else if (!ChildForceExpandHeight && IsLowerAlign)
			{
				yPos += (maxHeight - colHeight);
			}

			var extraHeight = 0f;
			var extraSpacing = 0f;

			if (ChildForceExpandHeight)
			{
				extraHeight = (maxHeight - colHeight) / contents.Count;
			}
			else if (ExpandHorizontalSpacing)
			{
				extraSpacing = (maxHeight - colHeight) / (contents.Count - 1);
				if (contents.Count > 1)
				{
					if (IsMiddleAlign)
					{
						yPos -= extraSpacing * 0.5f * (contents.Count - 1);
					}
					else if (IsLowerAlign)
					{
						yPos -= extraSpacing * (contents.Count - 1);
					}
				}
			}

			for (var j = 0; j < contents.Count; j++)
			{
				var index = IsRightAlign ? contents.Count - 1 - j : j;

				var rowChild = contents[index];

				var rowChildWidth = LayoutUtility.GetPreferredSize(rowChild, 0);
				var rowChildHeight = LayoutUtility.GetPreferredSize(rowChild, 1) + extraHeight;

				if (ChildForceExpandWidth)
				{
					rowChildWidth = colWidth;
				}

				rowChildHeight = Mathf.Min(rowChildHeight, maxHeight);

				var xPos = xOffset;

				if (IsCenterAlign)
				{
					xPos += (colWidth - rowChildWidth) * 0.5f;
				}
				else if (IsRightAlign)
				{
					xPos += (colWidth - rowChildWidth);
				}

				if (ExpandHorizontalSpacing && j > 0)
				{
					yPos += extraSpacing;
				}

				if (axis == 0)
				{
					SetChildAlongAxis(rowChild, 0, xPos, rowChildWidth);
				}
				else
				{
					SetChildAlongAxis(rowChild, 1, yPos, rowChildHeight);
				}

				// Don't do vertical spacing for the last one
				if (j < contents.Count - 1)
				{
					yPos += rowChildHeight + SpacingY;
				}
			}
		}

		public float GetGreatestMinimumChildWidth()
		{
			var max = 0f;
			for (var i = 0; i < rectChildren.Count; i++)
			{
				var w = LayoutUtility.GetMinWidth(rectChildren[i]);
				max = Mathf.Max(w, max);
			}
			return max;
		}

		public float GetGreatestMinimumChildHeigth()
		{
			var max = 0f;
			for (var i = 0; i < rectChildren.Count; i++)
			{
				var w = LayoutUtility.GetMinHeight(rectChildren[i]);
				max = Mathf.Max(w, max);
			}
			return max;
		}

		protected override void OnDisable()
		{
			m_Tracker.Clear();
			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
		}
	}
}