using System.Globalization;
using Godot;

namespace Leipzig3DGodot.Scripts.MVVM.View;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
public class MinuteOfDayConverter : MVVMConverter
{
    public override object Convert(object? sourceValue, object? targetValue, string param)
    {
        if (sourceValue is DateTimeResource dateTime)
        {
            // GD.Print($"{dateTime.Hour} {dateTime.Minute}  {dateTime.Hour * 60 + dateTime.Minute}");
            return dateTime.Hour * 60 + dateTime.Minute;
        }

        return base.Convert(sourceValue, targetValue, param);
    }

    public override object ConvertBack(object? sourceValue, object? targetValue, string param)
    {
        if (targetValue is DateTimeResource targetValueR && sourceValue is double sourceValueInt)
        {
            targetValueR.Hour = (int)(sourceValueInt / 60);
            targetValueR.Minute = (int)(sourceValueInt % 60);
            // GD.Print($"{sourceValueInt}  {targetValueR.Hour} {targetValueR.Minute}");
            return targetValue;
        }

        return base.ConvertBack(sourceValue, targetValue, param);
    }
}