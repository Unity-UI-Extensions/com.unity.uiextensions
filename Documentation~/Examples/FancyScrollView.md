# Example: Fancy Scroll View

## Overview

This sample is a full tour of the Fancy Scroll View control, shipping nine separate scenes that build from a simple animated list up to advanced layouts and effects. Use it when you need a high-performance, virtualised scroll view with custom cell animation, infinite/looping content, grid layouts, or runtime texture loading.

## Controls Featured

- [Fancy Scroll View](/ugui/controls/fancy-scroll-view/) — each scene wires a `FancyScrollView`-derived `ScrollView` to a scroll position controller and feeds it cell data programmatically.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/FancyScrollView/` into your project's `Assets/` folder.
2. Open the example scene(s) in that folder.
3. Press **Play**.

## What to Expect

The folder contains nine scenes that you can switch between using the in-scene scenes dropdown:

- **01_Basic** — a simple animated list populated with twenty cells at start-up.
- **02_FocusOn** — selectable cells that animate to focus when chosen.
- **03_InfiniteScroll** — endlessly looping content.
- **04_Metaball** and **05_Voronoi** — list scrolling combined with custom background visual effects.
- **06_LoopTabBar** — a looping tab bar with screen transitions.
- **07_ScrollRect** — Fancy Scroll View driven by a standard `ScrollRect` with configurable alignment.
- **08_GridView** — a multi-column grid layout.
- **09_LoadTexture** — cells that load their textures asynchronously at runtime.

At runtime each scene populates its scroll view with generated `ItemData`, and cells animate as they move through the viewport.

## Key Code Patterns

The basic example generates a set of `ItemData` and hands it to the scroll view:

```csharp
void Start()
{
    var items = Enumerable.Range(0, 20)
        .Select(i => new ItemData($"Cell {i}"))
        .ToArray();

    scrollView.UpdateData(items);
}
```
