/// Credit John Hattan (http://thecodezone.com/)
/// Sourced from - https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/issues/117/uigridrenderer


namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/UIGridRenderer")]
	public class UIGridRenderer : UILineRenderer
	{
		[SerializeField]
		private int m_GridColumns = 10;
		[SerializeField]
		private int m_GridRows = 10;

		/// <summary>
		/// Number of columns in the Grid
		/// </summary>
        public int GridColumns
		{
			get
			{
				return m_GridColumns;
			}

			set
			{
				if (m_GridColumns == value)
					return;
				m_GridColumns = value;
				SetAllDirty();
			}
		}

		/// <summary>
		/// Number of rows in the grid.
		/// </summary>
        public int GridRows
		{
			get
			{
				return m_GridRows;
			}

			set
			{
				if (m_GridRows == value)
					return;
				m_GridRows = value;
				SetAllDirty();
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			relativeSize = true;

			int ArraySize = (GridRows * 3) + 1;
			if(GridRows % 2 == 0)
				++ArraySize; // needs one more line

			ArraySize += (GridColumns * 3) + 1;

			m_points = new Vector2[ArraySize];

			int Index = 0;
			for(int i = 0; i < GridRows; ++i)
			{
				float xFrom = 1;
				float xTo = 0;
				if(i % 2 == 0)
				{
					// reach left instead
					xFrom = 0;
					xTo = 1;
				}

				float y = ((float)i) / GridRows;
				m_points[Index].x = xFrom;
				m_points[Index].y = y;
				++Index;
				m_points[Index].x = xTo;
				m_points[Index].y = y;
				++Index;
				m_points[Index].x = xTo;
				m_points[Index].y = (float)(i + 1) / GridRows;
				++Index;
			}

			if(GridRows % 2 == 0)
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
			for(int i = 0; i < GridColumns; ++i)
			{
				float yFrom = 1;
				float yTo = 0;
				if(i % 2 == 0)
				{
					// reach up instead
					yFrom = 0;
					yTo = 1;
				}

				float x = ((float)i) / GridColumns;
				m_points[Index].x = x;
				m_points[Index].y = yFrom;
				++Index;
				m_points[Index].x = x;
				m_points[Index].y = yTo;
				++Index;
				m_points[Index].x = (float)(i + 1) / GridColumns;
				m_points[Index].y = yTo;
				++Index;
			}

			if(GridColumns % 2 == 0)
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