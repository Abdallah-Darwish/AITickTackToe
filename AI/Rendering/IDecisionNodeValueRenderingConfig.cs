using Avalonia;
using Avalonia.Media;

namespace AITickTackToe.AI.Rendering
{
    public interface IDecisionNodeValueRenderer<T>
    {
        void Draw(DrawingContext drawingCtx, T val);
    }
}