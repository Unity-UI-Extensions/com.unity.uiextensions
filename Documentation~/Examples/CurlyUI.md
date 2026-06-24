# Example: CurlyUI

## Overview

This scene demonstrates CurlyUI, an effect that warps standard uGUI graphics (Image, Text, RawImage) along configurable bezier curves so your UI can bend and arc instead of sitting flat. Use it for stylised banners, curved menus, or any UI element that needs to follow a non-rectangular shape.

## Controls Featured

- [CurlyUI](/ugui/controls/curly-ui/) — the demo scene applies the CUI effect components to UI graphics, exposing the bezier handles that define how each element is curved.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/CurlyUI/` into your project's `Assets/` folder.
2. Open the example scene in that folder (`CurlyUIDemo.unity`).
3. Press **Play**.

## What to Expect

The scene shows UI graphics that have been bent along curves rather than drawn flat, driven by the CurlyUI effect's bezier control points. With **Is Curved** enabled the curve is applied to the attached element, and the **Resolution** setting controls how finely the underlying mesh is subdivided (higher resolution gives a smoother curve at the cost of more geometry). The curve shape is authored in the editor by dragging the bezier handles on each curved element, with helper buttons to fit the curve back to the RectTransform; the effect itself is visible both in the Scene/Game view at edit time and at runtime.
