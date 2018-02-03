namespace WeatherStationUpload.IntegrationTests

open Utils
open Xunit

open WeatherStationUpload
open WeatherStationUpload.IntegrationTests
open FsUnit.Xunit

type DataUploaderTests() = 
    inherit DbTests()

    let testTimeZone = TimeZone "FLE Standard Time"

    [<Fact>]
    member this.``UploadData should upload data correctly for the last days`` () = 
        let timeInterval = 
            { From = System.DateTime.UtcNow.AddDays(-3.0)
              To = System.DateTime.UtcNow }
        
        async {
            do! DataUploader.uploadDataAsync
                    this.Logger
                    Settings.ConnectionStrings.WeatherStation 
                    DbInsertOptions.Default
                    testTimeZone
                    timeInterval
                    (getTestDeviceInfo())
                    (getTestStationId())

            let! measurements = DbService.getMeasurementsAsync this.Logger Settings.ConnectionStrings.WeatherStation

            measurements.Length |> should be (greaterThan 250)
        }
        |> Async.RunSynchronously