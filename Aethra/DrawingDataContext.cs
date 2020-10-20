using Avalonia.Media;
using ReactiveUI;

namespace Aethra
{
    public class DrawingDataContext : ReactiveObject
    {
        public float PositionZ { get; set; } = 0;

        public float R
        {
            get => _r;
            set
            {
                _r = value;
                CreatedColor = new SolidColorBrush(new Color(255, (byte) _r,(byte) _g,(byte) _b));
            } 
        }

        public float G
        {
            get => _g;
            set
            {
                _g = value;
                CreatedColor = new SolidColorBrush(new Color(255, (byte) _r,(byte) _g,(byte) _b));
            } 
        }

        public float B
        {
            get => _b;
            set
            {
                _b = value;
                CreatedColor = new SolidColorBrush(new Color(255, (byte) _r,(byte) _g,(byte) _b));
            } 
        }

        private IBrush _createdColor;
        private float _r = 0;
        private float _g = 255;
        private float _b = 0;

        public IBrush CreatedColor
        {
            get => _createdColor;
            set => this.RaiseAndSetIfChanged(ref _createdColor, value);
        }
        
        public DrawingDataContext()
        {
            CreatedColor = new SolidColorBrush(new Color(255, (byte) _r,(byte) _g,(byte) _b));
            Instance = this;
        }
        
        public static DrawingDataContext Instance { get; private set; }
    }
}