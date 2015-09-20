# README #

This is an extension project for the new Unity UI system which can be found at:
[Unity UI Source](https://bitbucket.org/Unity-Technologies/ui)

#Updated for 5.2 - new asset package#

##Intro##
For more info, here's a little introduction video for the project:

[![View Intro Video](http://img.youtube.com/vi/njoIeE4akq0/0.jpg)](http://www.youtube.com/watch?v=njoIeE4akq0 "Unity UI Extensions intro video")

### What is this repository for? ###

In this repository is a collection of extension scripts to enhance your Unity UI experience. These scripts have been gathered from many sources and combined and improved over time.
(The majority of the scripts came from the Scripts thread on the [Unity UI forum here](http://bit.ly/UnityUIScriptsForumPost))
You can either download / fork the project to access the scripts, or you can download this precompiled Unity Asset, chock full of goodness
### [Unity UI Extensions Unity 4.x Asset](https://bitbucket.org/ddreaper/unity-ui-extensions/downloads/UnityUIExtensions-4.x.unitypackage)###
### [Unity UI Extensions Unity 5.1 Asset](https://bitbucket.org/ddreaper/unity-ui-extensions/downloads/UnityUIExtensions-5.1.unitypackage)###
### [Unity UI Extensions Unity 5.2 Asset](https://bitbucket.org/ddreaper/unity-ui-extensions/downloads/UnityUIExtensions-5.2.unitypackage)###

##Getting Started##
To get started with the project, here's a little guide:

[![View Getting Started Video](http://img.youtube.com/vi/sVLeYmsNQAI/0.jpg)](http://www.youtube.com/watch?v=sVLeYmsNQAI "Unity UI getting started video")
---

## Controls and extensions listed in this project are: ##

## Controls ##
================

Control | Description | Menu Command | Component Command | Notes | Credits
--------- | -------------- | ---------------------- | ---------------------------- | ------- | ----------
**Accordion** | An Acordian style control with animated segments. Sourced from [here]. | N/A | Component / UI / Extensions / AccordionGroup |[demo video](http://forum.unity3d.com/threads/accordion-type-layout.271818/)| ChoMPHi
 | | | Component / UI / Extensions / AccordionItem |[demo video](http://forum.unity3d.com/threads/accordion-type-layout.271818/)| ChoMPHi
**HSVPicker** | A colour picker UI | N/A | UI / Extensions / HSVPicker | Project folder includes prefab and sample scene | judah4
**SelectionBox** | An RTS style selection box control | UI / Extensions / Selection Box | UI / Extensions / Selection Box |[tutorial video](https://www.youtube.com/watch?v=UtM3HejKL5s)| Korindian, BenZed
**UIButton** | Improved Button control with additional events (PointerDown, PointerUp, PointerClick and PointerHeld) | UI / Extensions / UI Button | UI / Extensions / UI Button |Will update video, this has now been updated to a more reusable Selectable extension| AriathTheWise
**UIFlippable** | Improved Image control with image flipping options | UI / Extensions / UI Flippable | UI / Extensions / Flippable |[tutorial video](https://www.youtube.com/watch?v=Htt2RNa2qy0)| ChoMPHi
**UIWindowBase** | A draggable Window implementation | UI / Extensions / UI Window Base | UI / Extensions / UI Window Base |[tutorial video](https://www.youtube.com/watch?v=Htt2RNa2qy0)| GXMark, alexzzzz, CaoMengde777, TroyDavis 
**ComboBox** | A fixed combobox implementation for text | UI / Extensions / ComboBox | UI / Extensions / ComboBox |[tutorial video](https://www.youtube.com/watch?v=JrEfs47FoOE)| Perchik
**AutoCompleteComboBox** | A text combobox with autocomplete selection | UI / Extensions / AutoComplete ComboBox | UI / Extensions / AutoComplete ComboBox |[tutorial video](https://www.youtube.com/watch?v=JrEfs47FoOE)| Perchik
**DropDownList** | A basic drop down list with text and image support | UI / Extensions / Dropdown List | UI / Extensions / Dropdown List |[tutorial video](https://www.youtube.com/watch?v=JrEfs47FoOE)| Perchik
**BoundToolTip** | An alternate Tooltip implementation with central listener | UI / Extensions / Bound Tooltip / Tooltip | UI / Extensions / Bound Tooltip / Tooltip Item | Offset and tooltip placement needs work | Martin Sharkbomb
 | | | UI / Extensions / Bound Tooltip / Tooltip Trigger |  | Martin Sharkbomb


## Primitives ##
================

Control | Description | Menu Command | Component Command | Notes | Credits
--------- | -------------- | ---------------------- | ---------------------------- | ------- | ----------
**LineRenderer** | Graphic control for drawing lines in the UI System | UI / Extensions / Primitives / UI Line Renderer | UI / Extensions / Primitives / UI Line Renderer |[tutorial video](https://www.youtube.com/watch?v=OElcWAZGHi0)| jack.sydorenko
**UILineTextureRenderer** | Graphic control for drawing lines in the UI System | UI / Extensions / Primitives / UI Line Texture Renderer | UI / Extensions / Primitives / UI Line Texture Renderer |[tutorial video](https://www.youtube.com/watch?v=OElcWAZGHi0)| jack.sydorenko, jonbro5556
**UICircle** | Graphic control for drawing circles in the UI System | UI / Extensions / Primitives / UI Circle | UI / Extensions / Primitives / UI Circle || zge



## Layouts ##
================

Layout | Description | Menu Command | Component Command | Notes | Credits
--------- | -------------- | ---------------------- | ---------------------------- | ------- | ----------
**HorizontalScrollSnap** | A pages scroll rect that can work in steps / pages, includes button support | UI / Extensions / Horizontal Scroll Snap | Layout / Extensions / Horizontal Scroll Snap |[tutorial video](https://www.youtube.com/watch?v=KJlIlWHlfMo)| BinaryX
**VerticalScrollSnap** | A pages scroll rect that can work in steps / pages, includes button support | UI / Extensions / Vertical Scroll Snap | Layout / Extensions / Vertical Scroll Snap |[tutorial video](https://www.youtube.com/watch?v=KJlIlWHlfMo)| BinaryX, Simon Darkside Jackson
**FlowLayoutGroup** | A more rugged grid style layout group  || Layout / Extensions / Flow Layout Group | [Example Video](https://www.youtube.com/watch?v=tMe_3tJTZvc) | Simie
**RadialLayout** | A radial layout system || Layout / Extensions / Radial Layout || Danny Goodayle
**TileSizeFitter** | A fitter layout that will shink / expand content by tiles || Layout / Extensions / TileSizeFitter || Ges



## Effect components ##
=====================

Effect   | Description | Component Command | Notes  | Credits
--------- | -------------- | ---------------------------- | ------- | -----------
**BestFitOutline** | An improved outline effect | UI / Effects / Extensions / Best Fit Outline | | Melang
**CurvedText** | A Text vertex manipulator for those users NOT using TextMeshPro (why ever not?) | UI / Effects / Extensons / Curved Text | | Breyer
**Gradient**  | Apply vertex colours in a gradient on any UI object | UI / Effects / Extensions / Gradient | | Breyer
**LetterSpacing** | Allows finers control of text spacing |  UI / Effects / Extensions / Letter Spacing | | Deeperbeige
**NicerOutline** | Another outline control | UI / Effects / Extensions / Nicer Outline | | Melang
**RaycastMask** | An example of an enhanced mask component able to work with the image data. Enables picking on image parts and not just the Rect Transform | UI / Effects / Extensions / Raycast Mask | | senritsu
**UIFlippable** | Image component effect to flip the graphic | UI / Effects / Extensions / UI Flippable | | ChoMPHi

## VR Components##
=======================

Component | Description | Component Command | Notes | Credits
--------- | -------------- | ---------------------------- | ------- | ------
**VRCursor** | Cursor script for VR use (requires VRInputModule) | UI / Extensions / VR Cursoe | | Ralph Barbagallo
**VRInputModule** | Input module to support the VR Cursor | Event / Vr Input Module | | Ralph Barbagallo


## Additional Components##
=======================

Component | Description | Component Command | Notes | Credits
--------- | -------------- | ---------------------------- | ------- | ------
**ReturnKeyTrigger** | Does something?? | UI / Extensions / ReturnKey Trigger | | Melang, ddreaper
**TabNavigation**  | An example Tab navigation script, updated to add manual navigation | UI / Extensions / Tab Navigation | | Melang, omatase
**uGUITools** | | Menu / uGUI | | Senshi
**ScrollRectTweener** | Tweening solution for ScrollRects, add smoothing automatically  | UI / Extensions / ScrollRectTweener |  | Martin Sharkbomb
**ScrollRectLinker** | ScrollRect Linker script, enable multiple ScrollRects to move together  | UI / Extensions / ScrollRectLinker |  | Martin Sharkbomb
**ScrollRectEx** | Improved ScrollRect control, enables support for Nested ScrollRects  | UI / Extensions / ScrollRectEx |  | CaptainSchnittchen, GamesRUs
**InputFocus** | Enhanced InputField control for forms, enables Enter to submit and other features  | UI / Extensions / InputFocus |  | Zelek
**ImageExtended** | Improved Image control with rotation support and use of filled type without an Image (useful for masks)  | UI / Extensions / Image Extended |  | Ges
**UIScrollToSelection** | Enables a ScrollRect to scroll based on the selected child automatically  | UI / Extensions / UIScrollToSelection |  | zero3growlithe
**AimerInputModule** | Replacement Input module to allow for a reciclue to interace with WorldSpace canvas UI  | UI / Extensions / Aimer Input Module |  | Chris Trueman
**UISelectableExtension** | Refactor of original UI Button control, can now add Press/Release and Hold events to any Selectable control  | UI / Extensions / UI Selectable Extension | | AriathTheWise / Simon Jackson
**switchToRectTransform** | RectTransform extension method to move one Rect to another  | N/A | | Izitmee


*More to come*

=======================
### How do I get set up? ###

Either clone / download this repository to your machine and then copy the scripts in, or use the pre-packaged .UnityPackage for your version of Unity and import it as a custom package in to your project.

### Contribution guidelines ###

Got a script you want added, then just fork and submit a PR.  All contributions accepted (including fixes)
Just ensure 
* The header of the script matches the standard used in all scripts
* The script uses the **Unity.UI.Extensions** namespace so they do not affect any other developments
* (optional) Add Component and Editor options where possible (editor options are in the Editor\UIExtensionsMenuOptions.cs file)

### License ###
All scripts conform to the BSD license and are free to use / distribute.  See the [LICENSE](https://bitbucket.org/ddreaper/unity-ui-extensions/src/6d03f25b0150994afa97c6a55854d6ae696cad13/LICENSE?at=default) file for more information 

### Like what you see? ###

All these scripts were put together for my latest book Unity3D UI Essentials
Check out the [page on my blog](http://bit.ly/Unity3DUIEssentials) for more details and learn all about the inner workings of the new Unity UI System.

### The downloads ###
As this repo was created to support my new Unity UI Title ["Unity 3D UI Essentials"](http://bit.ly/Unity3DUIEssentials), in the downloads section you will find two custom assets (SpaceShip-DemoScene-Start.unitypackage and RollABallSample-Start.unitypackage).  These are just here as starter scenes for doing UI tasks in the book.

I will add more sample scenes for the UI examples in this repository and detail them above over time.