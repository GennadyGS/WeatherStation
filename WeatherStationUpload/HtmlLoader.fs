module WeatherStationUpload.HtmlLoader

open FSharp.Data
open System.Net
open System

[<Literal>]
let private url = "https://measurements.mobile-alerts.eu/Home/MeasurementDetails"

let private toEpoch dateTime = 
    DateTimeOffset(dateTime).ToUnixTimeSeconds()

let private checkStatusCode statusCode = 
    if statusCode <> HttpStatusCode.OK then
        failwith (sprintf "Status code %A received" statusCode)

let loadHtmlDocumentAsync
        (timeIntervalUtc : TimeInterval) 
        (deviceInfo : DeviceInfo)
        (pageSize: int)
        (page: int): Async<HtmlDocument> = 
    async {
        let! ({ StatusCode = statusCode; ResponseStream = responceStream }) = 
            Http.AsyncRequestStream(url, body = FormValues
                [("deviceId", deviceInfo.DeviceId)
                 ("vendorId", deviceInfo.VendorId.ToString())
                 ("command", "refresh")
                 ("pageSize", pageSize |> string)
                 ("page", page |> string)
                 ("appBundle", "eu.mobile_alerts.mobilealerts")
                 ("fromEpoch", timeIntervalUtc.From |> toEpoch |> string)
                 ("toEpoch", timeIntervalUtc.To |> toEpoch |> string)])
        checkStatusCode (enum statusCode)
        return HtmlDocument.Load responceStream
    }
