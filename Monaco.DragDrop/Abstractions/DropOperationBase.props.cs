using System.Windows.Input;
using Avalonia.Data;
using Avalonia;
using Avalonia.Controls;

namespace Monaco.DragDrop.Abstractions;
public abstract partial class DropOperationBase
{
    public static readonly StyledProperty<Control?> AttachedControlProperty =
        AvaloniaProperty.Register<DropOperationBase, Control?>(nameof(AttachedControl));

    /// <summary>
    /// Control that the DropOperation is attached to
    /// </summary>
    public Control? AttachedControl
    {
        get => GetValue(AttachedControlProperty);
        set => SetValue(AttachedControlProperty, value);
    }

    public static readonly StyledProperty<object?> PayloadTargetProperty =
        AvaloniaProperty.Register<DropOperationBase, object?>(nameof(PayloadTarget), defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// PayloadTarget will be assigned the Payload that was registered by the DragOperation when the Drop is applied
    /// </summary>
    public object? PayloadTarget
    {
        get => GetValue(PayloadTargetProperty);
        set => SetValue(PayloadTargetProperty, value);
    }

    private ICommand? _payloadCommand;

    public static readonly DirectProperty<DropOperationBase, ICommand?> PayloadCommandProperty =
        AvaloniaProperty.RegisterDirect<DropOperationBase, ICommand?>(nameof(PayloadCommand), o => o.PayloadCommand, (o, v) => o.PayloadCommand = v);

    /// <summary>
    /// PayloadCommand will be executed when the Drop is applied.
    /// </summary>
    public ICommand? PayloadCommand
    {
        get => _payloadCommand;
        set => SetAndRaise(PayloadCommandProperty, ref _payloadCommand, value);
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
