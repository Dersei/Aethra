using System;
using Aethra.RayTracer.Basic;

namespace Aethra.RayTracer.Samplers.Generators
{
    internal class JitteredGenerator : ISampleGenerator
    {
        private readonly Random _random;

        public JitteredGenerator(int seed)
        {
            _random = new Random(seed);
        }

        public Vector2[] Sample(int count)
        {
            var sampleRow = (int) MathF.Sqrt(count);
            var result = new Vector2[sampleRow * sampleRow];
            for (var x = 0; x < sampleRow; x++)
            for (var y = 0; y < sampleRow; y++)
            {
                var fracX = (float) ((x + _random.NextDouble()) / sampleRow);
                var fracY = (float) ((y + _random.NextDouble()) / sampleRow);
                result[x * sampleRow + y] = new Vector2(fracX, fracY);
            }

            return result;
        }
    }
}