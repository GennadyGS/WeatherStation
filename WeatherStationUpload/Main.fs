module Main

open FSharp.Data
open WeatherStationUpload.HtmlParser

[<EntryPoint>]
let main argv = 
    let url = "http://measurements.mobile-alerts.eu/Home/MeasurementDetails?deviceid=07523951F222&vendorid=270f2261-3477-4872-9580-ead9cab3044c&appbundle=eu.mobile_alerts.mobilealerts"

    HtmlDocument.Load(url) 
    |> parseHtmlDocument 
    |> Seq.toList
    |> printf "%A"

    0

