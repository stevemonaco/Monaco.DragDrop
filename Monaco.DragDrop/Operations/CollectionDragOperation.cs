using Avalonia;
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

    protected override DragMetadata CreateMetadata()
    {
        return new DragMetadata()
        {
            DragOrigin = _dragOrigin!.Value,
            DragIds = InteractionIds.ToList(),
            PayloadCollection = PayloadCollection,
        };
    }
}
