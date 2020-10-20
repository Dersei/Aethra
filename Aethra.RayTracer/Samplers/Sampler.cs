using System;
using System.Collections.Generic;
using System.Linq;
using Aethra.RayTracer.Basic;

namespace Aethra.RayTracer.Samplers
{
    public class Sampler
    {
        private readonly Random _random;
        private readonly List<Vector2[]> _sets;
        private int _sampleNdx;
        private int _setNdx;

        public Sampler(ISampleGenerator sampler, ISampleDistributor mapper, int sampleCt, int setCt)
        {
            _sets = new List<Vector2[]>(setCt);
            _random = new Random(0);
            SampleCount = sampleCt;
            for (var i = 0; i < setCt; i++)
            {
                var samples = sampler.Sample(sampleCt);
                var mappedSamples = samples.Select(mapper.MapSample).ToArray();
                _sets.Add(mappedSamples);
            }
        }

        public Vector2 Single()
        {
            var sample = _sets[_setNdx][_sampleNdx];
            _sampleNdx++;
            if (_sampleNdx >= _sets[_setNdx].Length)
            { 
                _sampleNdx = 0;
                _setNdx = _random.Next(_sets.Count);
            }

            return sample;
        }

        public int SampleCount { get; }

        public int SetCount => _sets.Count;
    }
}