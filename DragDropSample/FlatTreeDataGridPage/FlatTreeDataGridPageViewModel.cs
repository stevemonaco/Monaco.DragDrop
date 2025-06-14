using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Bogus;
using DragDropSample.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DragDropSample.ViewModels;
public class FlatTreeDataGridPageViewModel : PageViewModel
{
    public FlatTreeDataGridSource<PersonViewModel> PersonSource { get; }
    public ObservableCollection<PersonViewModel> People { get; } = new();

    public FlatTreeDataGridPageViewModel()
    {
        Title = "FlatTreeDataGrid";

        People = new(CreateNewPeople(30));

        PersonSource = new FlatTreeDataGridSource<PersonViewModel>(People)
        {
            Columns = 
            {
                new TextColumn<PersonViewModel, string>("ID", p => $"{p.PersonId}"),
                new TextColumn<PersonViewModel, string>("Name", p => p.Name),
                new TextColumn<PersonViewModel, string>("Age", p => $"{p.Age}"),
                new TextColumn<PersonViewModel, string>("Address", p => p.Address)
            }
        };

        PersonSource.RowSelection!.SingleSelect = false;
    }

    private IEnumerable<PersonViewModel> CreateNewPeople(int personCount)
    {
        int personId = 1;
        var personFaker = new Faker<PersonViewModel>()
            .RuleFor(p => p.PersonId, f => personId++)
            .RuleFor(p => p.Name, f => f.Name.FullName())
            .RuleFor(p => p.Age, f => f.Random.Number(20, 60))
            .RuleFor(p => p.Address, f => f.Address.FullAddress());

        return personFaker.Generate(personCount);
    }
}
