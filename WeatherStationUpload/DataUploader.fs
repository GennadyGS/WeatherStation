module WeatherStationUpload.DataUploader

open DataCollector
open Serilog.Core

let uploadDataAsync
        (logger: Logger)
        (connectionString : string)
        (timeInterval : TimeInterval)
        (deviceInfo: DeviceInfo)
        (stationId: StationId): Async<unit> =
    collectDataAsync logger timeInterval deviceInfo
    |> AsyncUtils.map (List.map (fun measurement -> stationId, measurement))
    |> AsyncUtils.bind (DbService.insertMeasurementsAsync connectionString)

    