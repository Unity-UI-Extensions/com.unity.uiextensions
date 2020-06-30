/// Credit RahulOfTheRamanEffect
/// Sourced from - https://forum.unity3d.com/members/rahuloftheramaneffect.773241/

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Arranges child objects into a non-uniform grid, with fixed column widths and flexible row heights
    /// </summary>
    [AddComponentMenu("Layout/Extensions/Table Layout Group")]
    public class TableLayoutGroup : LayoutGroup
    {
        public enum Corner
        {
            UpperLeft = 0,
            UpperRight = 1,
            LowerLeft = 2,
            LowerRight = 3
        }

        [SerializeField]
        protected Corner startCorner = Corner.UpperLeft;
        /// <summary>
        /// The corner starting from which the cells should be arranged
        /// </summary>
        public Corner StartCorner
        {
            get { return startCorner; }
            set
            {
                SetProperty(ref startCorner, value);
            }
        }

        [SerializeField]
        protected float[] columnWidths = new float[1] { 96f };
        /// <summary>
        /// The widths of all the columns in the table
        /// </summary>
        public float[] ColumnWidths
        {
            get { return columnWidths; }
            set
            {
                SetProperty(ref columnWidths, value);
            }
        }

        [SerializeField]
        protected float minimumRowHeight = 32f;
        /// <summary>
        /// The minimum height for any row in the table
        /// </summary>
        public float MinimumRowHeight
        {
            get { return minimumRowHeight; }
            set
            {
                SetProperty(ref minimumRowHeight, value);
            }
        }

        [SerializeField]
        protected bool flexibleRowHeight = true;
        /// <summary>
        /// Expand rows to fit the cell with the highest preferred height?
        /// </summary>
        public bool FlexibleRowHeight
        {
            get { return flexibleRowHeight; }
            set
            {
                SetProperty(ref flexibleRowHeight, value);
            }
        }

        [SerializeField]
        protected float columnSpacing = 0f;
        /// <summary>
        /// The horizontal spacing between each cell in the table
        /// </summary>
        public float ColumnSpacing
        {
            get { return columnSpacing; }
            set
            {
                SetProperty(ref columnSpacing, value);
            }
        }

        [SerializeField]
        protected float rowSpacing = 0;
        /// <summary>
        /// The vertical spacing between each row in the table
        /// </summary>
        public float RowSpacing
        {
            get { return rowSpacing; }
            set
            {
                SetProperty(ref rowSpacing, value);
            }
        }

        // Temporarily stores data generated during the execution CalculateLayoutInputVertical for use in SetLayoutVertical
        private float[] preferredRowHeights;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            float horizontalSize = padding.horizontal;

            // We calculate the actual cell count for cases where the number of children is lesser than the number of columns
            int actualCellCount = Mathf.Min(rectChildren.Count, columnWidths.Length);

            for (int i = 0; i < actualCellCount; i++)
            {
                horizontalSize += columnWidths[i];
                horizontalSize += columnSpacing;
            }

            horizontalSize -= columnSpacing;

            SetLayoutInputForAxis(horizontalSize, horizontalSize, 0, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            int columnCount = columnWidths.Length;
            int rowCount = Mathf.CeilToInt(rectChildren.Count / (float)columnCount);

            preferredRowHeights = new float[rowCount];

            float totalMinHeight = padding.vertical;
            float totalPreferredHeight = padding.vertical;

            if (rowCount > 1)
            {
                float heightFromSpacing = ((rowCount - 1) * rowSpacing);
                totalMinHeight += heightFromSpacing;
                totalPreferredHeight += heightFromSpacing;
            }

            if (flexibleRowHeight)
            {
                // If flexibleRowHeight is enabled, find the max value for minimum and preferred heights in each row

                float maxMinimumHeightInRow = 0;
                float maxPreferredHeightInRow = 0;

                for (int i = 0; i < rowCount; i++)
                {
                    maxMinimumHeightInRow = minimumRowHeight;
                    maxPreferredHeightInRow = minimumRowHeight;

                    for (int j = 0; j < columnCount; j++)
                    {
                        int childIndex = (i * columnCount) + j;

                        // Safeguard against tables with incomplete rows
                        if (childIndex == rectChildren.Count)
                            break;

                        maxPreferredHeightInRow = Mathf.Max(LayoutUtility.GetPreferredHeight(rectChildren[childIndex]), maxPreferredHeightInRow);
                        maxMinimumHeightInRow = Mathf.Max(LayoutUtility.GetMinHeight(rectChildren[childIndex]), maxMinimumHeightInRow);
                    }

                    totalMinHeight += maxMinimumHeightInRow;
                    totalPreferredHeight += maxPreferredHeightInRow;

                    // Add calculated row height to a commonly accessible array for reuse in SetLayoutVertical()
                    preferredRowHeights[i] = maxPreferredHeightInRow;
                }
            }
            else
            {
                // If flexibleRowHeight is disabled, then use the minimumRowHeight to calculate vertical layout information
                for (int i = 0; i < rowCount; i++)
                    preferredRowHeights[i] = minimumRowHeight;

                totalMinHeight += rowCount * minimumRowHeight;
                totalPreferredHeight = totalMinHeight;
            }

            totalPreferredHeight = Mathf.Max(totalMinHeight, totalPreferredHeight);
            SetLayoutInputForAxis(totalMinHeight, totalPreferredHeight, 1, 1);
        }

        public override void SetLayoutHorizontal()
        {
            // If no column width is defined, then assign a reasonable default
            if (columnWidths.Length == 0)
                columnWidths = new float[1] { 0f };

            int columnCount = columnWidths.Length;
            int cornerX = (int)startCorner % 2;

            float startOffset = 0;
            float requiredSizeWithoutPadding = 0;

            // We calculate the actual cell count for cases where the number of children is lesser than the number of columns
            int actualCellCount = Mathf.Min(rectChildren.Count, columnWidths.Length);

            for (int i = 0; i < actualCellCount; i++)
            {
                requiredSizeWithoutPadding += columnWidths[i];
                requiredSizeWithoutPadding += columnSpacing;
            }

            requiredSizeWithoutPadding -= columnSpacing;

            startOffset = GetStartOffset(0, requiredSizeWithoutPadding);

            if (cornerX == 1)
                startOffset += requiredSizeWithoutPadding;

            float positionX = startOffset;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                int currentColumnIndex = i % columnCount;

                // If it's the first cell in the row, reset positionX
                if (currentColumnIndex == 0)
                    positionX = startOffset;

                if (cornerX == 1)
                    positionX -= columnWidths[currentColumnIndex];

                SetChildAlongAxis(rectChildren[i], 0, positionX, columnWidths[currentColumnIndex]);

                if (cornerX == 1)
                    positionX -= columnSpacing;
                else
                    positionX += columnWidths[currentColumnIndex] + columnSpacing;
            }
        }

        public override void SetLayoutVertical()
        {
            int columnCount = columnWidths.Length;
            int rowCount = preferredRowHeights.Length;

            int cornerY = (int)startCorner / 2;

            float startOffset = 0;
            float requiredSizeWithoutPadding = 0;

            for (int i = 0; i < rowCount; i++)
                requiredSizeWithoutPadding += preferredRowHeights[i];

            if (rowCount > 1)
                requiredSizeWithoutPadding += (rowCount - 1) * rowSpacing;

            startOffset = GetStartOffset(1, requiredSizeWithoutPadding);

            if (cornerY == 1)
                startOffset += requiredSizeWithoutPadding;

            float positionY = startOffset;

            for (int i = 0; i < rowCount; i++)
            {
                if (cornerY == 1)
                    positionY -= preferredRowHeights[i];

                for (int j = 0; j < columnCount; j++)
                {
                    int childIndex = (i * columnCount) + j;

                    // Safeguard against tables with incomplete rows
                    if (childIndex == rectChildren.Count)
                        break;

                    SetChildAlongAxis(rectChildren[childIndex], 1, positionY, preferredRowHeights[i]);
                }

                if (cornerY == 1)
                    positionY -= rowSpacing;
                else
                    positionY += preferredRowHeights[i] + rowSpacing;
            }

            // Set preferredRowHeights to null to free memory
            preferredRowHeights = null;
        }
    }
}