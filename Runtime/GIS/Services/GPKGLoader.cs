// using System;
// using System.Collections.Generic;
// using System.Reflection;
// using System.Threading;
// using System.Threading.Tasks;
// using log4net;
// using OSGeo.OGR;
//
// namespace BackgroundTask.Demo;
//
// public class GPKGLoader : IDisposable
// {
//     private readonly ILog logger;
//     private readonly ICityContext _cityContext;
//     public string dataSet = "../Data/Leipzig.gpkg";
//
//     public Dictionary<string, string> Config = new() //TODO: make config
//     {
//         { CityContext.STREET_KEY, "ver01_l" }
//     };
//
//     public GPKGLoader(ILog logger, ICityContext cityContext)
//     {
//         this.logger = logger;
//         _cityContext = cityContext;
//     }
//
//     public void Dispose()
//     {
//     }
//
//     public async Task StartAsync(CancellationToken cancellationToken)
//     {
//         logger.Info(MethodBase.GetCurrentMethod() + "");
//         Ogr.RegisterAll();
//         _cityContext.DataSet = Ogr.Open(dataSet, 0);
//         _cityContext.Config = Config;
//         Driver? drv = _cityContext.DataSet.GetDriver();
//
//         if (drv == null)
//         {
//             throw new Exception("Can't get driver.");
//         }
//     }
//
//     public Task StopAsync(CancellationToken cancellationToken)
//     {
//         logger.Info(MethodBase.GetCurrentMethod() + "");
//         return Task.CompletedTask;
//     }
// }