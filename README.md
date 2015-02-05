# README #

This is an extension project for the new Unity UI system which can be found at:
[Unity UI Source](https://bitbucket.org/Unity-Technologies/ui)

### What is this repository for? ###

In this repository is a collection of extension scripts to enhance your Unity UI experience. These scripts have been gathered from many sources and combined and improved over time.
(The majority of the scripts came from the Scripts thread on the [Unity UI forum here](http://bit.ly/UnityUIScriptsForumPost))

These include:

## New Controls ##
================

Control | Description | Menu Command | Component Command | Notes
--------- | -------------- | ---------------------- | ---------------------------- | -------
**Accordion** | An Acordian style control with animated segments. Sourced from [here](http://forum.unity3d.com/threads/accordion-type-layout.271818/). For more details, [see this video demonstration](https://www.youtube.com/watch?v=YSOSVTU5klw) | N/A | Component / UI / Extensions / AccordionGroup |
 | | | Component / UI / Extensions / AccordionItem | 
**ComboBox** | A styled Combo Box control | UI / Extensions / Combobox | UI / Extensions / Combobox | Still being improved by author
**HSVPicker** | A colour picker UI | N/A | UI / Extensions / HSVPicker | Project folder incluses prefab
**SelectionBox** | An RTS style selection box control | UI / Extensions / Selection Box | UI / Extensions / Selection Box | Needs documentation on use, selection area defines the area on screen that is selectable.  Uses script on selectable items on screen
**HorizontalScrollSnap** | A pages scroll rect that can work in steps / pages, includes button support | UI / Extensions / Horizontal Scroll Snap | UI / Extensions / Horizontal Scroll Snap |
**UIButton** | Improved Button control with additional events | UI / Extensions / UI Button | UI / Extensions / UI Button |
**UIWindowBase** | A draggable Window implementation | UI / Extensions / UI Window Base | UI / Extensions / UI Window Base |

## Effect components ##
=====================

Effect   | Description | Component Command | Notes
--------- | -------------- | ---------------------------- | -------
**BestFitOutline** | An improved outline effect | UI / Effects / Extensions / Best Fit Outline | 
**CurvedText** | A Text vertex manipulator for those users NOT using TextMeshPro (why ever not?) | UI / Effects / Extensons / Curved Text | 
**Gradient**  | Apply vertex colours in a gradient on any UI object | UI / Effects / Extensions / Gradient |
**LetterSpacing** | Allows finers control of text spacing |  UI / Effects / Extensions / Letter Spacing |
**NicerOutline** | Another outline control | UI / Effects / Extensions / Nicer Outline |
**RaycastMask** | An example of an enhanced mask component able to work with the image data. Enables picking on image parts and not just the Rect Transform | UI / Effects / Extensions / Raycast Mask |
**UIFlippable** | Image component effect to flip the graphic | UI / Effects / Extensions / UI Flippable |


## Additional Components##
=======================

Component | Description | Component Command | Notes
--------- | -------------- | ---------------------------- | -------
**ReturnKeyTrigger** | Does something?? | UI / Extensions / ReturnKey Trigger |
**TabNavigation**  | An example Tb navigation script | UI / Extensions / Tab Navigation |
**uGUITools | | Menu / uGUI |


*More to come*

### How do I get set up? ###

Either clone / download this repository to your machine and then copy the scripts in, or use the pre-packaged .UnityPackage and import it as a custom package in to your project.

### Contribution guidelines ###

Got a script you want added, then just fork and submit a PR.  All contributions accepted (including fixes)

### Like what you see? ###

All these scripts were put together for my latest book Unity3D UI Essentials
Check out the [page on my blog](http://bit.ly/Unity3DUIEssentials) for more details and learn all about the inner workings of the new Unity UI System.

### The downloads ###
As this repo was created to support my new Unity UI Title ["Unity 3D UI Essentials"](http://bit.ly/Unity3DUIEssentials), in the downloads section you will find two custom assets (SpaceShip-DemoScene-Start.unitypackage and RollABallSample-Start.unitypackage).  These are just here as starter scenes for doing UI tasks in the book.

I will add more sample scenes for the UI examples in this repository and detail them above over time.