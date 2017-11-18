module WeatherStationUpload.DataUploader

open System
open DataCollector

let uploadData
        (connectionString : string)
        (timeInterval : TimeInterval)
        (deviceInfo: DeviceInfo)
        (stationId: int) =
    collectData timeInterval deviceInfo
    |> List.map (fun measurement -> stationId, measurement)
    |> DatabaseUtils.writeDataContextForList 
        DbService.insertMeasurement connectionString
    