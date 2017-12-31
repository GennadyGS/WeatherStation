module WeatherStationUpload.DataCollector

open HtmlLoader
open HtmlParser
open System
open FSharp.Control
open Serilog.Core

[<Literal>]
let private maxPageSize = 250;

let private collectDataPageAsync timeInterval deviceInfo pageSize page = 
    loadHtmlDocumentAsync timeInterval deviceInfo pageSize page
    |> AsyncUtils.map parseHtmlDocument 

let private timeInsideInterval interval time = 
    time >= interval.From && time <= interval.To

let collectDataAsync 
        (logger: Logger)
        (timeInterval : TimeInterval) 
        (deviceInfo: DeviceInfo): Async<Measurement list> =
    logger.Information("Collecting data for device {deviceId}", deviceInfo.DeviceId)
    AsyncSeq.unfoldAsync
        (fun page ->
            async {
                let! measurements = collectDataPageAsync timeInterval deviceInfo maxPageSize page
                if measurements.Length > 0 then
                    return Some (measurements, page + 1)
                else return None
            })
        1
    |> AsyncSeq.concatSeq
    |> AsyncSeq.toListAsync
    |> AsyncUtils.map (List.filter (fun measurement -> timeInsideInterval timeInterval measurement.Timestamp ))
    |> AsyncUtils.map (List.distinctBy (fun measurement -> measurement.Timestamp))
    |> AsyncUtils.combineWithAndInore (fun results -> logger.Information("Collect data for device {deviceId} complete with {measurementCount} measurements", deviceInfo.DeviceId, results.Length))
