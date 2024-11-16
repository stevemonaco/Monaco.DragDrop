using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Monaco.DragDrop;
public static class ThrowIf
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
    public static void NotNull(object? value, string? message = default, [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {
        if (value is null)
            return;

        if (message is not null)
            throw new ArgumentException($"{message}: {valueExpression} was not null");
        throw new ArgumentException($"{valueExpression} was not null");
    }
}
