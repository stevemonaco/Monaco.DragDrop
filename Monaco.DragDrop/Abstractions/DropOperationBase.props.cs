﻿using Avalonia.Data;
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
