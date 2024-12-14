using DragDropSample.ViewModels.Nodes;
using System.Collections.ObjectModel;

namespace DragDropSample.ViewModels;
public partial class TreeViewPageViewModel : PageViewModel
{
    public ObservableCollection<NodeViewModel> Solutions { get; }
    public ObservableCollection<NodeViewModel> FlatItems { get; } = [];

    public TreeViewPageViewModel()
    {
        Title = "TreeView";

        Solutions = new()
        {
            new ProjectNodeViewModel("DragDropSample")
            {
                Children = new()
                {
                    new FolderNodeViewModel("Assets")
                    {
                        Children =
                        [
                            new FileNodeViewModel("avalonia-logo.ico")
                        ]
                    },
                    new FolderNodeViewModel("Shell")
                    {
                        Children =
                        [
                            new FileNodeViewModel("MainWindow.axaml"),
                            new FileNodeViewModel("MainWindow.axaml.cs"),
                            new FileNodeViewModel("MainWindowViewModel.cs"),
                            new FileNodeViewModel("PageViewModel.cs")
                        ]
                    },
                    new FileNodeViewModel("App.axaml"),
                    new FileNodeViewModel("App.axaml.cs"),
                    new FileNodeViewModel("Program.cs"),
                    new FileNodeViewModel("ViewLocator.cs")
                }
            },
            new ProjectNodeViewModel("Monaco.DragDrop")
            {
                Children = new()
                {
                    new FileNodeViewModel("DragDrop.cs"),
                    new FileNodeViewModel("DragDrop.props.cs"),
                    new FileNodeViewModel("DragDropIds.cs")
                }
            }
        };
    }
}
