// using System.Net;
// using BackgroundTask.Demo;
// using CityJson.Viewing;
// using Microsoft.AspNetCore.Mvc;
// using OSGeo.OGR;
// using OSGeo.OSR;
// using Geometry = CityJson.Geometry;
//
// namespace Leipzig3DASP.Controllers;
//
// [ApiController]
// [Route("[controller]")]
// public class RasterBasedFeatureController : ControllerBase
// {
//     private readonly ILogger<VectorBasedFeatureController> _logger;
//     public string dataSet = "../Data/osm.L.shp/l_gis_osm_buildings_a_free_1.shp";
//
//     public RasterBasedFeatureController(ILogger<VectorBasedFeatureController> logger)
//     {
//         _logger = logger;
//     }
//
//     [HttpGet]
//     [Produces("application/json")]
//     public async Task<HttpResponseMessage> Get()
//     {
//         Console.WriteLine("");
//
//         /* -------------------------------------------------------------------- */
//         /*      Register format(s).                                             */
//         /* -------------------------------------------------------------------- */
//         Ogr.RegisterAll();
//
//
//         /* -------------------------------------------------------------------- */
//         /*      Open data source.                                               */
//         /* -------------------------------------------------------------------- */
//         DataSource ds = Ogr.Open(dataSet, 0);
//
//         HttpResponseMessage? httpResponseMessage;
//         if (ds == null)
//         {
//             httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
//             httpResponseMessage.Content = new StringContent("Can't open " + dataSet);
//             return httpResponseMessage;
//         }
//
//         /* -------------------------------------------------------------------- */
//         /*      Get driver                                                      */
//         /* -------------------------------------------------------------------- */
//         var drv = ds.GetDriver();
//
//         if (drv == null)
//         {
//             httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
//             httpResponseMessage.Content = new StringContent("Can't get driver.");
//             return httpResponseMessage;
//         }
//
//         // TODO: drv.name is still unsafe with lazy initialization (Bug 1339)
//         Console.WriteLine("Using driver " + drv.name);
//
//         /* -------------------------------------------------------------------- */
//         /*      Iterating through the layers                                    */
//         /* -------------------------------------------------------------------- */
//
//         for (int iLayer = 0; iLayer <ds.GetLayerCount(); iLayer++)
//         {
//             Layer layer = ds.GetLayerByIndex(iLayer);
//         
//             if (layer == null)
//             {
//                 Console.WriteLine("FAILURE: Couldn't fetch advertised layer " + iLayer);
//                 Environment.Exit(-1);
//             }
//         
//             Console.WriteLine(layer.GetName() + ": "+ layer.GetFeatureCount(0));
//             // ReportLayer(layer);
//         }
//
//         httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
//         httpResponseMessage.Content = new StringContent($"{ds.name}(count: {ds.GetLayerCount()})");
//         return httpResponseMessage;
//     }
// }