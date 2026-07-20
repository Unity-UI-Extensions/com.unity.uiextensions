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

## Installing Unity UI Extensions

To install this package, follow the instructions in the Package Manager documentation.

For more details on [Getting Started](https://unity-ui-extensions.github.io/GettingStarted) please checkout the [online documentation here](https://unity-ui-extensions.github.io/).

## Using Unity UI Extensions

The UI Extensions project provides many automated functions to add the various controls contained within the project commonly accessed via "***GameObject -> UI -> Extensions -> 'Control'***" from the editor menu.  This will add the UI object and all the necessary components to make that control work in the scene in a default state.

Some of the features are also available through the GameObject "Add Component" menu in the inspector.

For a full list of the controls and how they are used, please see the [online documentation](https://unity-ui-extensions.github.io/Controls.html) for the project.

## Technical details

## Requirements

This version of the Unity UI Extensions is compatible with the following versions of the Unity Editor:

- 2019 and above - the recommended path for 2019+ is to use the Unity Package Manager to get access to the package.  Full details for installing via UPM can be [found here](https://unity-ui-extensions.github.io/UPMInstallation.html).

> Alternatively, the Asset packages have been tested to work with 2019 as well if you prefer to install that way.

- 2018 and below - for 2018 and use this package, you will have to import the asset package(s), either from the Asset Store or from the alternate download locations [listed here](https://unity-ui-extensions.github.io/Downloads).

## [Release Notes](https://unity-ui-extensions.github.io/ReleaseNotes/RELEASENOTES)

## Release 3.0.0 - Unity 6, reimagined - 2026/06/19

The V3 relaunch brings **full Unity 6 support**, a refreshed brand, and the start of a two-package ecosystem — the proven uGUI library you know, now joined by a modern UI Toolkit companion.

> **Two packages. One ecosystem.** These notes cover the **uGUI** package (`com.unity.uiextensions`). Meet its new companion: [UI Toolkit Extensions](https://github.com/Unity-UI-Extensions/com.unity.uitoolkitextensions).

To get up to speed with the Unity UI Extensions, check out the [Getting Started](https://unity-ui-extensions.github.io/GettingStarted.html) Page.

> Ways to get in touch:
>
> - [GitHub Discussions](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/discussions), if you have any questions, queries or suggestions
> - [Gitter Chat](https://gitter.im/Unity-UI-Extensions/Lobby) site for the UI Extensions project
>
> Much easier that posting a question / issue on YouTube, Twitter or Facebook :D

### Added

- Full support for Unity 6, updating all controls and editor components for the new Unity UI framework, including compile flag support for the updated Unity 6 API (#493, #497)
- Added new GridRawImage control, applying a texture as a repeating grid on a RawImage
- Added new UI_Knob2 control, an updated take on the rotary UI Knob
- Added new UISegmentedCircle control, for drawing segmented circular UI
- Added new UI Graphic Selector control
- Added SetXWithoutNotify and SetYWithoutNotify methods to the BoxSlider, to set values without firing OnValueChanged (@Kurante2801)
- The Pivot can now be used as the reference point when drawing lines with the UILineRenderer (#490)
- Added a "close line" option to the UILineRenderer which finishes the line off with a closer to fill any gaps at the end - Resolves: #449
- Added a new "CullingMode" option to the UI Particle System that when enabled alters the control to resolve unscaled delta time issues - Fixes #486, #487

### Changed

- fix: Optimized Gradient2 when ModifyMesh is called, and it now responds to gradient key updates in both the inspector and at runtime (@bluefallsky)
- fix: Corrected the radial triangle add order (#384) (@bluefallsky)
- fix: The UILineConnector now refreshes when the global scale changes and its point array calculation has been corrected (#495) (@hugoymh)
- fix: The ReorderableList now keeps an item's rotation configuration while dragging (@JavierMonton)
- fix: Addressed a null reference exception in the ReorderableList
- fix: Resolved a stacking issue with the ReorderableList when moving elements "slightly" - Resolves: #470
- fix: Force ScrollRect.content setup on initialization (#485)
- fix: Resolved a race condition in the ScrollSnap controls which could raise a NaN error when lerping - Resolves: #452 / #508
- fix: Patched the HSS/VSS against a potential divide by zero error if the scroll snap has a single page
- fix: Updated GetCurrentPage on the ScrollSnaps to be more resilient - Fixes #254
- fix: Updated the ScrollSnaps to be more resilient to rescaling and patched the full screen scroll snap RIF - Fixes #257, #260
- fix: Resolved out of bounds issues with the Infinite scroll control - Fixes #237
- fix: Addressed layout issues with the FlowLayoutGroup - Fixes #456
- Layout groups updated to rebuild on disable/enable - Resolves: #468
- Updated the UIVertical Scroller to be more efficient for Unity 6 and updated its example
- Updated the Picker control and samples to the latest version
- Removed cap points from the line renderers as they caused LOD and jagged-texture issues
- Renamed Segment to SegmentedControlSegment to avoid class name conflicts
- Updated components to maintain both Text and TextMeshPro compatibility where possible, including a debug option to allow both (#477)
- Reverted Curly Text back to the old Text component as it is not compatible with TextMeshPro - alternatives are being investigated

### Deprecated

- With the move to Unity 6, the old legacy Text based controls have been cleared out as they are no longer valid, along with a general script clean-up to remove legacy dependencies.  For any affected component, use the TextMeshPro alternatives.

## Additional Notes

### [Installation Instructions](https://unity-ui-extensions.github.io/UPMInstallation.html)

The recommended way to add the Unity UI Extensions project to your solution is to use the Unity package Manager. Simply use the Unity Package Manager to reference the project to install it

New for 2020, we have added OpenUPM support and the package can be installed using the following [OpenUPM CLI](https://openupm.com/docs/) command:

```cli
`openupm add com.unity.uiextensions`
```

> For more details on using [OpenUPM CLI, check the docs here](https://github.com/openupm/openupm-cli#installation).

- [UI Extensions Issue log](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/issues)

## Upgrade Notes

We recommend using the UPM delivery method. If you are using the Unity asset, there should be no issues updating but if you have a problem, just deleted the old Unity-UI-Extensions folder and import the asset new.

## Document revision history

|Date|Details|
|-|-|
|July 9th, 2020|2019.4 (v2.2) released, first UPM deployment live |
|September 3rd, 2019|2019.1 (v2.1) released, First major update for the 2.0 series.|
|August 8th, 2020|2019.4 (v2.2) released, New UPM Delivery.|
|October 10th, 2020|2019.5 (v2.2) released, New UPM fast delivery|
|February 7th, 2022|v2.3 released, New Home, UPM fast delivery via OpenUPM|
|June 6th, 2026|v3.0 released, Unity6, the only path|
