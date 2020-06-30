/// Credit setchi (https://github.com/setchi)
/// Sourced from - https://github.com/setchi/FancyScrollView

using System;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// <see cref="FancyGridView{TItemData, TContext}"/> のコンテキスト基底クラス.
    /// </summary>
    public class FancyGridViewContext : IFancyGridViewContext, IFancyScrollRectContext
    {
        Func<(float ScrollSize, float ReuseMargin)> IFancyScrollRectContext.CalculateScrollSize { get; set; }

        GameObject IFancyGridViewContext.CellTemplate { get; set; }

        ScrollDirection IFancyGridViewContext.ScrollDirection { get; set; }

        public Func<int> GetColumnCount { get; set; }

        public Func<float> GetColumnSpacing { get; set; }
    }
}