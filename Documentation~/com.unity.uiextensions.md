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

## Using Unity UI Extensions

The UI Extensions project provides many automated functions to add the various controls contained within the project commonly accessed via "***GameObject -> UI -> Extensions -> 'Control'***" from the editor menu.  This will add the UI object and all the necessary components to make that control work in the scene in a default state.

Some of the features are also available through the GameObject "Add Component" menu in the inspector.

For a full list of the controls and how they are used, please see the [online documentation](https://unity-ui-extensions.github.io/Controls.html) for the project.

## Technical details

## Requirements

This version of the Unity UI Extensions is compatible with Unity 6 and above.

> Although, at this time there are some known issues with 6000.5 and above, which will be addressed in the next release.

The recommended path is to use the Unity Package Manager to get access to the package.  Full details for installing via UPM can be [found here](https://unity-ui-extensions.github.io/UPMInstallation.html).

## [Release Notes](https://unity-ui-extensions.github.io/ReleaseNotes/RELEASENOTES)

## Release 3.0.0 - Unity 6, reimagined - 2026/06

The V3 relaunch brings **full Unity 6 support**, a refreshed brand, and the start of a two-package ecosystem — the proven uGUI library you know, now joined by a modern UI Toolkit companion.

> **Two packages. One ecosystem.** These notes cover the **uGUI** package (`com.unity.uiextensions`). Meet its new companion: [UI Toolkit Extensions](https://github.com/Unity-UI-Extensions/com.unity.uitoolkitextensions).

### Highlights

- **Full Unity 6 support** — the whole library verified and updated for Unity 6, with legacy dependencies cleared out and the examples refreshed.
- **Two-package ecosystem** — the new UI Toolkit Extensions package launches alongside under the shared 3.0 banner.

To get up to speed with the Unity UI Extensions, check out the [Getting Started](https://unity-ui-extensions.github.io/GettingStarted.html) Page.

> Ways to get in touch:
>
> - [GitHub Discussions](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/discussions), if you have any questions, queries or suggestions
> - [Gitter Chat](https://gitter.im/Unity-UI-Extensions/Lobby) site for the UI Extensions project
>
> Much easier that posting a question / issue on YouTube, Twitter or Facebook :D

## Deprecated

- All deprecated Text based components now have "obsolete" tags, to avoid breaking code.  Note, these do not function in 2022 and above, as Unity have "changed" things.  For any affected component, I recommend updating to use TextMeshPro native features.

- [UI Extensions Issue log](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/issues)

### Added

- New control: **GridRawImage**
- New control: **UI Knob 2** (`UI_Knob2`)
- New control: **UI Segmented Circle** / Segmented Control
- New control: **UI Graphic Selector**
- UILineConnector: the pivot can now be used as the reference point when drawing lines (#490)
- UILineConnector: new "close line" option to finish a line off and fill any gaps at the end
- BoxSlider: added `SetXWithoutNotify` and `SetYWithoutNotify`

### Changed / Fixed

- Reorderable List: fixed a null-reference exception, and resolved element-stacking when moving elements slightly
- Scroll Snap: resolved a race condition that could raise a NaN error when lerping; made rescaling and full-screen scroll snap more resilient
- HSS/VSS: guarded against a divide-by-zero when the scroll snap has a single page; `GetCurrentPage` made more resilient
- Infinite Scroll: resolved out-of-bounds issues
- Flow Layout Group: addressed layout issues and fixed the last line overflowing the rect bounds
- UI Particle System: new "CullingMode" option to resolve unscaled delta time (#486 / #487)
- Gradient2: optimised `ModifyMesh`; fixed radial triangle add order (#384)
- ScrollRect: force `ScrollRect.content` setup (#485)
- UILineConnector: improved point-array calculation (#495); refresh on global scale change
- Layout groups now rebuild on disable/enable
- General TMPro/Text compatibility housekeeping (#477)
- Compile-flag support for Unity 6 (#493)

### Upgrade Notes

We recommend using the UPM delivery method. If you are using the Unity asset, there should be no issues updating but if you have a problem, just deleted the old Unity-UI-Extensions folder and import the asset new.

## Document revision history

|Date|Details|
|-|-|
|July 9th, 2020|2019.4 (v2.2) released, first UPM deployment live |
|September 3rd, 2019|2019.1 (v2.1) released, First major update for the 2.0 series.|
|August 8th, 2020|2019.4 (v2.2) released, New UPM Delivery.|
|October 10th, 2020|2019.5 (v2.2) released, New UPM fast delivery|
|February 7th, 2022|v2.3 released, New Home, UPM fast delivery via OpenUPM|
|June 20th, 2026|v3.0.0 released, Now part 1 of two packages, uGUI and UIToolkit, all refreshed for Unity 6.

