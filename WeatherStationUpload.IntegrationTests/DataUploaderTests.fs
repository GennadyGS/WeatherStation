namespace WeatherStationUpload.IntegrationTests

open System
open Utils
open Xunit

open WeatherStationUpload
open WeatherStationUpload.IntegrationTests
open WeatherStationUpload.ResultUtils

type DataUploaderTests() = 
    [<Fact>]
    member this.``UploadData should upload data correctly for the last days`` () = 
        let dateFrom = System.DateTime.Now.AddDays(-3.0)
        let dateTo = System.DateTime.Now
        let uploadResult = 
            DataUploader.uploadData
                Settings.ConnectionStrings.WeatherStation 
                dateFrom 
                dateTo 
                (getTestDeviceInfo())
        Assert.Equal(get uploadResult, ())
        
        let measurements = 
            DatabaseUtils.readDataContext 
                DbService.getMeasurements 
                Settings.ConnectionStrings.WeatherStation
        Assert.True((get measurements).Length > 250)