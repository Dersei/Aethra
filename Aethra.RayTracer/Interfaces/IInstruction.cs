using System;
using System.Collections.Generic;
using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Rendering;

namespace Aethra.RayTracer.Interfaces
{
    public interface IInstruction
    {
        Scene? Scene { get; }
        uint[,]? Result { get; }
        IEnumerable<bool> Render();
        void AddObject(IHittable hittable) => throw new NotImplementedException(nameof(AddObject));

        IEnumerable<bool> Render(int startHeight, int endHeight, int startWidth, int endWidth)
        {
            yield break;
        }
        void CreateScene(int width, int height, FloatColor color, bool useAntialiasing);
    }
}