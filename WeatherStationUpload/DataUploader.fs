module WeatherStationUpload.DataUploader

open DataCollector
open Serilog.Core

let uploadDataAsync
        (logger: Logger)
        (connectionString : string)
        (timeInterval : TimeInterval)
        (deviceInfo: DeviceInfo)
        (stationId: StationId): Async<unit> =
    logger.Information("Upload data for device {device} from {from} to {to}", deviceInfo.DeviceId, timeInterval.From, timeInterval.To)
    collectDataAsync logger timeInterval deviceInfo
    |> AsyncUtils.map (List.map (fun measurement -> stationId, measurement))
    |> AsyncUtils.combineWithAndInore (fun records -> logger.Information("Inserting {measurementCount} measurements in database", records.Length))
    |> AsyncUtils.bind (DbService.insertMeasurementsAsync logger connectionString)
    |> AsyncUtils.combineWithAndInore (fun _ -> logger.Information("Upload data for station {stationId} complete", stationId))

    