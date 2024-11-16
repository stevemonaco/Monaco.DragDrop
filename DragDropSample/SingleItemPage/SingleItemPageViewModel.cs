using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DragDropSample.ViewModels;
public partial class SingleItemPageViewModel : PageViewModel
{
    [ObservableProperty] private string _mealChoiceA = "Pizza";
    [ObservableProperty] private string _mealChoiceB = "Tacos";
    [ObservableProperty] private string _mealChoiceC = "Steak";
    [ObservableProperty] private string _mealChoiceD = "Salad";

    [ObservableProperty] private string _drinkChoiceA = "Water";
    [ObservableProperty] private string _drinkChoiceB = "Tea";
    [ObservableProperty] private string _drinkChoiceC = "Soda";
    [ObservableProperty] private string _drinkChoiceD = "Juice";

    [ObservableProperty] private string? _selectedMeal;
    [ObservableProperty] private string? _selectedDrink;

    public SingleItemPageViewModel()
    {
        Title = "Single Item";
    }

    [RelayCommand]
    public void RemoveMeal()
    {
        SelectedMeal = null;
    }

    [RelayCommand]
    public void RemoveDrink()
    {
        SelectedDrink = null;
    }
}
