using Aethra.RayTracer.Rendering;

namespace Aethra.RayTracer.Basic.Materials
{
    public class ReflectiveMaterial : Material
    {
        private readonly PhongMaterial _direct;
        private readonly float _reflectivity;
        private readonly FloatColor _reflectionColor;

        public ReflectiveMaterial(FloatColor materialColor, float diffuse, float specular, float exponent,
            float reflectivity, TextureInfo? texture = null)
        {
            _direct = new PhongMaterial(materialColor, diffuse, specular, exponent, 1, texture);
            _reflectivity = reflectivity;
            _reflectionColor = materialColor;
        }

        public override FloatColor CalculateColor(Scene scene, Ray ray, RayHit hit)
        {
            var toCameraDirection = -ray.Direction;
            var radiance = _direct.CalculateColor(scene, ray, hit);
            var reflectionDirection = toCameraDirection.Reflect(hit.Normal);
            var reflectedRay = new Ray(hit.Position, reflectionDirection);
            radiance += scene.Camera.CalculateColor(reflectedRay, hit.Depth) * _reflectionColor * _reflectivity;
            return radiance;
        }
    }
}