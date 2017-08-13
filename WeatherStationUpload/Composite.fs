module WeatherStationUpload.Composite

open HtmlLoader
open HtmlParser

let processWeatherData fromDate toDate stationId =
    stationId
    |> loadHtmlDocument fromDate toDate
    |> parseHtmlDocument 
