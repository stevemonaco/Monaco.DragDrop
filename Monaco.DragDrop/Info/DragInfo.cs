using Avalonia;
using Monaco.DragDrop.Abstractions;

namespace Monaco.DragDrop;

/// <summary>
/// Contains metadata about the app state at the time of drag initiation
/// </summary>
public class DragInfo
{
    /// <summary>
    /// Drag Operation that created this metadata
    /// </summary>
    public required IDragOperation DragOperation { get; init; }

    /// <summary>
    /// Pointer location when first clicked
    /// </summary>
    public Point DragOrigin { get; init; }

    /// <summary>
    /// Ids specified by the DragOperation
    /// </summary>
    public required IList<string> DragIds { get; init; }
}
