module WeatherStationUpload.Uploader

open FSharp.Data
open FSharp.Text.RegexProvider
open System
open System.Globalization

type 
    Mesasurement = 
        { TemperatureInside: decimal
          TemperatureOutside: decimal
          HimidityInside: decimal
          HimidityOutside: decimal }
type 
    MobileAlertsMeasurements = HtmlProvider<"http://measurements.mobile-alerts.eu/Home/MeasurementDetails?deviceid=07523951F222&vendorid=270f2261-3477-4872-9580-ead9cab3044c&appbundle=eu.mobile_alerts.mobilealerts">

let url = "http://measurements.mobile-alerts.eu/Home/MeasurementDetails?deviceid=07523951F222&vendorid=270f2261-3477-4872-9580-ead9cab3044c&appbundle=eu.mobile_alerts.mobilealerts"

type TemperatureRegex = Regex< @"(?<Temperature>\d+.\d+)\s*C" >

let measurements = 
        MobileAlertsMeasurements.Load(url).Tables.``Obolon Measurements``.Rows
        |> Seq.map (fun row -> 
            { TemperatureInside = Decimal.Parse(TemperatureRegex().TypedMatch(row.``Temperature Inside``).Temperature.Value, CultureInfo.InvariantCulture)
              TemperatureOutside = Decimal.Parse(TemperatureRegex().TypedMatch(row.``Temperature Outside``).Temperature.Value, CultureInfo.InvariantCulture)
              HimidityInside = row.``Humidity Inside``
              HimidityOutside =row.``Humidity Outside``})
