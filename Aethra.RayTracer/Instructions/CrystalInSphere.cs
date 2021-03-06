﻿using System.Collections.Generic;
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
    public class CrystalInSphere : IInstruction
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

            var reflectiveFloor = new ReflectiveMaterial(FloatColor.White, 0.4f, 1, 300, 0.5f);

            var crystalMaterial = new PbrMaterial(FloatColor.White,
                Texture.LoadFrom(@"_Resources/Textures/crystal-green.png").ToInfo(1),
                Texture.LoadFrom(@"_Resources/Textures/crystal-green.png").ToInfo(1),
                Texture.LoadFrom(@"_Resources/Textures/crystal-roughness.png").ToInfo(1),
                Texture.LoadFrom(@"_Resources/Textures/crystal-normals.png").ToInfo(1))
            {
                EmissionFactor = 1,
                DiffuseCoefficient = 1,
                Specular = 10,
                SpecularExponent = 50,
                AmbientPower = 1
            };

            var reflectiveCrystal = new ReflectiveMaterial(FloatColor.White, 0.7f, 0.5f, 1000, 0.6f);
            var transparentCrystal = new TransparentMaterial(FloatColor.White, 0.1f, 0, 0.3f, 1.05f, 0.9f);
            var transparentSphere = new TransparentMaterial(FloatColor.White, 0.1f, 0, 0.3f, 1.05f, 0.9f);

            var crystal = Model.LoadFromFile("_Resources/Models/crystal.obj", crystalMaterial, 1.5f);
            var crystal2 = Model.LoadFromFile("_Resources/Models/crystal.obj", reflectiveCrystal, 1.5f,
                Vector3.Left * 1.5f, Vector3.Forward, 45);
            var crystal3 = Model.LoadFromFile("_Resources/Models/crystal.obj", transparentCrystal, 1.5f,
                Vector3.Right * 1.5f, Vector3.Forward, -45);
            renderTarget.Clear(color);

            var objects = new List<IHittable>
            {
                new Plane(new Vector3(-2, 0, 0), new Vector3(1, 0, 0), reflectiveFloor),
                new Plane(new Vector3(2, 0, 0), new Vector3(-1, 0, 0), reflectiveFloor),
                new Plane(new Vector3(5, -2f, 0), new Vector3(0, 1, 0), reflectiveFloor),
                new Plane(new Vector3(5, 2f, 0), new Vector3(0, -1, 0), reflectiveFloor),
                new Plane(new Vector3(0, 2, 6), new Vector3(0, 0, -1), reflectiveFloor),
                new Plane(new Vector3(0, 2, -8), new Vector3(0, 0, 1), reflectiveFloor),
                new Sphere(Vector3.Zero, 1, transparentSphere)
            };
            
            objects.Add(crystal);
            objects.Add(crystal2);
            objects.Add(crystal3);
            
            var sampler = new Sampler(new RegularGenerator(), new SquareDistributor(), 25, 1);
            var camera = new PerspectiveCamera(renderTarget, new Vector3(0f, 0, -5), Vector3.Forward, Vector3.Up)
            {
                Sampler = sampler,
                MaxDepth = 5,
            };

            Scene = new Scene(objects, camera,
                new List<Light>
                {
                    new PointLight
                    {
                        Position = new Vector3(0, 0, 5),
                        Color = FloatColor.White,
                        Intensity = 1
                    }
                },
                FloatColor.Black);
        }
    }
}