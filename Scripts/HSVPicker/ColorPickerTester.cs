///Credit judah4
///Sourced from - http://forum.unity3d.com/threads/color-picker.267043/

namespace UnityEngine.UI.Extensions
{
    public class ColorPickerTester : MonoBehaviour
    {
        public Renderer renderer;
        public HSVPicker picker;

        // Use this for initialization
        void Start()
        {
            picker.onValueChanged.AddListener(color =>
            {
                renderer.material.color = color;
            });
        }
    }
}