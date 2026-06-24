# Example: Grid Raw Image

## Overview

This scene demonstrates the Grid Raw Image control, a custom uGUI graphic that draws a grid of texture cells from a single texture, GameObject, and draw call. Use it for tile-based UI such as Tetris-style inventories or grid backgrounds where you want efficient rendering and per-cell geometry raycasting.

## Controls Featured

- [Grid Raw Image](/ugui/controls/grid-raw-image/) — the scene renders a configurable grid of textured cells using one Grid Raw Image component.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/GridRawImage/` into your project's `Assets/` folder.
2. Open the `Demo GridImage` example scene in that folder.
3. Press **Play**.

## What to Expect

The scene displays a Grid Raw Image laid out as a grid of cells drawn from a single texture. Properties such as the cell size, resize factor, extrude amount, and shape (which cells are active) control how the grid is built, letting you preview non-rectangular grids and skipped cells while still rendering in a single draw call.
