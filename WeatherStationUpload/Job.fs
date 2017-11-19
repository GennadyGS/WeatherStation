module WeatherStationUpload.Job

open System

let execute 
        (connectionString: string) 
        (intervalEndTime: DateTime)
        (maxTimeInterval: TimeSpan) : unit = 
    let getIntervalStartTime = function
        | Some time -> time
        | None -> intervalEndTime.Add(-maxTimeInterval)
    
    DatabaseUtils.readDataContext 
        DbService.getStationsInfo connectionString
    |> List.map 
        (fun (stationId, deviceInfo, lastMeasurementTime) -> 
            DataUploader.uploadData 
                connectionString 
                { From = (getIntervalStartTime lastMeasurementTime)
                  To = intervalEndTime }
                deviceInfo
                stationId)
    |> ignore
