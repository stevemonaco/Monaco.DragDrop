using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace DragDropSample.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public ObservableCollection<PageViewModel> Pages { get; } = new()
    {
        new SingleItemPageViewModel(),
        new SimpleCollectionPageViewModel(),
        new ListBoxPageViewModel(),
        new TreeViewPageViewModel(),
        new FlatTreeDataGridPageViewModel()
    };

    [ObservableProperty]
    public partial PageViewModel SelectedPage { get; set; }

    public MainWindowViewModel()
    {
        SelectedPage = Pages.First();
    }
}
