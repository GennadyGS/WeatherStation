namespace WeatherStationUpload.IntegrationTests

open System
open Utils
open Xunit

open WeatherStationUpload
open WeatherStationUpload.IntegrationTests
open FsUnit.Xunit

type DataUploaderTests() = 
    inherit DbTests()

    [<Fact>]
    member this.``UploadData should upload data correctly for the last days`` () = 
        let timeInterval = 
            { From = System.DateTime.Now.AddDays(-3.0)
              To = System.DateTime.Now }
        
        async {
            do! DataUploader.uploadDataAsync
                    this.Logger
                    Settings.ConnectionStrings.WeatherStation 
                    DbInsertOptions.Default
                    timeInterval
                    (getTestDeviceInfo())
                    (getTestStationId())

            let! measurements = DbServiceDapper.getMeasurementsAsync this.Logger Settings.ConnectionStrings.WeatherStation

            measurements.Length |> should be (greaterThan 250)
        }
        |> Async.RunSynchronously