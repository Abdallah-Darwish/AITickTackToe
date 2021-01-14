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
        /// <summary>
        /// Calculates space required to draw the node value and its information
        /// </summary>
        Size CalcValueSize(TValue node);
    }
    public class DecisionNodeRenderingConfig<TValue>
    {
        /// <summary>
        /// Space between the top of node and the end of edge connecting it to its parent or the bottom of the node and start of the edge connection it to its decendants
        /// </summary>
        public double SpaceBetweenNodeAndEdge { get; init; } = 3;
        /// <summary>
        /// <see cref="SKPaint"/> used to draw and arc at node bottom edges joint
        /// </summary>
        public SKPaint AndArcPaint { get; init; }
        /// <summary>
        /// Horizontal space between node value and its info like its Decision
        /// </summary>
        public double SpaceBetweenNodeValueAndInfo { get; init; } = 2;
        /// <summary>
        /// Vertical space between node info sections
        /// </summary>
        public double SpaceBetweenInfoSections { get; init; } = 2;
        /// <summary>
        /// Horizontal space between nodes in same level
        /// </summary>
        public double SpacingBetweenNodes { get; init; } = 5;
        /// <summary>
        /// Vertical space between levels of nodes
        /// </summary>
        public double SpacingBetweenLevels { get; init; } = 50;
        public IBrush DecisionValueBrush { get; init; } = Brushes.Salmon;
        public IPen EdgesPen { get; init; } = new Pen(Brushes.Black, 3);
        /// <summary>
        /// <see cref="IPen"/> used to draw edges connecting only nodes that lay on the path from root to best leaf
        /// </summary>
        public IPen BestEdgesPen { get; init; } = new Pen(Brushes.Red, 3);
        public Typeface DecisionTypeface { get; init; }
        public IDecisionNodeValueConfig<TValue> ValueConfig { get; init; }
        /// <summary>
        /// Image or icon to draw next to node value if its selected
        /// If its null then it will be ignored
        /// </summary>
        public IBitmap? SelectedNodeIdentifier { get; init; }
        /// <summary>
        /// Image or icon to draw next to node value if its the best between its siblings only
        /// If its null then it will be ignored
        /// </summary>
        public IBitmap? BestNodeIdentifier { get; init; }
        ///<inheritdoc/>
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

            if (node.IsSelected && SelectedNodeIdentifier != null)
            {
                additionalWidth = Math.Max(additionalWidth, SelectedNodeIdentifier.Size.Width);
                requiredHeight += SpaceBetweenInfoSections + SelectedNodeIdentifier.Size.Height;
            }
            if (node.IsBest && BestNodeIdentifier != null)
            {
                additionalWidth = Math.Max(additionalWidth, BestNodeIdentifier.Size.Width);
                requiredHeight += SpaceBetweenInfoSections + BestNodeIdentifier.Size.Height;
            }
            additionalWidth += SpaceBetweenNodeValueAndInfo;
            return new Size(nodeSize.Width + additionalWidth, Math.Max(requiredHeight, nodeSize.Height));
        }
    }
}