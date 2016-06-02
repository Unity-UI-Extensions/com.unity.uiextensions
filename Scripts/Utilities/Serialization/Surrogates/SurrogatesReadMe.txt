ISerializationSurrogates are used to tell a BinaryFormatter how to serialize and deserialize certain types. 
For example, a BinaryFormatter can't (de)serialize a Vector3 instance. 
This is true for most, or even all, Unity-specific types like Transform, GameObject, Color, Mesh, and so on.
What a ISerializationSurrogate does is tell the Formatter, if it is used, how to serialize and deserialize each field of a type.
A Vector3 is just made of three floats: x, y and z. These can easily be serialized one by one.
An ISS can't be written for the type GameObject that easily, as it is vastly more complex. The Formatter will throw an error when trying to serialize it.
However, we are using a workaround for saving & loading GOs anyway (by storing their positions, rotations, scales, active states, and components one by one),
so we can use a ISS for type GameObject that uses none of a GO's fields. Nothing will be serialized, but we won't get an error either, which is the point of the empty ISS.
Note that we could use the attribute [DontSerializeField] to make our algorithm skip a field of type GameObject, 
but this will only work if the GO field sits in the class that is to be serialized itself. If we want to save a MonoBehavior script which contains a field of type (CustomClass),
which in turn contains a field of type GameObject, then this will not work as we can only go through each field of a component, not through each field of each field, so to speak.

