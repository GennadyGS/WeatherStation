module WeatherStationUpload.DataUploader

open DataCollector
open Serilog

let uploadDataAsync
        (logger: ILogger)
        (connectionString : string)
        (dbInsertOptions: DbInsertOptions)
        (timeZone: TimeZone)
        (timeIntervalUtc : TimeInterval)
        (deviceInfo: DeviceInfo)
        (stationId: StationId)
        : Async<unit> =
    logger.Information("Upload data for device {device} from UTC time {from} to {to}", deviceInfo.DeviceId, timeIntervalUtc.From, timeIntervalUtc.To)
    collectDataAsync logger timeZone timeIntervalUtc deviceInfo
    |> AsyncUtils.map (List.map (fun measurement -> stationId, measurement))
    |> AsyncUtils.bind 
        (function 
        | [] -> logger.Information("No new data to insert for device {device}", deviceInfo.DeviceId) |> async.Return
        | measurements -> DbService.insertMeasurementsAsync logger connectionString dbInsertOptions measurements)
    |> AsyncUtils.combineWithAndInore (fun _ -> logger.Information("Upload data for device {device} complete", deviceInfo.DeviceId))

    