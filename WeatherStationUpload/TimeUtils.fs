module WeatherStationUpload.TimeUtils

open System

let timeInsideInterval (interval: TimeInterval) (time: DateTime) : bool = 
    time >= interval.From && time <= interval.To

let timeToUtc (TimeZone timeZoneName) (time : DateTime) : DateTime =
    let timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName)
    TimeZoneInfo.ConvertTimeToUtc(time, timeZoneInfo)

