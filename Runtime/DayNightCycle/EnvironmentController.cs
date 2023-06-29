using Godot;

namespace Leipzig3DGodot.Scripts;

[Tool]
public partial class EnvironmentController : WorldEnvironment
{
	public static EnvironmentController Instance { get; private set; }

	public Sky Sky => Environment.Sky;
	public Sun? Sun => this.GetChild<Sun>();
	public Moon? Moon => this.GetChild<Moon>();
	public TimeController? TimeController => this.GetChild<TimeController>();

	public override void _Ready()
	{
		Instance = this;
		if (TimeController != null && Sun != null)
			TimeController.DateTimeChanged += dateTime => Sun.SetPosition(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
		// if (TimeController != null && Moon != null)
		//     TimeController.DateTimeChanged += dateTime => Moon.SetPosition(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
		base._Ready();
	}
}
