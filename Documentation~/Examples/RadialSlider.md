# Example: Radial Slider

## Overview

This scene demonstrates the Radial Slider, a circular arc slider that fills in a radial pattern from a start angle. It shows reading the slider value at runtime, driving the slider value/angle programmatically from an input field, and reporting pointer interaction. Use it when you need a dial- or gauge-style input instead of a straight linear slider.

## Controls Featured

- [Radial Slider](/ugui/controls/radial-slider/) — the central dial that the user drags or clicks to set a value, with optional Lerp-to-target behaviour.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/RadialSlider/` into your project's `Assets/` folder.
2. Open the `radial_slider` example scene in that folder.
3. Press **Play**.

## What to Expect

A radial slider is shown on the canvas. Dragging or clicking around the arc changes the fill and raises the slider's value-changed events (an integer value and a text value). Pointer enter, down, and up events are written to an on-screen text field so you can see the raw interaction state. An input field lets you type a number and push it back into the slider as either a value or an angle, demonstrating two-way control of the slider from script.

## Key Code Patterns

Setting the slider value (or angle) from external input:

```csharp
public class UpdateRadialValue : MonoBehaviour
{
    public InputField input;
    public RadialSlider slider;

    public void UpdateSliderValue()
    {
        float value;
        float.TryParse(input.text, out value);
        slider.Value = value;
    }

    public void UpdateSliderAndle()
    {
        int value;
        int.TryParse(input.text, out value);
        slider.Angle = value;
    }
}
```
