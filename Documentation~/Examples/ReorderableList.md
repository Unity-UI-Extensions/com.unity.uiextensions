# Example: Reorderable List

## Overview

This scene demonstrates the Reorderable List, a set of list and grid containers whose child items can be dragged and dropped to reorder them or move them between containers. It shows the different layout flavours (vertical, horizontal and grid) and the per-container options for grabbing, transferring, cloning and dropping items. Use it when you need drag-and-drop inventories, hotbars, or rankable lists.

## Controls Featured

- [Reorderable List](/ugui/controls/reorderable-list/) — multiple list/grid containers that let the user drag items within and between them, with visual reordering feedback.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/ReorderableList/` into your project's `Assets/` folder.
2. Open the `ReorderableList` example scene in that folder.
3. Press **Play**.

## What to Expect

Several reorderable containers are laid out on the canvas. You can grab a child item and drag it to a new position within its list, and the surrounding items shift to make room. Where containers are configured as transferable, items can be dragged from one container and dropped into another; where cloning is enabled, dragging leaves the original in place and drops a copy. Containers set as draggable-only or droppable-only restrict the direction items can move, and the element grabbed/dropped/added/removed events fire as the items are interacted with.
