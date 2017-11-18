namespace WeatherStationUpload.IntegrationTests

open Xunit
open WeatherStationUpload
open System
open Utils

type DataCollectorTests() = 
    [<Fact>]
    member this.``CollectData should return result with appropriate size for the last days`` () = 
        let timeInterval =
            { From = System.DateTime.Now.AddDays(-3.0)
              To = System.DateTime.Now }
        
        let result = DataCollector.collectData timeInterval (getTestDeviceInfo())
        
        Assert.True (result.Length > 250)
