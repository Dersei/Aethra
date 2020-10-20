using System;
using System.Drawing;
using Aethra.RayTracer.Extensions;

namespace Aethra.RayTracer.Basic.Textures.Generators
{
    public enum TextureType
    {
        Wood,
        Metal,
        Clouds,
        Marble,
        Sand,
        WoodStripes
    }
    
    public class ProceduralGenerator
    {
        public readonly TextureType TextureType;
        public int NoiseLayer = 0;
        public int TurbolenceSize = 8;
        public float TurbolenceScaleX = 4.0f;
        public float TurbolenceScaleY = 4.0f;
        public float TurbolenceStrength = 1.0f;
        public int Width;
        public int Height;
        public float Scale = 1.0f;
        public FloatColor FromColor = FloatColor.White;
        public FloatColor ToColor = FloatColor.Black;
        public float Cutoff = -1.0f;

        public float XOrg;
        public float YOrg;

        private Bitmap? _noiseTex;
        private TextureType _lastSelectedType;


        private readonly PerlinNoise _noise = new PerlinNoise();

        public ProceduralGenerator(TextureType textureType, int width = 512, int height = 512)
        {
            TextureType = textureType;
            Width = width;
            Height = height;
        }

        private float Turbulence(float x, float y, float size)
        {
            var value = 0.0f;
            var initialSize = size;

            while (size >= 1)
            {
                value += _noise.Get(x / size, y / size) * size;
                size /= 2.0f;
            }

            return value / initialSize;
        }

        public Bitmap Generate()
        {
            if (_noiseTex == null)
            {
                if (_lastSelectedType != TextureType)
                {
                    // Set good default values
                    if (TextureType == TextureType.Marble)
                    {
                        Scale = 0.15f;
                        TurbolenceSize = 9;
                        TurbolenceStrength = 2.8f;
                    }
                    else if (TextureType == TextureType.Wood)
                    {
                        Scale = 294f;
                        TurbolenceSize = 6;
                        TurbolenceScaleX = 1.07f;
                        TurbolenceScaleY = 0.19f;
                        TurbolenceStrength = 11.4f;
                        FromColor = new FloatColor(223f / 255f, 188f / 255f, 112f / 255f);
                        ToColor = new FloatColor(169f / 255f, 122f / 255f, 60f / 255f);
                    }
                    else if (TextureType == TextureType.Sand)
                    {
                        TurbolenceSize = 2;
                        TurbolenceStrength = 1.0f;
                    }

                    _lastSelectedType = TextureType;
                }

                _noiseTex = new Bitmap(Width, Height);

                for (var y = 0; y < _noiseTex.Height; y++)
                {
                    for (var x = 0; x < _noiseTex.Width; x++)
                    {
                        if (TextureType == TextureType.Marble)
                        {
                            var xCoord = XOrg + x / (float) _noiseTex.Width * Scale;
                            var yCoord = YOrg + y / (float) _noiseTex.Height * Scale;

                            var xyValue = xCoord + yCoord +
                                          Turbulence(x * Scale, y * Scale, TurbolenceSize) * TurbolenceStrength;
                            var sample = MathF.Abs(MathF.Sin(xyValue));

                            var fromRatio = sample;
                            var toRatio = 1.0f - sample;
                            _noiseTex.SetPixel(x, y, (new FloatColor(FromColor.R * fromRatio, FromColor.G * fromRatio,
                                FromColor.B * fromRatio) + new FloatColor(ToColor.R * toRatio,
                                ToColor.G * toRatio,
                                ToColor.B * toRatio)).ToDrawingColor());
                        }
                        else if (TextureType == TextureType.Wood)
                        {
                            var xCoord = XOrg + x / (float) _noiseTex.Width * Scale;
                            //float yCoord = yOrg + y / (float)noiseTex.height * scale;

                            var xyValue = xCoord + Turbulence(x / (float) _noiseTex.Width * TurbolenceScaleX * Scale,
                                y / (float) _noiseTex.Height * TurbolenceScaleY * Scale,
                                TurbolenceSize) * TurbolenceStrength;
                            var sample = MathF.Sin(xyValue);

                            var fromRatio = sample;
                            var toRatio = 1.0f - sample;
                            _noiseTex.SetPixel(x, y, (new FloatColor(FromColor.R * fromRatio, FromColor.G * fromRatio,
                                                         FromColor.B * fromRatio, FromColor.A * fromRatio) +
                                                     new FloatColor(ToColor.R * toRatio, ToColor.G * toRatio,
                                                         ToColor.B * toRatio,
                                                         ToColor.A * toRatio)).ToDrawingColor());
                        }
                        else if (TextureType == TextureType.Sand)
                        {
                            var xCoord = XOrg + x / (float) _noiseTex.Width * Scale;
                            var yCoord = YOrg + y / (float) _noiseTex.Height * Scale;

                            var sample = TurbolenceSize <= 1
                                ? _noise.Get(xCoord, yCoord)
                                : Turbulence(xCoord, yCoord, TurbolenceSize) * TurbolenceStrength;
                            var fromRatio = sample;
                            var toRatio = 1.0f - sample;
                            _noiseTex.SetPixel(x, y, (new FloatColor(FromColor.R * fromRatio, FromColor.G * fromRatio,
                                FromColor.B * fromRatio) + new FloatColor(ToColor.R * toRatio,
                                ToColor.G * toRatio,
                                ToColor.B * toRatio)).ToDrawingColor());
                        }
                        else if (TextureType == TextureType.WoodStripes)
                        {
                            var xCoord = XOrg + x / (float) _noiseTex.Width * Scale;
                            //float yCoord = yOrg + y / (float)noiseTex.height * scale;

                            var xyValue = xCoord + Turbulence(x / (float) _noiseTex.Width * TurbolenceScaleX * Scale,
                                y / (float) _noiseTex.Height * TurbolenceScaleY * Scale,
                                TurbolenceSize) * TurbolenceStrength;
                            var sample = MathF.Sin(xyValue);

                            if (Cutoff > 0.0f)
                                sample = (sample - Cutoff) / (1.0f - Cutoff);
                            var fromRatio = sample;
                            var toRatio = 1.0f - sample;

                            _noiseTex.SetPixel(x, y, (new FloatColor(FromColor.R * fromRatio, FromColor.G * fromRatio,
                                                         FromColor.B * fromRatio, FromColor.A * fromRatio) +
                                                     new FloatColor(ToColor.R * toRatio, ToColor.G * toRatio,
                                                         ToColor.B * toRatio,
                                                         ToColor.A * toRatio)).ToDrawingColor());
                        }
                    }
                }
            }

            return _noiseTex;
        }
    }
}