# Example: Card UI

## Overview

This example ships a collection of scenes showing the different Card UI presentation styles in one place: 2D expanding cards, a 2D popup with falling cards, a 2D card stack, 3D expanding cards, and a superellipse playground. Use it when you want a card-based interface for content presentation, hands of cards, or stacked/expandable panels.

## Controls Featured

- [Card UI](/ugui/controls/card-ui/) — each scene exercises a different card behaviour (expand, popup/fall, stack depth, 3D rounded panels) from the Card UI suite.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/CardUI/` into your project's `Assets/` folder.
2. Open one of the example scenes in that folder: `2D Card Expanding.unity`, `2D Card Popup.unity`, `2D Card Stack.unity`, `3D Card Expanding.unity`, or `Superellipse Playground.unity`.
3. Press **Play**.

## What to Expect

What you see depends on the scene you open:

- **2D Card Expanding** — clickable cards that expand from a compact layout to a larger view and collapse again.
- **2D Card Popup** — physics-driven cards that pop up and fall into place.
- **2D Card Stack** — cards arranged in a depth-ordered stack, offset so each sits over the one behind it.
- **3D Card Expanding** — custom panel meshes with rounded (superellipse) corners for a 3D card effect.
- **Superellipse Playground** — a sandbox for tuning the rounded-corner superellipse shapes used by the 3D cards.

Expand/popup/stack animations run over the configured animation duration, and selecting or closing a card fires the relevant card event where exposed.
