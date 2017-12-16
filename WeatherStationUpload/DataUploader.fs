module WeatherStationUpload.DataUploader

open DataCollector

let uploadDataAsync
        (connectionString : string)
        (timeInterval : TimeInterval)
        (deviceInfo: DeviceInfo)
        (stationId: StationId): Async<unit> =
    collectDataAsync timeInterval deviceInfo
    |> AsyncUtils.map (List.map (fun measurement -> stationId, measurement))
    |> AsyncUtils.bind (DbService.insertMeasurementsAsync connectionString)

    