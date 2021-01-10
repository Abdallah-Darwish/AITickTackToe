using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AITickTackToe.ViewModels;
using AITickTackToe.Views;
using Avalonia.Media;
using AITickTackToe.XOGame.Rendering;
using Avalonia.Controls;
using AITickTackToe.Controls;
using SkiaSharp;
using Avalonia.Media.Imaging;

namespace AITickTackToe
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var w = new MainWindow();
                var tab2 = w.FindControl<TabItem>("tab2");

                var pg = w.FindControl<XOPlaygroundControl>("pg");
                pg.RenderingConfig = new PlaygroundRenderingConfig
                {
                    GridPen = new Pen(Brushes.Black, 60),
                    OBrush = Brushes.Red,
                    XBrush = Brushes.Blue,
                    HighlightBrush = Brushes.BlueViolet,
                    XOTypeface = new Typeface(FontFamily.Default, 500)
                };
                pg.WinnerCrossLinePen = new Pen(Brushes.Cyan, 30);
                var m = new MainWindowViewModel
                {
                    Player1 = new PlayerViewModel
                    {
                        MyChar = 'x',
                        CurrentGame = pg.Value,
                        MyCharBrush = Brushes.Blue,
                        AITreeDepth = 3
                    },
                    Player2 = new PlayerViewModel
                    {
                        MyChar = 'o',
                        CurrentGame = pg.Value,
                        MyCharBrush = Brushes.Red,
                        AITreeDepth = 3
                    },
                    PlaygroundControl = pg,
                    RenderingConfig = new AI.Rendering.DecisionNodeRenderingConfig<Playground>
                    {
                        AndArcsPaint = new SKPaint
                        {
                            Color = SKColors.Green,
                            IsStroke = true,
                            Style = SKPaintStyle.Stroke,
                            BlendMode = SKBlendMode.Color,
                            IsAntialias = true,
                            StrokeWidth = 10
                        },
                        BestEdgesPen = new Pen(Brushes.Red, 10),
                        EdgesPen = new Pen(Brushes.Black, 10),
                        BestNodeIdentifier = new Bitmap(@"C:\Users\abdal\source\repos\AIScratch\check.png").Resize(new PixelSize(30, 30)),
                        ValueConfig = new PlaygroundRenderingConfig
                        {
                            GridPen = new Pen(Brushes.Black, 8),
                            HighlightBrush = Brushes.BlueViolet,
                            XBrush = Brushes.Blue,
                            OBrush = Brushes.Red,
                            XOTypeface = new Typeface(FontFamily.Default, 100)
                        },
                        SpacingBetweenNodes = 0,
                        SpaceBetweenNodeAndInfo = 2,
                        SpaceBetweenInfoSections = 2,
                        SpacingBetweenLevels = 50,
                        DecisionTypeface = new Typeface(FontFamily.Default, 100),
                        DecisionValueBrush = Brushes.Purple,
                        SpaceBetweenNodeAndEdge = 3
                    }
                };
                m.Init();
                tab2.GotFocus += m.UpdateDecisionTree;
                w.DataContext = m;
                desktop.MainWindow = w;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}