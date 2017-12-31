module WeatherStationUpload.Job

open System
open Serilog.Core

let executeAsync 
        (logger: Logger)
        (connectionString: string) 
        (intervalEndTime: DateTime)
        (maxTimeInterval: TimeSpan): Async<unit> = 
    let uploadDataForDeviceAsync (stationId, deviceInfo, lastMeasurementTime) = 
        let getIntervalStartTime = function
            | Some (time: DateTime) -> time + TimeSpan.FromSeconds(1.0)
            | None -> intervalEndTime.Add(-maxTimeInterval)
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
            logger.Error(e, "Error uploading data for device {device}", deviceInfo.DeviceId) |> async.Return

    logger.Information("Start job; intervalEndTime: {intervalEndTime}; maxTimeInterval: {maxTimeInterval}", intervalEndTime, maxTimeInterval)
    DbService.getStationsLastMeasurementsAsync logger connectionString
    |> AsyncUtils.bind(
        List.map uploadDataForDeviceAsync
        >> AsyncUtils.runSequentially)
    |> AsyncUtils.combineWithAndInore (fun _ -> logger.Information("Job complete"))
