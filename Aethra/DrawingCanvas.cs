using System;
using System.IO;
using System.Threading.Tasks;
using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Basic.Materials;
using Aethra.RayTracer.Extensions;
using Aethra.RayTracer.Instructions;
using Aethra.RayTracer.Interfaces;
using Aethra.RayTracer.Primitives;
using Aethra.RayTracer.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Skia;
using MessageBox.Avalonia;
using SkiaSharp;
using Color = Aethra.RayTracer.Basic.FloatColor;
using Vector = Avalonia.Vector;

namespace Aethra
{
    public class DrawingCanvas : UserControl
    {
        private RenderTargetBitmap? _renderTarget;
        private ISkiaDrawingContextImpl? _skiaContext;
        private IInstruction? _instruction;
        private SKCanvas? _canvas;

        public override void EndInit()
        {
            _renderTarget =
                new RenderTargetBitmap(new PixelSize((int) Width, (int) Height), new Vector(96, 96));
            var context = _renderTarget.CreateDrawingContext(null);
            _skiaContext = context as ISkiaDrawingContextImpl;
            if (_skiaContext is null) throw new NullReferenceException(nameof(_skiaContext));
            _canvas = _skiaContext.SkCanvas;
            _canvas.Clear(SKColors.Black);
            if (_canvas is null) throw new NullReferenceException(nameof(_canvas));
            _instruction ??= new CrystalInSphere();
            PointerWheelChanged += OnPointerWheelChanged;
            base.EndInit();
        }

        private float _radius = 40;

        private void ClampRadius(float value)
        {
            _radius = _radius + value > 150 ? 150 : _radius + value < 1 ? 1 : _radius + value;
        }

        private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            if (e.Delta.Y > 0) ClampRadius(1);
            if (e.Delta.Y < 0) ClampRadius(-1);
        }

        private (float midX, float midY) GetCenter(float x, float y)
        {
            var ratio = (float) Width / (float) Height;
            x = (-1.0f + (x + 0.5f) * (float) (2.0f / Width)) * ratio;
            y = 1.0f - (y + 0.5f) * (float) (2.0f / Height);
            return (x*MathF.Tan(60 * MathExtensions.Deg2Rad), y*MathF.Tan(60 * MathExtensions.Deg2Rad));
        }

        private float GetRadius(float x)
        {
            var x2 = x / (float) Height;
            return x2 *4;
        }

        private void DrawCircle(PointerPressedEventArgs e)
        {
            if (_canvas is null) throw new ArgumentNullException(nameof(_canvas));
            if (_instruction is null) throw new ArgumentNullException(nameof(_instruction));
            var positionZ = DrawingDataContext.Instance.PositionZ;
            var r = DrawingDataContext.Instance.R;
            var g = DrawingDataContext.Instance.G;
            var b = DrawingDataContext.Instance.B;
            SKPaint paintFill = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(r,g,b),
            };

            var point = e.GetPosition(this);
            _canvas.DrawCircle((float) point.X, (float) point.Y, _radius, paintFill);
            if (positionZ.IsNotZero())
            {
                SKPaint paintStroke = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.White,
                    StrokeWidth = MathF.Abs(positionZ)
                };
                
                _canvas.DrawCircle((float) point.X, (float) point.Y, _radius - positionZ, paintStroke);
            }

            var xy = GetCenter((float) (point.X), (float) (point.Y));
            var center = new Vector3(xy.midX, xy.midY, positionZ);
            var floatColor = new FloatColor(r/255f,g/255f,b/255f);
            _instruction.AddObject(new Sphere(center,
                GetRadius(_radius), new ReflectiveMaterial(floatColor, 0.2f, 0.5f, 1000, 0.3f)));
            InvalidateVisual();
        }

        private SKBitmap? _bitmap;

        public async Task Draw(int width, int height)
        {
            var bitmap = new SKBitmap(width, height);
            if (width != (int) Width || height != (int) Height)
            {
                const double dpi = 96.0;
                const double scale = dpi / 96;
                _renderTarget = new RenderTargetBitmap(new PixelSize((int) (width * scale), (int) (height * scale)),
                    new Vector(dpi, dpi));
                var context = _renderTarget.CreateDrawingContext(null);
                _skiaContext = context as ISkiaDrawingContextImpl;
            }

            await RenderResult(bitmap);
            _skiaContext!.SkCanvas.DrawBitmap(bitmap, new SKPoint(0, 0));
            InvalidateVisual();
        }

        protected override async void OnPointerPressed(PointerPressedEventArgs e)
        {
            var pointerUpdateKind = e.GetCurrentPoint(null).Properties.PointerUpdateKind;

            switch (pointerUpdateKind)
            {
                case PointerUpdateKind.RightButtonPressed:
                    await SaveAsync($"output-raytracer{DateTime.Now:yyyy-dd-M--HH-mm-ss.fff}.png");
                    break;
                case PointerUpdateKind.LeftButtonPressed:
                    DrawCircle(e);
                    break;
            }

            base.OnPointerPressed(e);
        }

        private async Task RenderResult(SKBitmap bitmap)
        {
            Timer.Start();
            _instruction ??= new TwelfthInstruction();
            _instruction.CreateScene(bitmap.Width, bitmap.Height, FloatColor.Black, true);

            await Task.Run(() =>
            {
                foreach (var isRunning in _instruction.Render())
                {
                    if (isRunning)
                    {
                        Assign(bitmap, _instruction);
                        _skiaContext!.SkCanvas.DrawBitmap(bitmap, new SKPoint(0, 0));
                        InvalidateVisual();
                    }
                    else
                    {
                        break;
                    }
                }
            });
        
            Assign(bitmap, _instruction);
            _skiaContext!.SkCanvas.DrawBitmap(bitmap, new SKPoint(0, 0));
            InvalidateVisual();
            Timer.Stop(displayResult: DisplayTime);
            _bitmap = bitmap;
        }

        private static void DisplayTime(string info)
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow("Time", info);
            messageBoxStandardWindow.Show();
        }

        private static unsafe void Assign(SKBitmap bitmap, IInstruction instruction)
        {
            fixed (uint* ptr = instruction.Result)
            {
                bitmap.SetPixels((IntPtr) ptr);
            }
        }


        private void SaveBitmap()
        {
            // create an image and then get the PNG (or any other) encoded data
            using var image = SKImage.FromBitmap(_bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            // save the data to a stream
            using var stream = File.OpenWrite($"raw-output-raytracer{DateTime.Now:yyyy-dd-M--HH-mm-ss.fff}.png");
            data.SaveTo(stream);
        }

        private Task<bool> SaveAsync(string path)
        {
            return Task.Run(() =>
            {
                try
                {
                    _renderTarget?.Save(path);
                    SaveBitmap();
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            });
        }

        public override void Render(DrawingContext context)
        {
            context.DrawImage(_renderTarget, 1.0,
                new Rect(0, 0, _renderTarget?.PixelSize.Width ?? 100, _renderTarget?.PixelSize.Height ?? 100),
                new Rect(0, 0, Width, Height)
            );
        }
    }
}