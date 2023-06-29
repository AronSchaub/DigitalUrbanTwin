using System.Collections.Generic;
using Godot;

namespace Leipzig3DGodot.Scripts.MVVM.View;

public class MVVMConverter : IMVVMConverter
{
    public static MVVMConverter Default = new();

    public static Dictionary<string, MVVMConverter> Converters = new()
    {
        { nameof(MVVMConverter), Default },
        { nameof(DateTimeConverter), new DateTimeConverter() },
        { nameof(SelectionFilterConverter), new SelectionFilterConverter() },
        { nameof(MinuteOfDayConverter), new MinuteOfDayConverter() }
    };

    public virtual object? Convert(object? sourceValue, object? targetValue, string param)
    {
        if (sourceValue?.GetType() == targetValue?.GetType())
            return (sourceValue ?? targetValue) ?? default;
        if (typeof(string) == targetValue?.GetType())
            return sourceValue?.ToString() ?? string.Empty;
        if (typeof(int) == targetValue?.GetType())
            return System.Convert.ToInt32(sourceValue);
        if (typeof(float) == targetValue?.GetType())
            return System.Convert.ToSingle(sourceValue);
        if (typeof(double) == targetValue?.GetType())
            return System.Convert.ToDouble(sourceValue);
        if (typeof(bool) == targetValue?.GetType())
            return System.Convert.ToBoolean(sourceValue);
        GD.PrintErr($"unsupported type conversion: {sourceValue?.GetType()} -> {targetValue?.GetType()}");

        return targetValue;
    }

    public virtual object? ConvertBack(object? sourceValue, object? targetValue, string param)
    {
        return Convert(sourceValue, targetValue, param);
    }
}

public interface IMVVMConverter
{
    public object? Convert(object? sourceValue, object? targetValue, string param);

    public object? ConvertBack(object? sourceValue, object? targetValue, string param);
}