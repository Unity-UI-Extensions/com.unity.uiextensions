///Credit judah4
///Sourced from - http://forum.unity3d.com/threads/color-picker.267043/

namespace UnityEngine.UI.Extensions.ColorPicker
{
    /// <summary>
    /// Displays one of the color values of aColorPicker
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class ColorSlider : MonoBehaviour
    {
        public ColorPickerControl ColorPicker;

        /// <summary>
        /// Which value this slider can edit.
        /// </summary>
        public ColorValues type;

        private Slider slider;

        private bool listen = true;

        private void Awake()
        {
            slider = GetComponent<Slider>();

            ColorPicker.onValueChanged.AddListener(ColorChanged);
            ColorPicker.onHSVChanged.AddListener(HSVChanged);
            slider.onValueChanged.AddListener(SliderChanged);
        }

        private void OnDestroy()
        {
            ColorPicker.onValueChanged.RemoveListener(ColorChanged);
            ColorPicker.onHSVChanged.RemoveListener(HSVChanged);
            slider.onValueChanged.RemoveListener(SliderChanged);
        }

        private void ColorChanged(Color newColor)
        {
            listen = false;
            switch (type)
            {
                case ColorValues.R:
                    slider.normalizedValue = newColor.r;
                    break;
                case ColorValues.G:
                    slider.normalizedValue = newColor.g;
                    break;
                case ColorValues.B:
                    slider.normalizedValue = newColor.b;
                    break;
                case ColorValues.A:
                    slider.normalizedValue = newColor.a;
                    break;
                default:
                    break;
            }
        }

        private void HSVChanged(float hue, float saturation, float value)
        {
            listen = false;
            switch (type)
            {
                case ColorValues.Hue:
                    slider.normalizedValue = hue; //1 - hue;
                    break;
                case ColorValues.Saturation:
                    slider.normalizedValue = saturation;
                    break;
                case ColorValues.Value:
                    slider.normalizedValue = value;
                    break;
                default:
                    break;
            }
        }

        private void SliderChanged(float newValue)
        {
            if (listen)
            {
                newValue = slider.normalizedValue;
                //if (type == ColorValues.Hue)
                //    newValue = 1 - newValue;

                ColorPicker.AssignColor(type, newValue);
            }
            listen = true;
        }
    }
}