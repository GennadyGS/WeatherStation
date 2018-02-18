using System;
using System.Configuration;

internal static class ConfigurationReader
{
    public static string DbConnectionString => ConfigurationManager.ConnectionStrings["WeatherStationDb"].ConnectionString;

    public static TimeSpan MaxTimeInterval => 
        TimeSpan.FromDays(int.Parse(ConfigurationManager.AppSettings[nameof(MaxTimeInterval) + "Days"]));

    public static TimeSpan DbInsertTimeout => TimeSpan.Parse(ConfigurationManager.AppSettings[nameof(DbInsertTimeout)]);

    public static int DbInsertBatchSize => int.Parse(ConfigurationManager.AppSettings[nameof(DbInsertBatchSize)]);

    public static TimeSpan MaxInactiveTime => TimeSpan.Parse(ConfigurationManager.AppSettings[nameof(MaxInactiveTime)]);
}
