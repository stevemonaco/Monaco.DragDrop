using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Monaco.DragDrop.Abstractions;
using System.Collections;

namespace Monaco.DragDrop;
public class CollectionDragOperation : DragOperation
{
    public static readonly StyledProperty<IList?> PayloadCollectionProperty =
    AvaloniaProperty.Register<DragOperationBase, IList?>(nameof(PayloadCollection));

    /// <summary>
    /// Specifies the collection the Payload is inside of so it can be removed when 
    /// the Drop transfer occurs
    /// </summary>
    public IList? PayloadCollection
    {
        get => GetValue(PayloadCollectionProperty);
        set => SetValue(PayloadCollectionProperty, value);
    }

    public CollectionDragOperation()
    {
        _handledEventsToo = true;
    }

    protected override DragMetadata CreateMetadata(PointerEventArgs e)
    {
        var container = LocatePayloadContainer(e);
        var index = LocatePayloadContainerIndex(container);

        return new DragMetadata()
        {
            DragOperation = this,
            DragOrigin = _dragOrigin!.Value,
            DragIds = InteractionIds.ToList(),
            PayloadCollection = PayloadCollection,
            PayloadContainer = container,
            PayloadContainerIndex = LocatePayloadContainerIndex(container)
        };
    }

    protected virtual Control? LocatePayloadContainer(RoutedEventArgs triggerEvent)
    {
        if (AttachedControl is ItemsControl items && triggerEvent.Source is Control sourceItem)
        {
            var ancestors = sourceItem.GetSelfAndLogicalAncestors();
            return ancestors.TakeWhile(x => x != AttachedControl).OfType<Control>().LastOrDefault();
        }
        else
        {
            return null;
        }
    }

    protected virtual int? LocatePayloadContainerIndex(Control? container)
    {
        if (AttachedControl is ItemsControl items && container is not null)
        {
            return items.IndexFromContainer(container);
        }
        else
        {
            return null;
        }
    }
}
