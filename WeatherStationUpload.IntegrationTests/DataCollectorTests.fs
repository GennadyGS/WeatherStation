namespace WeatherStationUpload.IntegrationTests

open Xunit
open WeatherStationUpload
open WeatherStationUpload.DataCollector

type DataCollectorTests() = 

    let deviceInfo : DeviceInfo = 
        { DeviceId = "07523951F222"
          VendorId = "270f2261-3477-4872-9580-ead9cab3044c" }
    
    [<Fact>]
    member this.``CollectWeatherData should return result with appropriate size for the last days`` () = 
        let dateFrom = System.DateTime.Now.AddDays(-3.0)
        let dateTo = System.DateTime.Now
        let result = collectData dateFrom dateTo deviceInfo
        Assert.True (result.Length > 250)
