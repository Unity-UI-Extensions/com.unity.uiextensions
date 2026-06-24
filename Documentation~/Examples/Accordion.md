# Example: Accordion

## Overview

This scene demonstrates the Accordion control: a vertical stack of expandable/collapsible sections where opening one section animates its content into view. Use it when you need a compact panel of grouped content (settings categories, FAQs, inspector-style sections) that the user can reveal one piece at a time.

## Controls Featured

- [Accordion](/ugui/controls/accordion/) — the scene wires an Accordion Group around several Accordion Elements, each with a header and animated content body.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/Accordion/` into your project's `Assets/` folder.
2. Open the example scene in that folder (`Accordion.unity`).
3. Press **Play**.

## What to Expect

At runtime you see a list of Accordion Elements, each showing a header. Clicking a header expands that element to reveal its content, animating open with the group's configured transition (instant or a tween over the transition duration). By default opening one element collapses the previously open one; with **Allow Multiple** enabled, several can stay open at once, and **Toggle On** lets you click an open header to close it again. The **On Value Changed** event fires whenever the active element changes.
