using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Monaco.DragDrop.Abstractions;
using AvaDragDrop = Avalonia.Input.DragDrop;

namespace Monaco.DragDrop;
public abstract class DropAdornerBase : Control
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

    public void Attach()
    {
        if (TargetControl is null)
            return;

        var layer = AdornerLayer.GetAdornerLayer(TargetControl);
        layer?.Children.Add(this);
    }

    public void Detach()
    {
        if (TargetControl is null)
            return;

        var layer = AdornerLayer.GetAdornerLayer(TargetControl);
        layer?.Children.Remove(this);
    }
}
