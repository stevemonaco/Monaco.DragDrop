using Avalonia.Data.Converters;

namespace DragDropSample.Converters;
public static class AppConverters
{
    public static IValueConverter DislikedWorkers { get; } = new DislikedWorkersConverter();
}
