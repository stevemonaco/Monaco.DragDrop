using Avalonia.Controls;
using System.Collections;

namespace Monaco.DragDrop;

/// <summary>
/// Contains additional drag metadata for collection controls
/// </summary>
public class CollectionDragInfo : DragInfo
{
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
    /// Index of the PayloadCollection where the payload was dropped
    /// Should match the index in the PayloadCollection
    /// </summary>
    public int? PayloadContainerIndex { get; init; }
}
