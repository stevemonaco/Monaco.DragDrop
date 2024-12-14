using System.Collections.ObjectModel;

namespace DragDropSample.ViewModels.Nodes;
public partial class ProjectNodeViewModel : NodeViewModel, IParentNode
{
    public ObservableCollection<NodeViewModel> Children { get; init; } = [];

    public ProjectNodeViewModel(string name) : base(name)
    {
    }

    public bool CanAddNode(NodeViewModel node)
    {
        return node is FolderNodeViewModel or FileNodeViewModel;
    }
}
