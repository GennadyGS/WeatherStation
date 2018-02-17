using System;
using System.Configuration;

internal static class ConfigurationReader
{
    public static string DbConnectionString => ConfigurationManager.AppSettings[nameof(DbConnectionString)];

    public static TimeSpan MaxTimeInterval => 
        TimeSpan.FromDays(int.Parse(ConfigurationManager.AppSettings[nameof(MaxTimeInterval) + "Days"]));

    public static TimeSpan DbInsertTimeout => TimeSpan.Parse(ConfigurationManager.AppSettings[nameof(DbInsertTimeout)]);

    public static int DbInsertBatchSize => int.Parse(ConfigurationManager.AppSettings[nameof(DbInsertBatchSize)]);
}
