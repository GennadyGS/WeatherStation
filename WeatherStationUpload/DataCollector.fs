﻿module WeatherStationUpload.DataCollector

open HtmlLoader
open HtmlParser
open System

[<Literal>]
let private maxPageSize = 250;

let private collectDataPage timeInterval deviceInfo pageSize page = 
    loadHtmlDocument timeInterval deviceInfo pageSize page
    |> parseHtmlDocument 

let collectData (timeInterval : TimeInterval) (deviceInfo: DeviceInfo): Measurement list =
    let rec collectDataFromPage timeInterval deviceInfo pageSize startPage = 
        seq {
            let measurements = collectDataPage timeInterval deviceInfo pageSize startPage
            yield measurements
            if measurements.Length >= pageSize then
                yield! collectDataFromPage timeInterval deviceInfo pageSize (startPage + 1)
        }
    collectDataFromPage timeInterval deviceInfo maxPageSize 1 |> List.concat
