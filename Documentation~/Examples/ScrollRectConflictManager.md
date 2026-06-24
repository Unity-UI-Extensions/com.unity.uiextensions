# Example: Scroll Rect Conflict Manager

## Overview

This scene demonstrates the Scroll Conflict Manager, which resolves the dragging conflict that occurs when one ScrollRect is nested inside another. Natively, a child ScrollRect only scrolls along its own axis and swallows the drag; this component lets the gesture flow to the correct ScrollRect based on the initial drag direction. Use it whenever you have a horizontal pager containing vertically scrolling pages (or vice versa).

## Controls Featured

- [Scroll Conflict Manager](/ugui/controls/scroll-conflict-manager/) — added to the child ScrollRect and pointed at the parent ScrollRect to arbitrate which one handles each drag.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/ScrollRectConflictManager/` into your project's `Assets/` folder.
2. Open the `ScrollrectConflictManagerDemo` example scene in that folder.
3. Press **Play**.

## What to Expect

The scene shows a horizontally paged ScrollRect whose pages each contain their own vertically scrolling content. Dragging mostly sideways scrolls between the pages, while dragging mostly up and down scrolls the content inside the current page. The direction is locked to the initial drag, so a single gesture no longer gets stuck on the wrong ScrollRect the way nested ScrollRects behave by default.
