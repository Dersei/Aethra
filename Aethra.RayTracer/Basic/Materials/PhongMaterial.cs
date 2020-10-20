using System;
using Aethra.RayTracer.Extensions;
using Aethra.RayTracer.Rendering;

namespace Aethra.RayTracer.Basic.Materials
{
    public class PhongMaterial : Material
    {
        private readonly float _diffuseCoefficient;
        private readonly float _specular;
        private readonly float _specularExponent;
        private readonly float _ambientPower;

        public PhongMaterial(FloatColor color, float diffuseCoefficient, float specular, float specularExponent,
            float ambientPower = 1f, TextureInfo? texture = null)
        {
            Color = color;
            _diffuseCoefficient = diffuseCoefficient;
            _specular = specular;
            _specularExponent = specularExponent;
            _ambientPower = ambientPower;
            Texture = texture;
        }

        public PhongMaterial(FloatColor color, TextureInfo? texture = null)
        {
            Color = color;
            Texture = texture;
            _diffuseCoefficient = 1;
            _specular = 0;
            _specularExponent = 50;
            _ambientPower = 1f;
        }

        private FloatColor GetResultColor(FloatColor lightColor, float lightIntensity, FloatColor? texelColor,
            float diffuseFactor)
        {
            if (texelColor != null)
                return lightColor * lightIntensity * _ambientPower * texelColor.Value * Color * diffuseFactor *
                       _diffuseCoefficient;
            return lightColor * lightIntensity * _ambientPower * Color * diffuseFactor * _diffuseCoefficient;
        }

        private FloatColor GetAddingResult(FloatColor? texelColor, float phongFactor)
        {
            if (texelColor != null) return texelColor.Value * Color * phongFactor;
            return Color * phongFactor;
        }


        public override FloatColor CalculateColor(Scene scene, Ray ray, RayHit hit)
        {
            var texelColor = Texture?.GetColor(hit.TextureCoords);
            var totalColor = FloatColor.Black;
            foreach (var light in scene.Lights)
            {
                var inDirection = light.GetDirection(hit);
                var diffuseFactor = inDirection.Dot(hit.Normal);
                if (diffuseFactor < 0)
                {
                    continue;
                }

                if (scene.IsAnythingBetween(hit.Position, light.Position))
                {
                    continue;
                }

                var result = GetResultColor(light.GetLightColor(hit.Position), light.Intensity, texelColor,
                    diffuseFactor);
                var phongFactor = CalculatePhong(inDirection, hit.Normal, -ray.Direction) * _specular;
                if (phongFactor.IsNotZero())
                {
                    result += GetAddingResult(texelColor, phongFactor);
                }

                totalColor += result;
            }

            return totalColor;
        }

        private float CalculatePhong(Vector3 direction, Vector3 normal, Vector3 cameraDirection)
        {
            var reflection = direction.Reflect(normal);
            var angle = reflection.Dot(cameraDirection);
            if (angle <= 0)
            {
                return 0;
            }

            return MathF.Pow(angle, _specularExponent);
        }
    }
}