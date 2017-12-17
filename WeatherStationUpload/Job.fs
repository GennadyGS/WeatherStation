module WeatherStationUpload.Job

open System
open Serilog.Core

let executeAsync 
        (logger: Logger)
        (connectionString: string) 
        (intervalEndTime: DateTime)
        (maxTimeInterval: TimeSpan): Async<unit> = 
    let getIntervalStartTime = function
        | Some (time: DateTime) -> time + TimeSpan.FromSeconds(1.0)
        | None -> intervalEndTime.Add(-maxTimeInterval)
    DbService.getStationsLastMeasurementsAsync connectionString
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
                    |> AsyncUtils.retn )
        >> Async.Parallel 
        >> Async.Ignore)
