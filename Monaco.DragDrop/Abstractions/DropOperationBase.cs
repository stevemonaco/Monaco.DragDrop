using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaDragDrop = Avalonia.Input.DragDrop;

namespace Monaco.DragDrop.Abstractions;
public abstract partial class DropOperationBase : AvaloniaObject, IDropOperation
{
    public IdList InteractionIds { get; set; } = [DragDropIds.DefaultOperation];
    public Control? AttachedControl { get; private set; }
    public object? Context { get; set; }

    public void Attach(Control control)
    {
        ThrowIf.NotNull(AttachedControl);

        AttachedControl = control;
        AvaDragDrop.SetAllowDrop(control, true);
        SubscribeDropEvents(control);

        DropAdorner = DropAdorner ?? new DropHighlightAdorner();
        if (DropAdorner.TargetControl is null)
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
        control.AddHandler(AvaDragDrop.DragEnterEvent, DragEnter);
        control.AddHandler(AvaDragDrop.DragLeaveEvent, DragLeave);
        control.AddHandler(AvaDragDrop.DragOverEvent, DragOver);
        control.AddHandler(AvaDragDrop.DropEvent, Drop);
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
        var payload = InteractionIds.Select(e.Data.Get).FirstOrDefault();

        if (payload is null)
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }

        ((IPseudoClasses)AttachedControl!.Classes).Set(":dropover", true);
        DropAdorner?.Attach();
    }

    protected virtual void DragLeave(object? sender, RoutedEventArgs e)
    {
        ((IPseudoClasses)AttachedControl!.Classes).Set(":dropover", false);
        DropAdorner?.Detach();
    }

    protected virtual void DragOver(object? sender, DragEventArgs e)
    {
        var payload = InteractionIds.Select(e.Data.Get).FirstOrDefault();
        var metadata = e.Data.Get(DragDropIds.DragMetadata) as DragMetadata;

        if (payload is null)
        {
            e.DragEffects = DragDropEffects.None;
        }

        DropAdorner?.InvalidateVisual();
    }

    protected virtual void Drop(object? sender, DragEventArgs e)
    {
        var payload = InteractionIds.Select(e.Data.Get).FirstOrDefault();

        if (payload is not null)
        {
            DropTarget = payload;
        }

        ((IPseudoClasses)AttachedControl!.Classes).Set(":dropover", false);
        DropAdorner?.Detach();
    }

    protected virtual bool CanDrop()
    {
        return true;
    }
}
