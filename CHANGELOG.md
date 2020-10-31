# Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/).

## 2019.5 (v2.3) - 2020-10-31

Since the move to UPM, the team have been able to react quicker and push out fixes a lot easier, without affecting previous installation (whilst still adhering to Unity's backwards compatibility pattern).  So it is with great news we announce this new release, faster that ever :D  (and thanks to UPM, easier to upgrade than ever).
Be sure to also check out the "Examples" option in the Package Manager window to import the samples to your project.

### Added

- Add squircle primitive
- Adding new magnetic scroll control
- Added a static library to collate shaders on first use.
- Finalized new InputManagerHelper, which translates input based on the operating input system, new or old Updated CardStack2D to have defined keyboard input or specific gamepad input over the older axisname for new input system.
- Updated DropDown and Autocomplete controls based on feedback in #204

### Changed

- Examples now included with UPM delivery and available as a button on the UPM package manager window
- Updated DropDown and Autocomplete controls based on feedback in #204
- Updated Accordion to support both Vertical as well as Horizontal layout
- Updated ComboBox controls to improve better programmatic controls
- Updates to the Infinite scroll to support content of various sizes
- Updated UI Knob control - enabled dragging outside the target area, added example scene
- Minor update to MagneticInfinite Scroll
- Refactored and extended the ContentScrollSnap control
- Added protection against errors and empty scrollrect content
- Added new SetNewItems function to add children programmatically to the control and reset accordingly
- Patch supplied by a contributor to improve the texture sheet use with the UIParticlesystem
- Added "SetKnobValue" function which allows the setting of Value and loops
- Added the programmatic capability to change the parent scroll rect on the ScrollConflictManager at runtime.

### Deprecated

None

### Fixed

- Fix to add a "RequireComponent" to Primitives as Unity 2020 does not add them by default
- Remove old Examples submodule
- Updated submodules to hide Examples folder Additionally, updated Package manifest to allow importing of examples direct from UPM package.
- Fixed hard swipe to ensure it only ever moves one page, no matter how far you swipe.
- Fixed a conflict when using the ScrollConflictManager in child content of a HSS or VSS
- Fix for UI Particle system looping
- Fixed public GoToScreen call to only raise events internally (not multiple)
- Final roll-up and fix. Resolved race condition for associated pagination controls.
- Fixed issue with page events not being raised when inertia was disabled (velocity was always zero)
- When cloned, reorderable list was creating a second List Content component that was not initialized. Refactored to ensure only one list content was present and is initialized correctly
- Reorderable list items marked as transferable, remain transferable after being dropped
- Patch to resolve issues without the new Input System installed
- Refined magnetic scroll and dependencies while documenting Updated example
- Patch Tooltip

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
