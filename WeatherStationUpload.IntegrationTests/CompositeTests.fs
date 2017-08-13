namespace WeatherStationUpload.IntegrationTests

open Xunit

open WeatherStationUpload.Composite

type CompositeTests() = 
    let stationId = "07523951F222"
    
    [<Fact>]
    member this.``Test`` () = 
        let dateFrom = System.DateTime.Now.AddDays(-1.0)
        let dateTo = System.DateTime.Now
        let result = processWeatherData dateFrom dateTo stationId
        ()
