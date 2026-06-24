# Example: Cooldown Button

## Overview

This scene demonstrates the Cooldown Button control, which makes a Selectable (Button, Toggle, etc.) unusable for a set time after it is clicked. It also shows how to drive visual feedback from the button's cooldown progress by pairing it with a Unity Image fill and with the SoftMask effect. Use it for ability/power-up buttons, rate-limited actions, or any UI where an action needs a recovery delay.

## Controls Featured

- [Cooldown Button](/ugui/controls/cooldown-button/) — the scene attaches a CooldownButton and reads its timing properties (`CooldownTimeRemaining`, `CooldownTimeElapsed`, `CooldownTimeout`, `CooldownPercentComplete`) each frame to animate the cooldown effect.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/Cooldown/` into your project's `Assets/` folder.
2. Open the example scene in that folder (`CooldownExamples.unity`).
3. Press **Play**.

## What to Expect

Clicking a cooldown button starts its timer and triggers the **On Cooldown Start** event. While the timer runs, the linked visuals update every frame: one example sweeps a Unity Image's radial fill from empty to full, optionally showing the cooldown percentage as text, while another drives the `CutOff` of a SoftMask to reveal/wipe the graphic over the cooldown. Clicking the button again before it finishes fires **On Button Click During Cooldown** so you can give "still cooling down" feedback, and **On Cooldown Finish** fires once the timeout elapses and the button is ready again.

## Key Code Patterns

Driving a Unity Image's radial fill from the button's remaining time:

```csharp
public CooldownButton coolDown;
private Image target;

void Update()
{
    target.fillAmount = Mathf.Lerp(0, 1, coolDown.CooldownTimeRemaining / coolDown.CooldownTimeout);
    if (displayText)
    {
        displayText.text = string.Format("{0}%", coolDown.CooldownPercentComplete);
    }
}
```

Driving the SoftMask cutoff from elapsed time:

```csharp
void Update()
{
    sauim.CutOff = Mathf.Lerp(0, 1, cooldown.CooldownTimeElapsed / cooldown.CooldownTimeout);
}
```
