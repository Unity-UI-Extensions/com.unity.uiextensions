# README #

This is an extension project for the new Unity UI system which can be found at: [Unity UI Source](https://bitbucket.org/Unity-Technologies/ui)
###For Unity 5.2.2+ - Use the new 5.3 package!###
###*Note, due to limited demand, this is the last release we will update the 4.x/5.1/5.2 asset package, we'll be focusing on 5.3/5.4 from now on.###

-----
#[Supporting the UI Extensions project](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=89L8T9N6BR7LJ)#
If you wish to support the Unity UI Extensions project itself, then you can using the PayPal link below. 

(paypal account not required and you can remain anonymous if you wish)

##[>> Donate <<](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=89L8T9N6BR7LJ)##

-----
#[Intro](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/GettingStarted)#
For more info, here's a little introduction video for the project:

[![View Intro Video](http://img.youtube.com/vi/njoIeE4akq0/0.jpg)](http://www.youtube.com/watch?v=njoIeE4akq0 "Unity UI Extensions intro video")

You can follow the UI Extentions team for updates and news on:
### [Twitter](https://twitter.com/search?q=%23uiextensions) / [Facebook](https://www.facebook.com/UnityUIExtensions/) / [YouTube](https://www.youtube.com/channel/UCG3gZOkmL-2rmZat4ufv28Q)###
-----
#[ What is this repository for? ](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/About)#

In this repository is a collection of extension scripts to enhance your Unity UI experience. These scripts have been gathered from many sources and combined and improved over time.

> The majority of the scripts came from the Scripts thread on the [Unity UI forum here](http://bit.ly/UnityUIScriptsForumPost)

You can either download / fork this project to access the scripts, or you can also download these precompiled Unity Assets, chock full of goodness for each release:

#[Downloads](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/Downloads)#
* [Unity UI Extensions Unity 4.x Asset](https://bitbucket.org/ddreaper/unity-ui-extensions/downloads/UnityUIExtensions-4.x.unitypackage)

* [Unity UI Extensions Unity 5.1 Asset](https://bitbucket.org/ddreaper/unity-ui-extensions/downloads/UnityUIExtensions-5.1.unitypackage)

* [Unity UI Extensions Unity 5.2 Asset](https://bitbucket.org/ddreaper/unity-ui-extensions/downloads/UnityUIExtensions-5.2.unitypackage) <- 5.2.0 - 5.2.1 base releases ONLY

* [Unity UI Extensions Unity 5.3 (5.2.1P+) Asset](https://bitbucket.org/ddreaper/unity-ui-extensions/downloads/UnityUIExtensions-5.3.unitypackage) <- use this for 5.2.1P+ releases

> **Note** To retain 5.2 compatibility in the 5.3 package, you will see two warnings related to:
> ```
> `UnityEngine.UI.InputField.onValueChange' is obsolete.  
> ```
> This has no effect on the package working in 5.4 plus and is only there to maintain backwards compatibility.  We will look to update/remove this in a future release, likely after 5.4.  If you have any concerns, feel free to update your code in your project to add the missing "d".  Unity have ensured it still works as they have mapped the API change for now.

-----
#[Getting Started](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/GettingStarted)#
To get started with the project, here's a little guide:

[![View Getting Started Video](http://img.youtube.com/vi/sVLeYmsNQAI/0.jpg)](http://www.youtube.com/watch?v=sVLeYmsNQAI "Unity UI getting started video")

-----
#[Updates:](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/ReleaseNotes/RELEASENOTES)#

##Update 1.1##
[![View 1.1 update Video](http://img.youtube.com/vi/JuE0ja5DmV4/0.jpg)](https://www.youtube.com/watch?v=JuE0ja5DmV4 "Update 1.1 for the Unity UI Extensions Project")

###New / updated features###
* New Polygon primitive
* New UI Vertical Scroller control
* New Curved layout component
* New Shining effect
* New UI Particle system
* New Scroll Conflict Manager
* Soft Alpha Mask updated in line with SAUI 1.3 release
* Line Renderer has had a complete overhaul, including full programmatic support, Line list and Bezier line rendering
* Horizontal and Vertical Scroll Snaps updated to include a Starting page, current page and transition speed parameters. Plus a new GoToPage, Add and Remove page functions
* Added some script helper functions for LZF compression and Serialization
* Two utilities to help manage drag thresholds on high PPI systems

###Fixes###
* Line Render almost completely re-written with tons of fixes
* Radial layout updated to avoid 360 overlap (first and last)
* Scroll Snaps updates to better handle children.
* Scroll Snaps distribute function updated so it can be called onDirty more efficiently.

##Upgrade Notes##
A few scripts from earlier releases were moved and need their originals need deleting post upgrade.  Please remove the following files if found:

* Scripts\ImageExtended.cs
* Scripts\UIScrollToSelection.cs
* Scripts\UIScrollToSelectionXY.cs
* Scripts\UISelectableExtension.cs
* Scripts\Effects\UIImageCrop.cs

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
Accordion|HSVPicker|SelectionBox|UIButton|UIFlippable
ComboBox|AutoCompleteComboBox|DropDownList|BoundToolTip|UIWindowBase
UI_Knob|TextPic|||
||||

[Primitives](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/Controls#Primitives)|||||
------|------|------|------|
UILineRenderer|UILineTextureRenderer|UICircle|DiamondGraph|UICornerCut
UIPolygon||||
||||

[Layouts](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/Controls#Layouts)|||||
------|------|------|------|
HorizontalScrollSnap|VerticalScrollSnap|FlowLayoutGroup|RadialLayout|TileSizeFitter
ScrollSnap|ReorderableList|UIVerticalScroller|CurvedLayout|
||||

[Effects](https://bitbucket.org/ddreaper/unity-ui-extensions/wiki/Controls#Effects)|||||
------|------|------|------|
BestFitOutline|CurvedText|Gradient|LetterSpacing|NicerOutline
RaycastMask|UIFlippable|UIImageCrop|SoftAlphaMask|CylinderText
UIParticleSystem||||
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
ScrollRectEx|InputFocus|ImageExtended|UIScrollToSelection|UISelectableExtension
switchToRectTransform|ScrollConflictManager|CLFZ2 (Encryption)|Serialization|DragCorrector
PPIViewer|UI_TweenScale|UI_ScrollRectOcclusion|UI_InfiniteScroll|
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