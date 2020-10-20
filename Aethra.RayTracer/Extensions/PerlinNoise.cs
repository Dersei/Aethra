using System;

namespace Aethra.RayTracer.Extensions
{
    public class PerlinNoise
    {
        private readonly Random _rnd = new Random();
        private readonly int[,] _noise;
        private readonly int _noiseWidth;
        private readonly int _noiseHeight;
        private readonly float _scaleX;
        private readonly float _scaleY;

        public float Get(float x, float y = 0.5F)
        {
            var xInt = (int) x;
            var yInt = (int) y;
            var xFrac = x - xInt;
            var yFrac = y - yInt;
            var x0Y0 = SmoothNoise(xInt, yInt); //find the noise values of the four corners
            var x1Y0 = SmoothNoise(xInt + 1, yInt);
            var x0Y1 = SmoothNoise(xInt, yInt + 1);
            var x1Y1 = SmoothNoise(xInt + 1, yInt + 1);
            //interpolate between those values according to the x and y fractions
            var v1 = Interpolate(x0Y0, x1Y0, xFrac); //interpolate in x direction (y)
            var v2 = Interpolate(x0Y1, x1Y1, xFrac); //interpolate in x direction (y+1)
            var fin = Interpolate(v1, v2, yFrac); //interpolate in y direction
            return fin;
        }

        private static float Interpolate(float x, float y, float a)
        {
            var b = 1 - a;
            var fac1 = 3 * b * b - 2 * b * b * b;
            var fac2 = 3 * a * a - 2 * a * a * a;
            return x * fac1 + y * fac2; //add the weighted factors
        }

        public float GetRandomValue(int x, int y)
        {
            x = (x + _noiseWidth) % _noiseWidth;
            y = (y + _noiseHeight) % _noiseHeight;
            var fVal = (float) _noise[(int) (_scaleX * x), (int) (_scaleY * y)];
            return fVal / 255 * 2 - 1f;
        }

        public float SmoothNoise(int x, int y)
        {
            var corners = (Noise2d(x - 1, y - 1) + Noise2d(x + 1, y - 1) + Noise2d(x - 1, y + 1) +
                           Noise2d(x + 1, y + 1)) / 16.0f;
            var sides = (Noise2d(x - 1, y) + Noise2d(x + 1, y) + Noise2d(x, y - 1) + Noise2d(x, y + 1)) / 8.0f;
            var center = Noise2d(x, y) / 4.0f;
            return corners + sides + center;
        }

        public float Noise2d(int x, int y)
        {
            x = (x + _noiseWidth) % _noiseWidth;
            y = (y + _noiseHeight) % _noiseHeight;
            var fVal = (float) _noise[(int) (_scaleX * x), (int) (_scaleY * y)];
            return fVal / 255 * 2 - 1f;
        }

        public PerlinNoise()
        {
            _noiseWidth = 100;
            _noiseHeight = 100;
            _scaleX = 1.0F;
            _scaleY = 1.0F;
            _noise = new int[_noiseWidth, _noiseHeight];
            for (var x = 0; x < _noiseWidth; x++)
            {
                for (var y = 0; y < _noiseHeight; y++)
                {
                    _noise[x, y] = _rnd.Next(255);
                }
            }
        }
    }
}