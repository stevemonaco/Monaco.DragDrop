using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Monaco.DragDrop.Abstractions;
using System.Collections;

namespace Monaco.DragDrop;

/// <summary>
/// Handles drop operations into flat collection controls
/// </summary>
public class CollectionDropOperation : DropOperationBase
{
    public static readonly StyledProperty<DropInsertionAdorner?> ItemDropAdornerProperty =
        AvaloniaProperty.Register<CollectionDropOperation, DropInsertionAdorner?>(nameof(DropAdorner), defaultBindingMode: BindingMode.OneTime);

    /// <summary>
    /// Adorner responsible for rendering overlay visual when dragging over and in a valid state
    /// </summary>
    public DropInsertionAdorner? ItemDropAdorner
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

    public static readonly StyledProperty<bool> IsTreeProperty = AvaloniaProperty.Register<CollectionDropOperation, bool>(
        nameof(IsTree));

    public bool IsTree
    {
        get => GetValue(IsTreeProperty);
        set => SetValue(IsTreeProperty, value);
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
            e.DragEffects = OnDragEnter(e, canDrop);
            this.lastEffects = e.DragEffects;
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

        e.Handled = true;
        ((IPseudoClasses)AttachedControl!.Classes).Set(":dropover", true);
    }

    protected override void DragLeave(object? sender, DragEventArgs e)
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
        
        e.DragEffects = this.OnDragOver();
        e.Handled = true;
    }

    protected override void Drop(object? sender, DragEventArgs e)
    {
        if (!TryGetDragInfo<CollectionDragInfo>(e, out var dragInfo) || !TryGetPayload<object>(e, out var payload))
            return;

        if (PayloadTarget is IList targetCollection && dragInfo.PayloadCollection is { } payloadCollection)
        {
            int actualTargetIndex = -1;

            if (_targetIndex.HasValue)
            {
                actualTargetIndex = TransferPayloadIndexed(payload, targetCollection, _targetIndex.Value, payloadCollection, dragInfo.PayloadContainerIndex);
            }
            else
            {
                actualTargetIndex = TransferPayload(payload, targetCollection, payloadCollection, dragInfo.PayloadContainerIndex);
            }

            var dropInfo = new CollectionDropInfo()
            {
                DragEventArgs = e,
                HoveredControl = (Control)sender,
                TargetCollection = targetCollection,
                TargetIndex = actualTargetIndex,
                TargetContainer = (Control)sender,
                HoverLocation = e.GetPosition((Visual)sender)
            };

            dragInfo.DragOperation.DropCompleted(e.DragEffects, dragInfo, dropInfo);
        }
        
        this.InvokePayloadCommand(e, dragInfo, this.ItemDropAdorner?.Target ?? DropTargetOffset.AfterTarget);
        
        e.Handled = true;
        ((IPseudoClasses)AttachedControl!.Classes).Set(":dropover", false);
        ItemDropAdorner?.Detach();
        DropAdorner?.Detach();
    }

    /// <summary>
    /// Transfers the payload into the target collection
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="targetCollection"></param>
    /// <param name="payloadCollection"></param>
    /// <param name="payloadIndex"></param>
    /// <returns>Actual index where payload was stored</returns>
    protected virtual int TransferPayload(object payload, IList targetCollection, IList payloadCollection, int? payloadIndex)
    {
        targetCollection.Add(payload);
        return targetCollection.Count - 1;
    }

    /// <summary>
    /// Transfers a payload to a specific index in the target collection
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="targetCollection"></param>
    /// <param name="targetIndex"></param>
    /// <param name="payloadCollection"></param>
    /// <param name="payloadIndex"></param>
    /// <returns>Actual index where payload was stored</returns>
    protected virtual int TransferPayloadIndexed(object payload, IList targetCollection, int targetIndex, IList payloadCollection, int? payloadIndex)
    {
        int indexDelta = 0;
        if (ItemDropAdorner?.TargetControl?.Classes.Contains(":dropbottom") ?? false)
            indexDelta++;

        targetCollection.Insert(targetIndex + indexDelta, payload);
        return targetIndex + indexDelta;
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
        if (this.IsTree)
        {
            if (this.DropAdorner is DropHighlightAdorner dropAdorner)
            {
                this.DropAdorner.Detach();
                this.DropAdorner = null;
            }
        }
        
        // TODO: Generalize
        if (ItemDropAdorner is DropInsertionAdorner insertionAdorner)
        {
            ItemDropAdorner.Detach();
            ItemDropAdorner.TargetControl = itemContainer;
            ItemDropAdorner = new DropInsertionAdorner()
            {
                TargetControl = itemContainer,
                SupportsChildInsertion = this.IsTree,
            };
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
        }

        {
            if (AttachedControl is TreeDataGrid treeDataGrid && triggerEvent.Source is Control sourceItem)
            {
                return sourceItem;
            }
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

    protected virtual DropInfo CreateDropMetadata(Control hoveredControl, DragEventArgs e)
    {
        var pos = e.GetPosition(hoveredControl);

        return new DropInfo()
        {
            HoveredControl = hoveredControl,
            HoverLocation = pos,
            DragEventArgs = e
        };
    }

    protected virtual DropTargetOffset GetDropKind(DragEventArgs e) => DropTargetOffset.OnTarget;
}
