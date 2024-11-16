using Avalonia.Data;
using Avalonia;

namespace Monaco.DragDrop.Abstractions;
public abstract partial class DropOperationBase
{
    public static readonly StyledProperty<object?> DropTargetProperty =
    AvaloniaProperty.Register<DropOperationBase, object?>(nameof(DropTarget), defaultBindingMode: BindingMode.OneWayToSource);

    /// <summary>
    /// DropTarget is updated with the Payload that was registered by the DragOperation
    /// </summary>
    public object? DropTarget
    {
        get => GetValue(DropTargetProperty);
        set => SetValue(DropTargetProperty, value);
    }

    public static readonly StyledProperty<DropAdornerBase?> DropAdornerProperty =
        AvaloniaProperty.Register<DropOperationBase, DropAdornerBase?>(nameof(DropAdorner), defaultBindingMode: BindingMode.OneTime);

    /// <summary>
    /// Adorner responsible for rendering overlay visual when dragging over and in a valid state
    /// </summary>
    public DropAdornerBase? DropAdorner
    {
        get => GetValue(DropAdornerProperty);
        set => SetValue(DropAdornerProperty, value);
    }
}
