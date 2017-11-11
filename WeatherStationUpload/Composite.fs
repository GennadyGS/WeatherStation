module WeatherStationUpload.Uploader

open System
open DataCollector

let uploadWeatherData
        (fromDate: DateTime) 
        (toDate: DateTime) 
        (deviceInfo: DeviceInfo): Measurement list =
    collectData fromDate toDate deviceInfo