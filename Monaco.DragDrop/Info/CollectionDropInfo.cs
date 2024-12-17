using Avalonia.Controls;
using System.Collections;

namespace Monaco.DragDrop;
public class CollectionDropInfo : DropInfo
{
    /// <summary>
    /// Collection that the payload has been dropped into
    /// </summary>
    public IList? TargetCollection { get; init; }

    /// <summary>
    /// UI container in the collection control associated with the drop
    /// ie. ListBoxItem, TreeViewItem, or topmost for ItemsControl
    /// </summary>
    public Control? TargetContainer { get; init; }

    /// <summary>
    /// Index of the TargetCollection where the payload was dropped
    /// Should match the index in the PayloadCollection
    /// </summary>
    public int? TargetIndex { get; init; }
}
