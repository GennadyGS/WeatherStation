﻿namespace WeatherStationUpload.IntegrationTests

open Xunit
open WeatherStationUpload
open Utils
open System
open WeatherStationUpload.DatabaseUtils

type JobTests() =
    inherit DbTests()

    let testConnectionString = Settings.ConnectionStrings.WeatherStation
    let testTime = DateTime.Now

    [<Fact>]
    let ``Execute should load data incrementally`` () =
        let maxTimeInterval = TimeSpan.FromDays(3.0)
    
        Job.execute testConnectionString (testTime.AddDays(-1.0)) maxTimeInterval

        let result1 = readDataContext DbService.getMeasurements testConnectionString
        Assert.True (result1.Length > 250)

        Job.execute testConnectionString testTime maxTimeInterval

        let result2 = readDataContext DbService.getMeasurements testConnectionString
        Assert.True (result2.Length > result1.Length)
