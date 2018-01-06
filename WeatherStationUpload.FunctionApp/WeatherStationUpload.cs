using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Serilog;
using Serilog.Sinks.AzureWebJobsTraceWriter;

namespace WeatherStationUpload.FunctionApp
{
    public static class WeatherStationUpload
    {
        [FunctionName("WeatherStationUpload")]
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, TraceWriter traceWriter)
        {
            ILogger logger = new LoggerConfiguration()
                .WriteTo.TraceWriter(traceWriter)
                .CreateLogger();

            logger.Information($"WeatherStationUpload function started at: {DateTime.Now}");
        }
    }
}
