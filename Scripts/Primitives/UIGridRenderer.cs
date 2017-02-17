/// Credit John Hattan (http://thecodezone.com/)
/// Sourced from - https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/issues/117/uigridrenderer

using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Primitives/UIGridRenderer")]
	public class UIGridRenderer : UILineRenderer
	{
		[SerializeField]
		private int m_gridWidth = 10;
		[SerializeField]
		private int m_gridHeight = 10;

		/// <summary>
		/// Width of the grid in Cells.
		/// </summary>
        public int GridWidth
		{
			get
			{
				return m_gridWidth;
			}

			set
			{
				if (m_gridWidth == value)
					return;
				m_gridWidth = value;
				SetAllDirty();
			}
		}

		/// <summary>
		/// Height of the Grid in cells.
		/// </summary>
        public int GridHeight
		{
			get
			{
				return m_gridHeight;
			}

			set
			{
				if (m_gridHeight == value)
					return;
				m_gridHeight = value;
				SetAllDirty();
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			relativeSize = true;

			int ArraySize = (GridHeight * 3) + 1;
			if(GridHeight % 2 == 0)
				++ArraySize; // needs one more line

			ArraySize += (GridWidth * 3) + 1;

			m_points = new Vector2[ArraySize];

			int Index = 0;
			for(int i = 0; i < GridHeight; ++i)
			{
				float xFrom = 1;
				float xTo = 0;
				if(i % 2 == 0)
				{
					// reach left instead
					xFrom = 0;
					xTo = 1;
				}

				float y = ((float)i) / GridHeight;
				m_points[Index].x = xFrom;
				m_points[Index].y = y;
				++Index;
				m_points[Index].x = xTo;
				m_points[Index].y = y;
				++Index;
				m_points[Index].x = xTo;
				m_points[Index].y = (float)(i + 1) / GridHeight;
				++Index;
			}

			if(GridHeight % 2 == 0)
			{
				// two lines to get to 0, 1
				m_points[Index].x = 1;
				m_points[Index].y = 1;
				++Index;
			}

			m_points[Index].x = 0;
			m_points[Index].y = 1;
			++Index;

			// line is now at 0,1, so we can draw the columns
			for(int i = 0; i < GridWidth; ++i)
			{
				float yFrom = 1;
				float yTo = 0;
				if(i % 2 == 0)
				{
					// reach up instead
					yFrom = 0;
					yTo = 1;
				}

				float x = ((float)i) / GridWidth;
				m_points[Index].x = x;
				m_points[Index].y = yFrom;
				++Index;
				m_points[Index].x = x;
				m_points[Index].y = yTo;
				++Index;
				m_points[Index].x = (float)(i + 1) / GridWidth;
				m_points[Index].y = yTo;
				++Index;
			}

			if(GridWidth % 2 == 0)
			{
				// one more line to get to 1, 1
				m_points[Index].x = 1;
				m_points[Index].y = 1;
			}
			else
			{
				// one more line to get to 1, 0
				m_points[Index].x = 1;
				m_points[Index].y = 0;
			}

			base.OnPopulateMesh(vh);
		}
	}
}