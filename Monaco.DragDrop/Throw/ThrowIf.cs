using System.Runtime.CompilerServices;

namespace Monaco.DragDrop;
internal static class ThrowIf
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void False(bool condition, [CallerArgumentExpression(nameof(condition))] string conditionExpression = "")
    {
        if (condition)
            return;

        throw new ArgumentException($"{conditionExpression} was true");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void True(bool condition, [CallerArgumentExpression(nameof(condition))] string conditionExpression = "")
    {
        if (condition is false)
            return;

        throw new ArgumentException($"{conditionExpression} was false");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Null(object? value, string? message = default, [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {
        if (value is not null)
            return;

        if (message is not null)
            throw new ArgumentException($"{message}: {valueExpression} was null");
        throw new ArgumentException($"{valueExpression} was null");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void NotNull(object? value, string? message = default, [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {
        if (value is null)
            return;

        if (message is not null)
            throw new ArgumentException($"{message}: {valueExpression} was not null");
        throw new ArgumentException($"{valueExpression} was not null");
    }
}
