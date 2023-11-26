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

|Accordion|ColorPicker|Selection Box|UI Flippable|ComboBox|
|-|-|-|-|-|
|AutoComplete ComboBox|DropDown List|BoundToolTip|UIWindowBase|UI Knob|
|TextPic|Input Focus|Box Slider|Cooldown Button|Segmented Control|
|Stepper|Range Slider|Radial Slider|MultiTouch Scroll Rect|MinMax SLider|

[Primitives](https://unity-ui-extensions.github.io/Controls.html#primitives)

|UILineRenderer|UILineTextureRenderer|UICircle|DiamondGraph|UICornerCut|
|-|-|-|-|-|
|UIPolygon|UISquircle||||

[Layouts](https://unity-ui-extensions.github.io/Controls.html#layouts)

|Horizontal Scroll Snap|Vertical Scroll Snap|Flow Layout Group|Radial Layout|Tile Size Fitter|
|-|-|-|-|-|
|Scroll Snap (alt implementation)|Reorderable List|UI Vertical Scroller|Curved Layout|Table Layout|
|FancyScrollView|Card UI|Scroll Position Controller (obsolete)|Content Scroll Snap Horizontal|Scroller|
|ResizePanel|RescalePanel|RescaleDragPanel|||

[Effects](https://unity-ui-extensions.github.io/Controls.html#effect-components)

|Best Fit Outline|Curved Text|Gradient|Gradient2|Letter Spacing|
|-|-|-|-|-|
|NicerOutline|RaycastMask|UIFlippable|UIImageCrop|SoftAlphaMask|
|CylinderText|UIParticleSystem|CurlyUI|Shine Effect|Shader Effects|

> Text Effects are not supported with TextMeshPro due to its architecture, try using the native TextMeshPro effects instead.

[Additional Components](https://unity-ui-extensions.github.io/Controls.html#additional-components)

|ReturnKeyTrigger|TabNavigation|uGUITools|ScrollRectTweener|ScrollRectLinker|
|-|-|-|-|-|
|ScrollRectEx|UI_InfiniteScroll|UI_ScrollRectOcclusion|UIScrollToSelection|UISelectableExtension|
|switchToRectTransform|ScrollConflictManager|CLFZ2 (Encryption)|DragCorrector|PPIViewer|
|UI_TweenScale|UI_MagneticInfiniteScroll|UI_ScrollRectOcclusion|NonDrawingGraphic|
|UILineConnector|
|UIHighlightable|Menu Manager|Pagination Manager|||

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

All scripts conform to the BSD3 license and are free to use / distribute.  See the [LICENSE](https://raw.githubusercontent.com/Unity-UI-Extensions/com.unity.uiextensions/release/LICENSE.md) file for more information =

## [Like what you see?](https://unity-ui-extensions.github.io/FurtherInfo)

All these scripts were put together for my latest book Unity3D UI Essentials
Check out the [page on my blog](http://bit.ly/Unity3DUIEssentials) for more details and learn all about the inner workings of the new Unity UI System.

## [The downloads](https://unity-ui-extensions.github.io/Downloads)

As this repo was created to support my new Unity UI Title ["Unity 3D UI Essentials"](http://bit.ly/Unity3DUIEssentials), in the downloads section you will find two custom assets (SpaceShip-DemoScene-Start.unitypackage and RollABallSample-Start.unitypackage).   These are just here as starter scenes for doing UI tasks in the book.

I will add more sample scenes for the UI examples in this repository and detail them above over time.

## [Previous Releases](https://unity-ui-extensions.github.io/Downloads)

Please see the [full downloads list](https://unity-ui-extensions.github.io/Downloads) for all previous releases and their corresponding download links.
