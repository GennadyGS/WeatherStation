module WeatherStationUpload.DataUploader

open DataCollector
open Serilog

let uploadDataAsync
        (logger: ILogger)
        (connectionString : string)
        (dbInsertOptions: DbInsertOptions)
        (timeIntervalUtc : TimeInterval)
        (stationInfo: StationInfo)
        : Async<unit> =
    logger.Information("Upload data for device {device} from UTC time {from} to {to}", 
        stationInfo.DeviceInfo.DeviceId, timeIntervalUtc.From.ToString("s"), timeIntervalUtc.To.ToString("s"))
    collectDataAsync logger timeIntervalUtc stationInfo
    |> AsyncUtils.map (List.map (fun measurement -> stationInfo.StationId, measurement))
    |> AsyncUtils.bind 
        (function 
        | [] -> logger.Information("No new data to insert for device {device}", stationInfo.DeviceInfo.DeviceId) |> async.Return
        | measurements -> DbService.insertMeasurementsAsync logger connectionString dbInsertOptions measurements)
    |> AsyncUtils.combineWithAndInore (fun _ -> logger.Information("Upload data for device {device} complete", stationInfo.DeviceInfo.DeviceId))

    