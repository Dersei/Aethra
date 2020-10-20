using System;
using Aethra.RayTracer.Instructions;
using Aethra.RayTracer.Interfaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Skia;
using SkiaSharp;

namespace Aethra
{
    public class CursorGraphic : UserControl
    {
        private RenderTargetBitmap? _renderTarget;
        private ISkiaDrawingContextImpl? _skiaContext;
        private SKCanvas? _canvas;
        
        public override void EndInit()
        {
            _renderTarget =
                new RenderTargetBitmap(new PixelSize((int) Width, (int) Height), new Vector(96, 96));
            var context = _renderTarget.CreateDrawingContext(null);
            _skiaContext = context as ISkiaDrawingContextImpl;
            if (_skiaContext is null) throw new NullReferenceException(nameof(_skiaContext));
            _canvas = _skiaContext.SkCanvas;
            _canvas.Clear(SKColors.Empty);
            if (_canvas is null) throw new NullReferenceException(nameof(_canvas));
            //IsHitTestVisible = false;
            PointerMoved += OnPointerMoved;
            base.EndInit();
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            _canvas.Clear(SKColor.Empty);
            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Red,
                StrokeWidth = 1
            };
            var point = e.GetPosition(this);
            _canvas.DrawCircle((float) point.X, (float) point.Y, 40, paint);
            InvalidateVisual();
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