using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using AvaDragDrop = Avalonia.Input.DragDrop;

namespace Monaco.DragDrop.Abstractions;
public abstract class DragOperationBase : AvaloniaObject, IDragOperation
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

    public IList<string> InteractionIds { get; set; } = [DragDropIds.DefaultOperation];
    public Control? AttachedControl { get; private set; }

    private Control? _trackedControl; // Control that has been clicked and may be initiating a drag
    private Point? _dragOrigin;
    private bool _isCaptured; // True if Control has been clicked, mouse is being held, but drag has not started yet

    public void Attach(Control control)
    {
        ThrowIf.NotNull(AttachedControl, "DragOperation is already attached to a control");

        AttachedControl = control;
        SubscribeDragEvents(control);
    }

    public void Detach(Control control)
    {
        if (AttachedControl is null)
            return;

        UnsubscribeDragEvents(control);
        AttachedControl = null;
    }

    protected virtual void SubscribeDragEvents(Control control)
    {
        control.AddHandler(InputElement.PointerPressedEvent, Control_PointerPressed);
        control.AddHandler(InputElement.PointerMovedEvent, Control_PointerMoved);
        control.AddHandler(InputElement.PointerReleasedEvent, Control_PointerReleased);
    }

    protected virtual void UnsubscribeDragEvents(Control control)
    {
        control.RemoveHandler(InputElement.PointerPressedEvent, Control_PointerPressed);
        control.RemoveHandler(InputElement.PointerMovedEvent, Control_PointerMoved);
        control.RemoveHandler(InputElement.PointerReleasedEvent, Control_PointerReleased);
    }

    protected virtual void Control_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control c || _isCaptured)
            return;

        var point = e.GetCurrentPoint(c);
        if (point.Properties.IsLeftButtonPressed)
        {
            if (e.Source is Control control)
            {
                _trackedControl = control;
                _dragOrigin = point.Position;
            }
        }
    }

    protected virtual void Control_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _dragOrigin = null;
        _isCaptured = false;
    }

    protected virtual async void Control_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (AttachedControl is null)
            return;

        if (_trackedControl is null || _dragOrigin is null)
            return;

        var local = e.GetCurrentPoint(_trackedControl);
        var delta = local.Position - _dragOrigin.Value;
        if (delta.X * delta.X + delta.Y * delta.Y < 16)
            return;

        if (!_isCaptured)
            await StartDrag(e);
    }

    protected virtual async Task StartDrag(PointerEventArgs e)
    {
        var payload = DragPayload ?? AttachedControl?.DataContext;

        if (payload is null || _dragOrigin is null)
            return;

        var metadata = new DragMetadata()
        {
            DragOrigin = _dragOrigin.Value
        };

        _trackedControl = null;
        _dragOrigin = null;
        _isCaptured = false;

        await DoDragDrop(e, metadata, payload);
    }

    protected virtual async Task DoDragDrop(PointerEventArgs triggerEvent, DragMetadata metadata, object payload)
    {
        var data = new DataObject();
        foreach (var id in InteractionIds)
            data.Set(id, payload);

        data.Set(DragDropIds.DropMetadata, metadata);

        var effect = DragDropEffects.Move;
        await AvaDragDrop.DoDragDrop(triggerEvent, data, effect);
    }
}
