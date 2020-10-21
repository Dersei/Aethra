using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace Aethra.RayTracer.Basic.Textures.Generators
{
    public static class FractalGenerator
    {
        public static Bitmap BurningShip(int width, int height)
        {
            var resultBitmap = new Bitmap(width, height);
            const int maxIterations = 256;

            const float graphTop = 1.5F;
            const float graphBottom = -1.5F;
            const float graphLeft = -2F;
            const float graphRight = 1.5F;

            int y;

            var incrementX = ((graphRight - graphLeft) / (resultBitmap.Width - 1));
            var decrementY = ((graphTop - graphBottom) / (resultBitmap.Height - 1));
            var coordsImaginary = graphTop;
            var palette = new int[256];

            for (var n = 0; n < 256; n++)
            {
                palette[n] = (int) (n + 512 - 512 * Math.Exp(-n / 50.0) / 3.0);
                palette[n] = palette[n] << 16 | palette[n] << 8 | palette[n];
                /*
                 * The maximum value should be 255.??? so the (int) conversion
                 * should give 255 as the maximum value
                 */
            }

            using var graphics = Graphics.FromImage(resultBitmap);
            palette[255] = 0;
            for (y = 0; y < resultBitmap.Height; y++)
            {
                var coordsReal = graphLeft;
                // line
                int x;
                for (x = 0; x < resultBitmap.Width; x++)
                {
                    var iterations = 0;
                    var zx = coordsReal;
                    var zy = coordsImaginary;
                    var squaredX = zx * zx;
                    var squaredY = zy * zy;
                    do
                    {
                        // (Zx + Zyi)^2 = Zx*Zx - Zy * Zy + 2Zx * Zyi
                        zy = Math.Abs(zx * zy);
                        zy = zy + zy - coordsImaginary; // adding Zy to itself removes
                        // a slower times two
                        // multiply operation
                        zx = squaredX - squaredY + coordsReal;
                        squaredX = zx * zx;
                        squaredY = zy * zy;
                        iterations++;
                    } while (iterations < maxIterations && (squaredX + squaredY) < 4.0); // Squareroot(n)

                    iterations--;
                    resultBitmap.SetPixel(x, y, Color.FromArgb((byte) (palette[iterations] % 50 + 200),
                        (byte) (255 - palette[iterations]),
                        (byte) palette[iterations],
                        (byte) (palette[iterations] % 50 + 200)));
                    coordsReal += incrementX; // Increment to the next place on the graph
                }

                coordsImaginary -= decrementY; // Go down one line on the graph
            }

            return resultBitmap;
        }

        public static Bitmap SierpinskyCarpet(int width, int height, Color color, Color color2)
        {
            return SierpinskyCarpet(width, height, b => b ? color : color2);
        }

        public static Bitmap SierpinskyCarpet(int width, int height, Func<bool, Color> func)
        {
            var resultBitmap = new Bitmap(width, height);

            static bool CheckIfFilled(int x, int y)
            {
                while (x != 0 && y != 0)
                {
                    if (x % 3 == 1 && y % 3 == 1)
                        return false;
                    x /= 3;
                    y /= 3;
                }

                return true;
            }

            for (var i = 0; i < resultBitmap.Width; i++)
            {
                for (var j = 0; j < resultBitmap.Height; j++)
                {
                    resultBitmap.SetPixel(i, j, func(CheckIfFilled(i, j)));
                }
            }

            return resultBitmap;
        }

        private static List<Color> GenerateColorPalette()
        {
            var retVal = new List<Color>();
            for (var i = 0; i <= 255; i++)
            {
                retVal.Add(Color.FromArgb(255, (byte) i, (byte) i, 255));
            }

            return retVal;
        }

        public static Bitmap Mandelbrot(int width, int height, double rMin, double rMax, double iMin, double iMax)
        {
            var resultBitmap = new Bitmap(width, height);
            var palette = GenerateColorPalette();

            var rScale = (Math.Abs(rMin) + Math.Abs(rMax)) / resultBitmap.Width; // Amount to move each pixel in the real numbers
            var iScale =
                (Math.Abs(iMin) + Math.Abs(iMax)) / resultBitmap.Height; // Amount to move each pixel in the imaginary numbers

            for (var x = 0; x < resultBitmap.Width; x++)
            {
                for (var y = 0; y < resultBitmap.Height; y++)
                {
                    var c = new Complex(x * rScale + rMin, y * iScale + iMin); // Scaled complex number
                    var z = c;
                    foreach (var t in palette)
                    {
                        if (z.Magnitude >= 2.0)
                        {
                            resultBitmap.SetPixel(x, y, t); // Set the pixel if the magnitude is greater than two
                            break; // We're done with this loop
                        }

                        z = c + Complex.Pow(z, 2); // Z = Zlast^2 + C
                    }
                }
            }

            return resultBitmap;
        }
        
        private static Color FromHsv(double h, double s, double v)
        {
            var range = Convert.ToInt32(Math.Floor(h / 60.0)) % 6;
            var f = h / 60.0 - Math.Floor(h / 60.0);

            var v2 = v * 255.0;
            var p = v2 * (1 - s);
            var q = v2 * (1 - f * s);
            var t = v2 * (1 - (1 - f) * s);

            switch (range)
            {
                case 0:
                    return Color.FromArgb((byte)v2, (byte)t, (byte)p);
                case 1:
                    return Color.FromArgb((byte)q, (byte)v2, (byte)p);
                case 2:
                    return Color.FromArgb((byte)p, (byte)v2, (byte)t);
                case 3:
                    return Color.FromArgb((byte)p, (byte)q, (byte)v2);
                case 4:
                    return Color.FromArgb((byte)t, (byte)p, (byte)v2);
            }
            return Color.FromArgb((byte)v2, (byte)p, (byte)q);
        }
        
        public static Bitmap Julia(int width, int height)
        {
            var resultBitmap = new Bitmap(width, height);
            const int maxIterations = 300;
            const double cr = -0.70000;
            const double ci = 0.27015;

            for (var y = 0; y < resultBitmap.Height; ++y)
            {
                for (var x = 0; x < resultBitmap.Width; ++x)
                {
                    var nextR = 1.5 * (2.0 * x / resultBitmap.Width - 1.0);
                    var nextI = 2.0 * y / resultBitmap.Height - 1.0;

                    for (var i = 0; i < maxIterations; ++i)
                    {
                        var prevR = nextR;
                        var prevI = nextI;

                        nextR = prevR * prevR - prevI * prevI + cr;
                        nextI = 2 * prevR * prevI + ci;

                        if (nextR * nextR + nextI * nextI > 4)
                        {
                            var color = FromHsv(i % 256, 255, 255);
                            resultBitmap.SetPixel(x, y, color);
                            break;
                        }
                    }
                }

            }

            return resultBitmap;
        }
    }
}