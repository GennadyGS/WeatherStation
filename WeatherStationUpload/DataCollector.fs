module WeatherStationUpload.DataCollector

open HtmlLoader
open HtmlParser
open FSharp.Control
open Serilog

[<Literal>]
let private maxPageSize = 250;

let private collectDataPageAsync timeIntervalUtc deviceInfo pageSize page = 
    loadHtmlDocumentAsync timeIntervalUtc deviceInfo pageSize page
    |> AsyncUtils.map parseHtmlDocument 

let collectDataAsync 
        (logger: ILogger)
        (timeIntervalUtc : TimeInterval)
        (stationInfo: StationInfo)
        : Async<Measurement list> =
    logger.Information("Collecting data for device {deviceId}", stationInfo.DeviceInfo.DeviceId)
    AsyncSeq.unfoldAsync
        (fun page ->
            async {
                let! measurements = collectDataPageAsync timeIntervalUtc stationInfo.DeviceInfo maxPageSize page
                if measurements.Length > 0 then
                    return Some (measurements, page + 1)
                else return None
            })
        1
    |> AsyncSeq.concatSeq
    |> AsyncSeq.toListAsync
    |> AsyncUtils.map (List.filter (fun measurement -> 
        let timeStampUtc = TimeUtils.timeToUtc stationInfo.TimeZone measurement.Timestamp 
        TimeUtils.timeInsideInterval timeIntervalUtc timeStampUtc ))
    |> AsyncUtils.map (List.distinctBy (fun measurement -> measurement.Timestamp))
    |> AsyncUtils.combineWithAndInore (fun results -> 
        logger.Information("Collect data for device {deviceId} complete with {measurementCount} measurements", stationInfo.DeviceInfo.DeviceId, results.Length))
