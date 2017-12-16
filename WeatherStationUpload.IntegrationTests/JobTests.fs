namespace WeatherStationUpload.IntegrationTests

open Xunit
open WeatherStationUpload
open System
open FsUnit.Xunit

type JobTests() =
    inherit DbTests()

    let testConnectionString = Settings.ConnectionStrings.WeatherStation
    let testTime = DateTime.Now

    [<Fact>]
    let ``Execute should load data incrementally`` () =
        async {
            let maxTimeInterval = TimeSpan.FromDays(3.0)
    
            do! Job.executeAsync testConnectionString (testTime.AddDays(-1.0)) maxTimeInterval

            let! result1 = DbService.getMeasurementsAsync testConnectionString
            Assert.True (result1.Length > 250)

            do! Job.executeAsync testConnectionString testTime maxTimeInterval

            let! result2 = DbService.getMeasurementsAsync testConnectionString
            result2.Length |> should be (greaterThan result1.Length)
        }
        |> Async.RunSynchronously
