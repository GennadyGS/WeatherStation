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
        let dbInsertOptions = 
            { Timeout = Some Settings.DbInsertTimeout
              BatchSize = Some Settings.DbInsertBatchSize }

        Job.executeAsync 
            logger 
            Settings.ConnectionStrings.WeatherStation
            dbInsertOptions
            DateTime.UtcNow
            (TimeSpan.FromDays(Settings.MaxTimeIntervalDays |> float))
        |> Async.RunSynchronously
        |> function
           | true -> 0
           | false -> 1
    with
    | _ as e -> 
        logger.Error(e, "Unhanded exception")
        1