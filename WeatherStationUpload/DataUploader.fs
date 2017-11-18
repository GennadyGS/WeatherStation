module WeatherStationUpload.DataUploader

open System
open DataCollector

let uploadData
        (connectionString : string)
        (fromDate: DateTime) 
        (toDate: DateTime) 
        (deviceInfo: DeviceInfo)
        (stationId: int) =
    collectData fromDate toDate deviceInfo
    |> List.map (fun measurement -> stationId, measurement)
    |> DatabaseUtils.writeDataContextForList 
        DbService.insertMeasurement connectionString
    