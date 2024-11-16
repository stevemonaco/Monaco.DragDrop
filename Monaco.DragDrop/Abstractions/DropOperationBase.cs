using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using AvaDragDrop = Avalonia.Input.DragDrop;

namespace Monaco.DragDrop.Abstractions;
public abstract class DropOperationBase : AvaloniaObject, IDropOperation
{
    public static readonly StyledProperty<object?> DropTargetProperty =
        AvaloniaProperty.Register<DropOperationBase, object?>(nameof(DropTarget), defaultBindingMode: BindingMode.OneWayToSource);

    /// <summary>
    /// DropTarget is updated with the Payload that was registered by the DragOperation
    /// </summary>
    public object? DropTarget
    {
        get => GetValue(DropTargetProperty);
        set => SetValue(DropTargetProperty, value);
    }

    public IdList InteractionIds { get; set; } = [DragDropIds.DefaultOperation];
    public Control? AttachedControl { get; private set; }
    public object? Context { get; set; }

    public void Attach(Control control)
    {
        ThrowIf.NotNull(AttachedControl);

        AttachedControl = control;

        AvaDragDrop.SetAllowDrop(control, true);
        SubscribeDropEvents(control);
    }

    public void Detach(Control control)
    {
        if (AttachedControl is null)
            return;

        AvaDragDrop.SetAllowDrop(control, true);
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

        if (sender is Control c)
        {
            c = c.GetLogicalParent() as Control ?? c;
            ((IPseudoClasses)c.Classes).Set(":dragOver", true);
        }
    }

    protected virtual void DragLeave(object? sender, RoutedEventArgs e)
    {
        if (sender is Control c)
        {
            c = c.GetLogicalParent() as Control ?? c;
            ((IPseudoClasses)c.Classes).Set(":dragOver", false);
        }
    }

    protected virtual void DragOver(object? sender, DragEventArgs e)
    {
        var payload = InteractionIds.Select(e.Data.Get).FirstOrDefault();
        var metadata = e.Data.Get(DragDropIds.DropMetadata) as DragMetadata;

        if (payload is null)
        {
            e.DragEffects = DragDropEffects.None;
        }
    }

    protected virtual void Drop(object? sender, DragEventArgs e)
    {
        var payload = InteractionIds.Select(e.Data.Get).FirstOrDefault();

        if (payload is not null)
        {
            DropTarget = payload;
        }
    }
}
