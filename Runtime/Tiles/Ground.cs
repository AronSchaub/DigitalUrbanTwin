using Godot;
using Leipzig3DGodot;
using Leipzig3DGodot.Scripts;
using OSGeo.GDAL;
using OSGeo.OGR;

public partial class Ground : Node
{
	[Export]
	private int centerWebmercatorX = 1747139;

	[Export]
	private int centerWebmercatorY = 6154178;

	[Export]
	private float radius = 1000.0f;

	[Export]
	private int maxStreets = 300;

	public override void _Ready()
	{
		// GdalConfiguration.ConfigureGdal();
		//
		// Gdal.AllRegister();
		//
		// var dataSet = Gdal.Open("/media/data/Projects/masterprojekt/Downloads/Leipzig/dop20rgbi_33316_5700_2_sn.tif", Access.GA_ReadOnly);
		//
		// GD.Print(dataSet.GetDriver());
		//
		// GdalConfiguration.ConfigureOgr();
		//
		// Ogr.RegisterAll();
		//
		// var dataSource = Ogr.Open("/media/data/Projects/masterprojekt/Data/Leipzig.gpkg", 1);
		//
		// GD.Print(dataSource.GetDriver());
		//
		// for (var layerId = 0; layerId < dataSource.GetLayerCount(); layerId++)
		// {
		//     GD.Print($"{layerId}: {dataSource.GetLayerByIndex(layerId)}");
		// }

		// FindNode() 


		// var geoDataset = GeodotWrapper.Instance.GetDataset("/media/data/Projects/geodot-plugin/demo/geodata/geopackage_sample.gpkg");
		// GD.Print(geoDataset);
		// GD.Print(geoDataset.IsValid());
		// // GD.Print(geoDataset.GetFeatureLayers());
		// foreach (var layer in geoDataset.GetRasterLayers())
		// 	GD.Print(layer);
		// foreach (var layer in geoDataset.GetFeatureLayers())
		// {
		// 	// var layer = geoDataset.GetFeatureLayer("2_linknetz_ogd_WM");
		// 	GD.Print(layer);
		// 	// var lines = layer.GetFeaturesNearPosition(centerWebmercatorX, centerWebmercatorY, radius, maxStreets);
		// 	var lines = layer.GetAllFeatures();
		// 	foreach (var line in lines)
		// 	{
		// 		// line.GetOffsetCurve();
		// 		GD.Print(line);
		// 	}
		// }
		//
		// GD.Print(GeodotWrapper.Instance.GetRasterLayer("/media/data/Projects/geodot-plugin/demo/geodata/vienna-test-ortho.jpg"));
		// GD.Print(GeodotWrapper.Instance.GetRasterLayerForPyramid("", ""));
		// GD.Print(GeodotWrapper.Instance.TransformCoordinates(Vector3.One, "", ""));
	}

//  public override void _Process(double delta)
//  {
//      
//  }
}
