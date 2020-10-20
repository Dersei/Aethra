using System;
using Aethra.RayTracer.Rendering;

namespace Aethra.RayTracer.Basic.Materials
{
    public class TransparentMaterial : Material
    {
        private readonly PhongMaterial _direct;
        private readonly float _refraction;
        private readonly float _transmission;
        private readonly float _reflection;
        private readonly FloatColor _baseColor;

        public TransparentMaterial(FloatColor materialColor, float diffuse,
            float exponent, float reflection, float refraction, float transmission)
        {
            _direct = new PhongMaterial(materialColor, diffuse, 0, exponent);
            _transmission = transmission;
            _baseColor = materialColor;
            _reflection = reflection;
            _refraction = refraction;
        }

        private FloatColor ComputeTransmissionColor(float eta)
        {
            return FloatColor.White * _transmission / (eta * eta);
        }

        private static float FindRefractionCoefficient(float eta, float cosIncidentAngle)
        {
            return 1 - (1 - cosIncidentAngle * cosIncidentAngle) / (eta * eta);
        }

        private static bool IsTotalInternalReflection(float refractionCoeff)
        {
            return refractionCoeff < 0;
        }

        private static Ray ComputeTransmissionDirection(Vector3 hitPoint, Vector3 toCameraDirection, Vector3 normal,
            float eta, float cosTransmittedAngle, float cosIncidentAngle)
        {
            if (cosIncidentAngle < 0)
            {
                normal = -normal;
                cosIncidentAngle = -cosIncidentAngle;
            }

            var direction = -toCameraDirection / eta
                            - normal * (cosTransmittedAngle - cosIncidentAngle / eta);
            return new Ray(hitPoint, direction);
        }


        private static Vector3 Refract(Vector3 incident, Vector3 normal, float n1, float n2)
        {
            var n = n1 / n2;
            var cosI = -normal.Dot(incident);
            var sinT2 = n * n * (1.0f - cosI * cosI);
            var cosT = MathF.Sqrt(1.0f - sinT2);
            return incident * n + normal * (n * cosI - cosT);
        }


        public override FloatColor CalculateColor(Scene scene, Ray ray, RayHit hit)
        {
            var final = _direct.CalculateColor(scene, ray, hit);
            var toCameraDirection = -ray.Direction;
            var cosIncidentAngle = hit.Normal.Dot(toCameraDirection);
            var eta = cosIncidentAngle > 0 ? _refraction : 1 / _refraction;
            var refractionCoefficient = FindRefractionCoefficient(eta, cosIncidentAngle);

            var reflectedRay = new Ray(hit.Position, toCameraDirection.Reflect(hit.Normal));

            var reflectionColor = _baseColor * _reflection;

            if (IsTotalInternalReflection(refractionCoefficient))
            {
                final += scene.Camera.CalculateColor(ray, hit.Depth);
            }
            else
            {
                var transmittedRay = ComputeTransmissionDirection(hit.Position, toCameraDirection, hit.Normal, eta,
                    MathF.Sqrt(refractionCoefficient), cosIncidentAngle);

                var transmissionColor = ComputeTransmissionColor(eta);

                final += reflectionColor * scene.Camera.CalculateColor(reflectedRay, hit.Depth);
                final += transmissionColor * scene.Camera.CalculateColor(transmittedRay, hit.Depth);
            }

            return final;
        }
    }
}