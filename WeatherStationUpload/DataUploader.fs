module WeatherStationUpload.DataUploader

open DataCollector
open Serilog.Core
open System

let uploadDataAsync
        (logger: Logger)
        (connectionString : string)
        (dbInsertTimeout: TimeSpan option, dbInsertBatchSize: int option)
        (timeInterval : TimeInterval)
        (deviceInfo: DeviceInfo)
        (stationId: StationId)
        : Async<unit> =
    logger.Information("Upload data for device {device} from {from} to {to}", deviceInfo.DeviceId, timeInterval.From, timeInterval.To)
    collectDataAsync logger timeInterval deviceInfo
    |> AsyncUtils.map (List.map (fun measurement -> stationId, measurement))
    |> AsyncUtils.bind 
        (function 
        | [] -> logger.Information("No new data to insert for device {device}", deviceInfo.DeviceId) |> async.Return
        | measurements -> DbService.insertMeasurementsAsync logger connectionString (dbInsertTimeout, dbInsertBatchSize) measurements)
    |> AsyncUtils.combineWithAndInore (fun _ -> logger.Information("Upload data for device {device} complete", deviceInfo.DeviceId))

    