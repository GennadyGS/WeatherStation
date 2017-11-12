module WeatherStationUpload.Uploader

open System
open DataCollector

let uploadWeatherData
        (fromDate: DateTime) 
        (toDate: DateTime) 
        (deviceInfo: DeviceInfo): MeasurementData list =
    collectData fromDate toDate deviceInfo