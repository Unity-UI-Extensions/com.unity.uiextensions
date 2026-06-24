# Example: Input Field Enter Submit

## Overview

This scene demonstrates the Input Field Enter Submit utility, which fires a submit event from a standard Unity InputField when the Enter key (or numeric keypad Enter) is pressed. Use it for search boxes, chat input, command prompts, or any form-like field where Enter should commit the text.

## Controls Featured

- [Input Field Enter Submit](/ugui/controls/input-field-enter-submit/) — the scene attaches the component to an InputField so pressing Enter raises its `EnterSubmit` event with the current text.

## Scene Setup

1. Add the **UI Extensions Samples** sample to your project (open the package in the Unity Package Manager and import **UI Extensions Samples**), or copy `Examples~/InputFieldEnterSubmit/` into your project's `Assets/` folder.
2. Open the `InputFieldEnterSubmitDemo` example scene in that folder.
3. Press **Play**.

## What to Expect

The scene presents an InputField with the Input Field Enter Submit component attached. As you type and press Enter, the component invokes its `EnterSubmit` event, passing the current text. Depending on the **Defocus Input** setting, the field either keeps focus for further input or releases focus after submission.
