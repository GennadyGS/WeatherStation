using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Serilog;
using Serilog.Sinks.AzureWebJobsTraceWriter;

namespace WeatherStationUpload.FunctionApp
{
    public static class UploadFunction
    {
        [FunctionName(nameof(UploadFunction))]
        public static async Task Run(
            [TimerTrigger("%UploadSchedule%", RunOnStartup = true)]
            TimerInfo myTimer, 
            TraceWriter traceWriter)
        {
            ILogger logger = new LoggerConfiguration()
                .WriteTo.TraceWriter(traceWriter)
                .CreateLogger();
            logger.Information($"{nameof(UploadFunction)} function started");

            await JobAdapter.executeAsync(logger,
                connectionString: ConfigurationReader.DbConnectionString,
                dbInsertTimeout: ConfigurationReader.DbInsertTimeout,
                dbInsertBatchSize: ConfigurationReader.DbInsertBatchSize,
                intervalEndTimeUtc: DateTime.UtcNow,
                maxTimeInterval: ConfigurationReader.MaxTimeInterval);

            logger.Information($"{nameof(UploadFunction)} function finished");
        }
    }
}
