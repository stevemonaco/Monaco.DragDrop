using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Monaco.DragDrop.Abstractions;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Monaco.DragDrop;
public class CollectionDropOperation : DropOperationBase
{
    public static readonly StyledProperty<DropAdornerBase?> ItemDropAdornerProperty =
        AvaloniaProperty.Register<CollectionDropOperation, DropAdornerBase?>(nameof(DropAdorner), defaultBindingMode: BindingMode.OneTime);

    /// <summary>
    /// Adorner responsible for rendering overlay visual when dragging over and in a valid state
    /// </summary>
    public DropAdornerBase? ItemDropAdorner
    {
        get => GetValue(ItemDropAdornerProperty);
        set => SetValue(ItemDropAdornerProperty, value);
    }

    private Control? _targetItem;
    private int? _targetIndex;

    public override void Attach(Control control)
    {
        ThrowIf.NotNull(AttachedControl);

        base.Attach(control);
        ItemDropAdorner = ItemDropAdorner ?? new DropInsertionAdorner();
    }

    protected override void DragEnter(object? sender, DragEventArgs e)
    {
        var target = LocateTargetContainer(e);

        bool canDrop = CanDrop(e);

        if (DropAdorner is not null)
        {
            DropAdorner.Attach();
            DropAdorner.IsDropValid = canDrop;
        }

        if (sender == AttachedControl)
        {
            OnDragEnter(canDrop);
        }

        if (ItemDropAdorner is not null && target is not null)
        {
            _targetItem = target;
            _targetIndex = LocateTargetContainerIndex(_targetItem);

            OnItemDragEnter(target, e.GetPosition(target), canDrop);
        }

        if (!canDrop)
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }

        ((IPseudoClasses)AttachedControl!.Classes).Set(":dropover", true);
    }

    protected override void DragLeave(object? sender, RoutedEventArgs e)
    {
        if (sender is null)
            return;

        var target = LocateTargetContainer(e);

        if (target is null || sender == AttachedControl)
        {
            base.DragLeave(sender, e);
        }

        if (target is not null && target == ItemDropAdorner?.TargetControl)
        {
            OnItemDragLeave();
        }
    }

    protected override void DragOver(object? sender, DragEventArgs e)
    {
        var target = LocateTargetContainer(e);
        bool canDrop = CanDrop(e);

        if (DropAdorner is not null)
        {
            DropAdorner.IsDropValid = canDrop;
            DropAdorner.InvalidateVisual();
        }

        if (target is not null && target == ItemDropAdorner?.TargetControl)
        {
            var position = e.GetPosition(target);
            if (position.Y < target.Bounds.Height / 2)
                ItemDropAdorner.VerticalAlignment = VerticalAlignment.Top;
            else
                ItemDropAdorner.VerticalAlignment = VerticalAlignment.Bottom;
        }

        if (!canDrop)
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }
    }

    protected override void Drop(object? sender, DragEventArgs e)
    {
        if (!TryGetMetadata<CollectionDragMetadata>(e, out var metadata) || !TryGetPayload<object>(e, out var payload))
            return;

        if (PayloadTarget is IList targetCollection && metadata.PayloadCollection is { } payloadCollection)
        {
            if (_targetIndex.HasValue)
            {
                TransferPayloadIndexed(payload, targetCollection, _targetIndex.Value, payloadCollection, metadata.PayloadContainerIndex);
            }
            else
            {
                TransferPayload(payload, targetCollection, payloadCollection, metadata.PayloadContainerIndex);
            }
        }

        ((IPseudoClasses)AttachedControl!.Classes).Set(":dropover", false);
        ItemDropAdorner?.Detach();
        DropAdorner?.Detach();
    }

    protected virtual void TransferPayload(object payload, IList targetCollection, IList payloadCollection, int? payloadIndex)
    {
        targetCollection.Add(payload);

        if (payloadIndex.HasValue)
            payloadCollection.RemoveAt(payloadIndex.Value);
        else
            payloadCollection.Remove(payload);
    }

    protected virtual void TransferPayloadIndexed(object payload, IList targetCollection, int targetIndex, IList payloadCollection, int? payloadIndex)
    {
        int indexDelta = 0;
        if (ItemDropAdorner?.TargetControl?.Classes.Contains(":dropbottom") ?? false)
            indexDelta++;

        if (payloadIndex.HasValue)
            payloadCollection.RemoveAt(payloadIndex.Value);
        else
            payloadCollection.Remove(payload);

        // Same collection, item being moved lower
        if (ReferenceEquals(targetCollection, payloadCollection) && payloadIndex < (targetIndex + indexDelta))
        {
            indexDelta--;
        }

        targetCollection.Insert(targetIndex + indexDelta, payload);
    }

    protected override bool CanDrop(DragEventArgs e)
    {
        //var metadata = GetMetadata(e);
        var hasPayload = CanGetPayload(e);

        if (e.Source is Control sourceItem && !AttachedControl.IsLogicalAncestorOf(sourceItem))
        {
            return false;
        }

        return hasPayload;
    }

    ///// <summary>
    ///// Invoked when a drag enters the AttachedControl
    ///// </summary>
    ///// <param name="canDrop"></param>
    //protected virtual void OnDragEnter(bool canDrop)
    //{
    //    if (canDrop)
    //        ((IPseudoClasses)itemContainer.Classes).Set(":dropover", true);
    //}

    ///// <summary>
    ///// Invoked when a drag leaves the AttachedControl
    ///// </summary>
    ///// <param name="canDrop"></param>
    //protected virtual void OnDragLeave()
    //{
    //    if (ItemDropAdorner?.TargetControl is not null)
    //    {
    //        ((IPseudoClasses)ItemDropAdorner.TargetControl.Classes).Set(":dropover", false);
    //    }

    //    ItemDropAdorner?.Detach();
    //}

    /// <summary>
    /// Invoked when a drag enters an item contained by the AttachedControl
    /// </summary>
    /// <param name="itemContainer"></param>
    /// <param name="canDrop"></param>
    protected virtual DragDropEffects OnItemDragEnter(Control itemContainer, Point dragLocation, bool canDrop)
    {
        // TODO: Generalize
        if (ItemDropAdorner is DropInsertionAdorner insertionAdorner)
        {
            ItemDropAdorner.Detach();
            ItemDropAdorner.TargetControl = itemContainer;
            ItemDropAdorner = new DropInsertionAdorner() { TargetControl = itemContainer };
            ItemDropAdorner.Attach();

            (ItemDropAdorner as DropInsertionAdorner)?.Update(dragLocation);
            ItemDropAdorner.IsDropValid = canDrop;
        }

        if (canDrop)
        {
            ((IPseudoClasses)itemContainer.Classes).Set(":dropover", true);
            return DragDropEffects.Move;
        }

        return DragDropEffects.None;
    }

    /// <summary>
    /// Invoked when a drag leaves an item contained by the AttachedControl
    /// </summary>
    /// <param name="itemContainer"></param>
    /// <param name="canDrop"></param>
    protected virtual void OnItemDragLeave()
    {
        _targetIndex = null;

        if (ItemDropAdorner?.TargetControl is not null)
        {
            ((IPseudoClasses)ItemDropAdorner.TargetControl.Classes).Set(":dropover", false);
            ItemDropAdorner.Detach();
        }
    }

    protected virtual Control? LocateTargetContainer(RoutedEventArgs triggerEvent)
    {
        if (AttachedControl is ItemsControl items 
            && triggerEvent.Source is Control sourceItem
            && sourceItem.TemplatedParent != AttachedControl)
        {
            var ancestors = sourceItem.GetSelfAndLogicalAncestors();
            var target = ancestors.TakeWhile(x => x != AttachedControl).OfType<Control>().LastOrDefault();

            if (target?.TemplatedParent == AttachedControl)
                return null;

            return target;
        }

        return null;
    }

    protected virtual int? LocateTargetContainerIndex(Control? container)
    {
        if (AttachedControl is ItemsControl items && container is not null)
        {
            return items.IndexFromContainer(container);
        }

        return null;
    }

    protected virtual DropMetadata CreateDropMetadata(Control hoveredControl, DragEventArgs e)
    {
        var pos = e.GetPosition(hoveredControl);

        return new DropMetadata()
        {
            HoveredControl = hoveredControl,
            HoverLocation = pos,
            DragEventArgs = e
        };
    }
}
