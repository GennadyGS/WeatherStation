using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SendGrid.Helpers.Mail;
using Serilog;
using Serilog.Sinks.AzureWebJobsTraceWriter;
using System;
using System.Collections.Generic;
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
            logger.Information($"{nameof(NotificationFunction)} function started; MaxInactiveTime: {ConfigurationReader.MaxInactiveTime}; Schedule: {myTimer.Schedule}");

            var verificationTimeUtc = DateTime.UtcNow;

            var statuses = await GetStationStatusesAsync(logger, verificationTimeUtc);

            if (statuses.All(status => status.UpToDate))
            {
                logger.Information($"{nameof(NotificationFunction)} function finished without notification: all stations are up to date");
                return null;
            }

            Mail mail = CreateMail(statuses, verificationTimeUtc);
            logger.Information($"{nameof(NotificationFunction)} function finished with notification: some stations are not up to date");
            return mail;
        }

        private static async Task<IReadOnlyCollection<StationStatus>> GetStationStatusesAsync(ILogger logger, DateTime verificationTimeUtc)
        {
            var lastMeasurements = await JobAdapter.getStationsLastMeasurementsAsync(logger, ConfigurationReader.DbConnectionString);

            return lastMeasurements
                .Select<(StationInfo stationInfo, DateTime? lastMeasurementTime), StationStatus>(item =>
                {
                    var timePassed = item.lastMeasurementTime.HasValue
                        ?  verificationTimeUtc - TimeToUtc(item.lastMeasurementTime.Value, item.stationInfo.TimeZone.Item) : (TimeSpan?)null;
                    return new StationStatus()
                    {
                        StationInfo = item.stationInfo,
                        LastMeasurementTime = item.lastMeasurementTime,
                        InactiveDuration = timePassed,
                        UpToDate = timePassed.HasValue && timePassed <= ConfigurationReader.MaxInactiveTime
                    };
                }).ToList();
        }

        private static Mail CreateMail(IReadOnlyCollection<StationStatus> statuses, DateTime verificarionTimeUtc)
        {
            var mail = new Mail
            {
                Subject = $"{nameof(WeatherStationUpload)} notification"
            };

            Content content = new Content
            {
                Type = "text/plain",
                Value = string.Join(Environment.NewLine,
                    new[] { $"Verification time UTC: {verificarionTimeUtc}" }
                        .Concat(statuses.Select(status => status.ToString())))
            };
            mail.AddContent(content);

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
                var statusMessage = UpToDate 
                    ? $"OK (last measurement time: {LastMeasurementTime})"
                    : $"not up to date {inactiveMessage}";
                return $"Station {StationInfo.DeviceInfo.DeviceId} is {statusMessage}";
            }
        }
    }
}
