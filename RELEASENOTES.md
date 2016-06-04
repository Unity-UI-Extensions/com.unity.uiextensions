# Unity UI Extensions release notes #
This file contains the up to date release notes for each release of the UI Extensions project including release videos where required.

----------------
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



----------------
##Update 1.0.6.1##

- Minor update to enhance soft alpha mask and add cylinder text plus a fix to letter spacing 

----------------
##Update 1.0.6##

[![View 1.0.6 update Video](http://img.youtube.com/vi/jpyFiRvSmbg/0.jpg)](http://www.youtube.com/watch?v=jpyFiRvSmbg "Update 1.0.6 for the Unity UI Extensions Project")

* Added the awesome ReOrderable List control, plus some other minor bugfixes / changes.
* Added a new version of the Scroll Snap control as an alternative to the fixed versions.
* New set of controls including some shader enhanced solutions
* I've added a donate column to the lists.  If you are getting great use out of a control, help out the dev who created it. Optional of course.  Will update with links as I get them.

----------------
##Update 1.0.5##

Few minor fixes and a couple of additional scripts.  Predominately created the new 5.3 branch to maintain the UI API changes from the 5.2.1 Patch releases.  5.3 package is 100% compatible with 5.2.1 Patch releases.

----------------
##Update 1.0.4##

[![View Getting Started Video](http://img.youtube.com/vi/oF48Qpaq3ls/0.jpg)](http://www.youtube.com/watch?v=oF48Qpaq3ls "Update 1.0.0.4 for the Unity UI Extensions Project")
---



=======================
#Additional Info#
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