/// Credit RahulOfTheRamanEffect
/// Sourced from - https://forum.unity3d.com/members/rahuloftheramaneffect.773241/

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Arranges child objects into a non-uniform grid, with fixed column widths and flexible row heights
    /// </summary>
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

        private float[] maxPreferredHeightInRows;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            float horizontalSize = padding.horizontal;

            if (columnWidths.Length > 1)
                horizontalSize += ((columnWidths.Length - 1) * columnSpacing);

            // We calculate the actual cell count for cases where the number of children is lesser than the number of columns
            int actualCellCount = Mathf.Min(rectChildren.Count, columnWidths.Length);

            for (int i = 0; i < actualCellCount; i++)
                horizontalSize += columnWidths[i];

            SetLayoutInputForAxis(horizontalSize, horizontalSize, 0, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            if (columnWidths.Length == 0)
                return;

            int rowCount = Mathf.CeilToInt(rectChildren.Count / (float)columnWidths.Length);

            maxPreferredHeightInRows = new float[rowCount];

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
                // Find the max value for minimum and preferred heights in each row
                for (int i = 0; i < rowCount; i++)
                {
                    float maxMinimumHeightInRow = 0;
                    float maxPreferredHeightInRow = 0;

                    for (int j = 0; j < columnWidths.Length; j++)
                    {
                        int childIndex = (i * columnWidths.Length) + j;

                        if (childIndex >= rectChildren.Count)
                            break;

                        maxPreferredHeightInRow = Mathf.Max(LayoutUtility.GetPreferredHeight(rectChildren[childIndex]), maxPreferredHeightInRow);
                        maxMinimumHeightInRow = Mathf.Max(LayoutUtility.GetMinHeight(rectChildren[childIndex]), maxMinimumHeightInRow);
                    }

                    maxMinimumHeightInRow = Mathf.Max(minimumRowHeight, maxMinimumHeightInRow);
                    totalMinHeight += maxMinimumHeightInRow;

                    maxPreferredHeightInRow = Mathf.Max(minimumRowHeight, maxPreferredHeightInRow);
                    maxPreferredHeightInRows[i] = maxPreferredHeightInRow;
                    totalPreferredHeight += maxPreferredHeightInRow;
                }
            }
            else
            {
                for (int i = 0; i < rowCount; i++)
                {
                    maxPreferredHeightInRows[i] = minimumRowHeight;
                }

                totalMinHeight += rowCount * minimumRowHeight;
                totalPreferredHeight = totalMinHeight;
            }

            totalPreferredHeight = Mathf.Max(totalMinHeight, totalPreferredHeight);
            SetLayoutInputForAxis(totalMinHeight, totalPreferredHeight, 1, 1);
        }

        public override void SetLayoutHorizontal()
        {
            int columnCount = columnWidths.Length;

            if (columnCount == 0)
                return;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                RectTransform child = rectChildren[i];
                m_Tracker.Add(this, child,
                        DrivenTransformProperties.Anchors |
                        DrivenTransformProperties.AnchoredPosition |
                        DrivenTransformProperties.SizeDelta);
                child.anchorMin = Vector2.up;
                child.anchorMax = Vector2.up;
                Vector2 childSizeDelta = child.sizeDelta;
                childSizeDelta.x = columnWidths[i % columnCount];
                child.sizeDelta = childSizeDelta;
            }
        }

        public override void SetLayoutVertical()
        {
            int columnCount = columnWidths.Length;
            if (columnCount == 0)
                return;

            int rowCount = Mathf.CeilToInt(rectChildren.Count / (float)columnCount);

            float tableLayoutHeight = rectTransform.rect.height;

            int cornerX = (int)startCorner % 2;
            int cornerY = (int)startCorner / 2;

            Vector2 startOffset = new Vector2();
            Vector2 requiredSizeWithoutPadding = new Vector2();

            for (int i = 0; i < columnCount; i++)
            {
                requiredSizeWithoutPadding.x += columnWidths[i];
                requiredSizeWithoutPadding.x += columnSpacing;
            }
            requiredSizeWithoutPadding.x -= columnSpacing;

            startOffset.x = GetStartOffset(0, requiredSizeWithoutPadding.x);

            for (int i = 0; i < rowCount; i++)
            {
                requiredSizeWithoutPadding.y += maxPreferredHeightInRows[i];
                requiredSizeWithoutPadding.y += rowSpacing;
            }

            requiredSizeWithoutPadding.y -= rowSpacing;

            startOffset.y = GetStartOffset(1, requiredSizeWithoutPadding.y);

            if (cornerX == 1)
                startOffset.x += requiredSizeWithoutPadding.x;

            if (cornerY == 1)
                startOffset.y += requiredSizeWithoutPadding.y;

            float positionY = startOffset.y;

            for (int i = 0; i < rowCount; i++)
            {
                float positionX = startOffset.x;

                if (cornerY == 1)
                    positionY -= maxPreferredHeightInRows[i];

                for (int j = 0; j < columnCount; j++)
                {
                    int childIndex = (i * columnCount) + j;

                    if (childIndex >= rectChildren.Count)
                        break;

                    if (cornerX == 1)
                        positionX -= columnWidths[j];

                    SetChildAlongAxis(rectChildren[childIndex], 0, positionX, columnWidths[j]);
                    SetChildAlongAxis(rectChildren[childIndex], 1, positionY, maxPreferredHeightInRows[i]);

                    if (cornerX == 1)
                        positionX -= columnSpacing;
                    else
                        positionX += columnWidths[j] + columnSpacing;
                }

                if (cornerY == 1)
                    positionY -= rowSpacing;
                else
                    positionY += maxPreferredHeightInRows[i] + rowSpacing;
            }
        }
    }
}