using Avalonia.Media;
using ReactiveUI;

namespace Aethra
{
    public class DrawingDataContext : ReactiveObject
    {
        public float PositionZ { get; set; }

        public byte R
        {
            get => _r;
            set
            {
                _r = value;
                CreatedColor = new SolidColorBrush(new Color(255, _r,_g,_b));
            } 
        }

        public byte G
        {
            get => _g;
            set
            {
                _g = value;
                CreatedColor = new SolidColorBrush(new Color(255, _r,_g,_b));
            } 
        }

        public byte B
        {
            get => _b;
            set
            {
                _b = value;
                CreatedColor = new SolidColorBrush(new Color(255, _r,_g,_b));
            } 
        }

        private IBrush? _createdColor;
        private byte _r;
        private byte _g = 255;
        private byte _b;

        public IBrush CreatedColor
        {
            get => _createdColor!;
            set => this.RaiseAndSetIfChanged(ref _createdColor, value);
        }
        
        public DrawingDataContext()
        {
            CreatedColor = new SolidColorBrush(new Color(255, _r,_g,_b));
            Instance = this;
        }
        
        public static DrawingDataContext Instance { get; private set; } = null!;
    }
}