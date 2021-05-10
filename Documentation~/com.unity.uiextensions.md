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

### 2019.6 - 2.5  - Bug squash

#### Added

Nothing new this time, bugfix release.

#### Changed

- Updated UI Line connector to use relative position instead of anchored position to verify if the Lines need updating.
- Allow menu prefabs to not have to have canvas components. This allows you to use any type of prefab as a "menu". Adam Kapos mentions the concept on the Unite talk, https://youtu.be/wbmjturGbAQ?t=1654
- Updated segment line drawing for Line Lists. Seems Unity no longer needs UV's to be wrapped manually.
- Updated the AutoCompleteComboBox to display text as entered (instead of all lowercase)
- Updated the ComboBox to display text as entered (instead of all lowercase)
- Updated ComboBox Examples to include programmatic versions
- Further ComboBox improvements including:
  * Upwards panel
  * Start fixes
  * Item Template resize
  * Disabled sorting on combobox as it wasn't working
  * Disabled Slider handle when not in use
  * Updated Example
- Updated the new Input system switch and tested against 2021

#### Deprecated

None

#### Fixed

- Reordering issue resolved with ScrollRectOcclusion.
- Fixed Sorting at min and max positions for ScrollRect
- Updated ScrollToSelect script provided by zero3growlithe, tested and vastly reduces the previous jitter. Still present but barely noticeable now.
- Fixed Issue # 363 Update Combobox control that takes multiple items programmatically, to only allow distinct items
- Fixed the issues where dragging outside the range slider handle causes the range to update. - Resolves #369
- Resolves an issue with Unity putting the previous controls vertex array in an uninitialised control.
- Applied J.R. Mitchell's fix for the Accordion Controls/Accordion/AccordionElement.cs - resolves: #364
- Resolved issue where the Content Scroll snap issue with only 1 child. Resolves #362
- Updated the PaginationManager to override if the ScrollSnap is in motion.

#### Removed

None

# Document revision history

|Date|Details|
|-|-|
|July 9th, 2020|2019.4 (v2.2) released, first UPM deployment live |
|September 3rd, 2019|2019.1 (v2.1) released, First major update for the 2.0 series.|
|August 8th, 2020|2019.4 (v2.2) released, New UPM Delivery.|
|October 10th, 2020|2019.5 (v2.2) released, New UPM fast delivery|
