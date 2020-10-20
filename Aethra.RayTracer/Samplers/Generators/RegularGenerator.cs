using System;
using Aethra.RayTracer.Basic;

namespace Aethra.RayTracer.Samplers.Generators
{
    public class RegularGenerator : ISampleGenerator
    {
        public Vector2[] Sample(int sampleCt)
        {
            var sampleRow = (int) MathF.Sqrt(sampleCt);
            var result = new Vector2[sampleRow * sampleRow];
            for (var x = 0; x < sampleRow; x++)
            for (var y = 0; y < sampleRow; y++)
            {
                var fracX = (x + 0.5f) / sampleRow;
                var fracY = (y + 0.5f) / sampleRow;
                result[x * sampleRow + y] = new Vector2(fracX, fracY);
            }

            return result;
        }
    }
}