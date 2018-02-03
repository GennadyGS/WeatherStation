﻿module WeatherStationUpload.DataCollector

open HtmlLoader
open HtmlParser
open FSharp.Control
open Serilog

[<Literal>]
let private maxPageSize = 250;

let private collectDataPageAsync timeZone timeIntervalUtc deviceInfo pageSize page = 
    loadHtmlDocumentAsync timeIntervalUtc deviceInfo pageSize page
    |> AsyncUtils.map parseHtmlDocument 

let collectDataAsync 
        (logger: ILogger)
        (timeZone: TimeZone)
        (timeIntervalUtc : TimeInterval) 
        (deviceInfo: DeviceInfo): Async<Measurement list> =
    logger.Information("Collecting data for device {deviceId}", deviceInfo.DeviceId)
    AsyncSeq.unfoldAsync
        (fun page ->
            async {
                let! measurements = collectDataPageAsync timeZone timeIntervalUtc deviceInfo maxPageSize page
                if measurements.Length > 0 then
                    return Some (measurements, page + 1)
                else return None
            })
        1
    |> AsyncSeq.concatSeq
    |> AsyncSeq.toListAsync
    |> AsyncUtils.map (List.filter (fun measurement -> 
        let timeStampUtc = TimeUtils.timeToUtc timeZone measurement.Timestamp 
        TimeUtils.timeInsideInterval timeIntervalUtc timeStampUtc ))
    |> AsyncUtils.map (List.distinctBy (fun measurement -> measurement.Timestamp))
    |> AsyncUtils.combineWithAndInore (fun results -> logger.Information("Collect data for device {deviceId} complete with {measurementCount} measurements", deviceInfo.DeviceId, results.Length))
