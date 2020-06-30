/// Credit setchi (https://github.com/setchi)
/// Sourced from - https://github.com/setchi/FancyScrollView

using System;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// <see cref="FancyGridView{TItemData, TContext}"/> のコンテキストインターフェース.
    /// </summary>
    public interface IFancyGridViewContext
    {
        GameObject CellTemplate { get; set; }
        ScrollDirection ScrollDirection { get; set; }
        Func<int> GetColumnCount { get; set; }
        Func<float> GetColumnSpacing { get; set; }
    }
}