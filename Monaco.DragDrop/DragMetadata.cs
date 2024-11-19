using Avalonia;
using System.Collections;

namespace Monaco.DragDrop;
public class DragMetadata
{
    public Point DragOrigin { get; init; }
    public IList<string> DragIds { get; init; } = [];

    /// <summary>
    /// Collection containing the dropped Payload where items will be removed from
    /// after the Payload has been transferred
    /// </summary>
    public IList? PayloadCollection { get; init; }
}
