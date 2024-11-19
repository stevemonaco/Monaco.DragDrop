using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaDragDrop = Avalonia.Input.DragDrop;

namespace Monaco.DragDrop.Abstractions;
public abstract partial class DropOperationBase : AvaloniaObject, IDropOperation
{
    public IList<string> InteractionIds { get; set; } = [DragDropIds.DefaultOperation];
    public Control? AttachedControl { get; private set; }
    public object? Context { get; set; }
    public RoutingStrategies Routing { get; set; } = RoutingStrategies.Bubble;
    protected bool _handledEventsToo = false;

    public void Attach(Control control)
    {
        ThrowIf.NotNull(AttachedControl);

        AttachedControl = control;
        AvaDragDrop.SetAllowDrop(control, true);
        SubscribeDropEvents(control);

        DropAdorner = DropAdorner ?? new DropHighlightAdorner();
        DropAdorner.TargetControl = AttachedControl;
    }

    public void Detach(Control control)
    {
        if (AttachedControl is null)
            return;

        AvaDragDrop.SetAllowDrop(control, false);
        UnsubscribeDropEvents(control);
        AttachedControl = null;
    }

    protected virtual void SubscribeDropEvents(Control control)
    {
        control.AddHandler(AvaDragDrop.DragEnterEvent, DragEnter, Routing, _handledEventsToo);
        control.AddHandler(AvaDragDrop.DragLeaveEvent, DragLeave, Routing, _handledEventsToo);
        control.AddHandler(AvaDragDrop.DragOverEvent, DragOver, Routing, _handledEventsToo);
        control.AddHandler(AvaDragDrop.DropEvent, Drop, Routing, _handledEventsToo);
    }

    protected virtual void UnsubscribeDropEvents(Control control)
    {
        control.RemoveHandler(AvaDragDrop.DragEnterEvent, DragEnter);
        control.RemoveHandler(AvaDragDrop.DragLeaveEvent, DragLeave);
        control.RemoveHandler(AvaDragDrop.DragOverEvent, DragOver);
        control.RemoveHandler(AvaDragDrop.DropEvent, Drop);
    }

    protected virtual void DragEnter(object? sender, DragEventArgs e)
    {
        bool canDrop = CanDrop(e);

        if (DropAdorner is not null)
        {
            DropAdorner.Attach();
            DropAdorner.IsDropValid = canDrop;
        }

        if (!canDrop)
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }

        ((IPseudoClasses)AttachedControl!.Classes).Set(":dropover", true);
    }

    protected virtual void DragLeave(object? sender, RoutedEventArgs e)
    {
        ((IPseudoClasses)AttachedControl!.Classes).Set(":dropover", false);
        DropAdorner?.Detach();
    }

    protected virtual void DragOver(object? sender, DragEventArgs e)
    {
        bool canDrop = CanDrop(e);

        if (DropAdorner is not null)
        {
            DropAdorner.IsDropValid = canDrop;
            DropAdorner.InvalidateVisual();
        }

        if (!canDrop)
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }
    }

    /// <summary>
    /// Completes the DragDrop operation by assigning payload and cleaning up visuals
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void Drop(object? sender, DragEventArgs e)
    {
        var metadata = GetMetadata(e);
        var payload = GetPayload(e, metadata);

        if (payload is not null)
        {
            PayloadTarget = payload;
        }

        ((IPseudoClasses)AttachedControl!.Classes).Set(":dropover", false);

        if (DropAdorner is not null)
        {
            DropAdorner.IsDropValid = false;
            DropAdorner.Detach();
        }
    }

    /// <summary>
    /// Checks if the drop is expected to be valid
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    protected virtual bool CanDrop(DragEventArgs e)
    {
        var metadata = GetMetadata(e);
        var hasPayload = CanGetPayload(e);

        return hasPayload;
    }

    /// <summary>
    /// Checks if the drag payload is compatible with the DropTarget
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    protected virtual bool CanGetPayload(DragEventArgs e)
    {
        return InteractionIds.Any(x => e.Data.Get(x) is not null);
    }

    /// <summary>
    /// Gets the Payload to be assigned to the DropTarget
    /// This allows a data transform from the Payload set by the Drag operation into
    /// what the DropTarget expects, if it's not 1:1 compatible
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    protected virtual object? GetPayload(DragEventArgs e, DragMetadata? metadata)
    {
        return InteractionIds.Select(e.Data.Get).OfType<object>().FirstOrDefault();
    }

    protected DragMetadata? GetMetadata(DragEventArgs e)
    {
        return e.Data.Get(DragDropIds.DragMetadata) as DragMetadata;
    }
}
