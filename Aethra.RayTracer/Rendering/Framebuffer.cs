using System.Runtime.CompilerServices;
using Aethra.RayTracer.Basic;

namespace Aethra.RayTracer.Rendering
{
    public class Framebuffer
    {
        public uint[,] Pixels { get; }
        public int Width { get; }
        public int Height { get; }
        public int Count => Pixels.Length;

        public Framebuffer(int width, int height)
        {
            Width = width;
            Height = height;
            Pixels = new uint[height, width];
        }
        
        public uint this[int i, int j]
        {
            get => Pixels[j, i];
            set => Pixels[j, i] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CheckIfSafe(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixel(int x, int y, uint color)
        {
            if (!CheckIfSafe(x, y)) return;
            Pixels[y, x] = color;
        }

        public void Clear(FloatColor color)
        {
            for (var i = 0; i < Pixels.GetLength(0); i++)
            {
                for (var j = 0; j < Pixels.GetLength(1); j++)
                {
                    Pixels[i, j] = color;
                }
            }
        }
    }
}