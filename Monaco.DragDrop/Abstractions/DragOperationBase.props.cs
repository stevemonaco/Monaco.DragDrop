using Avalonia;

namespace Monaco.DragDrop.Abstractions;
public abstract partial class DragOperationBase
{
    public static readonly StyledProperty<object?> DragPayloadProperty =
        AvaloniaProperty.Register<DragOperationBase, object?>(nameof(DragPayload));

    /// <summary>
    /// Object that is transferred via DragDrop. If null, this falls back to AttachedControl.DataContext.
    /// </summary>
    public object? DragPayload
    {
        get => GetValue(DragPayloadProperty);
        set => SetValue(DragPayloadProperty, value);
    }

    public static readonly StyledProperty<int> DragThresholdProperty =
        AvaloniaProperty.Register<DragOperationBase, int>(nameof(DragPayload), defaultValue: 4);

    /// <summary>
    /// Threshold in pixels that the user must move a pressed pointer to initiate a drag operation
    /// </summary>
    public int? DragThreshold
    {
        get => GetValue(DragThresholdProperty);
        set => SetValue(DragThresholdProperty, value);
    }
}
