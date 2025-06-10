using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Monaco.DragDrop;

public class TreeDropOperation : CollectionDropOperation
{
    protected override void Drop(object? sender, DragEventArgs e)
    {
        if (!TryGetDragInfo<CollectionDragInfo>(e, out var dragInfo) || !TryGetPayload<object>(e, out var payload))
            return;
        
        this.InvokePayloadCommand(e, dragInfo, this.ItemDropAdorner?.Target ?? DropTargetOffset.AfterTarget);
        
        e.Handled = true;
        ((IPseudoClasses)AttachedControl!.Classes).Set(":dropover", false);
        ItemDropAdorner?.Detach();
        DropAdorner?.Detach();
    }
}
