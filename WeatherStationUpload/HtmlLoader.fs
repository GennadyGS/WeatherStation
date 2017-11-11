module WeatherStationUpload.HtmlLoader

open FSharp.Data
open System.Net
open System
open System.Globalization

[<Literal>]
let private url = "http://measurements.mobile-alerts.eu/Home/MeasurementDetails"

let private toEpoch dateTime = 
    DateTimeOffset(dateTime).ToUnixTimeSeconds()

let private checkStatusCode statusCode = 
    if statusCode <> HttpStatusCode.OK then
        failwith (sprintf "Status code %A received" statusCode)

let loadHtmlDocument (fromDate : DateTime) (toDate : DateTime) (deviceInfo : DeviceInfo): HtmlDocument = 
    let ({ StatusCode = statusCode; ResponseStream = responceStream }) = 
        Http.RequestStream(url, body = FormValues
            [("deviceId", deviceInfo.DeviceId)
             ("vendorId", deviceInfo.VendorId)
             ("command", "refresh")
             ("pageSize", "250")
             ("appBundle", "eu.mobile_alerts.mobilealerts")
             ("fromEpoch", fromDate |> toEpoch |> string)
             ("toEpoch", toDate |> toEpoch |> string)])
    checkStatusCode (enum statusCode)
    HtmlDocument.Load responceStream
