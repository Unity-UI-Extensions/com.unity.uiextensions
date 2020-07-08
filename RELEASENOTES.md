# Unity UI Extensions release notes #
This file contains the up to date release notes for each release of the UI Extensions project including release videos where required.

----------------
## Update 2019.4 - 2.2  - Back from the future

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
* Scroll Snaps have also been udpated to work better with the UIInfiniteScroll control
* New Uniy Card UI controls thanks to @RyanslikeSoCool
* Update to the Fancy Scoll controls with even more added fanciness
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

# [Installation Instructions](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/GettingStarted)

As of Unity 2019, there are now two paths for getting access to the Unity UI Extensions project:

- Unity 2019 or higher
The recommended way to add the Unity UI Extensions project to your solution is to use the Unity package Manager. Simply use the Unity Package Manager to reference the project to install it

Alternatively, you can also use the pre-compiled Unity packages if you wish, however, UPM offers full versioning support to allow you to switch versions as you wish.

- Unity 2018 or lower
The pre-compiled Unity assets are the only solution for Unity 2018 or earlier due to the changes in the Unity UI framework in Unity made for 2019.
Either clone / download this repository to your machine and then copy the scripts in, or use the pre-packaged .UnityPackage for your version of Unity and import it as a custom package in to your project.

## Upgrade Notes
Due to the restructure of the package to meet Unity's new package guidelines, we recommend **Deleting the current Unity UI Extensions** folder prior to importing the new package.

For Unity 2019 users using the new UPM deployment, be sure to delete the existing folder in your assets folder before adding the new package to avoid conflict.

---

## Update 2019.1 - formally 2.1  - Going with the times

Given that it's been a while since the last release and a fair few number of fixes have been introduced since the last update, it's only fair I get this point release out for the masses.
This is only a point release and we are still working hard on the next full update

> Be sure to logon to the new Gitter Chat site for the UI Extensions project, if you have any questions, queries or suggestions
> 
> Much easier that posting a question / issue on YouTube, Twitter or Facebook :D
> 
> ## [UIExtensions Gitter Chanel](https://gitter.im/Unity-UI-Extensions/Lobby)

### New / updated features

* Updated and tested with Unity 2018 / 2019
* FancyScrollView updated with newer version (note breaking change)
* Added test version of a LineRender control using a List instead of an array
* New CardUI layout control, for a snazzy flip card system
* New UI Circle Progress indicator control

### Examples / Examples / Examples
(All examples can be deleted without affecting the extensions)

* Added example for CardUI
* Added example for the LineRendererList experiment
* FancyScrollView examples updated to the new version
* Example for new UICircle progress control

### Fixes

* General clean up of build warnings
* Refactored primitive controls to be cleaner
* Various HSS / VSS updates, mostly from the community
* ScrollConflictManager updated to work better with nested HSS/VSS
* UI Knob resolved to with screen space camera
* Fix for the menu system, which was disabling prefabs instead of the scene instance
* Fixed shader in UIParticle System
* TextPic updated to support culling properly
* Reorderable List updated with additional options
* Screenspace overlay support added to the Tooltip control
* UIParticle system now supports 3D rotation
* UIVerticalScroller updated
* Radial slider updated with

### Known issues
No new issues in this release, but check the issues list for things we are currently working on:

* [UI Extensions Issue log](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/issues?status=new&status=open)

## Upgrade Notes
No significant concerns, should be able to update over the 2.1 package.  If upgrading prior to 2.1, we still recommend removing the UnityUIExtensions folder and then re-importing

----------------

## Update 2.0 - The update so big they had to name it twice
[![View 2.0 update Video](http://img.youtube.com/vi/Ivzt9_jhGfQ/0.jpg)](https://www.youtube.com/watch?v=Ivzt9_jhGfQ "Update 2.0 for the Unity UI Extensions Project")

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
## Update 1.2
[![View 1.2 update Video](http://img.youtube.com/vi/cWv0A6rEEc8/0.jpg)](https://www.youtube.com/watch?v=cWv0A6rEEc8 "Update 1.2 for the Unity UI Extensions Project")

### New / updated features
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

### Fixes
* H&V Scroll Snap indexing issues
* H&V Scroll Snap performance updates 
* H&V Scroll Snap Long swipe behavior updated
* H&V Scroll Snap support for Rect Resizing
* TextPic Set set before draw issues
* HSV picker replaced with more generic color picker

### Known issues
* The Image_Extended control has been removed due to Unity upgrade issues. Will return in a future update.

## Upgrade Notes
Although not specifically required, it is recommended to remove the old UI Extensions folder before importing the new asset
The HSS picker especially had a lot of file changes in this update.

>**Note** In Unity 5.5 the particle system was overhauled and several methods were marked for removal. However, the UI Particle System script currently still uses them
> Either ignore these errors or remove the *_UIParticleSystem_* script in the "*Unity UI Extensions / Scripts / Effects*" folder

----------------
## Update 1.1

[![View 1.1 update Video](http://img.youtube.com/vi/JuE0ja5DmV4/0.jpg)](https://www.youtube.com/watch?v=JuE0ja5DmV4 "Update 1.1 for the Unity UI Extensions Project")
> **Note** for 4.6 / 5.1, some features will not be available due to their incompatibility.
> Also the Line Renderer remains unchanged in these releases as the updates do not work with the older system

### New / updated features
* New Polygon primitive
* New UI Vertical Scroller control
* New Curved layout component
* New Shining effect
* New UI Particle system **<-5.3+ only**
* New Scroll Conflict Manager
* Soft Alpha Mask updated in line with SAUI 1.3 release
* Line Renderer has had a complete overhaul, including full programmatic support, Line list and Bezier line rendering
* Horizontal and Vertical Scroll Snaps updated to include a Starting page, current page and transition speed parameters. Plus a new GoToPage, Add and Remove page functions
* Added some script helper functions for LZF compression and Serialization
* Two utilities to help manage drag thresholds on high PPI systems

### Fixes
* Line Render almost completely re-written with tons of fixes
* Radial layout updated to avoid 360 overlap (first and last)
* Scroll Snaps updates to better handle children.
* Scroll Snaps distribute function updated so it can be called onDirty more efficiently.

## Upgrade Notes
Two scripts were moved and need their originals need deleting post upgrade.  Please remove the following files:
* Scripts\ImageExtended
* Scripts\UIImageCrop

----------------
## Update 1.0.6.1

- Minor update to enhance soft alpha mask and add cylinder text plus a fix to letter spacing 

----------------
## Update 1.0.6

[![View 1.0.6 update Video](http://img.youtube.com/vi/jpyFiRvSmbg/0.jpg)](http://www.youtube.com/watch?v=jpyFiRvSmbg "Update 1.0.6 for the Unity UI Extensions Project")

* Added the awesome ReOrderable List control, plus some other minor bugfixes / changes.
* Added a new version of the Scroll Snap control as an alternative to the fixed versions.
* New set of controls including some shader enhanced solutions
* I've added a donate column to the lists.  If you are getting great use out of a control, help out the dev who created it. Optional of course.  Will update with links as I get them.

----------------
## Update 1.0.5

Few minor fixes and a couple of additional scripts.  Predominately created the new 5.3 branch to maintain the UI API changes from the 5.2.1 Patch releases.  5.3 package is 100% compatible with 5.2.1 Patch releases.

----------------
## Update 1.0.4

[![View Getting Started Video](http://img.youtube.com/vi/oF48Qpaq3ls/0.jpg)](http://www.youtube.com/watch?v=oF48Qpaq3ls "Update 1.0.0.4 for the Unity UI Extensions Project")
---

=======================
# Additional Info
=======================
### How do I get set up?
Either clone / download this repository to your machine and then copy the scripts in, or use the pre-packaged .UnityPackage for your version of Unity and import it as a custom package in to your project.

### Contribution guidelines
Got a script you want added, then just fork and submit a PR.  All contributions accepted (including fixes)
Just ensure 
* The header of the script matches the standard used in all scripts
* The script uses the **Unity.UI.Extensions** namespace so they do not affect any other developments
* (optional) Add Component and Editor options where possible (editor options are in the Editor\UIExtensionsMenuOptions.cs file)

### License
All scripts conform to the BSD license and are free to use / distribute.  See the [LICENSE](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/src/6d03f25b0150994afa97c6a55854d6ae696cad13/LICENSE?at=default) file for more information 

### Like what you see?
All these scripts were put together for my latest book Unity3D UI Essentials
Check out the [page on my blog](http://bit.ly/Unity3DUIEssentials) for more details and learn all about the inner workings of the new Unity UI System.

### The downloads
As this repo was created to support my new Unity UI Title ["Unity 3D UI Essentials"](http://bit.ly/Unity3DUIEssentials), in the downloads section you will find two custom assets (SpaceShip-DemoScene-Start.unitypackage and RollABallSample-Start.unitypackage).  These are just here as starter scenes for doing UI tasks in the book.

I will add more sample scenes for the UI examples in this repository and detail them above over time.