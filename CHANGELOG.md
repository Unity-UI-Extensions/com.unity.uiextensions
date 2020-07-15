# Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/).

## 2019.4 (v2.2) - 2020-07-09

Here we write upgrading notes for brands. It's a team effort to make them as
straightforward as possible.

### Added

- New UPM deployment for Unity 2019, 2018 will still need to use the asset packages due to Unity compatibility issues.
- Updated the project to the new Unity packaging guidelines, including separating out the examples to a separate package.
- Many line drawing updates, including the ability to draw using a mouse (check the examples)
- New Unity Card UI controls thanks to @RyanslikeSoCool

### Changed

- Scroll Snaps (HSS/VSS) now have a "Hard Swipe" feature to restrict movement to a single page for each swipe
- Scroll Snaps have also been updated to work better with the UIInfiniteScroll control
- Update to the Fancy Scroll controls with even more added fanciness
- Several updates to adopt newer Unity standards in the controls to ensure full forwards and backwards compatibility  

### Deprecated

None

### Fixed

- Mouse position use updated in
    * RadialSlider
    * ColorSampler
    * TiltWindow
- Check compiler warnings (#197)
- Line Renderer click to add lines (#183)
- ScrollSnap Swiping options - hard fast swipe (#176)
- Shader Loading issue / UIParticleSystem (#229)
- Issue where Menu Prefabs would be disabled instead of their Clones (#210)
- Check ScrollSnapBase update (#265)
- UIInfiniteScroller support for VSS updated and fixes
- Fix to allow radial slider to start from positions other than left
- Fix UI Particles: Texture sheet animation + Random row(#256)
- Fix for wandering ScrollSnap controls due to Local Positioning drift
- Divide By Zero fix for Gradient (#58)

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
