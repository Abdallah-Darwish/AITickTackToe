using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using AITickTackToe.AI.Rendering;
namespace AITickTackToe.XOGame.Rendering
{

    public class PlaygroundRenderingConfig : IDecisionNodeValueConfig<Playground>
    {
        private Typeface _xoTypeface = Typeface.Default;
        private IPen _gridPen = new Pen(Brushes.Black);

        public IBrush HighlightBrush { get; init; } = Brushes.DarkCyan;
        public IBrush XBrush { get; init; } = Brushes.Blue;
        public IBrush OBrush { get; init; } = Brushes.Red;
        public Typeface XOTypeface
        {
            get => _xoTypeface;
            init
            {
                _xoTypeface = value;
                CalcGridSize();
            }
        }
        public IPen GridPen
        {
            get => _gridPen;
            init
            {
                _gridPen = value;
                CalcGridSize();
            }
        }
        public Size GridSize { get; private set; }
        public Size CellSize { get; private set; }
        void CalcGridSize()
        {
            var txt = new FormattedText()
            {
                Text = "O",
                TextAlignment = TextAlignment.Center,
                Typeface = _xoTypeface
            };
            CellSize = txt.Bounds.Size;
            var gridWidth = (_gridPen.Thickness * 2) + (CellSize.Width * 3);
            var gridHeight = (_gridPen.Thickness * 2) + (CellSize.Height * 3);
            GridSize = new Size(gridWidth, gridHeight);
        }
        public (Point Start, Point End) GetVerticalLine(int i, Point gridLocation = default)
        {
            Point p0 = new Point((_gridPen.Thickness + CellSize.Width) * i + CellSize.Width + _gridPen.Thickness / 2 + gridLocation.X, gridLocation.Y);
            return (p0, new Point(p0.X, p0.Y + GridSize.Height));
        }

        public (Point Start, Point End) GetHorizontalLine(int i, Point gridLocation = default)
        {
            Point p0 = new Point(gridLocation.X, (_gridPen.Thickness + CellSize.Height) * i + CellSize.Height + _gridPen.Thickness / 2 + gridLocation.Y);
            return (p0, new Point(p0.X + GridSize.Width, p0.Y));
        }
        public Point GetCellLocation(int r, int c, Point gridLocation = default)
        {
            var p = new Point(gridLocation.X + (c * (_gridPen.Thickness + CellSize.Width)), gridLocation.Y + (r * (_gridPen.Thickness + CellSize.Height)));
            return p;
        }

        public Size CalcValueSize(Playground node) => GridSize;

        public PlaygroundRenderingConfig()
        {
            CalcGridSize();
        }
    }
}