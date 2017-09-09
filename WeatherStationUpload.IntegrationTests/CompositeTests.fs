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
    member this.``ProcessWeatherData should return non-empty result for the last day`` () = 
        let dateFrom = System.DateTime.Now.AddDays(-1.0)
        let dateTo = System.DateTime.Now
        let result = processWeatherData dateFrom dateTo deviceInfo
        Assert.NotEmpty result
