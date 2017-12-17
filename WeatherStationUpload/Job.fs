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
                DataUploader.uploadDataAsync
                    logger
                    connectionString
                    { From = (getIntervalStartTime lastMeasurementTime)
                      To = intervalEndTime }
                    deviceInfo
                    stationId)
        >> Async.Parallel 
        >> Async.Ignore)
