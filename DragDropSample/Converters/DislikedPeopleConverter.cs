using Avalonia.Data;
using Avalonia.Data.Converters;
using DragDropSample.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DragDropSample.Converters;
public class DislikedPeopleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IList<PersonViewModel> people)
            return BindingOperations.DoNothing;

        return people switch
        {
            { Count: 0 } => "Dislikes: None",
            { Count: 1 } => $"Dislikes: {people[0].Name}",
            { Count: 2 } => $"Dislikes: {people[0].Name} and {people[1].Name}",
            _ => $"Dislikes: {string.Join(", ", people.Select(x => x.Name))}"
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
