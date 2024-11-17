using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using AvaDragDrop = Avalonia.Input.DragDrop;

namespace Monaco.DragDrop.Abstractions;
public abstract partial class DragOperationBase : AvaloniaObject, IDragOperation
{
    public IList<string> InteractionIds { get; set; } = [DragDropIds.DefaultOperation];
    public Control? AttachedControl { get; private set; }

    private Control? _trackedControl; // Control that has been clicked and may be initiating a drag
    private Point? _dragOrigin;
    private bool _isDragPending; // True if Control has been clicked, mouse is being held, but drag operation has not started yet

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
        if (sender is not Control c || _isDragPending)
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
        _isDragPending = false;
    }

    protected virtual async void Control_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (AttachedControl is null)
            return;

        if (_trackedControl is null || _dragOrigin is null)
            return;

        var local = e.GetCurrentPoint(_trackedControl);
        var delta = local.Position - _dragOrigin.Value;
        if (delta.X * delta.X + delta.Y * delta.Y < DragThreshold * DragThreshold)
            return;

        if (!_isDragPending)
            await StartDrag(e);
    }

    protected virtual async Task StartDrag(PointerEventArgs e)
    {
        var payload = DragPayload ?? AttachedControl?.DataContext;

        if (payload is null || _dragOrigin is null)
            return;

        var metadata = new DragMetadata()
        {
            DragOrigin = _dragOrigin.Value,
            DragIds = InteractionIds.ToList()
        };

        _trackedControl = null;
        _dragOrigin = null;
        _isDragPending = false;

        await DoDragDrop(e, metadata, payload);
    }

    protected virtual async Task DoDragDrop(PointerEventArgs triggerEvent, DragMetadata metadata, object payload)
    {
        var data = new DataObject();
        foreach (var id in InteractionIds)
            data.Set(id, payload);

        data.Set(DragDropIds.DragMetadata, metadata);

        var effect = DragDropEffects.Move;
        await AvaDragDrop.DoDragDrop(triggerEvent, data, effect);
    }
}
