using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AITickTackToe.Views
{
    public class PlayerView : UserControl
    {
        public PlayerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}