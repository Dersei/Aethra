using System.Collections.Generic;
using System.Drawing;
using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Basic.Materials;
using Aethra.RayTracer.Basic.Textures;
using Aethra.RayTracer.Basic.Textures.Generators;
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
    public class CircuitFloor : IInstruction
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
            
            var sierpinsky = Texture.CreateFrom(FractalGenerator.SierpinskyCarpet(100, 100, Color.White, Color.Black))
                .ToInfo();
            var reflectiveMaterial = new ReflectiveMaterial(FloatColor.White, 0.4f, 1, 300, 1f, sierpinsky);
            var transparentMaterial = new TransparentMaterial(FloatColor.White, 0.1f, 0, 0.3f, 1.05f, 0.9f);

            var circuitryMaterial = new PbrMaterial(FloatColor.White,
                Texture.LoadFrom(@"_Resources/Textures/circuitry-albedo.png").ToInfo(0.25f),
                Texture.LoadFrom(@"_Resources/Textures/circuitry-emission.png").ToInfo(0.25f),
                Texture.LoadFrom(@"_Resources/Textures/circuitry-smoothness.png").ToInfo(0.25f),
                Texture.LoadFrom(@"_Resources/Textures/circuitry-normals.png").ToInfo(0.25f))
            {
                EmissionFactor = 2,
                DiffuseCoefficient = 1,
                Specular = 10,
                SpecularExponent = 50,
                AmbientPower = 1
            };

            var greenLavaMaterial = new PbrMaterial(FloatColor.White,
                Texture.LoadFrom(@"_Resources/Textures/lava-albedo-smoothness-green.png").ToInfo(3),
                Texture.LoadFrom(@"_Resources/Textures/lava-emission-green.png").ToInfo(3),
                Texture.LoadFrom(@"_Resources/Textures/lava-albedo-smoothness.png").ToInfo(3),
                Texture.LoadFrom(@"_Resources/Textures/lava-normals.png").ToInfo(3))
            {
                EmissionFactor = 4,
                DiffuseCoefficient = 1,
                Specular = 10,
                SpecularExponent = 50,
                AmbientPower = 1
            };

            var reflectiveSphere = new Sphere(new Vector3(2f, -1f, 4), 1f, reflectiveMaterial);
            var transparentSphere = new Sphere(new Vector3(0f, -1f, 2), 1f, transparentMaterial);
            var textureSphere = new Sphere(new Vector3(-1.5f, -1.5f, 3), 0.5f, greenLavaMaterial);
            
            objects.Add(reflectiveSphere);
            objects.Add(transparentSphere);
            objects.Add(textureSphere);
            objects.Add(new Plane(new Vector3(5, -2f, 0), new Vector3(0, 1, 0), circuitryMaterial));

            var sampler = new Sampler(new JitteredGenerator(0), new SquareDistributor(), 16, 32);
            var camera = new PerspectiveCamera(renderTarget, new Vector3(0f, 0, -5), Vector3.Forward, Vector3.Up)
            {
                Sampler = sampler,
                MaxDepth = 6,
            };

            Scene = new Scene(objects, camera,
                new List<Light>
                {
                    new PointLight
                    {
                        Position = new Vector3(1, 1, 0),
                        Color = FloatColor.White,
                    },
                    new PointLight
                    {
                        Position = new Vector3(-1, 1, 0),
                        Color = FloatColor.White,
                    },
                    new PointLight
                    {
                        Position = new Vector3(-2, 2, 15),
                        Color = FloatColor.White,
                        Intensity = 1
                    },
                    new PointLight
                    {
                        Position = new Vector3(1, 1f, 0),
                        Color = FloatColor.Green,
                    },
                    new PointLight
                    {
                        Position = new Vector3(-2, -1f, 0),
                        Color = FloatColor.Red,
                    },
                },
                FloatColor.Black);
        }
    }
}