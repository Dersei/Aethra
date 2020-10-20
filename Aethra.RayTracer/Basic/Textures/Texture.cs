using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace Aethra.RayTracer.Basic.Textures
{
    public class Texture : ITexture
    {
        public readonly int Width;
        public readonly int Height;
        public readonly FloatColor[,] ColorMap;

        private Texture(int width, int height, FloatColor[,] colorMap)
        {
            Width = width;
            Height = height;
            ColorMap = colorMap;
        }

        private static int ChangeValue(int value, int limit)
        {
            var counter = value / limit;
            for (var i = 0; i < counter; i++)
            {
                value -= limit;
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FixWrongValues(ref int x, ref int y)
        {
            if (x < 0) x = 0;
            if (y < 0) y = 0;
        }

        public FloatColor GetColor(Vector2 uv, Vector2 scale, Vector2 offset)
        {
            var x = (int) ((uv.X + offset.X) * Width * scale.X);
            var y = (int) ((uv.Y + offset.Y) * Height * scale.Y);
            if (x >= Width)
            {
                x = ChangeValue(x, Width);
            }

            if (y >= Height)
            {
                y = ChangeValue(y, Height);
            }

            FixWrongValues(ref x, ref y);
            return ColorMap[x, y];
        }

        public FloatColor GetColor(Vector2 uv)
        {
            var x = (int) (uv.X * Width);
            var y = (int) (uv.Y * Height);
            if (x >= Width)
            {
                x = ChangeValue(x, Width);
            }

            if (y >= Height)
            {
                y = ChangeValue(y, Height);
            }

            FixWrongValues(ref x, ref y);
            return ColorMap[x, y];
        }

        private static readonly Dictionary<string, Texture> Textures = new Dictionary<string, Texture>();

        public static unsafe Texture CreateFrom(Bitmap bitmap, bool mirrorX = false, bool mirrorY = false)
        {
            var hash = bitmap.GetHashCode().ToString();
            if (Textures.TryGetValue(hash, out var texture))
            {
                return texture;
            }

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            var pixels = new FloatColor[bitmap.Width, bitmap.Height];
            const int pixelSize = 4;
            for (var y = 0; y < data.Height; y++)
            {
                var row = (byte*) data.Scan0 + y * data.Stride;

                for (var x = 0; x < data.Width; x++)
                {
                    var indexX = x;
                    var indexY = y;
                    if (mirrorY) indexX = data.Width - 1 - x;
                    if (mirrorX) indexY = data.Height - 1 - y;
                    pixels[indexX, indexY] =
                        FloatColor.FromRGBA(row[x * pixelSize + 2], row[x * pixelSize + 1],
                            row[x * pixelSize],
                            row[x * pixelSize + 3]);
                }
            }

            bitmap.UnlockBits(data);
            var result = new Texture(bitmap.Width, bitmap.Height, pixels);
            Textures.Add(hash, result);
            return result;
        }

        public static unsafe Texture LoadFrom(string filename, bool mirrorX = false, bool mirrorY = false)
        {
            if (Textures.TryGetValue(filename, out var texture))
            {
                return texture;
            }

            var bitmap = new Bitmap(filename);
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            var pixels = new FloatColor[bitmap.Width, bitmap.Height];
            const int pixelSize = 4;
            for (var y = 0; y < data.Height; y++)
            {
                var row = (byte*) data.Scan0 + y * data.Stride;

                for (var x = 0; x < data.Width; x++)
                {
                    var indexX = x;
                    var indexY = y;
                    if (mirrorY) indexX = data.Width - 1 - x;
                    if (mirrorX) indexY = data.Height - 1 - y;
                    pixels[indexX, indexY] =
                        FloatColor.FromRGBA(
                            row[x * pixelSize + 2], row[x * pixelSize + 1],
                            row[x * pixelSize],
                            row[x * pixelSize + 3]);
                    // row[x * PixelSize] = 0; //Blue  0-255
                    // row[x * PixelSize + 1] = 255; //Green 0-255
                    // row[x * PixelSize + 2] = 0; //Red   0-255
                    // row[x * PixelSize + 3] = 50; //Alpha 0-255
                }
            }

            bitmap.UnlockBits(data);
            var result = new Texture(bitmap.Width, bitmap.Height, pixels);
            Textures.Add(filename, result);
            return result;
        }

        public static Texture GenerateWith(Func<int, int, FloatColor> colorGenerator)
        {
            const int height = 512;
            const int width = 512;
            if (Textures.TryGetValue(colorGenerator.ToString(), out var texture))
            {
                return texture;
            }

            var pixels = new FloatColor[width, height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    pixels[x, y] = colorGenerator(x, y);
                }
            }

            var result = new Texture(width, height, pixels);
            Textures.Add(colorGenerator.ToString(), result);
            return result;
        }
    }
}