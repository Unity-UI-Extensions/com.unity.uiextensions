<!-- Offline documentation -->

# About Unity UI Extensions

The Unity UI Extensions project is a collection of extension scripts/effects and controls to enhance your Unity UI experience. This includes over 70+ controls, utilities, effects and some much-needed love to make the most out of the Unity UI system (formally uGUI) in Unity.
[Check out our Tumblr page for a sneak peek](https://www.tumblr.com/blog/unityuiextensions)

> Contact the UI Extensions Team
> Be sure to logon to the new [Gitter Chat site](https://gitter.im/Unity-UI-Extensions/Lobby) for the UI Extensions project, if you have any questions, queries or suggestions
> Much easier than posting a question / issue on [YouTube](http://www.youtube.com/c/UnityUIExtensions), [Twitter](https://twitter.com/hashtag/UnityUIExtensions) or [Facebook](https://www.facebook.com/UnityUIExtensions) :D
> 
> [**UIExtensions Gitter Chanel**](https://gitter.im/Unity-UI-Extensions/Lobby)

# Installing Unity UI Extensions

To install this package, follow the instructions in the Package Manager documentation.

For more details on [Getting Started](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/GettingStarted) please checkout the [online documentation here](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/GettingStarted).

# Using Unity UI Extensions

The UI Extensions project provides many automated functions to add the various controls contained within the project commonly accessed via "***GameObject -> UI -> Extensions -> 'Control'***" from the editor menu.  This will add the UI object and all the necessary components to make that control work in the scene in a default state.

Some of the features are also available through the GameObject "Add Component" menu in the inspector.

For a full list of the controls and how they are used, please see the [online documentation](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Controls) for the project.

# Technical details

## Requirements

This version of the Unity UI Extensions is compatible with the following versions of the Unity Editor:

* 2019 and above - the recommended path for 2019+ is to use the Unity Package Manager to get access to the package.  Full details for installing via UPM can be [found here](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/UPMInstallation).

> Alternatively, the Asset packages have been tested to work with 2019 as well if you prefer to install that way.

* 2018 and below - for 2018 and use this package, you will have to import the asset package(s), either from the Asset Store or from the alternate download locations [listed here](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Downloads).

## [Release Notes](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/ReleaseNotes/RELEASENOTES)

### 2019.4 - 2.2  - Back from the future

* New UPM deployment for Unity 2019, 2018 will still need to use the asset packages due to Unity compatibility issues.
* Updated the project to the new Unity packaging guidelines, including separating out the examples to a separate package.
* Many line drawing updates, including the ability to draw using a mouse (check the examples)
* Scroll Snaps (HSS/VSS) now have a "Hard Swipe" feature to restrict movement to a single page for each swipe
* Scroll Snaps have also been updated to work better with the UIInfiniteScroll control
* New Unity Card UI controls thanks to @RyanslikeSoCool
* Update to the Fancy Scroll controls with even more added fanciness
* Several updates to adopt newer Unity standards in the controls to ensure full forwards and backwards compatibility

# Document revision history

|Date|Details|
|-|-|
|July 9th, 2020|2019.4 (v2.2) released, first UPM deployment live |
|September 3rd, 2019|2019.1 (v2.1) released, First major update for the 2.0 series.|