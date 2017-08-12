namespace WeatherStationUpload.IntegrationTests

open Xunit
open WeatherStationUpload.Composite

type CompositeTests() = 
    [<Fact>]
    member this.``Test`` () = 
        loadWeatherData (System.DateTime.Now.AddHours(-1.0)) System.DateTime.Now 
        ()
