using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AITickTackToe.ViewModels;
using AITickTackToe.Controls;
using Avalonia.Media;
using SkiaSharp;
using Avalonia.Media.Imaging;
using AITickTackToe.XOGame.Rendering;
using AITickTackToe.XOGame;
namespace AITickTackToe.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}