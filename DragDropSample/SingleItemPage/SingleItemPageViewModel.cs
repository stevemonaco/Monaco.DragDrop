using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DragDropSample.ViewModels;
public partial class SingleItemPageViewModel : PageViewModel
{
    [ObservableProperty]
    public partial string MealChoiceA { get; set; } = "Pizza";

    [ObservableProperty]
    public partial string MealChoiceB { get; set; } = "Tacos";

    [ObservableProperty]
    public partial string MealChoiceC { get; set; } = "Steak";

    [ObservableProperty]
    public partial string MealChoiceD { get; set; } = "Salad";

    [ObservableProperty]
    public partial string DrinkChoiceA { get; set; } = "Water";

    [ObservableProperty]
    public partial string DrinkChoiceB { get; set; } = "Tea";

    [ObservableProperty]
    public partial string DrinkChoiceC { get; set; } = "Soda";

    [ObservableProperty]
    public partial string DrinkChoiceD { get; set; } = "Juice";

    [ObservableProperty]
    public partial string? SelectedMeal { get; set; }

    [ObservableProperty]
    public partial string? SelectedDrink { get; set; }

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
