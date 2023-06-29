using System;
using System.Globalization;
using System.Runtime.Serialization;
using Godot;

namespace Leipzig3DGodot.Scripts.MVVM.View;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
public partial class DateTimeConverter : MVVMConverter
{
    public override object Convert(object? sourceValue, object? targetValue, string param)
    {
        if (sourceValue is DateTimeResource dateTime)
        {
            return dateTime.ToString(param);
        }

        GD.PrintErr(sourceValue + " not instance of " + nameof(DateTimeResource));
        return base.Convert(sourceValue, targetValue, param);
    }

    public override object ConvertBack(object? sourceValue, object? targetValue, string param)
    {
        return targetValue is DateTimeResource
            ? DateTimeResource.ParseExact(sourceValue + "", param, CultureInfo.InvariantCulture)
            : base.ConvertBack(sourceValue, targetValue, param);
    }
}