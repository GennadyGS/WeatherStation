namespace WeatherStationUpload

open System

type Mesasurement = 
    { Timestamp: DateTime
      TemperatureInside: decimal
      HimidityInside: decimal
      TemperatureOutside: decimal
      HimidityOutside: decimal }

module Parser = 
    open FSharp.Data
    open FSharp.Text.RegexProvider
    open System.Globalization

    type private TemperatureRegex = Regex< @"(?<Temperature>\d+.\d+)\s*C" >

    type private HumidityRegex = Regex< @"(?<Humidity>\d+.\d+)%" >

    let private parseTemperature str = 
        Decimal.Parse(TemperatureRegex().TypedMatch(str).Temperature.Value, CultureInfo.InvariantCulture)

    let private parseHumidity str = 
        Decimal.Parse(HumidityRegex().TypedMatch(str).Humidity.Value, CultureInfo.InvariantCulture)

    let parseHtmlDocument (htmlDocument : HtmlDocument) = 
        let table = htmlDocument.Descendants("table") |> Seq.head
        let tBody = (table.Descendants("tbody") |> Seq.exactlyOne)
        tBody.Descendants("tr")
        |> Seq.map (fun row -> 
                match row.Descendants("td") |> Seq.toList with
                | [tdTimeStamp; tdTemparatureInside; tdHumidityInside; tdTemperatureOutside; tdHumidityOutside] ->
                    { Timestamp = DateTime.Parse(tdTimeStamp.InnerText(), CultureInfo.InvariantCulture)
                      TemperatureInside = tdTemparatureInside.InnerText() |> parseTemperature
                      HimidityInside = tdHumidityInside.InnerText() |> parseHumidity
                      TemperatureOutside = tdTemperatureOutside.InnerText() |> parseTemperature
                      HimidityOutside = tdHumidityOutside.InnerText() |> parseHumidity }
                | _ -> failwith "Parsing error")


