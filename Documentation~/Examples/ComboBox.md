# Example: ComboBox & Dropdowns

## Overview

This scene places the three Unity UI Extensions selection controls side by side so you can compare them: the plain ComboBox, the AutoComplete ComboBox, and the DropDown List. It ships demo scripts that populate the controls, refresh their options at runtime, add items from an input field, and log every change event. Use it to decide which selection control best fits your UI and to see the correct way to manage their contents from code.

## Controls Featured

- [ComboBox](/ugui/controls/combobox/) — a fixed text combobox; the demo adds items to it and logs its selection-changed event.
- [AutoComplete ComboBox](/ugui/controls/autocomplete-combobox/) — a text field with live filtering against a vocabulary; the demo logs its text, validity, selection, and item-selected events.
- [DropDown List](/ugui/controls/dropdown-list/) — a richer dropdown supporting text and images; the demo refreshes its items and logs its changed event.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/ComboBox/` into your project's `Assets/` folder.
2. Open the example scene in that folder (`ComboBox.unity`).
3. Press **Play**.

## What to Expect

At runtime each of the three controls is pre-populated with a shared set of options (`More`, `ComboBox`, `Example`, `Goodness`). Open the ComboBox or DropDown List to pick from the list, or type into the AutoComplete ComboBox to filter matches as you go. An input field plus buttons let you add new entries into each control live. Every interaction is written to the Console: the ComboBox and DropDown log their selection changes, while the AutoComplete logs separate events for text changes, validity changes, selection changes, and item selection. This makes it a useful reference for wiring up the controls' events and for the recommended `SetAvailableOptions` / `RefreshItems` / `AddItem` APIs rather than editing the options list directly.

## Key Code Patterns

Populate (and reset) each control's options at startup — note `RefreshItems` for the DropDown List versus `SetAvailableOptions` for the others:

```csharp
private string[] dropDownItems = { "More", "ComboBox", "Example", "Goodness" };

void Start()
{
    acb.SetAvailableOptions(dropDownItems);   // AutoCompleteComboBox
    cb.SetAvailableOptions(dropDownItems);    // ComboBox
    ddl.RefreshItems(dropDownItems);          // DropDownList
}
```

Add a single item from an input field at runtime:

```csharp
public void AddComboBox()
{
    if (!string.IsNullOrWhiteSpace(input.text))
    {
        comboBox.AddItem(input.text);
    }
}
```
