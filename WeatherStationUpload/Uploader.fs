module WeatherStationUpload.Uploader

open FSharp.Data

type MobileAlertsMeasurements = HtmlProvider<"http://measurements.mobile-alerts.eu/Home/MeasurementDetails?deviceid=0301548CBC4A&vendorid=270f2261-3477-4872-9580-ead9cab3044c&appbundle=eu.mobile_alerts.mobilealerts">

let url = "http://measurements.mobile-alerts.eu/Home/MeasurementDetails"