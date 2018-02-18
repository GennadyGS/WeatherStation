using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SendGrid.Helpers.Mail;
using Serilog;
using Serilog.Sinks.AzureWebJobsTraceWriter;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherStationUpload.FunctionApp
{
    public static class NotificationFunction
    {
        [FunctionName(nameof(NotificationFunction))]
        [return: SendGrid(ApiKey = "SendGridKey", From = "%MailFrom%", To = "%MailTo%")]
        public static async Task<Mail> Run(
            [TimerTrigger("%NotificationSchedule%", RunOnStartup = true)] TimerInfo myTimer,
            TraceWriter traceWriter)
        {
            ILogger logger = new LoggerConfiguration()
                .WriteTo.TraceWriter(traceWriter)
                .CreateLogger();
            logger.Information($"{nameof(NotificationFunction)} function started");

            var lastMeasurements = await JobAdapter.getStationsLastMeasurementsAsync(logger, ConfigurationReader.DbConnectionString);

            var now = DateTime.UtcNow;

            var statuses = lastMeasurements
                .Select<(StationInfo stationInfo, DateTime? lastMeasurementTime), StationStatus>(item => {
                    var timePassed = item.lastMeasurementTime.HasValue
                        ? now - TimeToUtc(item.lastMeasurementTime.Value, item.stationInfo.TimeZone.Item) : (TimeSpan?)null;
                    return new StationStatus()
                    {
                        StationInfo = item.stationInfo,
                        LastMeasurementTime = item.lastMeasurementTime,
                        InactiveDuration = timePassed,
                        UpToDate = timePassed.HasValue && timePassed <= ConfigurationReader.MaxInactiveTime
                    };
                });

            var mail = new Mail
            {
                Subject = $"{nameof(WeatherStationUpload)} notification"
            };

            Content content = new Content
            {
                Type = "text/plain",
                Value = string.Join(Environment.NewLine,
                    new[] { $"Verification time UTC: {now}" }
                        .Concat(statuses.Select(status => status.ToString())))
            };
            mail.AddContent(content);

            logger.Information($"{nameof(NotificationFunction)} function finished");

            return mail;
        }

        private static DateTime TimeToUtc(DateTime time, string timeZone)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            return TimeZoneInfo.ConvertTimeToUtc(time, timeZoneInfo);
        }

        private class StationStatus
        {
            public StationInfo StationInfo { get; set; }
            public DateTime? LastMeasurementTime { get; set; }
            public TimeSpan? InactiveDuration { get; set; }
            public bool UpToDate { get; set; }

            public override string ToString()
            {
                var inactiveMessage = InactiveDuration.HasValue ? $"for {InactiveDuration}" : "";
                var statusMessage = UpToDate ? "OK" : $"not up to date {inactiveMessage}";
                return $"Station {StationInfo.DeviceInfo.DeviceId} is {statusMessage}";
            }
        }
    }
}
