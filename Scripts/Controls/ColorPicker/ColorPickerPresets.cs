///Credit judah4
///Sourced from - http://forum.unity3d.com/threads/color-picker.267043/


namespace UnityEngine.UI.Extensions.ColorPicker
{
    public class ColorPickerPresets : MonoBehaviour
    {
        public ColorPickerControl picker;
        public GameObject[] presets;
        public Image createPresetImage;

        void Awake()
        {
            //		picker.onHSVChanged.AddListener(HSVChanged);
            picker.onValueChanged.AddListener(ColorChanged);
        }

        public void CreatePresetButton()
        {
            for (var i = 0; i < presets.Length; i++)
            {
                if (!presets[i].activeSelf)
                {
                    presets[i].SetActive(true);
                    presets[i].GetComponent<Image>().color = picker.CurrentColor;
                    break;
                }
            }
        }

        public void PresetSelect(Image sender)
        {
            picker.CurrentColor = sender.color;
        }

        // Not working, it seems ConvertHsvToRgb() is broken. It doesn't work when fed
        // input h, s, v as shown below.
        //	private void HSVChanged(float h, float s, float v)
        //	{
        //		createPresetImage.color = HSVUtil.ConvertHsvToRgb(h, s, v, 1);
        //	}
        private void ColorChanged(Color color)
        {
            createPresetImage.color = color;
        }
    }
}