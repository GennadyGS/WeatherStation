using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Serilog;
using Serilog.Sinks.AzureWebJobsTraceWriter;

namespace WeatherStationUpload.FunctionApp
{
    public static class WeatherStationUpload
    {
        [FunctionName("WeatherStationUpload")]
        public static async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, TraceWriter traceWriter)
        {
            ILogger logger = new LoggerConfiguration()
                .WriteTo.TraceWriter(traceWriter)
                .CreateLogger();
            logger.Information($"WeatherStationUpload function started at: {DateTime.Now}");

            string connectionString = ConfigurationManager.AppSettings["DbConnectionString"];
            var maxTimeInterval = TimeSpan.FromDays(100);

            var result = await JobAdapter.executeAsync(logger, connectionString, null, null, DateTime.Now, maxTimeInterval);
            logger.Information($"WeatherStationUpload function finished with result {result} at: {DateTime.Now}");
        }
    }
}
