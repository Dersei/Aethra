using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Basic.Materials;
using Aethra.RayTracer.Rendering;
using Aethra.RayTracer.Samplers;

namespace Aethra.RayTracer.Cameras
{
    public abstract class Camera
    {
        public Vector3 Position { get; }

        public Vector3 Direction { get; }

        public Vector3 Up { get; }

        public float NearPlane { get; set; }

        public float FarPlane { get; set; }

        public int MaxDepth { get; set; } = 10;

        public Scene? Scene { get; set; }

        public Sampler? Sampler { get; set; }

        public Func<Ray, RayHit, FloatColor>? SpecialColoring { get; set; }

        public Framebuffer RenderTarget { get; }

        public ColorSpace ColorSpace { get; set; } = ColorSpace.Linear;

        protected Vector3 U;

        protected Vector3 V;

        protected Vector3 W;

        private readonly float _pixelWidth;
        private readonly float _pixelHeight;

        private void CalculateUvw()
        {
            W = -Direction;
            U = Up.Cross(-W);
            V = W.Cross(-U);
        }

        protected Camera(Framebuffer renderTarget, Vector3 position, Vector3 direction, Vector3 up)
        {
            RenderTarget = renderTarget;
            Position = position;
            Direction = direction;
            NearPlane = 0.00001f;
            FarPlane = 1000;
            Up = up;
            _pixelWidth = 2.0f / renderTarget.Width;
            _pixelHeight = 2.0f / renderTarget.Height;
            CalculateUvw();
        }

        public FloatColor CalculateColor(Ray ray, int currentDepth)
        {
            if (currentDepth > MaxDepth)
            {
                return FloatColor.Black;
            }

            var result = Scene!.TestRay(ray, out var info);
            info.Depth = currentDepth + 1;
            if (!result)
            {
                return Scene.Background;
            }

            Material material = info.Material;

            if (SpecialColoring != null)
            {
                return SpecialColoring(ray, info);
            }

            return material.CalculateColor(Scene, ray, info);
        }

        public IEnumerable<bool> Render()
        {
            for (var j = 0; j < RenderTarget.Height; j++)
            {
                for (var i = 0; i < RenderTarget.Width; i++)
                {
                    var color = FloatColor.Black;
                    if (Sampler == null)
                    {
                        var ray = CreateRay(i, j);
                        color = CalculateColor(ray, 0);
                    }
                    else
                    {
                        for (var s = 0; s < Sampler.SampleCount; s++)
                        {
                            var (x, y) = Sampler.Single();
                            var ray = CreateRay(i + x, j + y);
                            color += CalculateColor(ray, 0) / Sampler.SampleCount;
                        }
                    }

                    //pow( color, vec3(1.0/2.2) );
                    color = ColorSpace switch
                    {
                        ColorSpace.Linear => color,
                        ColorSpace.Gamma => new FloatColor(
                            MathF.Pow(color.R, 1.0f / 2.2f),
                            MathF.Pow(color.G, 1.0f / 2.2f),
                            MathF.Pow(color.B, 1.0f / 2.2f)
                        ),
                        ColorSpace.Srgb => new FloatColor(
                            ConvertToSRgb(color.R),
                            ConvertToSRgb(color.G),
                            ConvertToSRgb(color.B)
                        ),
                        _ => FloatColor.Error
                    };

                    RenderTarget.SetPixel(i, j, color);
                }

                yield return true;
            }

            yield return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ConvertToSRgb(float value)
        {
            return value > 0.0031308f ? 1.055f * MathF.Pow(value, 1.0f / 2.4f) - 0.055f : 12.92f * value;
        }

        public IEnumerable<bool> Render(int startHeight, int endHeight, int startWidth, int endWidth)
        {
            for (var j = startHeight; j < endHeight; j++)
            {
                for (var i = startWidth; i < endWidth; i++)
                {
                    var color = FloatColor.Black;
                    if (Sampler == null)
                    {
                        var ray = CreateRay(i, j);
                        color = CalculateColor(ray, 0);
                    }
                    else
                    {
                        for (var s = 0; s < Sampler.SampleCount; s++)
                        {
                            var (x, y) = Sampler.Single();
                            var ray = CreateRay(i + x, j + y);
                            color += CalculateColor(ray, 0) / Sampler.SampleCount;
                        }
                    }

                    RenderTarget.SetPixel(i, j, color);
                }

                yield return true;
            }

            yield return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected (float midX, float midY) GetCenter(float x, float y)
        {
            var ratio = RenderTarget.Width / (float) RenderTarget.Height;
            return ((-1.0f + (x + 0.5f) * _pixelWidth) * ratio,
                1.0f - (y + 0.5f) * _pixelHeight);
        }

        protected abstract Ray CreateRay(float x, float y);
        private Ray CreateRay(Vector2 vector2) => CreateRay(vector2.X, vector2.Y);
    }

    public enum ColorSpace
    {
        Linear,
        Gamma,
        Srgb
    }
}