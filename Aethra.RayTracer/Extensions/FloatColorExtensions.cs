using Aethra.RayTracer.Basic;

namespace Aethra.RayTracer.Extensions
{
    public static class FloatColorExtensions
    {
        private static void RGBToHSVHelper(float offset, float dominantColor, float colorOne, float colorTwo, out float h,
            out float s, out float v)
        {
            v = dominantColor;
            //we need to find out which is the minimum color
            if (v.IsNotZero())
            {
                //we check which color is smallest
                var small = colorOne > colorTwo ? colorTwo : colorOne;

                var diff = v - small;

                //if the two values are not the same, we compute the like this
                if (diff.IsNotZero())
                {
                    //S = max-min/max
                    s = diff / v;
                    //H = hue is offset by X, and is the difference between the two smallest colors
                    h = offset + ((colorOne - colorTwo) / diff);
                }
                else
                {
                    //S = 0 when the difference is zero
                    s = 0;
                    //H = 4 + (R-G) hue is offset by 4 when blue, and is the difference between the two smallest colors
                    h = offset + (colorOne - colorTwo);
                }

                h /= 6;

                //conversion values
                if (h < 0)
                    h += 1.0f;
            }
            else
            {
                s = 0;
                h = 0;
            }
        }

        public static void ToHSV(this FloatColor rgbColor, out float h, out float s, out float v)
        {
            // when blue is highest valued
            if (rgbColor.B > rgbColor.G && rgbColor.B > rgbColor.R)
                RGBToHSVHelper(4, rgbColor.B, rgbColor.R, rgbColor.G, out h, out s, out v);
            //when green is highest valued
            else if (rgbColor.G > rgbColor.R)
                RGBToHSVHelper(2, rgbColor.G, rgbColor.B, rgbColor.R, out h, out s, out v);
            //when red is highest valued
            else
                RGBToHSVHelper(0, rgbColor.R, rgbColor.G, rgbColor.B, out h, out s, out v);
        }

       
    }
}