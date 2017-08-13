module Main

open WeatherStationUpload.Composite
open System

let stationId = "07523951F222"
let dateFrom = DateTime(2017, 08, 12, 14, 0, 0)
let dateTo = DateTime(2017, 08, 12, 15, 0, 0)

[<EntryPoint>]
let main argv = 
    stationId
    |> processWeatherData dateFrom dateTo
    |> printf "%A"
    0

