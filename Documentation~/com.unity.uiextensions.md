<!-- Offline documentation -->

# About Unity UI Extensions

The Unity UI Extensions project is a collection of extension scripts/effects and controls to enhance your Unity UI experience. This includes over 70+ controls, utilities, effects and some much-needed love to make the most out of the Unity UI system (formally uGUI) in Unity.
[Check out our Tumblr page for a sneak peek](https://www.tumblr.com/blog/unityuiextensions)

> Contact the UI Extensions Team
> Be sure to logon to the new [Gitter Chat site](https://gitter.im/Unity-UI-Extensions/Lobby) for the UI Extensions project, if you have any questions, queries or suggestions
> Much easier than posting a question / issue on [YouTube](http://www.youtube.com/c/UnityUIExtensions), [Twitter](https://twitter.com/hashtag/UnityUIExtensions) or [Facebook](https://www.facebook.com/UnityUIExtensions) :D
>
> [**UIExtensions Gitter Chanel**](https://gitter.im/Unity-UI-Extensions/Lobby)

# Installing Unity UI Extensions

To install this package, follow the instructions in the Package Manager documentation.

For more details on [Getting Started](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/GettingStarted) please checkout the [online documentation here](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/GettingStarted).

# Using Unity UI Extensions

The UI Extensions project provides many automated functions to add the various controls contained within the project commonly accessed via "***GameObject -> UI -> Extensions -> 'Control'***" from the editor menu.  This will add the UI object and all the necessary components to make that control work in the scene in a default state.

Some of the features are also available through the GameObject "Add Component" menu in the inspector.

For a full list of the controls and how they are used, please see the [online documentation](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Controls) for the project.

# Technical details

## Requirements

This version of the Unity UI Extensions is compatible with the following versions of the Unity Editor:

- 2019 and above - the recommended path for 2019+ is to use the Unity Package Manager to get access to the package.  Full details for installing via UPM can be [found here](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/UPMInstallation).

> Alternatively, the Asset packages have been tested to work with 2019 as well if you prefer to install that way.

- 2018 and below - for 2018 and use this package, you will have to import the asset package(s), either from the Asset Store or from the alternate download locations [listed here](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Downloads).

## [Release Notes](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/ReleaseNotes/RELEASENOTES)

### 2019.5 - 2.3  - Accelerated Deployment

#### Added

- Add squircle primitive
- Adding new magnetic scroll control
- Added a static library to collate shaders on first use.
- Finalized new InputManagerHelper, which translates input based on the operating input system, new or old Updated CardStack2D to have defined keyboard input or specific gamepad input over the older axisname for new input system.
- Updated DropDown and Autocomplete controls based on feedback in #204

#### Changed

- Examples now included with UPM delivery and available as a button on the UPM package manager window
- Updated DropDown and Autocomplete controls based on feedback in #204
- Updated Accordion to support both Vertical as well as Horizontal layout
- Updated ComboBox controls to improve better programmatic controls
- Updates to the Infinite scroll to support content of various sizes
- Updated UI Knob control - enabled dragging outside the target area, added example scene
- Minor update to MagneticInfinite Scroll
- Refactored and extended the ContentScrollSnap control
- Added protection against errors and empty scrollrect content
- Added new SetNewItems function to add children programmatically to the control and reset accordingly
- Patch supplied by a contributor to improve the texture sheet use with the UIParticlesystem
- Added "SetKnobValue" function which allows the setting of Value and loops
- Added the programmatic capability to change the parent scroll rect on the ScrollConflictManager at runtime.

#### Deprecated

None

#### Fixed

- Fix to add a "RequireComponent" to Primitives as Unity 2020 does not add them by default
- Remove old Examples submodule
- Updated submodules to hide Examples folder Additionally, updated Package manifest to allow importing of examples direct from UPM package.
- Fixed hard swipe to ensure it only ever moves one page, no matter how far you swipe.
- Fixed a conflict when using the ScrollConflictManager in child content of a HSS or VSS
- Fix for UI Particle system looping
- Fixed public GoToScreen call to only raise events internally (not multiple)
- Final roll-up and fix. Resolved race condition for associated pagination controls.
- Fixed issue with page events not being raised when inertia was disabled (velocity was always zero)
- When cloned, reorderable list was creating a second List Content component that was not initialized. Refactored to ensure only one list content was present and is initialized correctly
- Reorderable list items marked as transferable, remain transferable after being dropped
- Patch to resolve issues without the new Input System installed
- Refined magnetic scroll and dependencies while documenting Updated example
- Patch Tooltip

#### Removed

None

# Document revision history

|Date|Details|
|-|-|
|July 9th, 2020|2019.4 (v2.2) released, first UPM deployment live |
|September 3rd, 2019|2019.1 (v2.1) released, First major update for the 2.0 series.|
|August 8th, 2020|2019.4 (v2.2) released, New UPM Delivery.|
|October 10th, 2020|2019.5 (v2.2) released, New UPM fast delivery|
