using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SendGrid.Helpers.Mail;
using Serilog;
using Serilog.Sinks.AzureWebJobsTraceWriter;

namespace WeatherStationUpload.FunctionApp
{
    public static class NotificationFunction
    {
        [FunctionName(nameof(NotificationFunction))]
        [return: SendGrid(ApiKey = "SendGridKey", From = "%MailFrom%", To = "%MailTo%")]
        public static Mail Run(
            [TimerTrigger("%NotificationSchedule%")] TimerInfo myTimer, 
            TraceWriter traceWriter)
        {
            ILogger logger = new LoggerConfiguration()
                .WriteTo.TraceWriter(traceWriter)
                .CreateLogger();
            logger.Information($"{nameof(NotificationFunction)} function started");

            var mail = new Mail
            {
                Subject = $"{nameof(WeatherStationUpload)} notification"
            };

            Content content = new Content
            {
                Type = "text/plain",
                Value = "Email Body"
            };
            mail.AddContent(content);

            logger.Information($"{nameof(NotificationFunction)} function finished");

            return null;
        }
    }
}
