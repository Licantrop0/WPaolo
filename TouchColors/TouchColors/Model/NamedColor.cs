using System;
using Windows.UI;

namespace TouchColors.Model
{
    public class NamedColor : IEquatable<NamedColor>
    {
        public string Name { get; }
        public Color RgbColor { get; }
        public float Luminosity { get; }

        public NamedColor(string name, Color rgbColor)
        {
            Name = name;
            RgbColor = rgbColor;
            Luminosity = GetLuminosity(rgbColor);
        }

        private static float GetLuminosity(Color color)
        {
            float r = (color.R / 255f);
            float g = (color.G / 255f);
            float b = (color.B / 255f);

            float min = Math.Min(Math.Min(r, g), b);
            float max = Math.Max(Math.Max(r, g), b);
            return (max + min) / 2.0f;
        }

        public bool Equals(NamedColor other)
        {
            return this.RgbColor.Equals(other.RgbColor);
        }
        public override int GetHashCode()
        {
            return RgbColor.GetHashCode() ^ "NamedColor".GetHashCode();
        }
    }
}
