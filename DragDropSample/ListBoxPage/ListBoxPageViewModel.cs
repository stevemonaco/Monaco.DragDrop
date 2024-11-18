using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DragDropSample.ViewModels;
public partial class ListBoxPageViewModel : PageViewModel
{
    [ObservableProperty] private ObservableCollection<PersonViewModel> _availableStaff = [];
    public ObservableCollection<PersonViewModel> MiningTeam { get; } = [];
    public ObservableCollection<PersonViewModel> CraftingTeam { get; } = [];
    public ObservableCollection<PersonViewModel> CombatTeam { get; } = [];

    public ListBoxPageViewModel()
    {
        Title = "ListBox";
        InitializeNewStaff(50);
    }

    private void InitializeNewStaff(int staffCount)
    {
        int personId = 1;
        var personFaker = new Faker<PersonViewModel>()
            //.StrictMode(true)
            .RuleFor(p => p.PersonId, f => personId++)
            .RuleFor(p => p.Name, f => f.Name.FullName())
            .RuleFor(p => p.Age, f => f.Random.Number(20, 60))
            .RuleFor(p => p.Salary, f => f.Random.Number(100, 5000))
            .RuleFor(p => p.MiningProficiency, f => f.Random.Number(1, 10))
            .RuleFor(p => p.CraftingProficiency, f => f.Random.Number(1, 10).OrNull(f, 0.5f))
            .RuleFor(p => p.CombatProficiency, f => f.Random.Number(1, 10).OrNull(f, 0.5f));

        var people = personFaker.Generate(staffCount);
        var peopleIndices = Enumerable.Range(0, staffCount).ToArray();

        foreach (var person in people)
        {
            var rand = Random.Shared.NextDouble();

            var dislikedCount = rand switch
            {
                < 0.02 => 3,
                < 0.05 => 2,
                < 0.10 => 1,
                _ => 0
            };

            if (dislikedCount == 0)
                continue;
            
            person.DislikedPeople = new(peopleIndices.OrderBy(x => Random.Shared.Next()).Take(3).Select(x => people[x]));
        }

        AvailableStaff = new(people);
    }
}
