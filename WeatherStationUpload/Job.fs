module WeatherStationUpload.Job

open System
open Serilog.Core

let executeAsync 
        (logger: Logger)
        (connectionString: string) 
        (intervalEndTime: DateTime)
        (maxTimeInterval: TimeSpan): Async<unit> = 
    logger.Information("Start job; intervalEndTime: {intervalEndTime}; maxTimeInterval: {maxTimeInterval}", intervalEndTime, maxTimeInterval)
    let getIntervalStartTime = function
        | Some (time: DateTime) -> time + TimeSpan.FromSeconds(1.0)
        | None -> intervalEndTime.Add(-maxTimeInterval)
    DbService.getStationsLastMeasurementsAsync logger connectionString
    |> AsyncUtils.bind(
        List.map 
            (fun (stationId, deviceInfo, lastMeasurementTime) -> 
                try
                    DataUploader.uploadDataAsync
                        logger
                        connectionString
                        { From = (getIntervalStartTime lastMeasurementTime)
                          To = intervalEndTime }
                        deviceInfo
                        stationId
                with
                | _ as e -> 
                    logger.Error(e, sprintf "Error uploading data from station %A" stationId) 
                    |> async.Return )
        >> AsyncUtils.runSequentially)
    |> AsyncUtils.combineWithAndInore (fun _ -> logger.Information("Job complete"))
