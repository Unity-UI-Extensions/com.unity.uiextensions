# Example: UI Knob

## Overview

This scene demonstrates the UI Knob, a rotary dial control that behaves like a knob on a stereo or amplifier. It shows reading the knob's live value, displaying it on screen, and driving the knob to a specific value from script. Use it when you need a rotational input rather than a linear slider.

## Controls Featured

- [UI Knob](/ugui/controls/ui-knob/) — the rotary knob the user drags to set a value, with its value read back and set programmatically.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/UIKnobExample/` into your project's `Assets/` folder.
2. Open the `UIKnobExample` scene in that folder.
3. Press **Play**.

## What to Expect

Drag the knob with the mouse (or touch) to rotate it. A text label continuously updates to show the knob's current `KnobValue`. An input field plus button let you type a target value and apply it; pressing the button rotates the knob to that value via `SetKnobValue`. The knob's range, direction, loops and snapping are configured on the `UI_Knob` component in the inspector.

## Key Code Patterns

```csharp
public class KnobManagement : MonoBehaviour
{
    public Text KnobValue;
    public InputField SetKnobValue;
    public UI_Knob Knob;

    public void UpdateKnobValue()
    {
        Knob.SetKnobValue(float.Parse(SetKnobValue.text));
    }

    void Update()
    {
        KnobValue.text = Knob.KnobValue.ToString();
    }
}
```
