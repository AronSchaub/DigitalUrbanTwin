using Godot;

namespace Leipzig3DGodot.Scripts;

[Tool]
public partial class TimeController : Node
{
    public static TimeController Instance { get; set; }
    public DateTimeResource DateTime => gameTime;

    private DateTimeResource gameTime = DateTimeResource.Now;
    private bool useRealTime;

    [Signal]
    public delegate void DateTimeChangedEventHandler(DateTimeResource dateTimeResource);

    public override void _EnterTree()
    {
        Instance = this;
    }

    [Export(PropertyHint.Range, "1900, 2100")]
    public int Year
    {
        get => gameTime.Year;
        set => gameTime.Year = value;
    }

    [Export(PropertyHint.Range, "1, 12")]
    public int Month
    {
        get => gameTime.Month;
        set => gameTime.Month = value;
    }

    [Export(PropertyHint.Range, "1, 31")]
    public int Day
    {
        get => gameTime.Day;
        set => gameTime.Day = value;
    }

    [Export(PropertyHint.Range, "0, 23")]
    public int Hour
    {
        get => gameTime.Hour;
        set => gameTime.Hour = value;
    }


    [Export(PropertyHint.Range, "0, 59")]
    public int Minutes
    {
        get => gameTime.Minute;
        set => gameTime.Minute = value;
    }

    [Export(PropertyHint.Range, "0, 59")]
    public int Seconds
    {
        get => gameTime.Second;
        set => gameTime.Second = value;
    }

    [Export(PropertyHint.Range, "0, 999")]
    public int Milliseconds
    {
        get => gameTime.Millisecond;
        set => gameTime.Millisecond = value;
    }

    [Export]
    public float Multiplier { get; set; } = 1f;

    [Export]
    public bool UseRealTime
    {
        get => useRealTime;
        set
        {
            if (value)
                gameTime = DateTimeResource.Now;
            useRealTime = false;
        }
    }

    public override void _Process(double delta)
    {
        if (!Engine.IsEditorHint())
        {
            // if (gameTime.Second == 0 && gameTime.Minute == 0)
            //     GetViewport().GetTexture().GetImage().SavePng(System.DateTime.Now.ToString("HH:mm") + ".png");
            gameTime.Second += (int)(Multiplier * 1);
            EmitSignal(SignalName.DateTimeChanged, gameTime);
        }
    }
}