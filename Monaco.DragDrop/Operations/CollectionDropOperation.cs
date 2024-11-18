using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Monaco.DragDrop.Abstractions;
using System.Collections;

namespace Monaco.DragDrop;
public class CollectionDropOperation : DropOperationBase
{
    public static readonly StyledProperty<IList?> SourceCollectionProperty =
        AvaloniaProperty.Register<DropOperationBase, IList?>(nameof(SourceCollection));

    /// <summary>
    /// SourceCollection for the dropped Payload where items will be removed from
    /// </summary>
    public IList? SourceCollection
    {
        get => GetValue(SourceCollectionProperty);
        set => SetValue(SourceCollectionProperty, value);
    }

    protected override void Drop(object? sender, DragEventArgs e)
    {
        var metadata = GetMetadata(e);
        var payload = GetPayload(e, metadata);

        if (payload is not null && PayloadTarget is IList targetCollection)
        {
            targetCollection.Add(payload);

            if (SourceCollection is IList)
                SourceCollection.Remove(payload);
        }

        ((IPseudoClasses)AttachedControl!.Classes).Set(":dropover", false);
        DropAdorner?.Detach();
    }
}
