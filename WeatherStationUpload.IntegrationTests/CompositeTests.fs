namespace WeatherStationUpload.IntegrationTests

open Xunit

open WeatherStationUpload.Composite
open Main
open WeatherStationUpload

type CompositeTests() = 

    let deviceInfo : DeviceInfo = 
        { DeviceId = "07523951F222"
          VendorId = "270f2261-3477-4872-9580-ead9cab3044c" }
    
    [<Fact>]
    member this.``ProcessWeatherData should return result with appropriate size for the last days`` () = 
        let dateFrom = System.DateTime.Now.AddDays(-3.0)
        let dateTo = System.DateTime.Now
        let result = processWeatherData dateFrom dateTo deviceInfo
        Assert.True (result.Length > 250)
