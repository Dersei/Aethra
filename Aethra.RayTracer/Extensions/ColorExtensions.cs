using System.Drawing;
using Aethra.RayTracer.Basic;

namespace Aethra.RayTracer.Extensions
{
    public static class ColorExtensions
    {
        public static FloatColor ToFloatColor(this Color color)
        {
            var r = color.R / 255f;
            var g = color.G / 255f;
            var b = color.B / 255f;
            var a = color.A / 255f;
            return new FloatColor(r, g, b, a);
        }

        public static Color ToDrawingColor(this FloatColor color)
        {
            var a = (color.A * 255).ClampToByte();
            var r = (color.R * 255).ClampToByte();
            var g =  (color.G * 255).ClampToByte();
            var b =  (color.B * 255).ClampToByte();
            return Color.FromArgb(a,r,g,b);
        }
    }
}