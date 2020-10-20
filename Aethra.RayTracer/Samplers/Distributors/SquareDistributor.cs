using Aethra.RayTracer.Basic;

namespace Aethra.RayTracer.Samplers.Distributors
{
    internal class SquareDistributor : ISampleDistributor
    {
        public Vector2 MapSample(Vector2 sample)
        {
            return sample;
        }
    }
}