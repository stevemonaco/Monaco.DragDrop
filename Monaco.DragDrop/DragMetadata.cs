using Avalonia;

namespace Monaco.DragDrop;
public class DragMetadata
{
    public Point DragOrigin { get; init; }
    public IList<string> DragIds { get; init; } = [];
}
