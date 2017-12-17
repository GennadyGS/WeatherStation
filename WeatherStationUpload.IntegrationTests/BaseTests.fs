namespace WeatherStationUpload.IntegrationTests

open Serilog

type BaseTests () =
    member this.Logger =
        LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
