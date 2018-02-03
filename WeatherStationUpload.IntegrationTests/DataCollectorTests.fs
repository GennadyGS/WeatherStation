namespace WeatherStationUpload.IntegrationTests

open Xunit
open WeatherStationUpload
open Utils
open FsUnit.Xunit

type DataCollectorTests() = 
    inherit BaseTests()

    [<Fact>]
    member this.``CollectData should return result with appropriate size for the last days`` () = 
        let timeInterval =
            { From = System.DateTime.UtcNow.AddDays(-3.0)
              To = System.DateTime.UtcNow }
        
        let result = 
            DataCollector.collectDataAsync this.Logger timeInterval (getTestStationInfo())
            |> Async.RunSynchronously
        
        result.Length |> should be (greaterThan 0)
