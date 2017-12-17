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

let collectDataAsync 
        (logger: Logger)
        (timeInterval : TimeInterval) 
        (deviceInfo: DeviceInfo): Async<Measurement list> =
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
