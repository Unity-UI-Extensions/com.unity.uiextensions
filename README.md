# README

This is an extension project for the new Unity UI system which can be found at: [Unity UI Source](https://bitbucket.org/Unity-Technologies/ui)


# [Intro](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/GettingStarted)
For more info, here's a little introduction video for the project:

[![View Intro Video](http://img.youtube.com/vi/njoIeE4akq0/0.jpg)](http://www.youtube.com/watch?v=njoIeE4akq0 "Unity UI Extensions intro video")

You can follow the UI Extensions team for updates and news on:
## [Twitter](https://twitter.com/hashtag/UnityUIExtensions?src=hash) / [Facebook](https://www.facebook.com/UnityUIExtensions/) / [YouTube](https://www.youtube.com/channel/UCG3gZOkmL-2rmZat4ufv28Q)

> ## Also, come chat live with the Unity UI Extensions community on Gitter here: [UI Extensions Live Chat](https://gitter.im/Unity-UI-Extensions/Lobby) 

-----

# [What is this repository for? ](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/About)

In this repository is a collection of extension scripts / effects and controls to enhance your Unity UI experience. These scripts have been gathered from many sources, combined and improved over time.

> The majority of the scripts came from the Scripts thread on the [Unity UI forum here](http://bit.ly/UnityUIScriptsForumPost)

You can either download / fork this project to access the scripts, or you can also download these pre-compiled Unity Assets, chock full of goodness for each release:

# [Download](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Downloads)

For the 2.0 release, we have expanded where you can download the UnityPackage asset and widened the options to contribute to the project.

> I will still stress however, ***contribution is optional***. **The asset / code will always remain FREE**

[![Download from Itch.IO](https://bytebucket.org/UnityUIExtensions/unity-ui-extensions/wiki/SiteImages/itchio.png)](https://unityuiextensions.itch.io/uiextensions2-0 "Download from Itch.IO")
[![Download from Itch.IO](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/SiteImages/unionassets.png)](https://unionassets.com/admin/AssetList "Download from Union Assets")
[![Download from Itch.IO](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/SiteImages/patreon.jpg)](https://www.patreon.com/UnityUIExtensions "Support Unity UI Extensions on Patreon & download")

> Still available on the [BitBucket site](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/downloads/UnityUIExtensions.unitypackage) if you prefer

To view previous releases, visit the [release archive](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Downloads)

-----

# [Supporting the UI Extensions project](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=89L8T9N6BR7LJ)

If you wish to further support the Unity UI Extensions project itself, then you can either subsidise your downloads above, or contribute direct using the PayPal link below.
All funds go to support the project, no matter the amount, donations in code are also extremely welcome :D

> (PayPal account not required and you can remain anonymous if you wish)

## [>> Donate via PayPal <<](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=89L8T9N6BR7LJ)

-----

# [Getting Started](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/GettingStarted)
To get started with the project, here's a little guide:

[![View Getting Started Video](http://img.youtube.com/vi/sVLeYmsNQAI/0.jpg)](http://www.youtube.com/watch?v=sVLeYmsNQAI "Unity UI getting started video")

-----

# [Updates:](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/ReleaseNotes/RELEASENOTES)

## Update 2.0 - The update so big they had to name it twice
[![View 2.0 update Video](http://img.youtube.com/vi/Ivzt9_jhGfQ/0.jpg)](https://www.youtube.com/watch?v=Ivzt9_jhGfQ "Update 2.0 for the Unity UI Extensions Project")

> Be sure to logon to the new Gitter Chat site for the UI Extensions project, if you have any questions, queries or suggestions
> Much easier that posting a question / issue on YouTube, Twitter or Facebook :D
> ## [UIExtensions Gitter Chanel](https://gitter.im/Unity-UI-Extensions/Lobby)

### New / updated features
* Major updates to the Line renderer for texture and positioning support, inc Editor support
* Line Renderer also includes "dotted" line support and the ability to increase the vertex count
* Reorderable list now also works in Screenspace-Camera & Worldspace
* H&V Scroll Snap controls now support scrollbars
* Minor updates to the Gradient 2 control
* Minor updates to all dropdown controls to manage control startup
* Update UI Particle Renderer with new updates, including Texture Sheet animation support
* New Selectable Scalar
* New MonoSpacing text effect
* New Multi-Touch Scrollrect support
* New UI Grid Renderer (handy if you want a UI grid background)
* New CoolDownButton control (adds a timer between button clicks)
* New Curly UI - for those who like their UI Bendy
* New Fancy Scroll View - A programmatic  scroll view
* New UI Line connector control - extends line renderer to draw lines between UI Objects
* New Radial Slider control - for those who like their sliders to curve
* New Stepper control - a +/- control similar to that found on iOS
* New Segmented Control - A button array control similar to that found on iOS
* New UIHighlightable control - just in case the user wasn't sure where they were

### Examples / Examples / Examples
Finally added some proper examples, especially for the newer controls.
These can be found in the Examples folder (which can be safely deleted if you wish)

* ColorPicker - shows the Color Picker UI in both SS and WS
* ComboBox - shows all the different combo box controls
* Cooldown - several example implementations of the cooldown button control using Unity image effects and SAUIM
* CurlyUI - shows off the CurlyUI control
* FancyScrollView - the only REAL way to understand this programmatic control (direct from the contributor)
* HSS-VSS-ScrollSnap - several working examples of the HSS/VSS scroll snaps (not ScrollSnap or FancyScrollView), including a full screen variant
* MenuExample - A demo menu implementation showing off the new MenuManager control
* Radial Slider - Just keep on sliding
* ReorderableList - Several examples of the re-orderable list in action, complete with managed drag / drop features
* ScrollConflictManager - Making ScrollRects get along
* SelectionBox - The RTS selector in action, showing examples of selecting 2D and 3D objects
* Serialisation - Unit test case examples for the serialisation components
* TextEffects - All the Text effects and shaders in one easy to view place
* UIlineRenderer - Several demos / examples for using the Line Renderer and UI Line connector controls
* UIVerticalScrollerDemo - A full screen example of a UIVertical Scroller implementation.

### Fixes
* H&V Scroll Snap Next/Previous Button interactable handler (only enables when there is a child to move to)
* H&V Scroll Snap Swipe logic updated and now includes scaling support
* Editor options for non-drawing graphic control
* Events in ComboBox, Dropdown and autocomplete controls updated to use UI events
* UIFlippable "Argument out of Range" bigfix (pesky component orders with effects)
* All primitive controls will now redraw when enabled in code
* Autocomplete has two lookup text methods now, Array and Linq
* Line renderer pivot fix (now respects the pivot point)
* TextPic rendering and event updates
* Minor tweaks to the UIParticle system to remove "upgrade" errors. Still needs a rework tbh
* Clean up of all unnecessary usings (swept under the rug)

### Known issues
Serialisation functionality has been removed due to several incompatibilities between platforms, suggest the UniversalSerialiser as an alternative. May bring back if there is demand. (or copy out the source from the previous release)

## Upgrade Notes
With this being a major version update, it is recommended to remove the old UI Extensions folder before importing the new asset.

----------------
## Release History

For the full release history, follow the below link to the full release notes page.

### [Release Notes](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/ReleaseNotes/RELEASENOTES)

---
# [Controls and extensions listed in this project](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Controls):

There are almost 70+ extension controls / effect and other utilities in the project which are listed on the following page:

## [UI Extensions controls list](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Controls)

[Controls](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Controls#markdown-header-controls)|||||
------|------|------|------|
Accordion|ColorPicker|SelectionBox|UIButton|UIFlippable
ComboBox|AutoCompleteComboBox|DropDownList|BoundToolTip|UIWindowBase
UI_Knob|TextPic|InputFocus|Box Slider|CooldownButton
Segmented Control|Stepper|||
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
FancyScrollView||||
||||

[Effects](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Controls#markdown-header-effect_components)|||||
------|------|------|------|
Best Fit Outline|Curved Text|Gradient|Gradient2|Letter Spacing
NicerOutline|RaycastMask|UIFlippable|UIImageCrop|SoftAlphaMask
CylinderText|UIParticleSystem|CurlyUI||
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


# [How do I get set up? ](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/GettingStarted)

Either clone / download this repository to your machine and then copy the scripts in, or use the pre-packaged .UnityPackage for your version of Unity and import it as a custom package in to your project.

# [Contribution guidelines ](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/ContributionGuidelines)

Got a script you want added? Then just fork the bitbucket repository and submit a PR.  All contributions accepted (including fixes)
Just ensure 
* The header of the script matches the standard used in all scripts
* The script uses the **Unity.UI.Extensions** namespace so they do not affect any other developments
* (optional) Add Component and Editor options where possible (editor options are in the Editor\UIExtensionsMenuOptions.cs file)

# [License ](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/License)

All scripts conform to the BSD license and are free to use / distribute.  See the [LICENSE](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/License) file for more information 

# [Like what you see? ](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/FurtherInfo)

All these scripts were put together for my latest book Unity3D UI Essentials
Check out the [page on my blog](http://bit.ly/Unity3DUIEssentials) for more details and learn all about the inner workings of the new Unity UI System.

# [The downloads ](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Downloads)

As this repo was created to support my new Unity UI Title ["Unity 3D UI Essentials"](http://bit.ly/Unity3DUIEssentials), in the downloads section you will find two custom assets (SpaceShip-DemoScene-Start.unitypackage and RollABallSample-Start.unitypackage).  These are just here as starter scenes for doing UI tasks in the book.

I will add more sample scenes for the UI examples in this repository and detail them above over time.

# [Previous Releases](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Downloads)

* [Unity UI Extensions Unity 4.x Asset](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/downloads/UnityUIExtensions-4.x.unitypackage)

* [Unity UI Extensions Unity 5.1 Asset](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/downloads/UnityUIExtensions-5.1.unitypackage)

* [Unity UI Extensions Unity 5.2 Asset](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/downloads/UnityUIExtensions-5.2.unitypackage) <- 5.2.0 - 5.2.1 base releases ONLY

* [Unity UI Extensions Unity 5.3 (5.2.1P+) Asset](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/downloads/UnityUIExtensions-5.3.unitypackage) <- use this for 5.2.1P+ releases

> **Note** To retain 5.2 compatibility in the 5.3 package, you will see two warnings related to:
> ```
> `UnityEngine.UI.InputField.onValueChange' is obsolete.  
> ```
> This has no effect on the package working in 5.4 plus and is only there to maintain backwards compatibility.  We will look to update/remove this in a future release, likely after 5.4.  
If you have any concerns, feel free to update your code in your project to add the missing "d".  Unity have ensured it still works as they have mapped the API change for now.