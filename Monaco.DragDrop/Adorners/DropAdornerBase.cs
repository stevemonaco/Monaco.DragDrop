using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.VisualTree;
using Monaco.DragDrop.Abstractions;
using AvaDragDrop = Avalonia.Input.DragDrop;

namespace Monaco.DragDrop;
public abstract class DropAdornerBase : Border, IDropAdorner
{
    public static readonly StyledProperty<Control?> TargetControlProperty =
    AvaloniaProperty.Register<DropOperationBase, Control?>(nameof(TargetControl), defaultBindingMode: BindingMode.OneWayToSource);

    /// <summary>
    /// Control which the adorner should be positioned above
    /// </summary>
    public Control? TargetControl
    {
        get => GetValue(TargetControlProperty);
        set => SetValue(TargetControlProperty, value);
    }

    public DropAdornerBase()
    {
        IsHitTestVisible = false;
        AvaDragDrop.SetAllowDrop(this, false);
    }

    public virtual void Attach()
    {
        if (TargetControl is null)
            return;

        var layer = AdornerLayer.GetAdornerLayer(TargetControl);
        AdornerLayer.SetAdorner(TargetControl, this);
    }

    public virtual void Detach()
    {
        if (TargetControl is null)
            return;

        var layer = AdornerLayer.GetAdornerLayer(TargetControl);
        AdornerLayer.SetAdorner(TargetControl, null);
    }

    /// <summary>
    /// Gets the rect overlaying the TargetControl in AdornerLayer coordinates
    /// </summary>
    /// <returns></returns>
    public Rect GetAdornerRect()
    {
        var root = (Window)TargetControl!.GetVisualRoot()!;
        var bounds = TargetControl!.Bounds;
        var point = TargetControl.TranslatePoint(bounds.TopLeft, root)!.Value;

        return new Rect(point.X - bounds.X, point.Y - bounds.Y, bounds.Width, bounds.Height);
    }
}
