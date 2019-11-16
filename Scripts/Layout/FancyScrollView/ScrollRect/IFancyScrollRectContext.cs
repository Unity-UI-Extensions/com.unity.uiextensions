/// Credit setchi (https://github.com/setchi)
/// Sourced from - https://github.com/setchi/FancyScrollView

using System;

namespace UnityEngine.UI.Extensions
{

    public interface IFancyScrollRectContext
    {
        Func<(float ScrollSize, float ReuseMargin)> CalculateScrollSize { get; set; }
    }
}