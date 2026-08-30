/// Credit [PeterParkers007]
/// Sourced from - [https://github.com/PeterParkers007]
/// Based on FlowLayoutGroup by Simie - http://forum.unity3d.com/threads/flowlayoutgroup.296709/
/// Enhanced with:
/// - Multiple sizing modes (AutoSize, UniformCell, UniformCellKeepAspect, StretchRow)
/// - Fixed columns per row support
/// - Reverse wrap direction
/// - Aspect ratio preservation
/// - Improved handling of nested LayoutGroups
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Smart Flow Layout Group - Enhanced version of FlowLayoutGroup with multiple sizing modes.
    /// Supports uniform cells, aspect ratio preservation, stretch rows, and reverse wrapping.
    /// </summary>
    [AddComponentMenu("Layout/Extensions/Smart Flow Layout Group")]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class SmartFlowLayoutGroup : LayoutGroup
    {
        public enum Axis { Horizontal = 0, Vertical = 1 }

        public enum SizingMode
        {
            /// <summary>Reads child PreferredSize (original FlowLayoutGroup behavior).</summary>
            AutoSize = 0,
            /// <summary>Forces all children to CellSize, ignoring original proportions.</summary>
            UniformCell = 1,
            /// <summary>Uniform cell size; children scale to fit inside while preserving aspect ratio.</summary>
            UniformCellKeepAspect = 2,
            /// <summary>List rows: width fills container, height uses PreferredHeight. CellSize.y as minimum row height.</summary>
            StretchRow = 3,
        }

        [Tooltip("Flow start axis: Horizontal = left to right, wrap on overflow; Vertical = top to bottom, wrap on overflow")]
        [SerializeField] Axis m_StartAxis = Axis.Horizontal;

        [Tooltip("AutoSize=read preferred; UniformCell=force CellSize; KeepAspect=icon grid; StretchRow=full width list")]
        [SerializeField] SizingMode m_SizingMode = SizingMode.UniformCellKeepAspect;

        [Tooltip("Uniform cell size (used by UniformCell/KeepAspect; StretchRow uses y as minimum row height)")]
        [SerializeField] Vector2 m_CellSize = new Vector2(100f, 100f);

        [Tooltip("≤0 = auto wrap by container width; >0 = fixed columns per row (Horizontal axis only)")]
        [SerializeField] int m_ColumnsPerRow = 0;

        [Tooltip("Horizontal spacing between elements")]
        [SerializeField] float m_SpacingX = 0f;

        [Tooltip("Vertical spacing between rows")]
        [SerializeField] float m_SpacingY = 0f;

        [Tooltip("Stretch children height to fill the row height")]
        [SerializeField] bool m_ChildForceExpandHeight = false;

        [Tooltip("AutoSize mode: stretch children width to fill container width")]
        [SerializeField] bool m_ChildForceExpandWidth = false;

        [Tooltip("Reverse wrap direction: true = first row at bottom, stacking upward (Horizontal axis only)")]
        [SerializeField] bool m_ReverseWrap = false;

        /// <summary>Flow start axis.</summary>
        public Axis StartAxis { get => m_StartAxis; set => SetProperty(ref m_StartAxis, value); }

        /// <summary>Sizing mode determines how child elements are sized.</summary>
        public SizingMode Sizing { get => m_SizingMode; set => SetProperty(ref m_SizingMode, value); }

        /// <summary>Uniform cell size for grid-based modes.</summary>
        public Vector2 CellSize { get => m_CellSize; set => SetProperty(ref m_CellSize, value); }

        /// <summary>Fixed columns per row (0 = auto).</summary>
        public int ColumnsPerRow
        {
            get => m_ColumnsPerRow;
            set => SetProperty(ref m_ColumnsPerRow, Mathf.Max(0, value));
        }

        /// <summary>Spacing between elements (X = horizontal, Y = vertical).</summary>
        public Vector2 Spacing
        {
            get => new Vector2(m_SpacingX, m_SpacingY);
            set
            {
                m_SpacingX = value.x;
                m_SpacingY = value.y;
                SetDirty();
            }
        }

        /// <summary>Horizontal spacing between elements.</summary>
        public float SpacingX { get => m_SpacingX; set => SetProperty(ref m_SpacingX, value); }

        /// <summary>Vertical spacing between rows.</summary>
        public float SpacingY { get => m_SpacingY; set => SetProperty(ref m_SpacingY, value); }

        /// <summary>Stretch children to fill row height.</summary>
        public bool ChildForceExpandHeight
        {
            get => m_ChildForceExpandHeight;
            set => SetProperty(ref m_ChildForceExpandHeight, value);
        }

        /// <summary>Stretch children to fill container width in AutoSize mode.</summary>
        public bool ChildForceExpandWidth
        {
            get => m_ChildForceExpandWidth;
            set => SetProperty(ref m_ChildForceExpandWidth, value);
        }

        /// <summary>Reverse wrap direction (first row at bottom).</summary>
        public bool ReverseWrap
        {
            get => m_ReverseWrap;
            set => SetProperty(ref m_ReverseWrap, value);
        }

        float _layoutWidth;
        float _layoutHeight;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            if (m_StartAxis == Axis.Horizontal)
            {
                float min = GetGreatestMinimumChildWidth() + padding.left + padding.right;
                SetLayoutInputForAxis(min, -1, -1, 0);
            }
            else
            {
                _layoutWidth = SetLayout(0, true);
            }
        }

        public override void CalculateLayoutInputVertical()
        {
            if (m_StartAxis == Axis.Horizontal)
            {
                _layoutHeight = SetLayout(1, true);
            }
            else
            {
                float min = GetGreatestMinimumChildHeight() + padding.top + padding.bottom;
                SetLayoutInputForAxis(min, -1, -1, 1);
            }
        }

        public override void SetLayoutHorizontal() => SetLayout(0, false);
        public override void SetLayoutVertical() => SetLayout(1, false);

        /// <summary>Main layout calculation. Returns total size along the cross axis.</summary>
        float SetLayout(int axis, bool layoutInput)
        {
            float groupWidth = rectTransform.rect.width;
            float groupHeight = rectTransform.rect.height;

            bool horizontal = m_StartAxis == Axis.Horizontal;
            float workingSize = horizontal
                ? groupWidth - padding.left - padding.right
                : groupHeight - padding.top - padding.bottom;

            // Handle zero-size rect on first frame
            if (workingSize <= 0.01f && horizontal)
                workingSize = Mathf.Max(0f, GetGreatestMinimumChildWidth());

            var entries = new List<Entry>(rectChildren.Count);
            for (int i = 0; i < rectChildren.Count; i++)
                entries.Add(ResolveEntry(rectChildren[i], workingSize, horizontal));

            // Group entries into lines (rows or columns)
            var lines = new List<List<Entry>>();
            var currentLine = new List<Entry>();
            float lineCursor = 0f;

            int columnsPerRow = (horizontal && m_SizingMode != SizingMode.StretchRow)
                ? m_ColumnsPerRow
                : 0;

            float mainSpacing = horizontal ? m_SpacingX : m_SpacingY;

            for (int i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                float itemSize = horizontal ? e.SlotW : e.SlotH;

                bool exceedsWidth = currentLine.Count > 0
                    && lineCursor + mainSpacing + itemSize > workingSize + 0.01f;
                bool exceedsColumns = columnsPerRow > 0 && currentLine.Count >= columnsPerRow;

                if (currentLine.Count > 0 && (exceedsWidth || exceedsColumns))
                {
                    lines.Add(currentLine);
                    currentLine = new List<Entry>();
                    lineCursor = 0f;
                }

                if (currentLine.Count > 0)
                    lineCursor += mainSpacing;
                currentLine.Add(e);
                lineCursor += itemSize;
            }
            if (currentLine.Count > 0)
                lines.Add(currentLine);

            // Fill incomplete rows when using fixed columns
            if (columnsPerRow > 0)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    while (lines[i].Count < columnsPerRow)
                        lines[i].Add(Entry.Empty);
                }
            }

            bool reverse = m_ReverseWrap && horizontal;
            float offset;
            if (reverse)
                offset = groupHeight - padding.bottom;
            else
                offset = horizontal ? padding.top : padding.left;

            float crossSpacing = horizontal ? m_SpacingY : m_SpacingX;
            float totalCross = 0f;

            for (int li = 0; li < lines.Count; li++)
            {
                var line = lines[li];
                float lineMain = 0f;
                float lineCross = 0f;

                for (int j = 0; j < line.Count; j++)
                {
                    var e = line[j];
                    lineMain += (j > 0 ? mainSpacing : 0f) + (horizontal ? e.SlotW : e.SlotH);
                    if (e.IsValid)
                        lineCross = Mathf.Max(lineCross, horizontal ? e.SlotH : e.SlotW);
                }

                if (m_ChildForceExpandHeight && horizontal
                    && m_SizingMode != SizingMode.UniformCellKeepAspect
                    && m_SizingMode != SizingMode.StretchRow)
                    lineCross = Mathf.Max(lineCross, m_CellSize.y);

                if (reverse)
                {
                    offset -= lineCross;
                    if (!layoutInput)
                        PlaceLine(line, lineMain, lineCross, workingSize, offset, axis, horizontal, mainSpacing);
                    offset -= crossSpacing;
                }
                else
                {
                    if (!layoutInput)
                        PlaceLine(line, lineMain, lineCross, workingSize, offset, axis, horizontal, mainSpacing);
                    offset += lineCross + crossSpacing;
                }
                totalCross += lineCross + (li > 0 ? crossSpacing : 0f);
            }
            totalCross += (horizontal ? padding.top + padding.bottom : padding.left + padding.right);

            if (layoutInput)
            {
                if (horizontal)
                {
                    _layoutHeight = totalCross;
                    SetLayoutInputForAxis(totalCross, totalCross, -1, 1);
                }
                else
                {
                    _layoutWidth = totalCross;
                    SetLayoutInputForAxis(totalCross, totalCross, -1, 0);
                }
            }

            return totalCross;
        }

        void PlaceLine(List<Entry> line, float lineMain, float lineCross, float workingSize,
            float crossOffset, int axis, bool horizontal, float mainSpacing)
        {
            float mainStart = horizontal ? padding.left : padding.top;
            float slack = workingSize - lineMain;

            if (slack > 0f)
            {
                if (IsCenterAlign) mainStart += slack * 0.5f;
                else if (IsRightAlign) mainStart += slack;
            }

            float cursor = mainStart;
            for (int j = 0; j < line.Count; j++)
            {
                var e = line[j];
                if (j > 0)
                    cursor += mainSpacing;

                if (e.IsValid)
                {
                    float slotW = e.SlotW;
                    float slotH = e.SlotH;
                    float childW = e.Width;
                    float childH = e.Height;

                    // Handle height expansion (except KeepAspect and StretchRow modes)
                    if (m_ChildForceExpandHeight && horizontal
                        && m_SizingMode != SizingMode.UniformCellKeepAspect
                        && m_SizingMode != SizingMode.StretchRow)
                    {
                        slotH = lineCross;
                        childH = lineCross;
                    }

                    float x = cursor + (slotW - childW) * 0.5f;
                    float y = crossOffset + (lineCross - childH) * 0.5f;

                    // Left-align for StretchRow and ExpandWidth modes
                    if (m_SizingMode == SizingMode.StretchRow
                        || (m_ChildForceExpandWidth && horizontal && m_SizingMode == SizingMode.AutoSize))
                    {
                        x = cursor;
                        y = crossOffset;
                    }

                    if (axis == 0)
                        SetChildAlongAxis(e.Child, 0, x, childW);
                    else
                        SetChildAlongAxis(e.Child, 1, y, childH);

                    if (horizontal)
                        SetChildAlongAxis(e.Child, 1, y, childH);
                    else
                        SetChildAlongAxis(e.Child, 0, x, childW);

                    // Rebuild nested LayoutGroups to prevent layout corruption
                    if (e.Child.TryGetComponent<LayoutGroup>(out var nestedLayout))
                        LayoutRebuilder.ForceRebuildLayoutImmediate(e.Child);
                }
                cursor += (horizontal ? e.SlotW : e.SlotH);
            }
        }

        struct Entry
        {
            public RectTransform Child;
            public float SlotW;
            public float SlotH;
            public float Width;
            public float Height;
            public bool IsValid => Child != null;

            public static Entry Empty => new Entry { Child = null, SlotW = 0f, SlotH = 0f, Width = 0f, Height = 0f };
        }

        /// <summary>Resolves child dimensions based on SizingMode.</summary>
        Entry ResolveEntry(RectTransform child, float workingMain, bool horizontal)
        {
            float prefW = LayoutUtility.GetPreferredSize(child, 0);
            float prefH = LayoutUtility.GetPreferredSize(child, 1);

            // Fallback when preferred sizes are not properly declared
            if (prefW <= 0.01f) prefW = child.rect.width;
            if (prefH <= 0.01f) prefH = child.rect.height;
            if (prefW <= 0.01f) prefW = m_CellSize.x > 0.01f ? m_CellSize.x : 1f;
            if (prefH <= 0.01f) prefH = m_CellSize.y > 0.01f ? m_CellSize.y : 1f;

            switch (m_SizingMode)
            {
                case SizingMode.UniformCell:
                    return new Entry
                    {
                        Child = child,
                        SlotW = m_CellSize.x,
                        SlotH = m_CellSize.y,
                        Width = m_CellSize.x,
                        Height = m_CellSize.y
                    };

                case SizingMode.UniformCellKeepAspect:
                    {
                        float scale = Mathf.Min(m_CellSize.x / prefW, m_CellSize.y / prefH);
                        return new Entry
                        {
                            Child = child,
                            SlotW = m_CellSize.x,
                            SlotH = m_CellSize.y,
                            Width = prefW * scale,
                            Height = prefH * scale
                        };
                    }

                case SizingMode.StretchRow:
                    {
                        float w = workingMain > 0.01f ? workingMain : prefW;
                        float h = ResolveRowHeight(child, prefH);
                        return new Entry
                        {
                            Child = child,
                            SlotW = w,
                            SlotH = h,
                            Width = w,
                            Height = h
                        };
                    }

                case SizingMode.AutoSize:
                default:
                    {
                        float w = prefW;
                        float h = prefH;

                        if (horizontal && m_ChildForceExpandWidth && workingMain > 0.01f)
                            w = workingMain;
                        else if (horizontal && workingMain > 0.01f && w > workingMain)
                            w = workingMain;

                        if (!horizontal && m_ChildForceExpandWidth && workingMain > 0.01f)
                            h = workingMain;

                        return new Entry
                        {
                            Child = child,
                            SlotW = w,
                            SlotH = h,
                            Width = w,
                            Height = h
                        };
                    }
            }
        }

        float ResolveRowHeight(RectTransform child, float prefH)
        {
            float h = prefH;

            if (child.TryGetComponent<LayoutElement>(out var le))
            {
                if (le.preferredHeight > 0.01f)
                    h = le.preferredHeight;
                else if (le.minHeight > 0.01f)
                    h = le.minHeight;
            }

            // Clamp to reasonable height to prevent layout corruption
            float cap = m_CellSize.y > 0.01f ? m_CellSize.y * 3f : 200f;
            if (h > cap)
                h = m_CellSize.y > 0.01f ? m_CellSize.y : 56f;

            if (m_CellSize.y > 0.01f)
                h = Mathf.Max(h, m_CellSize.y);

            return h;
        }

        bool IsCenterAlign =>
            childAlignment == TextAnchor.UpperCenter
            || childAlignment == TextAnchor.MiddleCenter
            || childAlignment == TextAnchor.LowerCenter;

        bool IsRightAlign =>
            childAlignment == TextAnchor.UpperRight
            || childAlignment == TextAnchor.MiddleRight
            || childAlignment == TextAnchor.LowerRight;

        float GetGreatestMinimumChildWidth()
        {
            float max = 0f;
            for (int i = 0; i < rectChildren.Count; i++)
                max = Mathf.Max(max, LayoutUtility.GetMinWidth(rectChildren[i]));
            return max;
        }

        float GetGreatestMinimumChildHeight()
        {
            float max = 0f;
            for (int i = 0; i < rectChildren.Count; i++)
                max = Mathf.Max(max, LayoutUtility.GetMinHeight(rectChildren[i]));
            return max;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }
    }
}
