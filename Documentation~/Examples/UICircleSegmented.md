# Example: UI Circle Segmented

## Overview

This scene showcases the UI Circle Segmented graphic, a circle split into individually controllable slices. It demonstrates the various fill patterns, segment counts and gap/offset options you can use to build ring charts, pie charts and radial selection wheels. Use it when you need a segmented circular indicator rather than a single smooth arc.

## Controls Featured

- [UI Circle Segmented](/ugui/controls/ui-circle-segmented/) — the sample circle graphic that renders the configurable segments, fill amount and fill patterns.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/UICircleSegmented/` into your project's `Assets/` folder.
2. Open the `UICircleSegmented Samples` scene in that folder.
3. Press **Play**.

## What to Expect

The scene presents the segmented circle drawn from the included segment sprites. By adjusting the control's inspector values you can see how **Segments Count** changes the number of slices, how **Fill Amount** and **Invert Fill** drive progress, and how the **Fill Pattern** (Default, Handle, Handle Inverted) and **Fill Deep** (Floating, Per Vertex, Per Segment) options change the way the fill is applied. Border thickness, gradients and the global/segment degree offsets let you fine-tune the look. The graphic is GC-free and redraws as the properties change.
