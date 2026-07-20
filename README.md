# Unity UI Extensions README

This is an extension project for the new Unity UI system which can be found at: [Unity UI Source](https://github.com/Unity-Technologies/uGUI)

> [Check out the control demos on our Tumblr page](https://unityuiextensions.tumblr.com/)

[![openupm](https://img.shields.io/npm/v/com.unity.uiextensions?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.unity.uiextensions/)
[![Publish main branch and increment version](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/actions/workflows/main-publish.yml/badge.svg)](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/actions/workflows/main-publish.yml)
[![Publish development branch on Merge](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/actions/workflows/development-publish.yml/badge.svg)](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/actions/workflows/development-publish.yml)
[![Build and test UPM packages for platforms, all branches except main](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/actions/workflows/development-buildandtestupmrelease.yml/badge.svg)](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/actions/workflows/development-buildandtestupmrelease.yml)

## [Intro](https://unity-ui-extensions.github.io/GettingStarted)

For more info, here's a little introduction video for the project:

[![View Intro Video](http://img.youtube.com/vi/njoIeE4akq0/0.jpg)](http://www.youtube.com/watch?v=njoIeE4akq0 "Unity UI Extensions intro video")

You can follow the UI Extensions team for updates and news on:

### [Twitter - #unityuiextensions](https://twitter.com/search?q=%23unityuiextensions) / [Facebook](https://www.facebook.com/UnityUIExtensions/) / [YouTube](https://www.youtube.com/@UnityUIExtensions)

> Ways to get in touch:
>
> - [Gitter Chat](https://gitter.im/Unity-UI-Extensions/Lobby) site for the UI Extensions project
> - [GitHub Discussions](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/discussions), if you have any questions, queries or suggestions

-----

## [What is this repository for?](https://unity-ui-extensions.github.io/About)

In this repository is a collection of extension scripts / effects and controls to enhance your Unity UI experience. These scripts have been gathered from many sources, combined and improved over time.

> The majority of the scripts came from the Scripts thread on the [Unity UI forum here](http://bit.ly/UnityUIScriptsForumPost)

You can either download / fork this project to access the scripts, or you can also download these pre-compiled Unity Assets, chock full of goodness for each release:

## [Download Latest - Version 2.3](https://unity-ui-extensions.github.io/Downloads)

We have expanded where you can download the UnityPackage asset and widened the options to contribute to the project.

> I will still stress however, ***contribution is optional***. **The assets / code will always remain FREE**

| [![Download from Itch.IO](https://unity-ui-extensions.github.io/SiteImages/itchio.png)](https://unityuiextensions.itch.io/uiextensions2-0 "Download from Itch.IO") | [![Download from Itch.IO](https://unity-ui-extensions.github.io/SiteImages/unionassets.png)](https://unionassets.com/unity-ui-extensions "Download from Union Assets") | [![Download from Itch.IO](https://unity-ui-extensions.github.io/SiteImages/patreon.jpg)](https://www.patreon.com/UnityUIExtensions "Support Unity UI Extensions on Patreon & download")|
| :--- | :--- | :--- |
| [Grab from Itchio](https://unityuiextensions.itch.io/uiextensions2-0) | [Obtain via Union Assets](https://unionassets.com/unity-ui-extensions) |[Support through Patreon](https://www.patreon.com/UnityUIExtensions) |

> Still available to download on the [GitHub site](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/releases) if you prefer

To view previous releases, visit the [release archive](https://unity-ui-extensions.github.io/Downloads)

-----

## [Supporting the UI Extensions project](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=89L8T9N6BR7LJ)

If you wish to further support the Unity UI Extensions project itself, then you can either subsidize your downloads above, or using the links below.

All funds go to support the project, no matter the amount. **Donations in code are also extremely welcome**

|[![Donate via PayPal](https://www.paypalobjects.com/webstatic/mktg/Logo/pp-logo-150px.png)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=89L8T9N6BR7LJ "Donating via Paypal")|[![Buy us a Coffee](https://uploads-ssl.webflow.com/5c14e387dab576fe667689cf/5cbed8a4ae2b88347c06c923_BuyMeACoffee_blue-p-500.png)](https://ko-fi.com/uiextensions "Buy us a Coffee")|
|-|-|
|||

> (PayPal account not required and you can remain anonymous if you wish)

-----

## [Getting Started](https://unity-ui-extensions.github.io/GettingStarted)

To get started with the project, here's a little guide:

[![View Getting Started Video](http://img.youtube.com/vi/sVLeYmsNQAI/0.jpg)](http://www.youtube.com/watch?v=sVLeYmsNQAI "Unity UI getting started video")

-----

## [Updates:](https://unity-ui-extensions.github.io/ReleaseNotes/RELEASENOTES)

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

-----

## Release History

For the full release history, follow the below link to the full release notes page.

### [Release Notes](https://unity-ui-extensions.github.io/ReleaseNotes/RELEASENOTES)

-----

## [Controls and extensions listed in this project](https://unity-ui-extensions.github.io/Controls)

There are almost 70+ extension controls / effect and other utilities in the project which are listed on the following page:

> ## [Check out the control demos on our Tumblr page](https://www.tumblr.com/blog/unityuiextensions)
>
> | [![UI Line Renderer](https://unity-ui-extensions.github.io/SiteImages/LineRenderer.gif)](https://www.tumblr.com/blog/unityuiextensions "UI Line Renderer") | [![UI Knob](https://unity-ui-extensions.github.io/SiteImages/UIKnob.gif)](https://www.tumblr.com/blog/unityuiextensions "UI Knob")   | [![ScrollSnap](https://unity-ui-extensions.github.io/SiteImages/ScrollSnap.gif)](https://www.tumblr.com/blog/unityuiextensions "Scroll Snap")|
> | :--- | :--- | :--- |
> | [UI Line Renderer](https://www.tumblr.com/blog/unityuiextensions) | [UI Knob](https://www.tumblr.com/blog/unityuiextensions) |[Scroll Snap](https://www.tumblr.com/blog/unityuiextensions) |

## [UI Extensions controls list](https://unity-ui-extensions.github.io/Controls)

[Controls](https://unity-ui-extensions.github.io/Controls.html#controls)

|Accordion|ColorPicker|Selection Box|Segmented Control|ComboBox|
|-|-|-|-|-|
|AutoComplete ComboBox|DropDown List|BoundToolTip|UIWindowBase|UI Knob|
|UI Knob2|TextPic|Input Focus|Box Slider|Cooldown Button|
|Stepper|Range Slider|Radial Slider|MultiTouch Scroll Rect|MinMax Slider|
|GridRawImage|UICircleSegmented|UIGraphicSector|||

[Primitives](https://unity-ui-extensions.github.io/Controls.html#primitives)

|UILineRenderer|UILineTextureRenderer|UILineRendererFIFO|UILineRendererList|UICircle|
|-|-|-|-|-|
|DiamondGraph|UICornerCut|UIPolygon|UISquircle|UIGridRenderer|

[Layouts](https://unity-ui-extensions.github.io/Controls.html#layouts)

|Horizontal Scroll Snap|Vertical Scroll Snap|Flow Layout Group|Radial Layout|Tile Size Fitter|
|-|-|-|-|-|
|Scroll Snap (alt implementation)|Reorderable List|UI Vertical Scroller|UI Horizontal Scroller|Curved Layout|
|Table Layout|FancyScrollView|Card UI|Scroll Position Controller (obsolete)|Content Scroll Snap Horizontal|
|Scroller|ResizePanel|RescalePanel|RescaleDragPanel||

[Effects](https://unity-ui-extensions.github.io/Controls.html#effect-components)

|Gradient|Gradient2|RaycastMask|SoftAlphaMask|UIFlippable|
|-|-|-|-|-|
|UIImageCrop|UIParticleSystem|CurlyUI|Shine Effect|Shader Effects|

> The legacy Text based effects (Best Fit Outline, Curved Text, Letter Spacing, NicerOutline and CylinderText) were removed in V3.0.0 as they are not supported with TextMeshPro due to its architecture, try using the native TextMeshPro effects instead.

[Additional Components](https://unity-ui-extensions.github.io/Controls.html#additional-components)

|ReturnKeyTrigger|TabNavigation|uGUITools|ScrollRectTweener|ScrollRectLinker|
|-|-|-|-|-|
|ScrollRectEx|UI_InfiniteScroll|UI_ScrollRectOcclusion|UIScrollToSelection|UISelectableExtension|
|switchToRectTransform|ScrollConflictManager|CLFZ2 (Compression)|DragCorrector|PPIViewer|
|UI_TweenScale|UI_MagneticInfiniteScroll|NonDrawingGraphic|UILineConnector|UIHighlightable|
|Menu Manager|Pagination Manager|ResetSelectableHighlight|SelectableScaler|InputFieldEnterSubmit|

*More to come*

-----

## [How do I get set up?](https://unity-ui-extensions.github.io/UPMInstallation.html)

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

## [Contribution guidelines](https://unity-ui-extensions.github.io/ContributionGuidelines)

Got a script you want added? Then just fork the [GitHub repository](https://github.com/unity-UI-Extensions/com.unity.uiextensions) and submit a PR.  All contributions accepted (including fixes)

Just ensure:

- The header of the script should match the standard used in all scripts.
- The script uses the **Unity.UI.Extensions** namespace so they do not affect any other developments.
- (optional) Add Component and Editor options where possible. (editor options are in the Editor\UIExtensionsMenuOptions.cs file)

## [License](https://raw.githubusercontent.com/Unity-UI-Extensions/com.unity.uiextensions/release/LICENSE.md)

All scripts conform to the BSD-3-Clause license and are free to use / distribute.  See the [LICENSE](https://raw.githubusercontent.com/Unity-UI-Extensions/com.unity.uiextensions/release/LICENSE.md) file for more information =

## [Like what you see?](https://unity-ui-extensions.github.io/FurtherInfo)

All these scripts were put together for my latest book Unity3D UI Essentials
Check out the [page on my blog](http://bit.ly/Unity3DUIEssentials) for more details and learn all about the inner workings of the new Unity UI System.

## [The downloads](https://unity-ui-extensions.github.io/Downloads)

As this repo was created to support my new Unity UI Title ["Unity 3D UI Essentials"](http://bit.ly/Unity3DUIEssentials), in the downloads section you will find two custom assets (SpaceShip-DemoScene-Start.unitypackage and RollABallSample-Start.unitypackage).   These are just here as starter scenes for doing UI tasks in the book.

I will add more sample scenes for the UI examples in this repository and detail them above over time.

## [Previous Releases](https://unity-ui-extensions.github.io/Downloads)

Please see the [full downloads list](https://unity-ui-extensions.github.io/Downloads) for all previous releases and their corresponding download links.
