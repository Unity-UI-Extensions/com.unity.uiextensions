using System.Runtime.Serialization;

namespace UnityEngine.UI.Extensions
{
    public sealed class Vector2Surrogate : ISerializationSurrogate
    {

        // Method called to serialize a Vector2 object
        public void GetObjectData(System.Object obj,
                                  SerializationInfo info, StreamingContext context)
        {
            Vector2 v2 = (Vector2)obj;
            info.AddValue("x", v2.x);
            info.AddValue("y", v2.y);
        }

        // Method called to deserialize a Vector2 object
        public System.Object SetObjectData(System.Object obj,
                                           SerializationInfo info, StreamingContext context,
                                           ISurrogateSelector selector)
        {
            Vector2 v2 = (Vector2)obj;
            v2.x = (float)info.GetValue("x", typeof(float));
            v2.y = (float)info.GetValue("y", typeof(float));
            obj = v2;
            return obj;
        }
    }
}