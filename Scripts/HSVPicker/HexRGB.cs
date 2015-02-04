///Credit judah4
///Sourced from - http://forum.unity3d.com/threads/color-picker.267043/

using System.Globalization;

namespace UnityEngine.UI.Extensions
{
    public class HexRGB : MonoBehaviour
    {
        public Text textColor;

        public HSVPicker hsvpicker;

        public void ManipulateViaRGB2Hex()
        {
            Color color = hsvpicker.currentColor;
            string hex = ColorToHex(color);
            textColor.text = hex;
        }

        public static string ColorToHex(Color color)
        {
            int r = (int)(color.r * 255);
            int g = (int)(color.g * 255);
            int b = (int)(color.b * 255);
            return string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
        }

        public void ManipulateViaHex2RGB()
        {
            string hex = textColor.text;

            Vector3 rgb = Hex2RGB(hex);
            Color color = NormalizeVector4(rgb, 255f, 1f); print(rgb);

            hsvpicker.AssignColor(color);
        }

        static Color NormalizeVector4(Vector3 v, float r, float a)
        {
            float red = v.x / r;
            float green = v.y / r;
            float blue = v.z / r;
            return new Color(red, green, blue, a);
        }

        Vector3 Hex2RGB(string hexColor)
        {
            //Remove # if present
            if (hexColor.IndexOf('#') != -1)
                hexColor = hexColor.Replace("#", "");

            int red = 0;
            int green = 0;
            int blue = 0;

            if (hexColor.Length == 6)
            {
                //#RRGGBB
                red = int.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                green = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
                blue = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);


            }
            else if (hexColor.Length == 3)
            {
                //#RGB
                red = int.Parse(hexColor[0].ToString() + hexColor[0].ToString(), NumberStyles.AllowHexSpecifier);
                green = int.Parse(hexColor[1].ToString() + hexColor[1].ToString(), NumberStyles.AllowHexSpecifier);
                blue = int.Parse(hexColor[2].ToString() + hexColor[2].ToString(), NumberStyles.AllowHexSpecifier);
            }

            return new Vector3(red, green, blue);

        }

    }
}