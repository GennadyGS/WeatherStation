module WeatherStationUpload.HtmlParser

open System
open FSharp.Data
open FSharp.Text.RegexProvider
open System.Globalization
open WeatherStationUpload.TryParser
open FSharp.Data.HtmlAttribute
open MeasureUtils

type private TemperatureRegex = Regex< @"(?<Temperature>\d+.\d+)\s*C" >

type private HumidityRegex = Regex< @"(?<Humidity>\d+.\d+)%" >

let private parseTemperature str = 
    parseDecimalInvariant 
        NumberStyles.AllowDecimalPoint 
        (TemperatureRegex().TypedMatch(str).Temperature.Value)
    |> Option.map valueToCelsius

let private parseHumidity str = 
    parseDecimalInvariant 
        NumberStyles.AllowDecimalPoint 
        (HumidityRegex().TypedMatch(str).Humidity.Value)
    |> Option.map valueToPercent

let parseHtmlDocument (htmlDocument : HtmlDocument): Measurement list = 
    let table = htmlDocument.Descendants("table") |> Seq.head
    let tBody = (table.Descendants("tbody") |> Seq.exactlyOne)
    tBody.Descendants("tr")
    |> Seq.map (fun row -> 
            match row.Descendants("td") |> Seq.toList with
            | [tdTimeStamp; tdTemparatureInside; tdHumidityInside; tdTemperatureOutside; tdHumidityOutside] ->
                { Timestamp = DateTime.Parse(tdTimeStamp.InnerText(), CultureInfo.InvariantCulture)
                  TemperatureInside = tdTemparatureInside.InnerText() |> parseTemperature
                  HumidityInside = tdHumidityInside.InnerText() |> parseHumidity
                  TemperatureOutside = tdTemperatureOutside.InnerText() |> parseTemperature
                  HumidityOutside = tdHumidityOutside.InnerText() |> parseHumidity }
            | _ -> failwith "Parsing error")
    |> Seq.toList
