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
    member this.``Execute should load data incrementally`` () =
        async {
            let maxTimeInterval = TimeSpan.FromDays(3.0)
    
            let! result1 = Job.executeAsync this.Logger testConnectionString DbInsertOptions.Default (testTime.AddDays(-1.0)) maxTimeInterval
            result1 |> should equal true

            let! measurements1 = DbServiceDapper.getMeasurementsAsync this.Logger testConnectionString
            Assert.True (measurements1.Length > 250)

            let! result2 = Job.executeAsync this.Logger testConnectionString DbInsertOptions.Default testTime maxTimeInterval
            result2 |> should equal true

            let! measurements2 = DbServiceDapper.getMeasurementsAsync this.Logger testConnectionString
            measurements2.Length |> should be (greaterThan measurements1.Length)
        }
        |> Async.RunSynchronously
