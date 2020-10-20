using System;
using Aethra.RayTracer.Basic;

namespace Aethra.RayTracer.Samplers.Generators
{
    public class NRooksGenerator : ISampleGenerator
    {
        private readonly Random _random;

        public NRooksGenerator(int seed)
        {
            _random = new Random(seed);
        }

        public Vector2[] Sample(int sampleCt)
        {
            var samples = new Vector2[sampleCt];
            for (var i = 0; i < sampleCt; i++)
            {
                samples[i] = new Vector2(
                    (float) ((i + _random.NextDouble()) / sampleCt),
                    (float) ((i + _random.NextDouble()) / sampleCt));
            }

            ShuffleX(samples, sampleCt);
            return samples;
        }

        private void ShuffleX(Vector2[] samples, int sampleCt)
        {
            for (var i = 0; i < sampleCt - 1; i++)
            {
                var target = _random.Next() % sampleCt;
                var temp = samples[i].X;
                samples[i] = new Vector2(samples[target].X, samples[i].Y);
                samples[target] = new Vector2(temp, samples[target].Y);
            }
        }
    }
}