using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.VisualTree;

namespace Monaco.DragDrop;
public class DropHighlightAdorner : DropAdornerBase
{
    public DropHighlightAdorner() //Control adornedControl) : base(adornedControl)
    {
        Opacity = 0.7;
    }

    public override void Render(DrawingContext context)
    {
        ThrowIf.Null(TargetControl);

        var root = (Window)TargetControl!.GetVisualRoot()!;
        var bounds = TargetControl!.Bounds;
        var point = TargetControl.TranslatePoint(bounds.TopLeft, root)!.Value;

        var pen = new ImmutablePen(0xFFFF0000);
        var rect = new Rect(point.X - bounds.X, point.Y - bounds.Y, bounds.Width, bounds.Height);

        context.DrawRectangle(Brushes.Green, pen, rect);
    }
}
