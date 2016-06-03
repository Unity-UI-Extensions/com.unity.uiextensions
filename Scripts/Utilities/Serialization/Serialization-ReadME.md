Sourced from Unity forums - [serializehelper-free-save-and-load-utility-de-serialize-all-objects-in-your-scene](http://forum.unity3d.com/threads/serializehelper-free-save-and-load-utility-de-serialize-all-objects-in-your-scene.338148/)

#Description#
The serializationHelper is a useful extension to Unity to be able to serialise the base Unity structs (such as Vector3 / Quanterion) without having to employ custom classes or convertors.
It uses the standard ISerializationSurrogates to provide a serialisation interface for the base classes themselves

This starter pack was provided via Unity forum guru [Cherno](http://forum.unity3d.com/members/cherno.245586/)

#What's included#
Not all classes have had surrogates provided for as yet, here is what is working:
* Color
* Quaternion
* Vector2
* Vector3
* Vector4

There are some initial templates for other items but they do not seem fully implemented yet:
* GameObject
* Texture2D
* Transform
Feel free to re-enable them by uncommenting their code (in the Surrogates folder) and test.  Have a look at the working examples for reference.

#Contribute#
Feel free to get other surrogates working and submit them back to the project to increase the scope and enjoy

#Notes#
Part of the reason I added this library, since it's not really UI, is to support the LZF compression component.  Both together provide a useful solution for packing data to save or send over a network.