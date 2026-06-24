# Example: Selection Box

## Overview

This scene demonstrates the Selection Box, an RTS-style rubber-band selection control that lets the user drag out a rectangle to select multiple objects at once. It shows selecting both screen-space UI content and world-space objects within a restricted selectable area. Use it when you need marquee/multi-select behaviour, such as selecting units in a strategy game.

## Controls Featured

- [Selection Box](/ugui/controls/selection-box/) — drag to draw a selection rectangle; objects inside the selection mask area are selected.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/SelectionBox/` into your project's `Assets/` folder.
2. Open the `Selection Box Test` example scene in that folder.
3. Press **Play**.

## What to Expect

Dragging the pointer across the canvas draws a selection box using the configured colour and background art. The scene includes both a screen-space selection canvas and world-space content (such as a cube and cylinder), demonstrating that 2D and 3D items can be selected. Selection is constrained to the assigned Selection Mask rect transform, so only objects within that area respond to the marquee.
