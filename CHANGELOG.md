# Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/).

## Release 3.0.0 - Unity 6, reimagined - 2026/06

The V3 relaunch brings **full Unity 6 support**, a refreshed brand, and the start of a two-package ecosystem — the proven uGUI library you know, now joined by a modern UI Toolkit companion.

> **Two packages. One ecosystem.** These notes cover the **uGUI** package (`com.unity.uiextensions`). Meet its new companion: [UI Toolkit Extensions](https://github.com/Unity-UI-Extensions/com.unity.uitoolkitextensions).

### Highlights

- **Full Unity 6 support** — the whole library verified and updated for Unity 6, with legacy dependencies cleared out and the examples refreshed.
- **Two-package ecosystem** — the new UI Toolkit Extensions package launches alongside under the shared 3.0 banner.

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

### Contributors

Huge thanks to everyone who contributed to this release:
[@SimonDarksideJ](https://github.com/SimonDarksideJ), [@bluefallsky](https://github.com/bluefallsky), [@hugoymh](https://github.com/hugoymh), [@JavierMonton](https://github.com/JavierMonton), [@Dover8](https://github.com/Dover8), [@fgrg2801](https://github.com/fgrg2801), [@Moderbord](https://github.com/Moderbord).

-----

## Release 2.3.2 - Rejuvenation - 2023/11/26

2023 is certainly an interesting year to keep you on your toes, and finding time to keep managing all the requests and updates that come in are taking their toll, especially for a FREE project, but nonetheless, I still do it.

Mainly bugfixes for the end of year update, promoting some resolutions that have been verified and tested since the last release.

To get up to speed with the Unity UI Extensions, check out the [Getting Started](https://unity-ui-extensions.github.io/GettingStarted.html) Page.

> Ways to get in touch:
>
> - [GitHub Discussions](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/discussions), if you have any questions, queries or suggestions
> - [Gitter Chat](https://gitter.im/Unity-UI-Extensions/Lobby) site for the UI Extensions project
>
> Much easier that posting a question / issue on YouTube, Twitter or Facebook :D

## Breaking changes

For customers upgrading from earlier versions of Unity to Unity 2020, please be aware of the Breaking change related to Text Based components.  You will need to manually replace any UI using the older ```Text``` component and replace them with ```TextMeshPro``` versions. This is unavoidable due to Unity deprecating the Text component.

> New users to 2022 are unaffected as all the Editor commands have been updated to use the newer TextMeshPro versions.

For more details, see the [deprecation notice](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/discussions/428) on GitHub.

## Added

- Add CalculatePointOnCurve for uilinerenderer (@victornor)

## Changed

- fix: Fixed an null reference exception with the ResetSelectableHighlight (@FejZa)
- fix: Resolved an issue where the last line in a flow layout group would overflow the rect bounds.
- fix: GetPosition when Segments is null (@victornor)
- fix: Fix Bug! NicerOutline color.a Loss when m_UseGraphicAlpha is true (wanliyun)
- fix: Update to force Enumerated start for Accordion elements, Resolves: #455
- Added argument to the UpdateLayout method for the HSS/VSS to move to a new starting page.
- Updated implementations to handle 2023 support, with 2023 moving in to public release.
- Added extra event on the AutoCompleteComboBox, to fire when an item in the list is selected, with its display name.
- FlowLayoutGroup components updated to latest (likely the last as the author has stopped development)

## Deprecated

- All deprecated Text based components now have "obsolete" tags, to avoid breaking code.  Note, these do not function in 2022 and above, as Unity have "changed" things.  For any affected component, I recommend updating to use TextMeshPro native features.

## Additional Notes

### [Installation Instructions](https://unity-ui-extensions.github.io/UPMInstallation.html)

The recommended way to add the Unity UI Extensions project to your solution is to use the Unity package Manager. Simply use the Unity Package Manager to reference the project to install it

New for 2020, we have added OpenUPM support and the package can be installed using the following [OpenUPM CLI](https://openupm.com/docs/) command:

```cli
`openupm add com.unity.uiextensions`
```

> For more details on using [OpenUPM CLI, check the docs here](https://github.com/openupm/openupm-cli#installation).

- Unity Package Manager - manual

Alternatively, you can also add the package manually through the Unity package manager using the scope ```com.unity.uiextensions```, see the [Unity Package Manager docs](https://learn.unity.com/tutorial/the-package-manager) for more information.

- Unity 2018 or lower
The pre-compiled Unity assets are the only solution for Unity 2018 or earlier due to the changes in the Unity UI framework in Unity made for 2019.
Either clone / download this repository to your machine and then copy the scripts in, or use the pre-packaged .UnityPackage for your version of Unity and import it as a custom package in to your project.

### Upgrade Notes

### UPM

If you are using UPM to gain access to the Unity UI Extensions, then you only need to update to the latest version in the Package Manager, no other changes needed.

### Customers using the .UnityPackage

Due to the restructure of the package to meet Unity's new package guidelines, we recommend **Deleting the current Unity UI Extensions** folder prior to importing the new package.

For Unity 2019 users using the new UPM deployment, be sure to delete the existing folder in your assets folder before adding the new package to avoid conflict.
