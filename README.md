# README #

This is an extension project for the new Unity UI system which can be found at: [Unity UI Source](https://bitbucket.org/Unity-Technologies/ui)

-----

#[Supporting the UI Extensions project](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=89L8T9N6BR7LJ)#

If you wish to support the Unity UI Extensions project itself, then you can using the PayPal link below.
All funds go to support the project, no matter the amount.

> Donations in code are also extremely welcome :D

(PayPal account not required and you can remain anonymous if you wish)

##[>> Donate <<](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=89L8T9N6BR7LJ)##

-----

#[Intro](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/GettingStarted)#
For more info, here's a little introduction video for the project:

[![View Intro Video](http://img.youtube.com/vi/njoIeE4akq0/0.jpg)](http://www.youtube.com/watch?v=njoIeE4akq0 "Unity UI Extensions intro video")

You can follow the UI Extensions team for updates and news on:
### [Twitter](https://twitter.com/search?q=%23unityuiextensions) / [Facebook](https://www.facebook.com/UnityUIExtensions/) / [YouTube](https://www.youtube.com/channel/UCG3gZOkmL-2rmZat4ufv28Q)###

-----

#[ What is this repository for? ](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/About)#

In this repository is a collection of extension scripts to enhance your Unity UI experience. These scripts have been gathered from many sources and combined and improved over time.

> The majority of the scripts came from the Scripts thread on the [Unity UI forum here](http://bit.ly/UnityUIScriptsForumPost)

You can either download / fork this project to access the scripts, or you can also download these pre-compiled Unity Assets, chock full of goodness for each release:

#[Download](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/Downloads)#

The asset has been full tested on all current versions of Unity 5 (for versions prior to Unity 5.3, please see the [archive](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/Downloads))

* [Unity UI Extensions Asset](https://bitbucket.org/ddreaper/unity-ui-extensions/downloads/UnityUIExtensions.unitypackage)

To view previous releases, visit the [release archive](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/Downloads)

-----

#[Getting Started](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/GettingStarted)#
To get started with the project, here's a little guide:

[![View Getting Started Video](http://img.youtube.com/vi/sVLeYmsNQAI/0.jpg)](http://www.youtube.com/watch?v=sVLeYmsNQAI "Unity UI getting started video")

-----

#[Updates:](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/ReleaseNotes/RELEASENOTES)#

##Update 1.2##
[![View 1.2 update Video](http://img.youtube.com/vi/cWv0A6rEEc8/0.jpg)](https://www.youtube.com/watch?v=cWv0A6rEEc8 "Update 1.2 for the Unity UI Extensions Project")

###New / updated features###
* Major updates to the Horizontal and Vertical Scroll Snap controls
* Replacement HSV/Color picker control (and new Box Slider control)
* Fixes / updates to the TextPic control
* Updates to SoftAlphaUI script - improved Text / worldspace support
* Updates to Extensions Toggle - Adds ID and event to publish ID on change
* New Gadient control (gradient 2)
* New UI ScrollRect Occlusion utility
* New UI Tween Scale utility
* New UI Infinite ScrollRect
* New Table Layout Group
* New Non Drawing Graphic Control

###Fixes###
* H&V Scroll Snap indexing issues
* H&V Scroll Snap performance updates 
* H&V Scroll Snap Long swipe behavior updated
* H&V Scroll Snap support for Rect Resizing
* TextPic Set set before draw issues
* HSV picker replaced with more generic color picker

###Known issues###
* The Image_Extended control has been removed due to Unity upgrade issues. Will return in a future update.

##Upgrade Notes##
Although not specifically required, it is recommended to remove the old UI Extensions folder before importing the new asset
The HSS picker especially had a lot of file changes in this update.

>**Note** In Unity 5.5 the particle system was overhauled and several methods were marked for removal. However, the UI Particle System script currently still uses them
> Either ignore these errors or remove the *_UIParticleSystem_* script in the "*Unity UI Extensions / Scripts / Effects*" folder


-------------------
##Release History##

For the full release history, follow the below link to the full release notes page.

### [Release Notes](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/ReleaseNotes/RELEASENOTES)###

---
#[Controls and extensions listed in this project](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/Controls):#

There are almost 70 extension controls / effect and other utilities in the project which are listed on the following page:

##[UI Extensions controls list](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/Controls)##

[Controls](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/Controls#Controls)|||||
------|------|------|------|
Accordion|ColorPicker|SelectionBox|UIButton|UIFlippable
ComboBox|AutoCompleteComboBox|DropDownList|BoundToolTip|UIWindowBase
UI_Knob|TextPic|InputFocus|Box Slider
||||

[Primitives](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/Controls#Primitives)|||||
------|------|------|------|
UILineRenderer|UILineTextureRenderer|UICircle|DiamondGraph|UICornerCut
UIPolygon||||
||||

[Layouts](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/Controls#Layouts)|||||
------|------|------|------|
Horizontal Scroll Snap|Vertical Scroll Snap|Flow Layout Group|Radial Layout|Tile Size Fitter
Scroll Snap (alt implementation)|Reorderable List|UI Vertical Scroller|Curved Layout|Table Layout
||||

[Effects](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/Controls#Effects)|||||
------|------|------|------|
Best Fit Outline|Curved Text|Gradient|Gradient2|Letter Spacing|
NicerOutline|RaycastMask|UIFlippable|UIImageCrop|SoftAlphaMask
CylinderText|UIParticleSystem|||
||||

[VR Components](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/Controls#VR)|||||
------|------|------|------|
VRCursor|VRInputModule|||
||||

[Input Modules](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/Controls#InputModules)|||||
------|------|------|------|
AimerInputModule|GamePadInputModule|||
||||

[Additional Components](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/Controls#Additional_Components)|||||
------|------|------|------|
ReturnKeyTrigger|TabNavigation|uGUITools|ScrollRectTweener|ScrollRectLinker
ScrollRectEx|UI_InfiniteScroll|UI_ScrollRectOcclusion|UIScrollToSelection|UISelectableExtension
switchToRectTransform|ScrollConflictManager|CLFZ2 (Encryption)|Serialization|DragCorrector
PPIViewer|UI_TweenScale|UI_InfiniteScroll|UI_ScrollRectOcclusion|
||||

*More to come*

---


#[ How do I get set up? ](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/GettingStarted)#

Either clone / download this repository to your machine and then copy the scripts in, or use the pre-packaged .UnityPackage for your version of Unity and import it as a custom package in to your project.

#[ Contribution guidelines ](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/ContributionGuidelines)#

Got a script you want added? Then just fork the bitbucket repository and submit a PR.  All contributions accepted (including fixes)
Just ensure 
* The header of the script matches the standard used in all scripts
* The script uses the **Unity.UI.Extensions** namespace so they do not affect any other developments
* (optional) Add Component and Editor options where possible (editor options are in the Editor\UIExtensionsMenuOptions.cs file)

#[ License ](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/License)#

All scripts conform to the BSD license and are free to use / distribute.  See the [LICENSE](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/License) file for more information 

#[ Like what you see? ](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/FurtherInfo)#

All these scripts were put together for my latest book Unity3D UI Essentials
Check out the [page on my blog](http://bit.ly/Unity3DUIEssentials) for more details and learn all about the inner workings of the new Unity UI System.

#[ The downloads ](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/Downloads)#

As this repo was created to support my new Unity UI Title ["Unity 3D UI Essentials"](http://bit.ly/Unity3DUIEssentials), in the downloads section you will find two custom assets (SpaceShip-DemoScene-Start.unitypackage and RollABallSample-Start.unitypackage).  These are just here as starter scenes for doing UI tasks in the book.

I will add more sample scenes for the UI examples in this repository and detail them above over time.

#[Previous Releases](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/Downloads)#

* [Unity UI Extensions Unity 4.x Asset](https://bitbucket.org/ddreaper/unity-ui-extensions/downloads/UnityUIExtensions-4.x.unitypackage)

* [Unity UI Extensions Unity 5.1 Asset](https://bitbucket.org/ddreaper/unity-ui-extensions/downloads/UnityUIExtensions-5.1.unitypackage)

* [Unity UI Extensions Unity 5.2 Asset](https://bitbucket.org/ddreaper/unity-ui-extensions/downloads/UnityUIExtensions-5.2.unitypackage) <- 5.2.0 - 5.2.1 base releases ONLY

* [Unity UI Extensions Unity 5.3 (5.2.1P+) Asset](https://bitbucket.org/ddreaper/unity-ui-extensions/downloads/UnityUIExtensions-5.3.unitypackage) <- use this for 5.2.1P+ releases

> **Note** To retain 5.2 compatibility in the 5.3 package, you will see two warnings related to:
> ```
> `UnityEngine.UI.InputField.onValueChange' is obsolete.  
> ```
> This has no effect on the package working in 5.4 plus and is only there to maintain backwards compatibility.  We will look to update/remove this in a future release, likely after 5.4.  
If you have any concerns, feel free to update your code in your project to add the missing "d".  Unity have ensured it still works as they have mapped the API change for now.