namespace WeatherStationUpload.IntegrationTests

open System
open Utils
open Xunit

open WeatherStationUpload
open WeatherStationUpload.IntegrationTests

type DataUploaderTests() = 
    [<Fact>]
    member this.``UploadData should upload data correctly for the last days`` () = 
        let dateFrom = System.DateTime.Now.AddDays(-3.0)
        let dateTo = System.DateTime.Now
        
        DataUploader.uploadData
            Settings.ConnectionStrings.WeatherStation 
            dateFrom 
            dateTo 
            (getTestDeviceInfo())
            Settings.StationId
        |> ResultUtils.get

        let measurements =
            DatabaseUtils.readDataContext 
                DbService.getMeasurements 
                Settings.ConnectionStrings.WeatherStation
            |> ResultUtils.get

        Assert.True(measurements.Length > 250)