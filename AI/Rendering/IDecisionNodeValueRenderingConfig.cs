using Avalonia;
using Avalonia.Media;

namespace AITickTackToe.AI.Rendering
{
    /// <summary>
    /// Used to render nodes values while drawing decision tree
    /// </summary>
    /// <typeparam name="T">Type of node value</typeparam>
    public interface IDecisionNodeValueRenderer<T>
    {
        /// <summary>
        /// Draw value of node using the supplied <see cref="DrawingContext"/>
        /// </summary>
        void Draw(DrawingContext drawingCtx, T val);
    }
}