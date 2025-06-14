using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using AvaDragDrop = Avalonia.Input.DragDrop;

namespace Monaco.DragDrop.Abstractions;
public abstract partial class DragOperationBase : AvaloniaObject, IDragOperation
{
    public IList<string> InteractionIds { get; set; } = [DragDropIds.DefaultOperation];
    public RoutingStrategies Routing { get; set; } = RoutingStrategies.Bubble;

    protected Control? _trackedControl; // Control that has been clicked and may be initiating a drag
    protected Point? _dragOrigin;
    protected bool _isDragPending; // True if Control has been clicked, mouse is being held, but drag operation has not started yet
    protected bool _handledEventsToo = false;

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
        control.AddHandler(InputElement.PointerPressedEvent, Control_PointerPressed, Routing, _handledEventsToo);
        control.AddHandler(InputElement.PointerMovedEvent, Control_PointerMoved, Routing, _handledEventsToo);
        control.AddHandler(InputElement.PointerReleasedEvent, Control_PointerReleased, Routing, _handledEventsToo);
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

        if (e.Source is Control sourceControl && sourceControl.GetVisualAncestors().Any(x => x is ScrollBar))
            return;

        var point = e.GetCurrentPoint(AttachedControl);
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
        _trackedControl = null;
        _dragOrigin = null;
        _isDragPending = false;
    }

    protected virtual async void Control_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (AttachedControl is null)
            return;

        if (_trackedControl is null || _dragOrigin is null)
            return;

        var local = e.GetCurrentPoint(AttachedControl);
        var delta = local.Position - _dragOrigin.Value;
        if (delta.X * delta.X + delta.Y * delta.Y < DragThreshold * DragThreshold)
            return;

        if (!_isDragPending)
            await StartDrag(e);
    }

    protected virtual async Task StartDrag(PointerEventArgs e)
    {
        var payload = Payload ?? AttachedControl?.DataContext;

        if (payload is null || _dragOrigin is null)
            return;

        var metadata = CreateMetadata(e);

        _trackedControl = null;
        _dragOrigin = null;
        _isDragPending = false;

        await DoDragDrop(e, metadata, payload);
    }

    protected virtual async Task DoDragDrop(PointerEventArgs triggerEvent, DragInfo metadata, object payload)
    {
        var data = new DataObject();
        foreach (var id in InteractionIds)
            data.Set(id, payload);

        data.Set(DragDropIds.DragMetadata, metadata);

        var effect = DragDropEffects.Move;
        var result = await AvaDragDrop.DoDragDrop(triggerEvent, data, effect);

        //if (result != DragDropEffects.None)
        //    DropCompleted(result);
    }

    protected virtual DragInfo CreateMetadata(PointerEventArgs e)
    {
        return new DragInfo()
        {
            DragOperation = this,
            DragOrigin = _dragOrigin!.Value,
            DragIds = InteractionIds.ToList(),
            PointerId = e.Pointer.Id
        };
    }

    /// <summary>
    /// Occurs when the Payload has been transferred
    /// Responsible for removing Payload from attached control, if necessary
    /// </summary>
    /// <param name="effect">Effect when the drop occurred</param>
    /// <param name="dragInfo">DragInfo associated with the dragdrop operation</param>
    /// <param name="dropInfo">DropInfo associated with the dragdrop operation</param>
    public virtual void DropCompleted(DragDropEffects effect, DragInfo dragInfo, DropInfo dropInfo)
    {
    }
}
