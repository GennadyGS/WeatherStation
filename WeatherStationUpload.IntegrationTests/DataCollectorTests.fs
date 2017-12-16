namespace WeatherStationUpload.IntegrationTests

open Xunit
open WeatherStationUpload
open System
open Utils
open FsUnit.Xunit

type DataCollectorTests() = 
    [<Fact>]
    member this.``CollectData should return result with appropriate size for the last days`` () = 
        let timeInterval =
            { From = System.DateTime.Now.AddDays(-3.0)
              To = System.DateTime.Now }
        
        let result = 
            DataCollector.collectDataAsync timeInterval (getTestDeviceInfo())
            |> Async.RunSynchronously
        
        result.Length |> should be (greaterThan 250)
