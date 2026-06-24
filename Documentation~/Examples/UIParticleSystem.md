# Example: UI Particle System

## Overview

This scene demonstrates rendering a native Unity particle system inside the uGUI canvas with the UI Particle System control. A small runtime control window lets you play, pause and stop the emitter and watch its live state. Use it when you want particle effects that live in your UI layer rather than in 3D world space.

## Controls Featured

- [UI Particle System](/ugui/controls/ui-particle-system/) — renders the attached Unity particle system in UI space so the effect draws correctly within the canvas.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/UIParticleSystem/` into your project's `Assets/` folder.
2. Open the `UIParticleSystem` scene in that folder.
3. Press **Play**.

## What to Expect

The particle effect renders within the UI canvas. A draggable on-screen control window (drawn with IMGUI by the `ParticleSystemControllerWindow` script) shows read-only toggles for the system's Playing, Emitting and Paused states, plus buttons to **Play**, **Pause**, **Stop Emitting**, and **Stop & Clear**. An "Include Children" toggle controls whether those calls cascade to child systems, and labels report the current time and live particle count.

## Key Code Patterns

```csharp
void DrawWindowContents(int windowId)
{
    if (system)
    {
        if (GUILayout.Button("Play"))
            system.Play(includeChildren);
        if (GUILayout.Button("Pause"))
            system.Pause(includeChildren);
        if (GUILayout.Button("Stop Emitting"))
            system.Stop(includeChildren, ParticleSystemStopBehavior.StopEmitting);
        if (GUILayout.Button("Stop & Clear"))
            system.Stop(includeChildren, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
    GUI.DragWindow();
}
```
