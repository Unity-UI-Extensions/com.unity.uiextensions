# Example: Scroll Snaps (Horizontal / Vertical / Infinite)

## Overview

This folder collects several scroll-snap demonstrations across multiple scenes, covering paged horizontal and vertical snapping, content-item snapping, infinite/looping snaps, pagination, and runtime page management. Use it when you need swipeable paged UI, carousels, or snapping lists with button- and pagination-driven navigation.

## Controls Featured

- [Horizontal Scroll Snap](/ugui/controls/horizontal-scroll-snap/) — paged horizontal snapping driven by swipes, buttons, and page jumps, including dynamic add/remove of pages.
- [Vertical Scroll Snap](/ugui/controls/vertical-scroll-snap/) — the vertical equivalent, with pages added, removed, and jumped to at runtime.
- [Content Scroll Snap Horizontal](/ugui/controls/content-scroll-snap-horizontal/) — snaps content items to the centre of the scroll rect rather than paging whole screens.
- [Infinite Scroll Snap](/ugui/controls/infinite-scroll-snap/) — endlessly looping scroll-snap content (deprecated for Unity 6, shown for legacy reference).

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/HSS-VSS-ScrollSnap/` into your project's `Assets/` folder.
2. Open one of the example scenes in that folder, for example `ContentSnapScrollExample`, `InfiniteScrollSnapExamples`, `FullScreenScrollSnap`, `PaginationManagerExample`, `ScrollSnapDynamic`, `InfiniteMagneticExample`, or `ScrollSnapManagedTests`.
3. Press **Play**.

## What to Expect

Each scene shows a different scroll-snap variant. Paged scenes let you swipe or use buttons to move between full-screen pages, and a pagination row of dots reflects and controls the current page. The content-snap scene snaps individual child items to the centre rather than moving whole screens. The dynamic/managed scenes demonstrate adding pages, removing the current or all pages, and jumping to a page entered into an input field at runtime; one scene also adds ten pages to a freshly instantiated scroll snap when you press the **K** key. Page-change, start, and end events are logged to the console as you navigate.

## Key Code Patterns

A pagination button routes a click straight to the scroll snap's `GoToScreen` to jump to a specific page:

```csharp
public class PaginationScript : MonoBehaviour, IPointerClickHandler
{
    public HorizontalScrollSnap hss;
    public int Page;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (hss != null)
        {
            hss.GoToScreen(Page);
        }
    }
}
```

Pages can also be added, removed, or jumped to at runtime against either a horizontal or vertical snap:

```csharp
public void AddButton()
{
    if (HSS)
    {
        var newHSSPage = GameObject.Instantiate(HorizontalPagePrefab);
        HSS.AddChild(newHSSPage);
    }
    // ...
}

public void JumpToPage()
{
    int jumpPage = int.Parse(JumpPage.text);
    if (HSS) { HSS.GoToScreen(jumpPage); }
    if (VSS) { VSS.GoToScreen(jumpPage); }
}
```
