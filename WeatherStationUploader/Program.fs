open WeatherStationUpload
open Serilog
open FSharp.Configuration
open System

type Settings = AppSettings<"app.config">

[<EntryPoint>]
let main argv = 
    let logger =
        LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
    try
        Job.executeAsync 
            logger 
            Settings.ConnectionStrings.WeatherStation
            DateTime.Now
            (TimeSpan.FromDays(Settings.MaxTimeIntervalDays |> float))
        |> Async.RunSynchronously
        0
    with
    | _ as e -> 
        logger.Error(e, "Unhanded exception")
        1