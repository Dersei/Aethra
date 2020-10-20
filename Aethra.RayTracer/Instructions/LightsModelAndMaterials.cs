using System.Collections.Generic;
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
    public class LightsModelAndMaterials : IInstruction
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
            var objects = new List<IHittable>();

            var reflectiveMaterial = new ReflectiveMaterial(FloatColor.White, 0.4f, 1, 300, 1f);
            var transparentMaterial = new TransparentMaterial(FloatColor.White, 0.1f, 0, 1, 3, 1);
            var redWallMaterial = new PhongMaterial(FloatColor.Red, 1, 0, 50, 1);
            var greenWallMaterial = new PhongMaterial(FloatColor.Green, 1, 0, 50, 1);
            var whiteWallMaterial = new PhongMaterial(FloatColor.White, 1, 0, 50, 1);
            var emissiveMaterial = new EmissiveMaterial(FloatColor.Green, 2,
                Texture.LoadFrom(@"_Resources/Textures/texel_density.png").ToInfo());

            var reflectiveSphere = new Sphere(new Vector3(-1.25f, -1, 3), 1f, reflectiveMaterial);
            var transparentSphere = new Sphere(new Vector3(1.25f, -1, 1), 1f, transparentMaterial);

            objects.Add(new Plane(new Vector3(-4, 0, 0), new Vector3(1, 0, 0), redWallMaterial));
            objects.Add(new Plane(new Vector3(4, 0, 0), new Vector3(-1, 0, 0), greenWallMaterial));
            objects.Add(new Plane(new Vector3(5, -2, 0), new Vector3(0, 1, 0), whiteWallMaterial));
            objects.Add(new Plane(new Vector3(5, 2, 0), new Vector3(0, -1, 0), whiteWallMaterial));
            objects.Add(new Plane(new Vector3(0, 2, 6), new Vector3(0, 0, -1), whiteWallMaterial));
            objects.Add(new Plane(new Vector3(0, 2, -8), new Vector3(0, 0, 1), whiteWallMaterial));
            objects.Add(reflectiveSphere);
            objects.Add(transparentSphere);

            // var crystalMaterial = new PhongMaterial(FloatColor.White, 1f, 8, 50, 1f,
            //     Texture.LoadFrom(@"_Resources/Textures/crystal.png").ToInfo());
            var crystalMaterial = new PbrMaterial(FloatColor.White, 
                Texture.LoadFrom(@"_Resources/Textures/crystal.png").ToInfo(),
                Texture.LoadFrom(@"_Resources/Textures/crystal.png").ToInfo(),
                null,
                Texture.LoadFrom(@"_Resources/Textures/crystal-normals.png").ToInfo());
            var crystal = Model.LoadFromFile("_Resources/Models/crystal.obj", crystalMaterial, 2f,
                Vector3.Zero);


            objects.Add(crystal);


            var sampler = new Sampler(new JitteredGenerator(0), new SquareDistributor(), 64, 64);
            var camera = new PerspectiveCamera(renderTarget, new Vector3(0f, 0, -6), Vector3.Forward, Vector3.Up)
            {
                Sampler = sampler,
                MaxDepth = 4
            };

            Scene = new Scene(objects, camera,
                new List<Light>
                {
                    new PointLight
                    {
                        Position = new Vector3(0, 1.98f, 2),
                        Color = FloatColor.White,
                        // Sampler = sampler
                    },
                    new PointLight
                    {
                        Position = new Vector3(-1, 0f, 0), 
                        Color = FloatColor.Blue, 
                        Sampler = sampler
                    },
                    new PointLight
                    {
                        Position = new Vector3(1, 1f, 0), 
                        Color = FloatColor.Purple, 
                        Sampler = sampler
                    },
                    new PointLight
                    {
                        Position = new Vector3(-2, -1f, 0), 
                        Color = FloatColor.Green, 
                        Sampler = sampler
                    },
                }, FloatColor.Black);
        }
    }
}