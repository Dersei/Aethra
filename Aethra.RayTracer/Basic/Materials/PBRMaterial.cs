using System;
using Aethra.RayTracer.Extensions;
using Aethra.RayTracer.Rendering;

namespace Aethra.RayTracer.Basic.Materials
{
    public class PBRMaterial : Material
    {
        public float DiffuseCoefficient;
        public float Specular;
        public float SpecularExponent;
        public float AmbientPower;
        private readonly TextureInfo? _emissiveMap;
        private readonly TextureInfo? _specularMap;
        private readonly TextureInfo? _normalMap;
        public float EmissionFactor = 1;


        public PBRMaterial(FloatColor color, TextureInfo? texture = null, TextureInfo? emissiveMap = null,
            TextureInfo? specularMap = null, TextureInfo? normalMap = null)
        {
            Color = color;
            Texture = texture;
            _emissiveMap = emissiveMap;
            _specularMap = specularMap;
            _normalMap = normalMap;
            DiffuseCoefficient = 1;
            Specular = 0;
            SpecularExponent = 50;
            AmbientPower = 1f;
        }

        private FloatColor GetResultColor(FloatColor lightColor, float lightIntensity, FloatColor? texelColor, float diffuseFactor)
        {
            if (texelColor != null)
                return lightColor * lightIntensity * AmbientPower * texelColor.Value * Color * diffuseFactor *
                       DiffuseCoefficient;
            return lightColor * lightIntensity * AmbientPower * Color * diffuseFactor * DiffuseCoefficient;
        }

        private FloatColor GetEmissionColor(Ray ray, RayHit hit)
        {
            var texelColor = _emissiveMap?.GetColor(hit.TextureCoords);
            if (-hit.Normal.Dot(ray.Direction) > 0 && texelColor != null)
            {
                return texelColor.Value * EmissionFactor;
            }

            return FloatColor.Black;
        }

        private FloatColor GetAddingResult(FloatColor? texelColor, float phongFactor)
        {
            if (texelColor != null) return texelColor.Value * Color * phongFactor;
            return Color * phongFactor;
        }

        private float GetSpecular(RayHit hit)
        {
            var texelColor = _specularMap?.GetColor(hit.TextureCoords);
            if (texelColor != null) return texelColor.Value.R;
            return 0;
        }


        public override FloatColor CalculateColor(Scene scene, Ray ray, RayHit hit)
        {
            var texelColor = Texture?.GetColor(hit.TextureCoords);
            var totalColor = FloatColor.Black;
            foreach (var light in scene.Lights)
            {
                var inDirection = light.GetDirection(hit);

                Vector3 normal;
                var normalColor = _normalMap?.GetColor(hit.TextureCoords);
                if (normalColor != null)
                {
                    var x = normalColor.Value.R * 2 - 1;
                    var y = normalColor.Value.G * 2 - 1;
                    var z = normalColor.Value.B * 2 - 1;
                    normal = (new Vector3(x, y, z) * hit.Normal).Normalize();
                }
                else
                {
                    normal = hit.Normal;
                }

                var diffuseFactor = inDirection.Dot(normal);
                if (diffuseFactor < 0)
                {
                    continue;
                }

                if (scene.IsAnythingBetween(hit.Position, light.Position))
                {
                    continue;
                }

                var result = GetResultColor(light.GetLightColor(hit.Position), light.Intensity, texelColor, diffuseFactor);
                var phongFactor = CalculatePhong(inDirection, normal, -ray.Direction)
                                  * Specular * GetSpecular(hit);
                if (phongFactor.IsNotZero())
                {
                    result += GetAddingResult(texelColor, phongFactor);
                }

                totalColor += result;
            }

            return totalColor + GetEmissionColor(ray, hit);
        }

        private float CalculatePhong(Vector3 direction, Vector3 normal, Vector3 cameraDirection)
        {
            var reflection = direction.Reflect(normal);
            var angle = reflection.Dot(cameraDirection);
            if (angle <= 0)
            {
                return 0;
            }

            return MathF.Pow(angle, SpecularExponent);
        }
    }
}