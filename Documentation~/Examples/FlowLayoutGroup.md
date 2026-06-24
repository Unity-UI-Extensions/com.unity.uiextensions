# Example: Flow Layout Group

## Overview

This scene demonstrates the Flow Layout Group, a layout component that arranges its child elements in rows (or columns) and automatically wraps them onto the next line when they run out of space. Use it when you need a flexible, wrapping layout for a variable number of items, similar to a CSS flexbox.

## Controls Featured

- [Flow Layout Group](/ugui/controls/flow-layout-group/) — the scene's container uses the component to auto-arrange and wrap its child UI elements.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/FlowLayoutGroup/` into your project's `Assets/` folder.
2. Open the `FlowLayoutGroup` example scene in that folder.
3. Press **Play**.

## What to Expect

The scene shows a parent container with the Flow Layout Group attached and a collection of child elements inside it. The children are laid out left to right and wrap onto a new row once the available width is exhausted, so the arrangement reflows to fit the container rather than overflowing or clipping.
