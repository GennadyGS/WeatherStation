﻿namespace WeatherStationUpload.IntegrationTests

open Xunit
open WeatherStationUpload
open WeatherStationUpload.DataCollector
open System
open Utils

type DataCollectorTests() = 
    [<Fact>]
    member this.``CollectWeatherData should return result with appropriate size for the last days`` () = 
        let dateFrom = System.DateTime.Now.AddDays(-3.0)
        let dateTo = System.DateTime.Now
        let result = collectData dateFrom dateTo (getTestDeviceInfo())
        Assert.True (result.Length > 250)
