module WeatherStationUpload.DataUploader

open System
open DataCollector

let uploadData
        (connectionString : string)
        (fromDate: DateTime) 
        (toDate: DateTime) 
        (deviceInfo: DeviceInfo) =
    collectData fromDate toDate deviceInfo
    |> List.map (fun measurement -> deviceInfo, measurement)
    |> DatabaseUtils.writeDataContextForList 
        DbService.insertMeasurement connectionString
    