using System;
using Aethra.RayTracer.Basic;

namespace Aethra.RayTracer.Samplers.Generators
{
    internal class PureRandomGenerator : ISampleGenerator
    {
        private readonly Random _random;

        public PureRandomGenerator(int seed)
        {
            _random = new Random(seed);
        }

        public Vector2[] Sample(int sampleCt)
        {
            var samples = new Vector2[sampleCt];
            for (var i = 0; i < sampleCt; i++)
            {
                samples[i] = new Vector2((float) _random.NextDouble(), (float) _random.NextDouble());
            }

            return samples;
        }
    }
}