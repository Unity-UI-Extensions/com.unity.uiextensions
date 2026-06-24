# Example: UI Circle Progress

## Overview

This scene demonstrates using the UI Circle primitive as an animated radial progress indicator. It shows driving the circle's progress value, recolouring both the base and progress portions of the arc, and changing the arc density (number of segments) at runtime. Use it for circular loading indicators, health/cooldown rings, or any progress display.

## Controls Featured

- [UI Circle](/ugui/controls/ui-circle/) — rendered as a hollow ring whose progress fill, colours, and segment count are updated live from UI sliders.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/UICircleProgress/` into your project's `Assets/` folder.
2. Open the `UICircleProgress` example scene in that folder.
3. Press **Play**.

## What to Expect

A circular progress ring is shown alongside controls that update it in real time. One slider drives the base (track) colour and another drives the progress colour, mapping a 0–1 value across the colour wheel. A density slider changes the number of arc segments and writes the current step count to an on-screen text field, so you can watch the ring go from coarse and faceted to smooth as the segment count rises.

## Key Code Patterns

Recolouring the base and progress portions of the circle:

```csharp
public void UpdateBaseColor(float value)
{
    baseColor = SetFixedColor(value, baseColor.a);
    TargetUICircle.GetComponent<UICircle>().color = baseColor;
}

public void UpdateProgressColor(float value)
{
    progressColor = SetFixedColor(value, progressColor.a);
    TargetUICircle.GetComponent<UICircle>().SetProgressColor(progressColor);
}
```

Changing the arc density (segment count) at runtime:

```csharp
public void UpdateDensity(float value)
{
    _uiCircleComponent.SetArcSteps((int)value);
    _densityOutput.text = value.ToString();
}
```
