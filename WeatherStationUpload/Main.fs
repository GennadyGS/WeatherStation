module Main

open FSharp.Data
open WeatherStationUpload.HtmlParser
open WeatherStationUpload.HtmlLoader

let stationId = "07523951F222"

[<EntryPoint>]
let main argv = 
    stationId
    |> loadHtmlDocument
    |> parseHtmlDocument 
    |> Seq.toList
    |> printf "%A"

    0

