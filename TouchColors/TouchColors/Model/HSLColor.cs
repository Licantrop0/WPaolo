using System;
using Windows.UI;

namespace TouchColors.Model
{
    public class HSLColor
    {
        public float Hue;
        public float Saturation;
        public float Luminosity;

        public HSLColor(float h, float s, float l)
        {
            Hue = h;
            Saturation = s;
            Luminosity = l;
        }

        public static HSLColor FromRGB(Color color)
        {
            float r = (color.R / 255f);
            float g = (color.G / 255f);
            float b = (color.B / 255f);

            float min = Math.Min(Math.Min(r, g), b);
            float max = Math.Max(Math.Max(r, g), b);
            float delta = max - min;

            float H = 0;
            float S = 0;
            float L = (max + min) / 2.0f;

            if (delta != 0)
            {
                if (L < 0.5f)
                {
                    S = delta / (max + min);
                }
                else
                {
                    S = delta / (2.0f - max - min);
                }

                if (r == max)
                {
                    H = (g - b) / delta;
                }
                else if (g == max)
                {
                    H = 2f + (b - r) / delta;
                }
                else if (b == max)
                {
                    H = 4f + (r - g) / delta;
                }
            }

            return new HSLColor(H, S, L);
        }
    }
}
