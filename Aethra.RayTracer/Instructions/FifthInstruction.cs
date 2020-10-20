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

namespace Aethra.RayTracer.Instructions
{
    public class FifthInstruction : IInstruction
    {
        public Scene? Scene { private set; get; }
        public uint[,]? Result => Scene?.Camera.RenderTarget.Pixels;

        public IEnumerable<bool> Render()
        {
            return Scene!.Render();
        }

        public void CreateScene(int width, int height, FloatColor color, bool useAntialiasing)
        {
            var renderTarget = new Framebuffer(width, height);
            renderTarget.Clear(color);
            var camera = new PerspectiveCamera(renderTarget, new Vector3(0f, 0, -10), Vector3.Forward, Vector3.Up);
            var objects = new List<IHittable>();
            var blueMaterial = new PhongMaterial(FloatColor.White, 1f, 8, 50, 0f,
                Texture.LoadFrom(@"_Resources/Textures/circuitry-albedo.png").ToInfo(3));
            var whiteMaterial = new PhongMaterial(FloatColor.White, 1f, 8, 50, 0f,
                Texture.LoadFrom(@"_Resources/Textures/sun.png").ToInfo());
            var sphere1 = new Sphere(new Vector3(2.5f, -1, 0), 0.5f, blueMaterial);
            var sphere2 = new Sphere(new Vector3(2.5f, -2.5f, 0), 0.75f, whiteMaterial);
            objects.Add(sphere1);
            objects.Add(sphere2);
            var modelMaterial = new PhongMaterial(FloatColor.White, 1f, 8, 50, 1f,
                Texture.LoadFrom(@"_Resources/Textures/texel_density.png").ToInfo());
            var model = Model.LoadFromFile("_Resources/Models/lowpolytree_unwrap.obj", modelMaterial, 1, Vector3.Down);
            var crystalMaterial = new PhongMaterial(FloatColor.White, 1f, 8, 50, 1f,
                Texture.LoadFrom(@"_Resources/Textures/crystal.png").ToInfo());
            var crystal = Model.LoadFromFile("_Resources/Models/crystal.obj", crystalMaterial, 3, Vector3.Left * 2);
            foreach (var triangle in model)
            {
                objects.Add(triangle);
            }
            foreach (var triangle in crystal)
            {
                objects.Add(triangle);
            }

            Scene = new Scene(objects, camera,
                new List<Light>
                {
                    //new PointLight {Position = new Vector(1, 2f, 0), Color = Color.White},
                    //new PointLight {Position = new Vector(-2, 2f, 0), Color = Color.White},
                    new PointLight {Position = new Vector3(-4, 2f, -3), Color = FloatColor.White},
                    //new PointLight {Position = new Vector(-2, -1f, 0), Color = Color.Green},
                }, FloatColor.Black);
        }
    }
}