using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using SkiaSharp;

namespace AITickTackToe.AI.Rendering
{
    public interface IDecisionNodeValueConfig<TValue>
    {
        Size CalcValueSize(TValue node);
    }
    public class DecisionNodeRenderingConfig<TValue>
    {
        public double SpaceBetweenNodeAndEdge { get; init; } = 3;
        public SKPaint AndArcsPaint { get; init; }
        public double SpaceBetweenNodeAndInfo { get; init; } = 2;
        public double SpaceBetweenInfoSections { get; init; } = 2;
        public double SpacingBetweenNodes { get; init; } = 5;
        public double SpacingBetweenLevels { get; init; } = 50;
        public IBrush DecisionValueBrush { get; init; } = Brushes.Salmon;
        public IPen EdgesPen { get; init; } = new Pen(Brushes.Black, 3);
        public IPen BestEdgesPen { get; init; } = new Pen(Brushes.Red, 3);
        public Typeface DecisionTypeface { get; init; }

        public IDecisionNodeValueConfig<TValue> ValueConfig { get; init; }
        public IBitmap SelectedNodeIdentifier { get; init; }
        public IBitmap BestNodeIdentifier { get; init; }

        public Size CalcNodeSize(DecisionNode<TValue> node)
        {
            Size CalcNodeTextInfoSize(DecisionNode<TValue> node)
            {
                var txt = new FormattedText
                {
                    Text = node.Decision.ToString(),
                    Typeface = DecisionTypeface,
                    TextAlignment = TextAlignment.Left,
                    Wrapping = TextWrapping.NoWrap
                };
                return txt.Bounds.Size;
            }
            var txtInfoSize = CalcNodeTextInfoSize(node);

            var nodeSize = ValueConfig.CalcValueSize(node.Value);
            double requiredHeight = txtInfoSize.Height,
             additionalWidth = txtInfoSize.Width;

            if (node.IsSelected)
            {
                additionalWidth = Math.Max(additionalWidth, SelectedNodeIdentifier.Size.Width);
                requiredHeight += SpaceBetweenInfoSections + SelectedNodeIdentifier.Size.Height;
            }
            if (node.IsBest)
            {
                additionalWidth = Math.Max(additionalWidth, BestNodeIdentifier.Size.Width);
                requiredHeight += SpaceBetweenInfoSections + BestNodeIdentifier.Size.Height;
            }
            additionalWidth += SpaceBetweenNodeAndInfo;
            return new Size(nodeSize.Width + additionalWidth, Math.Max(requiredHeight, nodeSize.Height));
        }
    }
}