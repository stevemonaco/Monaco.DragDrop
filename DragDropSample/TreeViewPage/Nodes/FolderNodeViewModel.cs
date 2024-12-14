using System.Collections.ObjectModel;

namespace DragDropSample.ViewModels.Nodes;
public partial class FolderNodeViewModel : NodeViewModel, IParentNode
{
    public ObservableCollection<NodeViewModel> Children { get; init; } = [];

    public FolderNodeViewModel(string name) : base(name)
    {
    }

    public bool CanAddNode(NodeViewModel node)
    {
        return node is FileNodeViewModel;
    }
}
