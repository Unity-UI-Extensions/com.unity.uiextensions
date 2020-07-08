/// Credit setchi (https://github.com/setchi)
/// Sourced from - https://github.com/setchi/FancyScrollView

using System;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// <see cref="FancyCellGroup{TItemData, TContext}"/> のコンテキストインターフェース.
    /// </summary>
    public interface IFancyCellGroupContext
    {
        GameObject CellTemplate { get; set; }
        Func<int> GetGroupCount { get; set; }
    }
}