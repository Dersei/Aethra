using System;
using Aethra.RayTracer.Basic;

namespace Aethra.RayTracer.Samplers.Distributors
{
    internal class DiskDistributor : ISampleDistributor
    {
        public Vector2 MapSample(Vector2 sample)
        {
            return new Vector2(sample.X * MathF.Cos(sample.Y * MathF.PI), 
                sample.X * MathF.Sin(sample.Y * MathF.PI));
        }
    }
}