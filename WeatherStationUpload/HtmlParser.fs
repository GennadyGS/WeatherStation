module WeatherStationUpload.HtmlParser

open System
open FSharp.Data
open FSharp.Text.RegexProvider
open System.Globalization

type private TemperatureRegex = Regex< @"(?<Temperature>\d+.\d+)\s*C" >

type private HumidityRegex = Regex< @"(?<Humidity>\d+.\d+)%" >

let private parseTemperature str = 
    parseDecimal (TemperatureRegex().TypedMatch(str).Temperature.Value)

let private parseHumidity str = 
    Decimal.Parse(HumidityRegex().TypedMatch(str).Humidity.Value, CultureInfo.InvariantCulture) * 1m<``%``>

let parseHtmlDocument (htmlDocument : HtmlDocument): MeasurementData list = 
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


