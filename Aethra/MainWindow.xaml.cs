using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Aethra
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private DrawingCanvas? _canvas;
        private ProgressBar? _renderProgress;
        private Button? _startButton;
        private TextBox? _heightBox;
        private TextBox? _widthBox;
        
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _startButton = this.FindControl<Button>("StartButton");
            _heightBox = this.FindControl<TextBox>("HeightText");
            _widthBox = this.FindControl<TextBox>("WidthText");
            _startButton.Click += OnClick;
            _canvas = this.FindControl<DrawingCanvas>("Canvas");
            _renderProgress = this.FindControl<ProgressBar>("RenderProgress");
            _heightBox.Text = _canvas.Height.ToString(CultureInfo.CurrentCulture);
            _widthBox.Text = _canvas.Width.ToString(CultureInfo.CurrentCulture);
        }
        
        private async void OnClick(object? sender, RoutedEventArgs e)
        {
            _renderProgress!.IsIndeterminate = true;
            _startButton!.IsEnabled = false;
            await _canvas!.Draw(int.Parse(_widthBox!.Text), int.Parse(_heightBox!.Text));
            _renderProgress!.IsIndeterminate = false;
            _startButton!.IsEnabled = true;
        }
    }
}
