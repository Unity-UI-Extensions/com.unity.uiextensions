# Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/).

## Release 2.3 - Reanimation - 2022/02/07

It has been a tough time for all since the last update, but things have been moving steadily along.  In the past few months there has been a concerted effort to revamp and update the project ready for Unity 2022, as well as migrating the source repository over to GitHub and refreshing all the things.
We hope the new release is better for everyone and we have paid close attention to the editor menus and places to find all the controls for this release.

To get up to speed with the Unity UI Extensions, check out the [Getting Started](https://unity-ui-extensions.github.io/GettingStarted.html) Page.

> Ways to get in touch:
>
> - [Gitter Chat](https://gitter.im/Unity-UI-Extensions/Lobby) site for the UI Extensions project
> - [GitHub Discussions](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/discussions), if you have any questions, queries or suggestions
>
> Much easier that posting a question / issue on YouTube, Twitter or Facebook :D

## Breaking changes

For customers upgrading from earlier versions of Unity to Unity 2020, please be aware of the Breaking change related to Text Based components.  You will need to manually replace any UI using the older ```Text``` component and replace them with ```TextMeshPro``` versions. This is unavoidable due to Unity deprecating the Text component.

> New users to 2022 are unaffected as all the Editor commands have been updated to use the newer TextMeshPro versions.

For more details, see the [deprecation notice](https://github.com/Unity-UI-Extensions/com.unity.uiextensions/discussions/428) on GitHub.

## Added

- Added new FIFO based UI Line Render when dynamic line rendering is needed.
- Added ResetSelectableHighlight component.
- Added SetArc method to UICircle as requested.
- Added new UIHorizontalScroller based on UIVerticalScroller.
- Added OnHighlightChanged and OnPressChanged events for UI Button.
- Added error handling around setting Unity UI Components for Vertical/Horizontal ScrollSnaps.
- Added Editor Menu Option to create a Min/Max slider.
- Added the ability to set a specific item for combobox controls on start and not just the first.
- Added the ability to disable the combo boxes and make them read-only.

## Changed

- Refresh FancyScrollView with the latest fixes
- All Text based components updated to use TextMeshPro from Unity 2022 **Breaking Change**

- Clean-up and reset pivots on scene start.
- Merged in feature/improved-ui-highlightable (pull request UILineRenderer - issues with specifying point locations at runtime #123).
- Merged in fix/rangesliderfix (pull request HorizontalScrollSnap Mask Area doesn't work when content created dynamically #125).
- Merged in fix/infinitescrollcontentsize (pull request Gradient initialization should be in Awake() #126).
- Merged in feature/controlTouchUp (pull request UILineRenderer mesh not updating in Editor scene view #127).
- Upgraded RangeSlider to work in both Horizontal and Vertical setups.
- Merged in RangeSlider-upgrade. (pull request Newtonsoft.Json.dll conflict #131)
- Updated UIVertical scroller to be 2022 compliant.
- Updated Curly UI to wait until end of the frame to recalculate positions.
- Updated Depth Texture sampler in UI Particles Shaders.
- Updated Points to always be an array of 1 when set to nothing for the Line Renderer.
- Updated Cooldown button to work with Keyboard input.
- Removed unneeded size calculation which caused some issues with mixed content.
- Resolved an issue whereby the last row in a flow layout group would not size correctly.
- Updated all components using "LayoutGroup" to override their OnDisable.
- Updated validation in the new MinMaxSlider.
- Updated Editor create options to add the correct Event System Input manager.
- Updated initialisation logic to not cause an endless loop in the TabNavigationHelper.
- Updated "Action" use to "UnityAction" to avoid Unity issues for DropDowns.
- Updated UIVerticalScroller for standards.
- Updated ReorderableList/ReorderableListElement to prevent creating a fake object for non-transferable items.
- Updated panel drawing for ComboBox controls and added DropdownOffset.
- Updated build issue with ReorderableListElement.
- Updated NonDrawingGraphic to require a CanvasRender, else it causes an error on run.

## Deprecated

- Marked ScrollPositionController as Obsolete, users should use the new Scoller.
- BestFitOutline - Deprecated in Unity 2020 onwards. (still available for earlier versions)
- NicerOutline - Deprecated in Unity 2020 onwards. (still available for earlier versions)
- Marked TileSizeFitter as obsolete as Unity has made this unworkable.

## Fixed

- Resolved issues with DisplayAbove and using a 0 ItemsToDisplay for ComboBox controls.
- Resolved startup issue that prevented the control from being used (Unity changed the start order in some instances), this was causing null reference issues with comboboxes.
- Patch fix for UILineRenderer.
- Resolves issue where the lower range value would become stuck when moving.
- Updated Infinite scroll to work with content of different sizes.
- Updated Dropdown list to NOT resize text Rect on draw.
- Revised the Curly UI fix as it was preventing the graphic from being updated in the scene view.
- Cleanup and ensuring the UIParticleSystem is disposed in onDestroy correctly.
- Clean up range slider unused variables.

## Additional Notes

### [Installation Instructions](https://unity-ui-extensions.github.io/UPMInstallation.html)

The recommended way to add the Unity UI Extensions project to your solution is to use the Unity package Manager. Simply use the Unity Package Manager to reference the project to install it

New for 2020, we have added OpenUPM support and the package can be installed using the following [OpenUPM CLI](https://openupm.com/docs/) command:

```cli
`openupm add com.unity.uiextensions`
```
> For more details on using [OpenUPM CLI, check the docs here](https://github.com/openupm/openupm-cli#installation).

- Unity Package Manager - manual

Alternatively, you can also add the package manually through the Unity package manager using the scope ```com.unity.uiextensions```, see the [Unity Package Manager docs](https://learn.unity.com/tutorial/the-package-manager) for more information.

- Unity 2018 or lower
The pre-compiled Unity assets are the only solution for Unity 2018 or earlier due to the changes in the Unity UI framework in Unity made for 2019.
Either clone / download this repository to your machine and then copy the scripts in, or use the pre-packaged .UnityPackage for your version of Unity and import it as a custom package in to your project.

### Upgrade Notes

### UPM

If you are using UPM to gain access to the Unity UI Extensions, then you only need to update to the latest version in the Package Manager, no other changes needed.

### Customers using the .UnityPackage

Due to the restructure of the package to meet Unity's new package guidelines, we recommend **Deleting the current Unity UI Extensions** folder prior to importing the new package.

For Unity 2019 users using the new UPM deployment, be sure to delete the existing folder in your assets folder before adding the new package to avoid conflict.
