/// Credit setchi (https://github.com/setchi)
/// Sourced from - https://github.com/setchi/FancyScrollView

using System;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// <see cref="FancyScrollRect{TItemData, TContext}"/> のコンテキストインターフェース.
    /// </summary>
    public interface IFancyScrollRectContext
    {
        Func<(float ScrollSize, float ReuseMargin)> CalculateScrollSize { get; set; }
    }
}