using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Diagnostics.CodeAnalysis;
using AvaDragDrop = Avalonia.Input.DragDrop;

namespace Monaco.DragDrop.Abstractions;
public abstract partial class DropOperationBase : AvaloniaObject, IDropOperation
{
    public IList<string> InteractionIds { get; set; } = [DragDropIds.DefaultOperation];
    public object? Context { get; set; }
    public RoutingStrategies Routing { get; set; } = RoutingStrategies.Bubble;
    protected bool _handledEventsToo = false;

    public virtual void Attach(Control control)
    {
        ThrowIf.NotNull(AttachedControl);

        AttachedControl = control;
        AvaDragDrop.SetAllowDrop(control, true);
        SubscribeDropEvents(control);

        DropAdorner = DropAdorner ?? new DropHighlightAdorner();
        DropAdorner.TargetControl = AttachedControl;
    }

    public virtual void Detach(Control control)
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
        e.DragEffects = OnDragEnter(canDrop);
    }

    protected virtual void DragLeave(object? sender, RoutedEventArgs e)
    {
        OnDragLeave();
    }

    /// <summary>
    /// Invoked when a drag enters the AttachedControl
    /// </summary>
    /// <param name="canDrop"></param>
    protected virtual DragDropEffects OnDragEnter(bool canDrop)
    {
        if (DropAdorner is not null)
        {
            DropAdorner.Attach();
            DropAdorner.IsDropValid = canDrop;
        }

        if (!canDrop)
        {
            //e.DragEffects = DragDropEffects.None;
            return DragDropEffects.None;
        }

        ((IPseudoClasses)AttachedControl!.Classes).Set(":dropover", true);
        return DragDropEffects.Move;
    }

    /// <summary>
    /// Invoked when a drag leaves the AttachedControl
    /// </summary>
    /// <param name="canDrop"></param>
    protected virtual void OnDragLeave()
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
        if (!TryGetMetadata<DragMetadata>(e, out var metadata))
            return;

        if (!TryGetPayload<object>(e, out var payload))
            return;

        SetCurrentValue(PayloadTargetProperty, payload);

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
        //var metadata = GetMetadata(e);
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
    /// Tries to get the Payload to be assigned to the DropTarget
    /// Overriding this allows a data transform from the Payload set by the Drag operation into
    /// what the DropTarget expects, if it's not 1:1 compatible
    /// </summary>
    /// <typeparam name="TPayload">Compatible type of payload expected</typeparam>
    /// <param name="e">Drag event that started the operation</param>
    /// <param name="payload">Payload to be transferred by the operation</param>
    /// <returns></returns>
    protected virtual bool TryGetPayload<TPayload>(DragEventArgs e, [MaybeNullWhen(false)] out TPayload payload)
        where TPayload : class
    {
        if (InteractionIds.Select(e.Data.Get).OfType<object>().FirstOrDefault() is TPayload p)
        {
            payload = p;
            return true;
        }

        payload = null;
        return false;
    }

    /// <summary>
    /// Tries to get metadata related to the initiation of the drag operation
    /// </summary>
    /// <typeparam name="TDragMetadata">Compatible type of drag metadata</typeparam>
    /// <param name="e">Drag event that started the operation</param>
    /// <param name="dragMetadata">Metadata from the drag initiation</param>
    /// <returns></returns>
    protected bool TryGetMetadata<TDragMetadata>(DragEventArgs e, [MaybeNullWhen(false)] out TDragMetadata dragMetadata)
        where TDragMetadata : DragMetadata
    {
        if (e.Data.Get(DragDropIds.DragMetadata) is TDragMetadata metadata)
        {
            dragMetadata = metadata;
            return true;
        }

        dragMetadata = null;
        return false;
    }
}
