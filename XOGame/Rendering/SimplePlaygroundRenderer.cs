using System;
using System.Collections.Generic;
using System.Linq;
using AITickTackToe.AI.Rendering;
using Avalonia;
using Avalonia.Direct2D1.Media;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Skia;

namespace AITickTackToe.XOGame.Rendering
{
    public class SimplePlaygroundRenderer : IDecisionNodeValueRenderer<Playground>
    {
        public PlaygroundRenderingConfig Config { get; init; }
        public bool[][] IsHighlightedCell { get; } = new bool[3][]
        {
            new bool[3],
            new bool[3],
            new bool[3]
        };
        public void Draw(DrawingContext ctx, Playground pg)
        {
            //1- Draw grid   
            for (int i = 0; i < 2; i++)
            {
                var (start, end) = Config.GetVerticalLine(i);
                ctx.DrawLine(Config.GridPen, start, end);
            }

            for (int i = 0; i < 2; i++)
            {
                var (start, end) = Config.GetHorizontalLine(i);
                ctx.DrawLine(Config.GridPen, start, end);
            }

            //2- Draw cells
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    if (pg[r, c] != 'x' && pg[r, c] != 'o') { continue; }
                    var cellText = new FormattedText
                    {
                        Text = pg[r, c].ToString().ToUpperInvariant(),
                        Typeface = Config.XOTypeface,
                        TextAlignment = TextAlignment.Center,
                    };
                    var cellTextBrush = IsHighlightedCell[r][c] ? Config.HighlightBrush :  pg[r, c] == 'x' ? Config.XBrush : Config.OBrush;
                    ctx.DrawText(cellTextBrush, Config.GetCellLocation(r, c), cellText);
                }
            }
        }
    }
}