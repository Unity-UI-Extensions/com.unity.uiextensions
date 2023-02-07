<!-- Offline documentation -->

# About Unity UI Extensions

The Unity UI Extensions project is a collection of extension scripts/effects and controls to enhance your Unity UI experience. This includes over 70+ controls, utilities, effects and some much-needed love to make the most out of the Unity UI system (formally uGUI) in Unity.
[Check out our Tumblr page for a sneak peek](https://www.tumblr.com/blog/unityuiextensions)

You can follow the UI Extensions team for updates and news on:

### [Twitter - #unityuiextensions](https://twitter.com/search?q=%23unityuiextensions) / [Facebook](https://www.facebook.com/UnityUIExtensions/) / [YouTube](https://www.youtube.com/@UnityUIExtensions)

> Ways to get in touch:
>
> - [Gitter Chat](https://gitter.im/Unity-UI-Extensions/Lobby) site for the UI Extensions project
> - [GitHub Discussions](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/discussions), if you have any questions, queries or suggestions

# Installing Unity UI Extensions

To install this package, follow the instructions in the Package Manager documentation.

For more details on [Getting Started](https://unity-ui-extensions.github.io/GettingStarted) please checkout the [online documentation here](https://unity-ui-extensions.github.io/).

# Using Unity UI Extensions

The UI Extensions project provides many automated functions to add the various controls contained within the project commonly accessed via "***GameObject -> UI -> Extensions -> 'Control'***" from the editor menu.  This will add the UI object and all the necessary components to make that control work in the scene in a default state.

Some of the features are also available through the GameObject "Add Component" menu in the inspector.

For a full list of the controls and how they are used, please see the [online documentation](https://unity-ui-extensions.github.io/Controls.html) for the project.

# Technical details

## Requirements

This version of the Unity UI Extensions is compatible with the following versions of the Unity Editor:

- 2019 and above - the recommended path for 2019+ is to use the Unity Package Manager to get access to the package.  Full details for installing via UPM can be [found here](https://unity-ui-extensions.github.io/UPMInstallation.html).

> Alternatively, the Asset packages have been tested to work with 2019 as well if you prefer to install that way.

- 2018 and below - for 2018 and use this package, you will have to import the asset package(s), either from the Asset Store or from the alternate download locations [listed here](https://unity-ui-extensions.github.io/Downloads).

## [Release Notes](https://unity-ui-extensions.github.io/ReleaseNotes/RELEASENOTES)

## Release 2.3 - Reanimation - 2022/02/07

It has been a tough time for all since the last update, but things have been moving steadily along.  In the past few months there has been a concerted effort to revamp and update the project ready for Unity 2022, as well as migrating the source repository over to GitHub and refreshing all the things.
We hope the new release is better for everyone and we have paid close attention to the editor menus and places to find all the controls for this release.

To get up to speed with the Unity UI Extensions, check out the [Getting Started](https://unity-ui-extensions.github.io/GettingStarted.html) Page.

> Ways to get in touch:
>
> - [Gitter Chat](https://gitter.im/Unity-UI-Extensions/Lobby) site for the UI Extensions project
> - [GitHub Discussions](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/discussions), if you have any questions, queries or suggestions
>
> Much easier that posting a question / issue on YouTube, Twitter or Facebook :D

## Breaking changes

For customers upgrading from earlier versions of Unity to Unity 2020, please be aware of the Breaking change related to Text Based components.  You will need to manually replace any UI using the older ```Text``` component and replace them with ```TextMeshPro``` versions. This is unavoidable due to Unity deprecating the Text component.

> New users to 2022 are unaffected as all the Editor commands have been updated to use the newer TextMeshPro versions.

For more details, see the [deprecation notice](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/discussions/428) on GitHub.

## Added

- Added new FIFO based UI Line Render when dynamic line rendering is needed.
- Added ResetSelectableHighlight component.
- Added SetArc method to UICircle as requested.
- Added new UIHorizontalScroller based on UIVerticalScroller.
- Added OnHighlightChanged and OnPressChanged events for UI Button.
- Added error handling around setting Unity UI Components for Vertical/Horizontal ScrollSnaps.
- Added Editor Menu Option to create a Min/Max slider.
- Added the ability to set a specific item for combobox controls on start and not just the first.
- Added the ability to disable the combo boxes and make them read-only.

## Changed

- Refresh FancyScrollView with the latest fixes
- All Text based components updated to use TextMeshPro from Unity 2022 **Breaking Change**

- Clean-up and reset pivots on scene start.
- Merged in feature/improved-ui-highlightable (pull request UILineRenderer - issues with specifying point locations at runtime #123).
- Merged in fix/ragesliderfix (pull request HorizontalScrollSnap Mask Area doesn't work when content created dynamically #125).
- Merged in fix/infinitescrollcontentsize (pull request Gradient initialization should be in Awake() #126).
- Merged in feature/controlTouchUp (pull request UILineRenderer mesh not updating in Editor scene view #127).
- Upgraded RangeSlider to work in both Horizontal and Vertical setups.
- Merged in RangeSlider-upgrade. (pull request Newtonsoft.Json.dll conflict #131)
- Updated UIVertical scroller to be 2022 compliant.
- Updated Curly UI to wait until end of the frame to recalculate positions.
- Updated Depth Texture sampler in UI Particles Shaders.
- Updated Points to always be an array of 1 when set to nothing for the Line Renderer.
- Updated Cooldown button to work with Keyboard input.
- Removed unneeded size calculation which caused some issues with mixed content.
- Resolved an issue whereby the last row in a flow layout group would not size correctly.
- Updated all components using "LayoutGroup" to override their OnDisable.
- Updated validation in the new MinMaxSlider.
- Updated Editor create options to add the correct Event System Input manager.
- Updated initialisation logic to not cause an endless loop in the TabNavigationHelper.
- Updated "Action" use to "UnityAction" to avoid Unity issues for DropDowns.
- Updated UIVerticalScroller for standards.
- Updated ReorderableList/ReorderableListElement to prevent creating a fake object for non-transferable items.
- Updated panel drawing for ComboBox controls and added DropdownOffset.
- Updated build issue with ReorderableListElement.
- Updated NonDrawingGraphic to require a CanvasRender, else it causes an error on run.

## Deprecated

- Marked ScrollPositionController as Obsolete, users should use the new Scoller.
- BestFitOutline - Deprecated in Unity 2020 onwards. (still available for earlier versions)
- NicerOutline - Deprecated in Unity 2020 onwards. (still available for earlier versions)
- Marked TileSizeFitter as obsolete as Unity has made this unworkable.

## Fixed

- Resolved issues with DisplayAbove and using a 0 ItemsToDisplay for ComboBox controls.
- Resolved startup issue that prevented the control from being used (Unity changed the start order in some instances), this was causing null reference issues with comboboxes.
- Patch fix for UILineRenderer.
- Resolves issue where the lower range value would become stuck when moving.
- Updated Infinite scroll to work with content of different sizes.
- Updated Dropdown list to NOT resize text Rect on draw.
- Revised the Curly UI fix as it was preventing the graphic from being updated in the scene view.
- Cleanup and ensuring the UIParticleSystem is disposed in onDestroy correctly.
- Clean up range slider unused variables.

- [UI Extensions Issue log](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/issues)

## Upgrade Notes

We recommend using the UPM delivery method. If you are using the Unity asset, there should be no issues updating but if you have a problem, just deleted the old Unity-UI-Extensions folder and import the asset new.

# Document revision history

|Date|Details|
|-|-|
|July 9th, 2020|2019.4 (v2.2) released, first UPM deployment live |
|September 3rd, 2019|2019.1 (v2.1) released, First major update for the 2.0 series.|
|August 8th, 2020|2019.4 (v2.2) released, New UPM Delivery.|
|October 10th, 2020|2019.5 (v2.2) released, New UPM fast delivery|
|February 7th, 2022|v2.3 released, New Home, UPM fast delivery via OpenUPM|
