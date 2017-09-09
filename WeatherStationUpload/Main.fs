module Main

open System
open WeatherStationUpload.Composite
open WeatherStationUpload

let deviceInfo : DeviceInfo = 
    { DeviceId = "07523951F222"
      VendorId = "270f2261-3477-4872-9580-ead9cab3044c" }

let dateFrom = DateTime(2017, 08, 12, 14, 0, 0)
let dateTo = DateTime(2017, 08, 12, 15, 0, 0)

[<EntryPoint>]
let main argv = 
    deviceInfo
    |> processWeatherData dateFrom dateTo
    |> printf "%A"
    0

