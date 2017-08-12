module WeatherStationUpload.HtmlLoader

open FSharp.Data
open System.Net

[<Literal>]
let private url = "http://measurements.mobile-alerts.eu/Home/MeasurementDetails"

let private checkStatusCode statusCode = 
    if statusCode <> HttpStatusCode.OK then
        failwith (sprintf "Status code %A received" statusCode)

let loadHtmlDocument stationId = 
    let ({ StatusCode = statusCode; ResponseStream = responceStream }) = 
        Http.RequestStream(url, body = FormValues
            [("deviceId", stationId)
             ("vendorid", "270f2261-3477-4872-9580-ead9cab3044c")
             ("from", "08/10/2017 3:49 PM")
             ("to", "08/11/2017 3:49 PM")
             ("command", "refresh")
             ("pagesize", "250")
             ("appbundle", "eu.mobile_alerts.mobilealerts")
             ("fromepoch", "1502369340")
             ("toepoch", "1502455740")])
    checkStatusCode (enum statusCode)
    HtmlDocument.Load responceStream
