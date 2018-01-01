module WeatherStationUpload.Job

open System
open Serilog.Core

let executeAsync 
        (logger: Logger)
        (connectionString: string) 
        (insertTimeout: TimeSpan option, insertBatchSize: int option)
        (intervalEndTime: DateTime)
        (maxTimeInterval: TimeSpan): Async<unit> = 
    let uploadDataForDeviceAsync (stationId, deviceInfo, lastMeasurementTime) = 
        let getIntervalStartTime = function
            | Some (time: DateTime) -> time + TimeSpan.FromSeconds(1.0)
            | None -> intervalEndTime.Add(-maxTimeInterval)
        async {
            try
                return! DataUploader.uploadDataAsync
                    logger
                    connectionString
                    (insertTimeout, insertBatchSize)
                    { From = (getIntervalStartTime lastMeasurementTime)
                      To = intervalEndTime }
                    deviceInfo
                    stationId
            with 
            | _ as e -> logger.Error(e, "Error uploading data for device {device}", deviceInfo.DeviceId)
        }

    async {
        logger.Information("Start job; intervalEndTime: {intervalEndTime}; maxTimeInterval: {maxTimeInterval}", intervalEndTime, maxTimeInterval)
        try 
            let! lastMeasurements = DbService.getStationsLastMeasurementsAsync logger connectionString
            for (stationId, deviceInfo, lastMeasurementTime) in lastMeasurements do
                do! uploadDataForDeviceAsync (stationId, deviceInfo, lastMeasurementTime)
            logger.Information("Job complete")
        with 
        | _ as e -> logger.Error(e, "Error running job")
    }
