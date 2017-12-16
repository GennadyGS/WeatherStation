module WeatherStationUpload.Job

open System

let execute 
        (connectionString: string) 
        (intervalEndTime: DateTime)
        (maxTimeInterval: TimeSpan) 
        : unit = 
    let getIntervalStartTime = function
        | Some (time: DateTime) -> time + TimeSpan.FromSeconds(1.0)
        | None -> intervalEndTime.Add(-maxTimeInterval)
    
    DbService.getStationsLastMeasurements connectionString
    |> List.map 
        (fun (stationId, deviceInfo, lastMeasurementTime) -> 
            DataUploader.uploadData 
                connectionString 
                { From = (getIntervalStartTime lastMeasurementTime)
                  To = intervalEndTime }
                deviceInfo
                stationId)
    |> ignore

let executeAsync 
        (connectionString: string) 
        (intervalEndTime: DateTime)
        (maxTimeInterval: TimeSpan): Async<unit> = 
    failwith "Not implemented"
    //let getIntervalStartTime = function
    //    | Some (time: DateTime) -> time + TimeSpan.FromSeconds(1.0)
    //    | None -> intervalEndTime.Add(-maxTimeInterval)
    
    //DbService.getStationsLastMeasurementsAsync connectionString
    //|> (AsyncUtils.bind (List.map
    //    (fun (stationId, deviceInfo, lastMeasurementTime) -> 
    //        DataUploader.uploadDataAsync
    //            connectionString 
    //            { From = (getIntervalStartTime lastMeasurementTime)
    //              To = intervalEndTime }
    //            deviceInfo
    //            stationId)))
