module WeatherStationUpload.DataCollector

open HtmlLoader
open HtmlParser
open System

[<Literal>]
let private maxPageSize = 250;

let private collectDataPage fromDate toDate deviceInfo pageSize page = 
    loadHtmlDocument fromDate toDate deviceInfo pageSize page
    |> parseHtmlDocument 

let collectData (fromDate: DateTime) (toDate: DateTime) (deviceInfo: DeviceInfo): Measurement list =
    let rec collectDataFromPage fromDate toDate deviceInfo pageSize startPage = 
        seq {
            let measurements = collectDataPage fromDate toDate deviceInfo pageSize startPage
            yield measurements
            if measurements.Length >= pageSize then
                yield! collectDataFromPage fromDate toDate deviceInfo pageSize (startPage + 1)
        }
    collectDataFromPage fromDate toDate deviceInfo maxPageSize 1 |> List.concat
