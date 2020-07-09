# README

This is an extension project for the new Unity UI system which can be found at: [Unity UI Source](https://bitbucket.org/Unity-Technologies/ui)
## [  Check out the control demos on our Tumblr page](https://unityuiextensions.tumblr.com/)

# [Intro](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/GettingStarted)
For more info, here's a little introduction video for the project:

[![View Intro Video](http://img.youtube.com/vi/njoIeE4akq0/0.jpg)](http://www.youtube.com/watch?v=njoIeE4akq0 "Unity UI Extensions intro video")

You can follow the UI Extensions team for updates and news on:
## [Twitter](https://twitter.com/hashtag/UnityUIExtensions?src=hash) / [Facebook](https://www.facebook.com/UnityUIExtensions/) / [YouTube](https://www.youtube.com/channel/UCG3gZOkmL-2rmZat4ufv28Q)

> ## Chat live with the Unity UI Extensions community on Gitter here: [UI Extensions Live Chat](https://gitter.im/Unity-UI-Extensions/Lobby) 

-----

# [What is this repository for? ](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/About)

In this repository is a collection of extension scripts / effects and controls to enhance your Unity UI experience. These scripts have been gathered from many sources, combined and improved over time.

> The majority of the scripts came from the Scripts thread on the [Unity UI forum here](http://bit.ly/UnityUIScriptsForumPost)

You can either download / fork this project to access the scripts, or you can also download these pre-compiled Unity Assets, chock full of goodness for each release:

# [Download - 2019.4 (aka 2.2)](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Downloads)

We have expanded where you can download the UnityPackage asset and widened the options to contribute to the project.

> I will still stress however, ***contribution is optional***. **The assets / code will always remain FREE**

| [![Download from Itch.IO](https://bytebucket.org/UnityUIExtensions/unity-ui-extensions/wiki/SiteImages/itchio.png)](https://unityuiextensions.itch.io/uiextensions2-0 "Download from Itch.IO") | [![Download from Itch.IO](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/SiteImages/unionassets.png)](https://unionassets.com/unity-ui-extensions "Download from Union Assets") | [![Download from Itch.IO](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/SiteImages/patreon.jpg)](https://www.patreon.com/UnityUIExtensions "Support Unity UI Extensions on Patreon & download")| 
| :--- | :--- | :--- |
| [Grab from Itchio](https://unityuiextensions.itch.io/uiextensions2-0) | [Obtain via Union Assets](https://unionassets.com/unity-ui-extensions) |[Support through Patreon](https://www.patreon.com/UnityUIExtensions) | 

> Still available to download on the [BitBucket site](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/downloads) if you prefer

To view previous releases, visit the [release archive](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Downloads)

-----

# [Supporting the UI Extensions project](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=89L8T9N6BR7LJ)

If you wish to further support the Unity UI Extensions project itself, then you can either subsidise your downloads above, or using the links below.

All funds go to support the project, no matter the amount. **Donations in code are also extremely welcome**

| | |
|---|---|
| [![Donate via PayPal](https://www.paypalobjects.com/webstatic/mktg/Logo/pp-logo-150px.png)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=89L8T9N6BR7LJ "Donating via Paypal") | [![Buy us a Coffee](https://uploads-ssl.webflow.com/5c14e387dab576fe667689cf/5cbed8a4ae2b88347c06c923_BuyMeACoffee_blue-p-500.png)](https://ko-fi.com/uiextensions "Buy us a Coffee") |

> (PayPal account not required and you can remain anonymous if you wish)

-----

# [Getting Started](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/GettingStarted)
To get started with the project, here's a little guide:

[![View Getting Started Video](http://img.youtube.com/vi/sVLeYmsNQAI/0.jpg)](http://www.youtube.com/watch?v=sVLeYmsNQAI "Unity UI getting started video")

-----

# [Updates:](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/ReleaseNotes/RELEASENOTES)

## Release 2019.4 - (v2.2)  - Back from the future

It's been a long year since the last official release of the Unity UI Extensions project and WOW, have there been a lot of ups and downs.  Big thanks to the community for their support of the project whether that was funds, code or even just testing and helping to iron out some pesky bugs.

> Be sure to logon to the new [Gitter Chat](https://gitter.im/Unity-UI-Extensions/Lobby) site for the UI Extensions project, if you have any questions, queries or suggestions
> 
> Much easier that posting a question / issue on YouTube, Twitter or Facebook :D
> 
> ## [UIExtensions Gitter Chanel](https://gitter.im/Unity-UI-Extensions/Lobby)

### New / updated features

* New UPM deployment for Unity 2019, 2018 will still need to use the asset packages due to Unity compatibility issues.
* Updated the project to the new Unity packaging guidelines, including separating out the examples to a separate package.
* Many line drawing updates, including the ability to draw using a mouse (check the examples)
* Scroll Snaps (HSS/VSS) now have a "Hard Swipe" feature to restrict movement to a single page for each swipe
* Scroll Snaps have also been updated to work better with the UIInfiniteScroll control
* New Unity Card UI controls thanks to @RyanslikeSoCool
* Update to the Fancy Scroll controls with even more added fanciness
* Several updates to adopt newer Unity standards in the controls to ensure full forwards and backwards compatibility

### Examples / Examples / Examples
Examples now have their own package, this simplifies their use and deployment. Especially in 2019 with the UPM deployment.

* Refreshed all examples for Unity 2019
* New Card UI Examples to supplement the new controls
* New Infinite Scroll Snap example
* Fancy Scroll view updated with 2 new examples
* New particle system example, demonstrating programmatic control of the particle system

### Fixes

* Mouse position use updated in
    - RadialSlider
    - ColorSampler
    - TiltWindow
* Check compiler warnings (#197)
* Line Renderer click to add lines (#183)
* ScrollSnap Swiping options - hard fast swipe (#176)
* Shader Loading issue / UIParticleSystem (#229)
* Issue where Menu Prefabs would be disabled instead of their Clones (#210)
* Check ScrollSnapBase update (#265)
* UIInfiniteScroller support for VSS updated and fixes
* Fix to allow radial slider to start from positions other than left
* Fix UI Particles: Texture sheet animation + Random row(#256)
* Fix for wandering ScrollSnap controls due to Local Positioning drift
* Divide By Zero fix for Gradient (#58)


### Known issues
No new issues in this release, but check the issues list for things we are currently working on:

* [UI Extensions Issue log](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/issues?status=new&status=open)

## Upgrade Notes
Due to the restructure of the package to meet Unity's new package guidelines, we recomment **Deleting the current Unity UI Extensions** folder prior to importing the new package

For Unity 2019 users using the new UPM deployment, be sure to delete the existing folder in your assets folder before adding the new package to avoid conflict

----------------
## Release History

For the full release history, follow the below link to the full release notes page.

### [Release Notes](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/ReleaseNotes/RELEASENOTES)

---
# [Controls and extensions listed in this project](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Controls):

There are almost 70+ extension controls / effect and other utilities in the project which are listed on the following page:

> ## [Check out the control demos on our Tumblr page](https://www.tumblr.com/blog/unityuiextensions)
> | [![UI Line Renderer](https://bytebucket.org/UnityUIExtensions/unity-ui-extensions/wiki/SiteImages/LineRenderer.gif)](https://www.tumblr.com/blog/unityuiextensions "UI Line Renderer") | [![UI Knob](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/SiteImages/UIKnob.gif)](https://www.tumblr.com/blog/unityuiextensions "UI Knob")   | [![ScrollSnap](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/SiteImages/ScrollSnap.gif)](https://www.tumblr.com/blog/unityuiextensions "Scroll Snap")| 
> | :--- | :--- | :--- |
> | [UI Line Renderer](https://www.tumblr.com/blog/unityuiextensions) | [UI Knob](https://www.tumblr.com/blog/unityuiextensions) |[Scroll Snap](https://www.tumblr.com/blog/unityuiextensions) | 

## [UI Extensions controls list](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Controls)

[Controls](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Controls#markdown-header-controls)|||||
------|------|------|------|
Accordion|ColorPicker|Selection Box|UI Flippable|ComboBox
AutoComplete ComboBox|DropDown List|BoundToolTip|UIWindowBase|UI Knob
TextPic|Input Focus|Box Slider|Cooldown Button|Segmented Control
Stepper|Range Slider|Radial Slider|MultiTouch Scroll Rect|
||||

[Primitives](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Controls#markdown-header-primitives)|||||
------|------|------|------|
UILineRenderer|UILineTextureRenderer|UICircle|DiamondGraph|UICornerCut
UIPolygon||||
||||

[Layouts](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Controls#markdown-header-layouts)|||||
------|------|------|------|
Horizontal Scroll Snap|Vertical Scroll Snap|Flow Layout Group|Radial Layout|Tile Size Fitter
Scroll Snap (alt implementation)|Reorderable List|UI Vertical Scroller|Curved Layout|Table Layout
FancyScrollView|Card UI|Scroll Position Controller||
||||

[Effects](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Controls#markdown-header-effect_components)|||||
------|------|------|------|
Best Fit Outline|Curved Text|Gradient|Gradient2|Letter Spacing
NicerOutline|RaycastMask|UIFlippable|UIImageCrop|SoftAlphaMask
CylinderText|UIParticleSystem|CurlyUI|Shine Effect|Shader Effects
||||

[VR Components](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Controls#markdown-header-vr_components)|||||
------|------|------|------|
VRCursor|VRInputModule|||
||||

[Input Modules](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Controls#markdown-header-input_modules)|||||
------|------|------|------|
AimerInputModule|GamePadInputModule|||
||||

[Additional Components](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Controls#markdown-header-additional_components)|||||
------|------|------|------|
ReturnKeyTrigger|TabNavigation|uGUITools|ScrollRectTweener|ScrollRectLinker
ScrollRectEx|UI_InfiniteScroll|UI_ScrollRectOcclusion|UIScrollToSelection|UISelectableExtension
switchToRectTransform|ScrollConflictManager|CLFZ2 (Encryption)|DragCorrector|PPIViewer
UI_TweenScale|UI_InfiniteScroll|UI_ScrollRectOcclusion|NonDrawingGraphic|UILineConnector
UIHighlightable|Menu Manager|Pagination Manager||
||||

*More to come*

---


# [How do I get set up?](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/GettingStarted)

As of Unity 2019, there are now two paths for getting access to the Unity UI Extensions project:

- Unity 2019 or higher
The recommended way to add the Unity UI Extensions project to your solution is to use the Unity package Manager. Simply use the [Unity Package Manager](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/UPMInstallation) to reference the project and install it.

Alternatively, you can also use the pre-compiled Unity packages if you wish, however, UPM offers full versioning support to allow you to switch versions as you wish.

- Unity 2018 or lower
The pre-compiled Unity assets are the only solution for Unity 2018 or earlier due to the changes in the Unity UI framework in Unity made for 2019.
Either clone / download this repository to your machine and then copy the scripts in, or use the pre-packaged .UnityPackage for your version of Unity and import it as a custom package in to your project.

# [Contribution guidelines ](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/ContributionGuidelines)

Got a script you want added? Then just fork the bitbucket repository and submit a PR.  All contributions accepted (including fixes)

Just ensure:

* The header of the script should match the standard used in all scripts
* The script uses the **Unity.UI.Extensions** namespace so they do not affect any other developments
* (optional) Add Component and Editor options where possible (editor options are in the Editor\UIExtensionsMenuOptions.cs file)

# [License ](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/License)

All scripts conform to the BSD3 license and are free to use / distribute.  See the [LICENSE](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/License) file for more information 

# [Like what you see? ](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/FurtherInfo)

All these scripts were put together for my latest book Unity3D UI Essentials
Check out the [page on my blog](http://bit.ly/Unity3DUIEssentials) for more details and learn all about the inner workings of the new Unity UI System.

# [The downloads ](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Downloads)

As this repo was created to support my new Unity UI Title ["Unity 3D UI Essentials"](http://bit.ly/Unity3DUIEssentials), in the downloads section you will find two custom assets (SpaceShip-DemoScene-Start.unitypackage and RollABallSample-Start.unitypackage).  These are just here as starter scenes for doing UI tasks in the book.

I will add more sample scenes for the UI examples in this repository and detail them above over time.

# [Previous Releases](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Downloads)

Please see the [full downloads list](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Downloads) for all previous releases and their corresponding download links.