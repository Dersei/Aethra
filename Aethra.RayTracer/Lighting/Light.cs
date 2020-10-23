using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Samplers;

namespace Aethra.RayTracer.Lighting
{
    public abstract class Light
    {
        public Vector3 Position { get; set; }
        public FloatColor Color { get; set; } = FloatColor.White;
        public float Intensity { get; set; } = 1;
        public Sampler? Sampler { get; set; }
        public abstract Vector3 GetDirection(RayHit hitInfo);
        public abstract FloatColor GetLightColor(Vector3 hitPosition);
    }
}