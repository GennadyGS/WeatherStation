module WeatherStationUpload.HtmlLoader

open FSharp.Data

let loadHtml stationId = 
    stationId
    |> sprintf "http://measurements.mobile-alerts.eu/Home/MeasurementDetails?deviceid=%s&vendorid=270f2261-3477-4872-9580-ead9cab3044c&appbundle=eu.mobile_alerts.mobilealerts"
    |> HtmlDocument.Load
