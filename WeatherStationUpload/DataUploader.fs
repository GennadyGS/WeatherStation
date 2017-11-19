module WeatherStationUpload.DataUploader

open DataCollector

let uploadData
        (connectionString : string)
        (timeInterval : TimeInterval)
        (deviceInfo: DeviceInfo)
        (stationId: StationId) : unit =
    collectData timeInterval deviceInfo
    |> List.map (fun measurement -> stationId, measurement)
    |> DatabaseUtils.writeDataContextForList 
        DbService.insertMeasurement connectionString

    