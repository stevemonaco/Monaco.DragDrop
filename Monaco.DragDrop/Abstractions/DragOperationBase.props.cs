﻿using Avalonia;
using Avalonia.Controls;

namespace Monaco.DragDrop.Abstractions;
public abstract partial class DragOperationBase
{
    public static readonly StyledProperty<Control?> AttachedControlProperty =
        AvaloniaProperty.Register<DragOperationBase, Control?>(nameof(AttachedControl));

    /// <summary>
    /// Control that the DragOperation is attached to
    /// </summary>
    public Control? AttachedControl
    {
        get => GetValue(AttachedControlProperty);
        set => SetValue(AttachedControlProperty, value);
    }

    public static readonly StyledProperty<object?> PayloadProperty =
        AvaloniaProperty.Register<DragOperationBase, object?>(nameof(Payload));

    /// <summary>
    /// Object that is transferred via DragDrop. If null, this falls back to AttachedControl.DataContext.
    /// </summary>
    public object? Payload
    {
        get => GetValue(PayloadProperty);
        set => SetValue(PayloadProperty, value);
    }

    public static readonly StyledProperty<int> DragThresholdProperty =
        AvaloniaProperty.Register<DragOperationBase, int>(nameof(DragThreshold), defaultValue: 6);

    /// <summary>
    /// Threshold in pixels that the user must move a pressed pointer to initiate a drag operation
    /// </summary>
    public int? DragThreshold
    {
        get => GetValue(DragThresholdProperty);
        set => SetValue(DragThresholdProperty, value);
    }
}
