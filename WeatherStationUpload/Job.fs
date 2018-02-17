module WeatherStationUpload.Job

open System
open Serilog

let executeAsync 
        (logger: ILogger)
        (connectionString: string) 
        (dbInsertOptions: DbInsertOptions)
        (intervalEndTimeUtc: DateTime)
        (maxTimeInterval: TimeSpan): Async<bool> = 
    let uploadDataForDeviceAsync (stationInfo, lastMeasurementTime) = 
        let getIntervalStartTime = function
            | Some (time: DateTime) -> 
                let timeUtc = TimeUtils.timeToUtc stationInfo.TimeZone time
                timeUtc + TimeSpan.FromSeconds(1.0)
            | None -> intervalEndTimeUtc.Add(-maxTimeInterval)
        async {
            try
                return! DataUploader.uploadDataAsync
                    logger
                    connectionString
                    dbInsertOptions
                    { From = (getIntervalStartTime lastMeasurementTime)
                      To = intervalEndTimeUtc }
                    stationInfo
            with 
            | _ as e -> logger.Error(e, "Error uploading data for device {device}", stationInfo.DeviceInfo.DeviceId)
        }

    async {
        logger.Information("Start job; intervalEndTimeUtc: {intervalEndTime: s}; maxTimeInterval: {maxTimeInterval}", intervalEndTimeUtc.ToString("s"), maxTimeInterval)
        let! lastMeasurements = DbService.getStationsLastMeasurementsAsync logger connectionString
        for lastMeasurementInfo in lastMeasurements do
            do! uploadDataForDeviceAsync lastMeasurementInfo
        logger.Information("Job complete")
        return true
    }
