namespace WeatherStationUpload.IntegrationTests

open System
open Utils
open Xunit

open WeatherStationUpload
open WeatherStationUpload.IntegrationTests

type DataUploaderTests() = 
    [<Fact>]
    member this.``UploadData should upload data correctly for the last days`` () = 
        let timeInterval = 
            { From = System.DateTime.Now.AddDays(-3.0)
              To = System.DateTime.Now }
        
        DataUploader.uploadData
            Settings.ConnectionStrings.WeatherStation 
            timeInterval
            (getTestDeviceInfo())
            (getTestStationId())
        |> ResultUtils.get

        let measurements =
            DatabaseUtils.readDataContext 
                DbService.getMeasurements 
                Settings.ConnectionStrings.WeatherStation
            |> ResultUtils.get

        Assert.True(measurements.Length > 250)