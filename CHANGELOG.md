# Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/).

## 2019.6 - 2.5 - Bug squash - 2021/05/10

Its been a while since the last update and although Unity keeps changing, thankfully the parts underneath do not.  THanks to some awesome work by our contributors and the test teams, we made a run on some underlying bugs and issues.  If you spot anything else, please log it on the BitBucket site for resolution.

> Be sure to logon to the new [Gitter Chat](https://gitter.im/Unity-UI-Extensions/Lobby) site for the UI Extensions project, if you have any questions, queries or suggestions
>
> Much easier that posting a question / issue on YouTube, Twitter or Facebook :D
>
> ## [UIExtensions Gitter Channel](https://gitter.im/Unity-UI-Extensions/Lobby)


### Added

Nothing new this time, bugfix release.

### Changed

- Updated UI Line connector to use relative position instead of anchored position to verify if the Lines need updating.
- Allow menu prefabs to not have to have canvas components. This allows you to use any type of prefab as a "menu". Adam Kapos mentions the concept on the Unite talk, https://youtu.be/wbmjturGbAQ?t=1654
- Updated segment line drawing for Line Lists. Seems Unity no longer needs UV's to be wrapped manually.
- Updated the AutoCompleteComboBox to display text as entered (instead of all lowercase)
- Updated the ComboBox to display text as entered (instead of all lowercase)
- Updated ComboBox Examples to include programmatic versions
- Further ComboBox improvements including:
  * Upwards panel
  * Start fixes
  * Item Template resize
  * Disabled sorting on combobox as it wasn't working
  * Disabled Slider handle when not in use
  * Updated Example
- Updated the new Input system switch and tested against 2021

### Deprecated

None

### Fixed

- Reordering issue resolved with ScrollRectOcclusion.
- Fixed Sorting at min and max positions for ScrollRect
- Updated ScrollToSelect script provided by zero3growlithe, tested and vastly reduces the previous jitter. Still present but barely noticeable now.
- Fixed Issue # 363 Update Combobox control that takes multiple items programmatically, to only allow distinct items
- Fixed the issues where dragging outside the range slider handle causes the range to update. - Resolves #369
- Resolves an issue with Unity putting the previous controls vertex array in an uninitialised control.
- Applied J.R. Mitchell's fix for the Accordion Controls/Accordion/AccordionElement.cs - resolves: #364
- Resolved issue where the Content Scroll snap issue with only 1 child. Resolves #362
- Updated the PaginationManager to override if the ScrollSnap is in motion.

### Removed

None

### Additional Notes

#### [Installation Instructions](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/GettingStarted)

As of Unity 2019, there are now two paths for getting access to the Unity UI Extensions project:

- Unity 2019 or higher
The recommended way to add the Unity UI Extensions project to your solution is to use the Unity package Manager. Simply use the Unity Package Manager to reference the project to install it

Alternatively, you can also use the pre-compiled Unity packages if you wish, however, UPM offers full versioning support to allow you to switch versions as you wish.

- Unity 2018 or lower
The pre-compiled Unity assets are the only solution for Unity 2018 or earlier due to the changes in the Unity UI framework in Unity made for 2019.
Either clone / download this repository to your machine and then copy the scripts in, or use the pre-packaged .UnityPackage for your version of Unity and import it as a custom package in to your project.

#### Upgrade Notes

Due to the restructure of the package to meet Unity's new package guidelines, we recommend **Deleting the current Unity UI Extensions** folder prior to importing the new package.

For Unity 2019 users using the new UPM deployment, be sure to delete the existing folder in your assets folder before adding the new package to avoid conflict.
