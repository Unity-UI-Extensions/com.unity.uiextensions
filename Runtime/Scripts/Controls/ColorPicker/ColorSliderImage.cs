///Credit judah4
///Sourced from - http://forum.unity3d.com/threads/color-picker.267043/

namespace UnityEngine.UI.Extensions.ColorPicker
{
    [RequireComponent(typeof(RawImage)), ExecuteInEditMode()]
    public class ColorSliderImage : MonoBehaviour
    {
        public ColorPickerControl picker;

        /// <summary>
        /// Which value this slider can edit.
        /// </summary>
        public ColorValues type;

        public Slider.Direction direction;

        private RawImage image;

        private RectTransform RectTransform
        {
            get
            {
                return transform as RectTransform;
            }
        }

        private void Awake()
        {
            image = GetComponent<RawImage>();
            if (image)
            {
                RegenerateTexture();
            }
            else
            {
                Debug.LogWarning("Missing RawImage on object [" + name + "]");
            }
        }

        private void OnEnable()
        {
            if (picker != null && Application.isPlaying)
            {
                picker.onValueChanged.AddListener(ColorChanged);
                picker.onHSVChanged.AddListener(ColorChanged);
            }
        }

        private void OnDisable()
        {
            if (picker != null)
            {
                picker.onValueChanged.RemoveListener(ColorChanged);
                picker.onHSVChanged.RemoveListener(ColorChanged);
            }
        }

        private void OnDestroy()
        {
            if (image.texture != null)
                DestroyImmediate(image.texture);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            image = GetComponent<RawImage>();
            if (image)
            {
                RegenerateTexture();
            }
            else
            {
                Debug.LogWarning("Missing RawImage on object [" + name + "]");
            }
        }
#endif

        private void ColorChanged(Color newColor)
        {
            switch (type)
            {
                case ColorValues.R:
                case ColorValues.G:
                case ColorValues.B:
                case ColorValues.Saturation:
                case ColorValues.Value:
                    RegenerateTexture();
                    break;
                case ColorValues.A:
                case ColorValues.Hue:
                default:
                    break;
            }
        }

        private void ColorChanged(float hue, float saturation, float value)
        {
            switch (type)
            {
                case ColorValues.R:
                case ColorValues.G:
                case ColorValues.B:
                case ColorValues.Saturation:
                case ColorValues.Value:
                    RegenerateTexture();
                    break;
                case ColorValues.A:
                case ColorValues.Hue:
                default:
                    break;
            }
        }

        private void RegenerateTexture()
        {
            if (!picker)
            {
                Debug.LogWarning("Missing Picker on object [" + name + "]");
            }
            Color32 baseColor = picker != null ? picker.CurrentColor : Color.black;

            float h = picker != null ? picker.H : 0;
            float s = picker != null ? picker.S : 0;
            float v = picker != null ? picker.V : 0;

            Texture2D texture;
            Color32[] colors;

            bool vertical = direction == Slider.Direction.BottomToTop || direction == Slider.Direction.TopToBottom;
            bool inverted = direction == Slider.Direction.TopToBottom || direction == Slider.Direction.RightToLeft;

            int size;
            switch (type)
            {
                case ColorValues.R:
                case ColorValues.G:
                case ColorValues.B:
                case ColorValues.A:
                    size = 255;
                    break;
                case ColorValues.Hue:
                    size = 360;
                    break;
                case ColorValues.Saturation:
                case ColorValues.Value:
                    size = 100;
                    break;
                default:
                    throw new System.NotImplementedException("");
            }
            if (vertical)
                texture = new Texture2D(1, size);
            else
                texture = new Texture2D(size, 1);

            texture.hideFlags = HideFlags.DontSave;
            colors = new Color32[size];

            switch (type)
            {
                case ColorValues.R:
                    for (byte i = 0; i < size; i++)
                    {
                        colors[inverted ? size - 1 - i : i] = new Color32(i, baseColor.g, baseColor.b, 255);
                    }
                    break;
                case ColorValues.G:
                    for (byte i = 0; i < size; i++)
                    {
                        colors[inverted ? size - 1 - i : i] = new Color32(baseColor.r, i, baseColor.b, 255);
                    }
                    break;
                case ColorValues.B:
                    for (byte i = 0; i < size; i++)
                    {
                        colors[inverted ? size - 1 - i : i] = new Color32(baseColor.r, baseColor.g, i, 255);
                    }
                    break;
                case ColorValues.A:
                    for (byte i = 0; i < size; i++)
                    {
                        colors[inverted ? size - 1 - i : i] = new Color32(i, i, i, 255);
                    }
                    break;
                case ColorValues.Hue:
                    for (int i = 0; i < size; i++)
                    {
                        colors[inverted ? size - 1 - i : i] = HSVUtil.ConvertHsvToRgb(i, 1, 1, 1);
                    }
                    break;
                case ColorValues.Saturation:
                    for (int i = 0; i < size; i++)
                    {
                        colors[inverted ? size - 1 - i : i] = HSVUtil.ConvertHsvToRgb(h * 360, (float)i / size, v, 1);
                    }
                    break;
                case ColorValues.Value:
                    for (int i = 0; i < size; i++)
                    {
                        colors[inverted ? size - 1 - i : i] = HSVUtil.ConvertHsvToRgb(h * 360, s, (float)i / size, 1);
                    }
                    break;
                default:
                    throw new System.NotImplementedException("");
            }
            texture.SetPixels32(colors);
            texture.Apply();

            if (image.texture != null)
                DestroyImmediate(image.texture);
            image.texture = texture;

            switch (direction)
            {
                case Slider.Direction.BottomToTop:
                case Slider.Direction.TopToBottom:
                    image.uvRect = new Rect(0, 0, 2, 1);
                    break;
                case Slider.Direction.LeftToRight:
                case Slider.Direction.RightToLeft:
                    image.uvRect = new Rect(0, 0, 1, 2);
                    break;
                default:
                    break;
            }
        }

    }
}