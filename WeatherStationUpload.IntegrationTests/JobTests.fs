namespace WeatherStationUpload.IntegrationTests

open Xunit
open WeatherStationUpload
open System

type JobTests() =
    inherit DbTests()

    let testConnectionString = Settings.ConnectionStrings.WeatherStation
    let testTime = DateTime.Now

    [<Fact>]
    let ``Execute should load data incrementally`` () =
        let maxTimeInterval = TimeSpan.FromDays(3.0)
    
        Job.execute testConnectionString (testTime.AddDays(-1.0)) maxTimeInterval

        let result1 = DbService.getMeasurements testConnectionString
        Assert.True (result1.Length > 250)

        Job.execute testConnectionString testTime maxTimeInterval

        let result2 = DbService.getMeasurements testConnectionString
        Assert.True (result2.Length > result1.Length)
