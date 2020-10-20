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
    public class NinthInstruction : IInstruction
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

            // var reflectiveMaterial = new ReflectiveMaterial(FloatColor.White, 0.4f, 1, 300, 1f);
            var transparentMaterial = new TransparentMaterial(FloatColor.White, 0.1f, 0, 0.3f, 1.05f, 0.9f);
            // var whiteWallMaterial = new PhongMaterial(FloatColor.White, 1, 0, 50, 1);
            // var emissiveMaterial = new EmissiveMaterial(FloatColor.White, 1, Texture.LoadFrom(@"_Resources/Textures/texel_density.png").ToInfo(3));
            // var redWallMaterial = new PhongMaterial(FloatColor.Red, 1, 0, 50, 1);
            var greenWallMaterial = new PhongMaterial(FloatColor.Green, 1, 0, 50, 1);
            // var blueMaterial = new PhongMaterial(FloatColor.Blue, 0.7f, 8, 50, 1);
            
            var circuitryMaterial = new PBRMaterial(FloatColor.White,
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

            // var sphereTextureMaterial = new PBRMaterial(FloatColor.White,
            //     Texture.LoadFrom(@"_Resources/Textures/lava-albedo-smoothness-green.png").ToInfo(3),
            //     Texture.LoadFrom(@"_Resources/Textures/lava-emission-green.png").ToInfo(3),
            //     Texture.LoadFrom(@"_Resources/Textures/lava-albedo-smoothness.png").ToInfo(3),
            //     Texture.LoadFrom(@"_Resources/Textures/lava-normals.png").ToInfo(3))
            // {
            //     EmissionFactor = 4,
            //     DiffuseCoefficient = 1,
            //     Specular = 10,
            //     SpecularExponent = 50,
            //     AmbientPower = 1
            // };
            
            // var bacteriaMaterial = new PBRMaterial(FloatColor.White,
            //     Texture.LoadFrom(@"_Resources/Textures/Bacteria_001_COLOR.jpg").ToInfo(3),
            //     Texture.LoadFrom(@"_Resources/Textures/Bacteria_001_COLOR.jpg").ToInfo(3),
            //     Texture.LoadFrom(@"_Resources/Textures/Bacteria_001_SPEC.jpg").ToInfo(3),
            //     Texture.LoadFrom(@"_Resources/Textures/Bacteria_001_NORM.jpg").ToInfo(3))
            // {
            //     EmissionFactor = 2,
            //     DiffuseCoefficient = 1,
            //     Specular = 10,
            //     SpecularExponent = 50,
            //     AmbientPower = 1
            // };

            var reflectiveSphere =   new Sphere(new Vector3(1f, -1f, 4), 1f, greenWallMaterial);
            var transparentSphere =  new Sphere(new Vector3(0f, -1f, 2), 1f, transparentMaterial);
            // var textureSphere =      new Sphere(new Vector3(-1.5f, -1.5f, 3), 0.5f, sphereTextureMaterial);
            // var testSphere =      new Sphere(new Vector3(0f, 0f, 1), 1f, emissiveMaterial);
            // var specialSphere =      new DistortedSphere(new Vector3(0f, 0f, 1), 1f,
                // Texture.LoadFrom(@"_Resources/Textures/Bacteria_001_DISP.png").ToInfo(3), 2, emissiveMaterial);

            //objects.Add(new Plane(new Vector3(-2, 0, 0), new Vector3(1, 0, 0), whiteWallMaterial));
            //objects.Add(new Plane(new Vector3(2, 0, 0), new Vector3(-1, 0, 0), redWallMaterial));
            objects.Add(new Plane(new Vector3(5, -2f, 0), new Vector3(0, 1, 0), circuitryMaterial));
            //objects.Add(new Plane(new Vector3(5, 2f, 0), new Vector3(0, -1, 0), blueMaterial));
            //objects.Add(new Plane(new Vector3(0, 2, 6), new Vector3(0, 0, -1), redWallMaterial));
            //objects.Add(new Plane(new Vector3(0, 2, -8), new Vector3(0, 0, 1), greenWallMaterial));
            objects.Add(reflectiveSphere);
            objects.Add(transparentSphere);
            //objects.Add(textureSphere);
            //objects.Add(specialSphere);
            //objects.Add(testSphere);

            // var sampler = new Sampler(new JitteredGenerator(0), new SquareDistributor(), 16, 32);
            var camera = new PerspectiveCamera(renderTarget, new Vector3(0f, 0, -5), Vector3.Forward, Vector3.Up)
            {
                //Sampler = sampler,
                //SpecialColoring = (ray, hit) => FloatColor.FromNormal(hit.Normal)
            };

            Scene = new Scene(objects, camera,
                new List<Light>
                {
                    new PointLight
                    {
                        Position = new Vector3(1, 1, 0),
                        Color = FloatColor.White,
                        // Sampler = sampler
                    },
                    new PointLight
                    {
                        Position = new Vector3(-1, 1, 0),
                        Color = FloatColor.White,
                        // Sampler = sampler
                    },
                    // new PointLight
                    // {
                    //     Position = new Vector3(0, 0, -4),
                    //     Color = FloatColor.White,
                    //    // Sampler = sampler
                    // },
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