using Aethra.RayTracer.Basic;

namespace Aethra.RayTracer.Samplers
{
    public interface ISampleGenerator
    {
        Vector2[] Sample(int count);
    }
}