module WeatherStationUpload.Composite

open HtmlLoader
open HtmlParser
open System


[<Literal>]
let private maxPageSize = 250;

let private loadWeatherDataPage fromDate toDate deviceInfo pageSize page = 
    loadHtmlDocument fromDate toDate deviceInfo pageSize page
    |> parseHtmlDocument 

let loadWeatherData (fromDate: DateTime) (toDate: DateTime) (deviceInfo: DeviceInfo): Measurement list =
    let rec loadWeatherDataFromPage fromDate toDate deviceInfo pageSize startPage = 
        seq {
            let measurements = loadWeatherDataPage fromDate toDate deviceInfo pageSize startPage
            yield measurements
            if measurements.Length >= pageSize then
                yield! loadWeatherDataFromPage fromDate toDate deviceInfo pageSize (startPage + 1)
        }
    loadWeatherDataFromPage fromDate toDate deviceInfo maxPageSize 1 |> List.concat

let processWeatherData
        (fromDate: DateTime) 
        (toDate: DateTime) 
        (deviceInfo: DeviceInfo): Measurement list =
    loadWeatherData fromDate toDate deviceInfo