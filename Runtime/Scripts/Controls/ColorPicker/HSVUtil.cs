///Credit judah4
///Sourced from - http://forum.unity3d.com/threads/color-picker.267043/

using System;

namespace UnityEngine.UI.Extensions.ColorPicker
{
    #region ColorUtilities

    public static class HSVUtil
    {
        public static HsvColor ConvertRgbToHsv(Color color)
        {
            return ConvertRgbToHsv((int)(color.r * 255), (int)(color.g * 255), (int)(color.b * 255));
        }

        //Converts an RGB color to an HSV color.
        public static HsvColor ConvertRgbToHsv(double r, double b, double g)
        {
            double delta, min;
            double h = 0, s, v;

            min = Math.Min(Math.Min(r, g), b);
            v = Math.Max(Math.Max(r, g), b);
            delta = v - min;

            if (v == 0.0)
                s = 0;
            else
                s = delta / v;

            if (s == 0)
                h = 360;
            else
            {
                if (r == v)
                    h = (g - b) / delta;
                else if (g == v)
                    h = 2 + (b - r) / delta;
                else if (b == v)
                    h = 4 + (r - g) / delta;

                h *= 60;
                if (h <= 0.0)
                    h += 360;
            }

            HsvColor hsvColor = new HsvColor()
            {
                H = 360 - h,
                S = s,
                V = v / 255
            };
            return hsvColor;

        }

        // Converts an HSV color to an RGB color.
        public static Color ConvertHsvToRgb(double h, double s, double v, float alpha)
        {

            double r = 0, g = 0, b = 0;

            if (s == 0)
            {
                r = v;
                g = v;
                b = v;
            }

            else
            {
                int i;
                double f, p, q, t;


                if (h == 360)
                    h = 0;
                else
                    h = h / 60;

                i = (int)(h);
                f = h - i;

                p = v * (1.0 - s);
                q = v * (1.0 - (s * f));
                t = v * (1.0 - (s * (1.0f - f)));


                switch (i)
                {
                    case 0:
                        r = v;
                        g = t;
                        b = p;
                        break;

                    case 1:
                        r = q;
                        g = v;
                        b = p;
                        break;

                    case 2:
                        r = p;
                        g = v;
                        b = t;
                        break;

                    case 3:
                        r = p;
                        g = q;
                        b = v;
                        break;

                    case 4:
                        r = t;
                        g = p;
                        b = v;
                        break;

                    default:
                        r = v;
                        g = p;
                        b = q;
                        break;
                }

            }

            return new Color((float)r, (float)g, (float)b, alpha);

        }
    }

    #endregion ColorUtilities

    // Describes a color in terms of
    // Hue, Saturation, and Value (brightness)
    #region HsvColor
    public struct HsvColor
    {
        /// <summary>
        /// The Hue, ranges between 0 and 360
        /// </summary>
        public double H;

        /// <summary>
        /// The saturation, ranges between 0 and 1
        /// </summary>
        public double S;

        // The value (brightness), ranges between 0 and 1
        public double V;

        public float NormalizedH
        {
            get
            {
                return (float)H / 360f;
            }

            set
            {
                H = (double)value * 360;
            }
        }

        public float NormalizedS
        {
            get
            {
                return (float)S;
            }
            set
            {
                S = value;
            }
        }

        public float NormalizedV
        {
            get
            {
                return (float)V;
            }
            set
            {
                V = (double)value;
            }
        }

        public HsvColor(double h, double s, double v)
        {
            this.H = h;
            this.S = s;
            this.V = v;
        }

        public override string ToString()
        {
            return "{" + H.ToString("f2") + "," + S.ToString("f2") + "," + V.ToString("f2") + "}";
        }
    }
    #endregion HsvColor
}