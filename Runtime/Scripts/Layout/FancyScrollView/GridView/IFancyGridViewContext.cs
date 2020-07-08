/// Credit setchi (https://github.com/setchi)
/// Sourced from - https://github.com/setchi/FancyScrollView

using System;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// <see cref="FancyGridView{TItemData, TContext}"/> のコンテキストインターフェース.
    /// </summary>
    public interface IFancyGridViewContext : IFancyScrollRectContext, IFancyCellGroupContext
    {
        Func<float> GetStartAxisSpacing { get; set; }
        Func<float> GetCellSize { get; set ; }
    }
}