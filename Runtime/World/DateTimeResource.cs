using System;
using System.Globalization;
using System.Linq;
using Godot;
using OsmSharp.IO.PBF;

namespace Leipzig3DGodot.Scripts;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
[Tool]
public partial class DateTimeResource : Resource
{
    private int millisecond;
    private int second;
    private int minute;
    private int hour;
    private int day = 1;
    private int month = 1;
    private int year = 1;
    public static DateTimeResource Now => FromDateTime(DateTime.Now);

    public delegate void DateTimeChangedEventHandler(DateTimeResource dateTimeResource);

    public delegate void YearChangedEventHandler(int year);

    public delegate void MonthChangedEventHandler(int month);

    public delegate void DayChangedEventHandler(int day);

    public delegate void HourChangedEventHandler(int hour);

    public delegate void MinuteChangedEventHandler(int minute);

    public delegate void SecondChangedEventHandler(int second);

    public delegate void MillisecondChangedEventHandler(int millisecond);

    private DateTimeChangedEventHandler? dateTimeChanged;

    public event DateTimeChangedEventHandler? DateTimeChanged
    {
        add
        {
            if (dateTimeChanged == null || !dateTimeChanged.GetInvocationList().Select(il => il.Method).Contains(value?.Method))
                dateTimeChanged += value;
        }
        remove
        {
            if (dateTimeChanged?.GetInvocationList().FirstOrDefault(il => il.Method == value?.Method) is DateTimeChangedEventHandler eventMethod)
                dateTimeChanged -= eventMethod;
        }
    }

    public int Year
    {
        get => year;
        set
        {
            if (value == year)
                return;
            var dateTime = ToDateTime();
            dateTime = dateTime.AddYears(value - year);
            day = dateTime.Day;
            month = dateTime.Month;
            year = dateTime.Year;
            // EmitSignal(SignalName.YearChanged, year);
            // EmitSignal(SignalName.MonthChanged, month);
            // EmitSignal(SignalName.DayChanged, day);
            // EmitSignal(SignalName.DateTimeChanged);
            dateTimeChanged?.Invoke(this);
        }
    }

    public int Month
    {
        get => month;
        set
        {
            if (value == month)
                return;
            var dateTime = ToDateTime();
            dateTime = dateTime.AddMonths(value - month);
            day = dateTime.Day;
            month = dateTime.Month;
            year = dateTime.Year;
            // EmitSignal(SignalName.YearChanged, year);
            // EmitSignal(SignalName.MonthChanged, month);
            // EmitSignal(SignalName.DayChanged, day);
            // EmitSignal(SignalName.DateTimeChanged);
            dateTimeChanged?.Invoke(this);
        }
    }

    public int Day
    {
        get => day;
        set
        {
            if (value == day)
                return;
            var dateTime = ToDateTime();
            dateTime = dateTime.AddDays(value - day);
            day = dateTime.Day;
            month = dateTime.Month;
            year = dateTime.Year;
            // EmitSignal(SignalName.YearChanged, year);
            // EmitSignal(SignalName.MonthChanged, month);
            // EmitSignal(SignalName.DayChanged, day);
            // EmitSignal(SignalName.DateTimeChanged);
            dateTimeChanged?.Invoke(this);
        }
    }

    public int Hour
    {
        get => hour;
        set
        {
            if (hour == value)
                return;
            hour = value;
            var days = 0;
            while (hour >= 24)
            {
                hour -= 24;
                days += 1;
            }

            while (hour < 0)
            {
                hour += 24;
                days -= 1;
            }

            Day += days;

            // EmitSignal(SignalName.HourChanged, hour);
            // EmitSignal(SignalName.DayChanged, day);
            // EmitSignal(SignalName.DateTimeChanged);
            if (days == 0)
                dateTimeChanged?.Invoke(this);
        }
    }

    public int Minute
    {
        get => minute;
        set
        {
            if (minute == value)
                return;
            minute = value;
            var hours = 0;
            while (minute >= 60)
            {
                minute -= 60;
                hours += 1;
            }

            while (minute < 0)
            {
                minute += 60;
                hours -= 1;
            }

            Hour += hours;
            // EmitSignal(SignalName.MinuteChanged, minute);
            // EmitSignal(SignalName.HourChanged, hour);
            // EmitSignal(SignalName.DateTimeChanged);
            if (hours == 0)
                dateTimeChanged?.Invoke(this);
        }
    }

    public int Second
    {
        get => second;
        set
        {
            if (second == value)
                return;
            second = value;
            var minutes = 0;
            while (second >= 60)
            {
                second -= 60;
                minutes += 1;
            }

            while (second < 0)
            {
                second += 60;
                minutes -= 1;
            }

            Minute += minutes;
            // EmitSignal(SignalName.SecondChanged, second);
            // EmitSignal(SignalName.MinuteChanged, minute);
            // EmitSignal(SignalName.DateTimeChanged);
            if (minutes == 0)
                dateTimeChanged?.Invoke(this);
        }
    }

    public int Millisecond
    {
        get => millisecond;
        set
        {
            if (millisecond == value)
                return;
            millisecond = value;
            var seconds = 0;
            while (millisecond >= 1000)
            {
                millisecond -= 1000;
                seconds += 1;
            }

            while (millisecond < 0)
            {
                millisecond += 1000;
                seconds -= 1;
            }

            Second += seconds;

            // EmitSignal(SignalName.MillisecondChanged, millisecond);
            // EmitSignal(SignalName.SecondChanged, second);
            // EmitSignal(SignalName.DateTimeChanged);
            if (seconds == 0)
                dateTimeChanged?.Invoke(this);
        }
    }


    public long TimeMillis()
    {
        return ToDateTime().ToUnixTime();
    }

    public string ToString()
    {
        return ToDateTime().ToString();
    }

    public string ToString(string? format)
    {
        return ToDateTime().ToString(format);
    }

    private DateTime ToDateTime()
    {
        // GD.Print($"{Year} {Month} {Day} {Hour} {Minute} {Second} {Millisecond}");
        return new DateTime(Year, Month, Day, Hour, Minute, Second, Millisecond);
    }

    private static DateTimeResource FromDateTime(DateTime dateTime)
    {
        var dateTimeResource = new DateTimeResource();
        dateTimeResource.Year = dateTime.Year;
        dateTimeResource.Month = dateTime.Month;
        dateTimeResource.Day = dateTime.Day;
        dateTimeResource.Hour = dateTime.Hour;
        dateTimeResource.Minute = dateTime.Minute;
        dateTimeResource.Second = dateTime.Second;
        dateTimeResource.Millisecond = dateTime.Millisecond;
        return dateTimeResource;
    }

    public string ToString(IFormatProvider? provider) => ToDateTime().ToString(provider);

    public static DateTimeResource ParseExact(string s, string param, IFormatProvider? provider)
    {
        return FromDateTime(DateTime.ParseExact(s, param, provider));
    }
}