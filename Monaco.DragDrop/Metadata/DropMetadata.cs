using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Monaco.DragDrop;
public class DropMetadata
{
    /// <summary>
    /// Pointer location relative to HoveredControl
    /// </summary>
    public Point HoverLocation { get; init; }

    public required Control HoveredControl { get; init; }

    public required DragEventArgs DragEventArgs { get; init; }
}
