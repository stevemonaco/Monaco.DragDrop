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
    };

    [ObservableProperty] private PageViewModel _selectedPage;

    public MainWindowViewModel()
    {
        SelectedPage = Pages.First();
    }
}
