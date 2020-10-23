using System.Collections.Generic;
using System.Drawing;
using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Basic.Materials;
using Aethra.RayTracer.Basic.Textures;
using Aethra.RayTracer.Cameras;
using Aethra.RayTracer.Extensions;
using Aethra.RayTracer.Interfaces;
using Aethra.RayTracer.Lighting;
using Aethra.RayTracer.Primitives;
using Aethra.RayTracer.Rendering;
using Aethra.RayTracer.Samplers;
using Aethra.RayTracer.Samplers.Distributors;
using Aethra.RayTracer.Samplers.Generators;

namespace Aethra.RayTracer.Instructions
{
    public class FourSpheres : IInstruction
    {
        public Scene? Scene { private set; get; }
        public uint[,]? Result => Scene?.Camera.RenderTarget.Pixels;

        public IEnumerable<bool> Render()
        {
            return Scene!.Render();
        }

        public IEnumerable<bool> Render(int startHeight, int endHeight, int startWidth, int endWidth)
        {
            return Scene!.Render(startHeight, endHeight, startWidth, endWidth);
        }

        public void CreateScene(int width, int height, FloatColor color, bool useAntialiasing)
        {
            var renderTarget = new Framebuffer(width, height);
            
            renderTarget.Clear(color);

            var objects = new List<IHittable>
            {
                new Plane(new Vector3(0, -2, 0), new Vector3(0, 1, 0),
                    new ReflectiveMaterial(FloatColor.White, 0.4f, 0, 1000, 0.6f, CheckerTexture.Create(FloatColor.Green, FloatColor.Black).ToInfo())),
                new Sphere(new Vector3(-3.5f, 0, 0), 2,
                    new ReflectiveMaterial(FloatColor.UnityYellow, 0.7f, 0.5f, 1000, 0.3f)),
                new Sphere(new Vector3(3.5f, 0, 0), 2,
                    new ReflectiveMaterial(Color.GreenYellow.ToFloatColor(), 0.7f, 0.5f, 1000, 0.3f)),
                new Sphere(new Vector3(0, 0, 3.5f), 2,
                    new ReflectiveMaterial(Color.DeepSkyBlue.ToFloatColor(), 0.7f, 0.5f, 1000, 0.3f)),
                new Sphere(new Vector3(0, 0, -3.5f), 2,
                    new TransparentMaterial(Color.Red.ToFloatColor(), 0.1f, 0, 0.3f, 1.05f, 0.9f)),
            };


            var sampler = new Sampler(new RegularGenerator(), new SquareDistributor(), 25, 1);
            var camera = new PinholeCamera(renderTarget, new Vector3(6, 2, -15),
                new Vector3(0, 0.3f, 0), new Vector3(0, -1, 0),
                new Vector2(0.7f, 0.7f * height / width), 2)
            {
                Sampler = sampler,
                MaxDepth = 5
            };

            Scene = new Scene(objects, camera,
                new List<Light>
                {
                    new PointLight
                    {
                        Position = new Vector3(-5, 5, -3),
                        Color = FloatColor.White,
                        Intensity = 2
                    },
                },
                FloatColor.Black);
        }
    }
}