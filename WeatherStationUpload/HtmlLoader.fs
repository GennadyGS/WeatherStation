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

let loadHtmlDocument (fromDate : DateTime) (toDate : DateTime) (deviceInfo : DeviceInfo) = 
    let ({ StatusCode = statusCode; ResponseStream = responceStream }) = 
        Http.RequestStream(url, body = FormValues
            [("deviceId", deviceInfo.DeviceId)
             ("vendorid", deviceInfo.VendorId)
             ("command", "refresh")
             ("pagesize", "250")
             ("appbundle", "eu.mobile_alerts.mobilealerts")
             ("fromepoch", fromDate |> toEpoch |> string)
             ("toepoch", toDate |> toEpoch |> string)])
    checkStatusCode (enum statusCode)
    HtmlDocument.Load responceStream
