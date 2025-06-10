using Avalonia.Data;
using Avalonia.Data.Converters;
using DragDropSample.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DragDropSample.Converters;
public class DislikedWorkersConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IList<WorkerViewModel> workers)
            return BindingOperations.DoNothing;

        return workers switch
        {
            { Count: 0 } => "Dislikes: None",
            { Count: 1 } => $"Dislikes: {workers[0].Name}",
            { Count: 2 } => $"Dislikes: {workers[0].Name} and {workers[1].Name}",
            _ => $"Dislikes: {string.Join(", ", workers.Select(x => x.Name))}"
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
