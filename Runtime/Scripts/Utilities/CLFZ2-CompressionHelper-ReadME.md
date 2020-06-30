Sourced from Unity forums - [LZF compression and decompression for Unity](http://forum.unity3d.com/threads/lzf-compression-and-decompression-for-unity.152579/)

#Description#
The LZF Compression component is a useful extension to Unity to be able to compress data for sending over a network or for more efficient saving.

This was provided via Unity forum guru [mrbroshkin](http://forum.unity3d.com/members/mrbroshkin.124231/) based of [Agent_007](http://forum.unity3d.com/members/agent_007.78088/) initial conversion work.

#What's included#
The library is complete and has even been patched a few times already through the forum thread, so I took the most stable version without issues. (issus were reported on other versions, so I didn't take them)

There are some Unit Tests included in the following file:
>Assets\unity-ui-extensions\Scripts\Editor\CompressionTests.cs

These details some sample use cases and also provide a good way to test that the compression/decompression works as expected.
To run these Unit Tests In Unity 5.3, open the following editor window from the menu to see the tests and run them.
>Window\Editor Tests Runner 

(For earlier versions of Unity, you will need to install the "Unity Test Tools" asset from the store and follow the instructions from there.)

#Contribute#
Feel free to submit patches (if required) back to the project to increase the scope and enjoy

#Notes#
When encoding data to byte array's for compression, it's best to use the Unicode character set to avoid issue.  Some other encodings don't work as well with this lib.  Example details found in the UnitTests.
Part of the reason I added this library, since it's not really UI, is to support the Serialisation component.  Both together provide a useful solution for packing data to save or send over a network.