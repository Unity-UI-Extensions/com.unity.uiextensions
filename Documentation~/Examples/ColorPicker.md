# Example: Color Picker

## Overview

This scene demonstrates the Color Picker control built from the `Picker 2.0` prefab, combining a box slider, HSV/RGB sliders, hex input, alpha, an on-screen colour sampler, and saveable presets. Use it whenever your project needs runtime colour selection, for example in an editor tool, character customiser, or paint feature.

## Controls Featured

- [Color Picker](/ugui/controls/color-picker/) — the `PickerTest` scene drops the `Picker 2.0` prefab into a canvas, exposing the combined HSV/RGB sliders, hex field, colour sampler, and preset swatches.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/ColorPicker/` into your project's `Assets/` folder.
2. Open the example scene in that folder (`PickerTest.unity`).
3. Press **Play**.

## What to Expect

At runtime you can drag the box slider and the HSV/RGB/alpha sliders, or type a hex code, to change the current colour; the picker raises its **On Value Changed** event each time the colour changes. The colour sampler lets you click anywhere on screen to pick that pixel's colour into the picker. Presets let you save the current colour as a swatch and reload it later; depending on the presets **Save Mode** (None, JSON, or PlayerPrefs) user-defined swatches persist between sessions, alongside any predefined preset colours. The picker and its presets/sampler are configured entirely in the prefab, so no scripting is required to try it.
