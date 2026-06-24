# Example: Menu System

## Overview

This scene is a full working implementation of the Menu System, a generic, reusable menu framework (refactored from the Unite 2017 demo). It shows a Menu Manager driving navigation between several menu screens. Use it as a template for building stacked, hierarchical menus such as main menu, options, in-game, and pause screens.

## Controls Featured

- [Menu System](/ugui/controls/menu-system/) — the scene's Menu Manager loads and transitions between menu prefabs, each backed by a `Menu` or `SimpleMenu` script.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/MenuExample/` into your project's `Assets/` folder.
2. Open the `MainScene` example scene in that folder.
3. Press **Play**.

## What to Expect

The Menu Manager opens the start screen (the main menu) at launch. From there you can navigate into the options and game menus, open an "awesome" menu, and trigger a pause menu, with the manager handling the flow and stacking between screens. Back actions move up the menu hierarchy, and quitting from the main menu calls `Application.Quit`. Each menu is a separate prefab with a matching script derived from the shared menu base classes.

## Key Code Patterns

Menu scripts derive from the generic `SimpleMenu<T>` (or `Menu<T>`) base and show or hide other menus via their static `Show` methods:

```csharp
public class MainMenu : SimpleMenu<MainMenu>
{
    public void OnPlayPressed()
    {
        GameMenu.Show();
    }

    public void OnOptionsPressed()
    {
        OptionsMenu.Show();
    }

    public override void OnBackPressed()
    {
        Application.Quit();
    }
}
```
