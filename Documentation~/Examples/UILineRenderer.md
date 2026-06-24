# Example: UI Line Renderer

## Overview

This folder collects several scenes that demonstrate drawing lines on a Canvas with the UI Line Renderer family of controls. They cover free-hand line drawing from pointer input, adding/clearing points at runtime, and animating an orbit path, using both the array-based `UILineRenderer` and the list-based `UILineRendererList`. Use these when you need to draw polylines, charts or dynamic paths in UI space.

## Controls Featured

- [UI Line Renderer](/ugui/controls/ui-line-renderer/) — the `LineDrawing` and `UILineRendererDemo` scenes draw and animate lines by setting its `Points` array.
- [UI Line Renderer (List)](/ugui/controls/ui-line-renderer-list/) — the `UILineRendererListDemo` scene builds the same effects using its `AddPoint`/`ClearPoints` list API.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/UILineRenderer/` into your project's `Assets/` folder.
2. Open one of the example scenes in that folder: `LineDrawing`, `UILineRendererDemo`, or `UILineRendererListDemo`.
3. Press **Play**.

## What to Expect

In `LineDrawing` you draw lines directly with the mouse. The `DrawLine` script supports two modes: **Drag and Draw**, where dragging rubber-bands a line between the start and release points, and **Click and Draw**, where each click extends a continuous line that follows the cursor until you right-click or press **Escape**. The demo scenes also show an animated orbit where a `UILineRenderer`/`UILineRendererList` traces a circle (driven by the `LineRendererOrbit`/`LineRendererOrbitList` scripts) and a small UI object travels around that path. Input fields with a button let you add new points by X/Y value or clear the line entirely. Remember the line renderer's RectTransform should sit at (0,0,0) for correct results.

## Key Code Patterns

```csharp
// UILineRenderer: replace the whole Points array
public void AddNewPoint()
{
    var point = new Vector2() { x = float.Parse(XValue.text), y = float.Parse(YValue.text) };
    var pointlist = new List<Vector2>(LineRenderer.Points);
    pointlist.Add(point);
    LineRenderer.Points = pointlist.ToArray();
}

// UILineRendererList: append/clear via the list API
public void AddNewPoint()
{
    var point = new Vector2() { x = float.Parse(XValue.text), y = float.Parse(YValue.text) };
    LineRenderer.AddPoint(point);
}
```
