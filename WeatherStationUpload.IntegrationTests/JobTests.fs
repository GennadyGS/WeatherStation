namespace WeatherStationUpload.IntegrationTests

open Xunit
open WeatherStationUpload
open System
open FsUnit.Xunit

type JobTests() =
    inherit DbTests()

    let testConnectionString = Settings.ConnectionStrings.WeatherStation
    let testTime = DateTime.Now
    let dbInsertOptions: TimeSpan option * int option = None, None

    [<Fact>]
    member this.``Execute should load data incrementally`` () =
        async {
            let maxTimeInterval = TimeSpan.FromDays(3.0)
    
            do! Job.executeAsync this.Logger testConnectionString dbInsertOptions (testTime.AddDays(-1.0)) maxTimeInterval

            let! result1 = DbService.getMeasurementsAsync this.Logger testConnectionString
            Assert.True (result1.Length > 250)

            do! Job.executeAsync this.Logger testConnectionString dbInsertOptions testTime maxTimeInterval

            let! result2 = DbService.getMeasurementsAsync this.Logger testConnectionString
            result2.Length |> should be (greaterThan result1.Length)
        }
        |> Async.RunSynchronously
