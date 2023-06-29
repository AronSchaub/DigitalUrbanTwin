using Godot;

namespace Leipzig3DGodot.Scripts.Tiles;

///<author email="aron.schaub@stud.htwk-leipzig.de">Aron Schaub</author>
public partial class Street : Path3D
{
	private CsgPolygon3D? shape;

	[Export]
	public Material Material { get; set; }

	[Export]
	public string StreetName { get; set; }
	
	public float Width
	{
		set
		{
			float rad = value / 2;
			const float height = 0.05f;
			Shape.Polygon = new[]
			{
				new Vector2(-rad, 0),
				new Vector2(-rad, height),
				new Vector2(0, height),
				new Vector2(rad, height),
				new Vector2(rad, 0)
			};
		}
	}

	public string Texture { get; set; }

	private CsgPolygon3D Shape
	{
		get
		{
			shape ??= this.GetChild<CsgPolygon3D>();
			return shape;
		}
	}

	public override void _Ready()
	{
		var label3D = this.GetChild<Label3D>(true);
		if (label3D != null)
		{
			label3D.Text = StreetName;
		}
	}
}
