using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Monaco.DragDrop.Abstractions;
using System.Collections;

namespace Monaco.DragDrop;
public class CollectionDropOperation : DropOperationBase
{
    protected override void Drop(object? sender, DragEventArgs e)
    {
        var metadata = GetMetadata(e);
        var payload = GetPayload(e, metadata);

        if (payload is not null && metadata is not null && PayloadTarget is IList targetCollection)
        {
            targetCollection.Add(payload);

            if (metadata.PayloadCollection is IList payloadCollection)
                payloadCollection.Remove(payload);
        }

        ((IPseudoClasses)AttachedControl!.Classes).Set(":dropover", false);
        DropAdorner?.Detach();
    }
}
