module WeatherStationUpload.Job

open System
open Serilog

let executeAsync 
        (logger: ILogger)
        (connectionString: string) 
        (dbInsertOptions: DbInsertOptions)
        (intervalEndTimeUtc: DateTime)
        (maxTimeInterval: TimeSpan): Async<bool> = 
    let uploadDataForDeviceAsync (stationId, deviceInfo, lastMeasurementTime, timeZone) = 
        let getIntervalStartTime = function
            | Some (time: DateTime) -> 
                let timeUtc = TimeUtils.timeToUtc timeZone time
                timeUtc + TimeSpan.FromSeconds(1.0)
            | None -> intervalEndTimeUtc.Add(-maxTimeInterval)
        async {
            try
                return! DataUploader.uploadDataAsync
                    logger
                    connectionString
                    dbInsertOptions
                    timeZone
                    { From = (getIntervalStartTime lastMeasurementTime)
                      To = intervalEndTimeUtc }
                    deviceInfo
                    stationId
            with 
            | _ as e -> logger.Error(e, "Error uploading data for device {device}", deviceInfo.DeviceId)
        }

    async {
        logger.Information("Start job; intervalEndTimeUtc: {intervalEndTime}; maxTimeInterval: {maxTimeInterval}", intervalEndTimeUtc, maxTimeInterval)
        let! lastMeasurements = DbService.getStationsLastMeasurementsAsync logger connectionString
        for lastMeasurementInfo in lastMeasurements do
            do! uploadDataForDeviceAsync lastMeasurementInfo
        logger.Information("Job complete")
        return true
    }
