using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DragDropSample.ViewModels;
public partial class ListBoxPageViewModel : PageViewModel
{
    [ObservableProperty]
    public partial ObservableCollection<WorkerViewModel> AvailableWorkers { get; set; } = [];
    public ObservableCollection<WorkerViewModel> MiningTeam { get; } = [];
    public ObservableCollection<WorkerViewModel> CraftingTeam { get; } = [];
    public ObservableCollection<WorkerViewModel> CombatTeam { get; } = [];

    [ObservableProperty]
    public partial int MiningBudgetRemaining { get; set; } = _maxBudget;

    [ObservableProperty]
    public partial int CraftingBudgetRemaining { get; set; } = _maxBudget;

    [ObservableProperty]
    public partial int CombatBudgetRemaining { get; set; } = _maxBudget;

    [ObservableProperty]
    public partial int MiningTeamProficiency { get; set; }

    [ObservableProperty]
    public partial int CraftingTeamProficiency { get; set; }

    [ObservableProperty]
    public partial int CombatTeamProficiency { get; set; }

    private const int _maxBudget = 20000;

    public ListBoxPageViewModel()
    {
        Title = "ListBox";
        InitializeNewWorkers(30);

        MiningTeam.CollectionChanged += (s, e) =>
        {
            MiningBudgetRemaining = _maxBudget - MiningTeam.Sum(x => x.Salary);
            MiningTeamProficiency = MiningTeam.Select(x => x.MiningProficiency).OfType<int>().Sum();
        };

        CraftingTeam.CollectionChanged += (s, e) =>
        {
            CraftingBudgetRemaining = _maxBudget - CraftingTeam.Sum(x => x.Salary);
            CraftingTeamProficiency = CraftingTeam.Select(x => x.CraftingProficiency).OfType<int>().Sum();
        };

        CombatTeam.CollectionChanged += (s, e) =>
        {
            CombatBudgetRemaining = _maxBudget - CombatTeam.Sum(x => x.Salary);
            CombatTeamProficiency = CombatTeam.Select(x => x.CombatProficiency).OfType<int>().Sum();
        };
    }

    private void InitializeNewWorkers(int workerCount)
    {
        int workerId = 1;
        var workerFaker = new Faker<WorkerViewModel>()
            //.StrictMode(true)
            .RuleFor(p => p.PersonId, f => workerId++)
            .RuleFor(p => p.Name, f => f.Name.FullName())
            .RuleFor(p => p.Age, f => f.Random.Number(20, 60))
            .RuleFor(p => p.Salary, f => f.Random.Number(100, 5000))
            .RuleFor(p => p.MiningProficiency, f => f.Random.Number(1, 10))
            .RuleFor(p => p.CraftingProficiency, f => f.Random.Number(1, 10).OrNull(f, 0.5f))
            .RuleFor(p => p.CombatProficiency, f => f.Random.Number(1, 10).OrNull(f, 0.5f));

        var workers = workerFaker.Generate(workerCount);
        var workerIndices = Enumerable.Range(0, workerCount).ToArray();

        foreach (var worker in workers)
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
            
            worker.DislikedWorkers = new(workerIndices.OrderBy(x => Random.Shared.Next()).Take(dislikedCount).Select(x => workers[x]));
        }

        AvailableWorkers = new(workers.OrderBy(x => x.Name));
    }
}
