using Aethra.RayTracer.Basic;

namespace Aethra.RayTracer.Samplers
{
    public interface ISampleDistributor
    {
        Vector2 MapSample(Vector2 sample);
    }
}