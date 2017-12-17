namespace WeatherStationUpload.IntegrationTests

open Xunit
open WeatherStationUpload
open System
open Utils
open FsUnit.Xunit

type DataCollectorTests() = 
    inherit BaseTests()

    [<Fact>]
    member this.``CollectData should return result with appropriate size for the last days`` () = 
        let timeInterval =
            { From = System.DateTime.Now.AddDays(-3.0)
              To = System.DateTime.Now }
        
        let result = 
            DataCollector.collectDataAsync this.Logger timeInterval (getTestDeviceInfo())
            |> Async.RunSynchronously
        
        result.Length |> should be (greaterThan 250)
