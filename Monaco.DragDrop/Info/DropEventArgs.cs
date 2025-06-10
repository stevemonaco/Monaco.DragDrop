namespace Monaco.DragDrop;

public record class DropEventArgs(
    object? Target,
    object? TargetContext,
    IEnumerable<object> Items,
    DropTargetOffset Offset);
