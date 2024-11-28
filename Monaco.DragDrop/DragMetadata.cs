using Avalonia;
using Avalonia.Controls;
using Monaco.DragDrop.Abstractions;
using System.Collections;

namespace Monaco.DragDrop;

/// <summary>
/// Contains metadata about the app state at the time of drag initiation
/// </summary>
public class DragMetadata
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

    /// <summary>
    /// Collection source containing the to-be-dropped Payload where items will be removed from
    /// after the Payload has been transferred
    /// </summary>
    public IList? PayloadCollection { get; init; }

    /// <summary>
    /// UI container in the collection control associated with the drag
    /// ie. ListBoxItem, TreeViewItem, or topmost for ItemsControl
    /// </summary>
    public Control? PayloadContainer { get; init; }

    /// <summary>
    /// Index of the container in the collection control
    /// Should match the index in the PayloadCollection
    /// </summary>
    public int? PayloadContainerIndex { get; init; }
}
