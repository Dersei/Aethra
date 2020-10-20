using System.Collections.Generic;
using System.Drawing;
using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Basic.Materials;
using Aethra.RayTracer.Basic.Textures;
using Aethra.RayTracer.Cameras;
using Aethra.RayTracer.Extensions;
using Aethra.RayTracer.Interfaces;
using Aethra.RayTracer.Lighting;
using Aethra.RayTracer.Models;
using Aethra.RayTracer.Primitives;
using Aethra.RayTracer.Rendering;
using Aethra.RayTracer.Samplers;
using Aethra.RayTracer.Samplers.Distributors;
using Aethra.RayTracer.Samplers.Generators;

namespace Aethra.RayTracer.Instructions
{
    public class ThirteenthInstruction : IInstruction
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

        public void AddObject(IHittable hittable)
        {
            Objects.Add(hittable);
        }
        
        public List<IHittable> Objects = new List<IHittable>
        {
            new Sphere(new Vector3(1, 0, 0), 0.5f, 
                new TransparentMaterial(FloatColor.Green, 0.1f, 0, 0.3f, 1.05f, 0.9f)),
            new Sphere(new Vector3(-1, 0, 0), 0.5f, 
                // new PhongMaterial(FloatColor.Green)),
                new ReflectiveMaterial(FloatColor.Green, 0.2f, 0.5f, 1000, 0.6f)),
            new Plane(new Vector3(0, 0, 1), Vector3.Back,
                new ReflectiveMaterial(FloatColor.White, 0.2f, 0.5f, 1000, 0.3f)),
            new Plane(new Vector3(0, -0.5f, 0), Vector3.Up,
                new ReflectiveMaterial(FloatColor.White, 0.2f, 0.5f, 1000, 0.3f))
        };

        public void CreateScene(int width, int height, FloatColor color, bool useAntialiasing)
        {
            var renderTarget = new Framebuffer(width, height);

            var sampler = new Sampler(new RegularGenerator(), new SquareDistributor(), 25, 1);
            var camera = new PerspectiveCamera(renderTarget, new Vector3(0f, 0, -5), Vector3.Forward, Vector3.Up)
            {
                //Sampler = sampler,
                MaxDepth = 5,
                ColorSpace = ColorSpace.SRGB
            };

            var orthoCamera = new OrthogonalCamera(renderTarget, new Vector3(0f, 0, -5), Vector3.Forward, Vector3.Up)
            {
                //Sampler = sampler,
                MaxDepth = 5,
                ColorSpace = ColorSpace.SRGB
            };
            Scene = new Scene(Objects, camera,
                new List<Light>
                {
                    new PointLight
                    {
                        Position = new Vector3(0, 0, 0),
                        Color = FloatColor.White,
                        Intensity = 1,
                        Radius = 1,
                        //Sampler = sampler
                    },
                    // new AreaLight
                    // {
                    //     Position = Vector3.Zero,
                    //     Color = FloatColor.White,
                    //     Intensity = 1,
                    //     Radius = 0.1f
                    // }
                    new DirectionalLight
                    {
                        Direction = Vector3.Down,
                        Color = FloatColor.Green
                    },
                    // new DirectionalLight
                    // {
                    //     Direction = Vector3.Forward,
                    //     Color = FloatColor.Purple
                    // },
                    // new AmbientLight()
                    // {
                    //     Color = FloatColor.White,
                    //     Intensity = 0.3f
                    // }
                },
                FloatColor.Black);
        }
    }
}