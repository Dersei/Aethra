using System;
using Aethra.RayTracer.Basic;

namespace Aethra.RayTracer.Lighting
{
    public class AreaLight : Light
    {
        public override Vector3 GetDirection(RayHit hitInfo)
        {
            throw new NotImplementedException();
        }

        public override FloatColor GetLightColor(Vector3 hitPosition)
        {
            throw new NotImplementedException();
        }
    }
}