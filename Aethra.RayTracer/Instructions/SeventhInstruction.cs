using System.Collections.Generic;
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
    public class SeventhInstruction : IInstruction
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
            // var redWallMaterial = new PhongMaterial(FloatColor.Red, 1, 0, 50, 1);
            // var greenWallMaterial = new PhongMaterial(FloatColor.Green, 1, 0, 50, 1);
            // var whiteWallMaterial = new PhongMaterial(FloatColor.White, 1, 0, 50, 1);
            // var blueMaterial = new PhongMaterial(FloatColor.Blue, 0.7f, 8, 50, 1);
            var reflectiveFloor = new ReflectiveMaterial(FloatColor.White, 0.4f, 1, 300, 0.5f);

            var circuitryMaterial = new PBRMaterial(FloatColor.White,
                Texture.LoadFrom(@"_Resources/Textures/circuitry-albedo.png").ToInfo(3),
                Texture.LoadFrom(@"_Resources/Textures/circuitry-emission.png").ToInfo(3),
                Texture.LoadFrom(@"_Resources/Textures/circuitry-smoothness.png").ToInfo(3),
                Texture.LoadFrom(@"_Resources/Textures/circuitry-normals.png").ToInfo(3))
            {
                EmissionFactor = 2,
                DiffuseCoefficient = 1,
                Specular = 10,
                SpecularExponent = 50,
                AmbientPower = 1
            };

            var sphereTextureMaterial = new PBRMaterial(FloatColor.White,
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

            var reflectiveSphere =   new Sphere(new Vector3(-0.5f, -1.5f, 3), 0.5f, reflectiveMaterial);
            var transparentSphere =  new Sphere(new Vector3(0.5f, -1.5f, 3), 0.5f, transparentMaterial);
            var textureSphere =      new Sphere(new Vector3(-1.5f, -1.5f, 3), 0.5f, sphereTextureMaterial);
            var specialSphere =      new Sphere(new Vector3(1.5f, -1.5f, 3), 0.5f, circuitryMaterial);
            
            var reflectiveSphere1 =  new Sphere(new Vector3(1.5f, -0.5f, 3), 0.5f, reflectiveMaterial);
            var transparentSphere1 = new Sphere(new Vector3(-1.5f, -0.5f, 3), 0.5f, transparentMaterial);
            var textureSphere1 =     new Sphere(new Vector3(-0.5f, -0.5f, 3), 0.5f, sphereTextureMaterial);
            var specialSphere1 =     new Sphere(new Vector3(0.5f, -0.5f, 3), 0.5f, circuitryMaterial);

            var reflectiveSphere2 =  new Sphere(new Vector3(-1.5f, 0.5f, 3), 0.5f, reflectiveMaterial);
            var transparentSphere2 = new Sphere(new Vector3(1.5f, 0.5f, 3), 0.5f, transparentMaterial);
            var textureSphere2 =     new Sphere(new Vector3(0.5f, 0.5f, 3), 0.5f, sphereTextureMaterial);
            var specialSphere2 =     new Sphere(new Vector3(-0.5f, 0.5f, 3), 0.5f, circuitryMaterial);
            
            var reflectiveSphere3 =  new Sphere(new Vector3(0.5f, 1.5f, 3), 0.5f, reflectiveMaterial);
            var transparentSphere3 = new Sphere(new Vector3(-0.5f, 1.5f, 3), 0.5f, transparentMaterial);
            var textureSphere3 =     new Sphere(new Vector3(1.5f, 1.5f, 3), 0.5f, sphereTextureMaterial);
            var specialSphere3 =     new Sphere(new Vector3(-1.5f, 1.5f, 3), 0.5f, circuitryMaterial);
            
            objects.Add(new Plane(new Vector3(-2, 0, 0), new Vector3(1, 0, 0), reflectiveFloor));
            objects.Add(new Plane(new Vector3(2, 0, 0), new Vector3(-1, 0, 0), reflectiveFloor));
            objects.Add(new Plane(new Vector3(5, -2f, 0), new Vector3(0, 1, 0), reflectiveFloor));
            objects.Add(new Plane(new Vector3(5, 2f, 0), new Vector3(0, -1, 0), reflectiveFloor));
            objects.Add(new Plane(new Vector3(0, 2, 6), new Vector3(0, 0, -1), reflectiveFloor));
            objects.Add(new Plane(new Vector3(0, 2, -8), new Vector3(0, 0, 1), reflectiveFloor));
            objects.Add(reflectiveSphere);
            objects.Add(transparentSphere);
            objects.Add(textureSphere);
            objects.Add(specialSphere);
            objects.Add(reflectiveSphere1);
            objects.Add(transparentSphere1);
            objects.Add(textureSphere1);
            objects.Add(specialSphere1);
            objects.Add(reflectiveSphere2);
            objects.Add(transparentSphere2);
            objects.Add(textureSphere2);
            objects.Add(specialSphere2);
            objects.Add(reflectiveSphere3);
            objects.Add(transparentSphere3);
            objects.Add(textureSphere3);
            objects.Add(specialSphere3);

            var sampler = new Sampler(new JitteredGenerator(0), new SquareDistributor(), 16, 32);
            var camera = new PerspectiveCamera(renderTarget, new Vector3(0f, 0, -5), Vector3.Forward, Vector3.Up)
            {
                Sampler = sampler,
                //SpecialFloatColoring = (ray, hit) => FloatColor.FromVector(ray.Direction)
            };

            Scene = new Scene(objects, camera,
                new List<Light>
                {
                    new PointLight
                    {
                        Position = new Vector3(0, 0, 3),
                        Color = FloatColor.White,
                        Sampler = sampler
                    },
                    new PointLight
                    {
                        Position = new Vector3(0, 0, 0),
                        Color = FloatColor.White / 2,
                        Sampler = sampler
                    },
                    // new PointLight
                    // {
                    //     Position = new Vector3(1, 1f, 0), 
                    //     Color = FloatColor.Purple,
                    // //    Sampler = sampler
                    // },
                    // new PointLight
                    // {
                    //     Position = new Vector3(-2, -1f, 0), 
                    //     Color = FloatColor.Green,
                    // //    Sampler = sampler
                    // },
                },
                FloatColor.Black);
        }
    }
}