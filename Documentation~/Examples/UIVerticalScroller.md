# Example: UI Vertical Scroller

## Overview

This scene builds a date picker from three UI Vertical Scrollers, one each for days, months and years. Each scroller scales its elements by distance from a centre point so the focused entry is zoomed and selectable. Use it when you want a wheel-style picker where the centred item is the current selection.

## Controls Featured

- [UI Vertical Scroller](/ugui/controls/ui-vertical-scroller/) — three instances act as the day, month and year wheels; the centred element is read back as the selected value.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/UIVerticalScrollerDemo/` into your project's `Assets/` folder.
2. Open the `VerticalCalendar` scene in that folder.
3. Press **Play**.

## What to Expect

Three vertical wheels appear: days (1-31), months (Jan-Dec) and years (1900 to the current year), each populated from a button prefab at startup. Scrolling a wheel zooms whichever item sits at its centre, and scroll up/down buttons step through entries. The currently focused values are combined into a formatted date string (with the correct ordinal suffix, e.g. "Jan 1st 1900"). Input fields plus a "set date" action snap all three wheels to a chosen day/month/year via `SnapToElement`.

## Key Code Patterns

```csharp
// Read the centred element of each scroller every frame and build the date
string dayString = daysVerticalScroller.Result;
string monthString = monthsVerticalScroller.Result;
string yearsString = yearsVerticalScroller.Result;
dateText.text = monthString + " " + dayString + " " + yearsString;

// Jump all three wheels to a specific entry
daysVerticalScroller.SnapToElement(daysSet);
monthsVerticalScroller.SnapToElement(monthsSet);
yearsVerticalScroller.SnapToElement(yearsSet);
```
