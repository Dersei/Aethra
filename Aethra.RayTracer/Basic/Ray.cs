using System.Runtime.CompilerServices;
using Aethra.RayTracer.Interfaces;

namespace Aethra.RayTracer.Basic
{
    public readonly struct Ray
    {
        public readonly Vector3 Origin;
        public readonly Vector3 Direction;
        public readonly Vector3 Destination;
        public readonly float Distance;
        public readonly float NearPlane;
        public readonly float FarPlane;

        public Ray(Vector3 origin, Vector3 direction, float nearPlane = 0.00001f, float farPlane = float.PositiveInfinity)
        {
            Origin = origin;
            Direction = direction;
            Distance = 0;
            Destination = Vector3.Zero;
            NearPlane = nearPlane;
            FarPlane = farPlane;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 PointAt(float distance) => Origin + Direction * distance;

        public bool CheckHit(IHittable hittable, out RayHit rayHit)
        {
            return hittable.Hit(this, out rayHit);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInsidePlanes(float distance)
        {
            return distance > NearPlane && distance < FarPlane;
        }
    }
}