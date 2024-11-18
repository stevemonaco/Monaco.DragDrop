using Avalonia.Data.Converters;

namespace DragDropSample.Converters;
public static class AppConverters
{
    public static IValueConverter DislikedPeople { get; } = new DislikedPeopleConverter();
}
