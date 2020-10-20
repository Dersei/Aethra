using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Basic.Materials;

namespace Aethra.RayTracer.Interfaces
{
    public interface IHittable
    {
        public Material Material { get; }
        public bool Hit(Ray ray, out RayHit hit);
    }
}