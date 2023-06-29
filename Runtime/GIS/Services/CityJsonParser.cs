// using System.Text.Json;
// using CityJson;
// using CityJson.Parsing;
//
// namespace BackgroundTask.Demo; 
//
// public class CityJsonParser : IHostedService, IDisposable {
//     private readonly ILogger<CityJsonParser> logger;
//     private readonly ICityContext _cityContext;
//
//     public CityJsonParser(ILogger<CityJsonParser> logger, ICityContext cityContext) {
//         this.logger = logger;
//         _cityContext = cityContext;
//     }
//
//     public void Dispose() {
//     }
//
//     public async Task StartAsync(CancellationToken cancellationToken) {
//         _cityContext.City = await JsonSerializer.DeserializeAsync<City>(
//             File.OpenRead("Citygml/lod2_33324_5682_2_sn.json")
//             , new JsonSerializerOptions(JsonSerializerDefaults.Web) {
//                 Converters = {
//                     new JsonStringConverter(),
//                     new Vector3Converter()
//                 }
//             }
//             , cancellationToken) ?? new City();
//         logger.LogInformation("CityJsonParser StartAsync");
//     }
//
//     public Task StopAsync(CancellationToken cancellationToken) {
//         logger.LogInformation("CityJsonParser StopAsync");
//         return Task.CompletedTask;
//     }
// }