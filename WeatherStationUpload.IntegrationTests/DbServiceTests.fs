namespace WeatherStationUpload.IntegrationTests

open System
open WeatherStationUpload.DatabaseUtils
open WeatherStationUpload
open Xunit

type DbServiceTests() =
    inherit DbTests()
    
    let connectionString = Settings.ConnectionStrings.WeatherStation

    let currentTime = DateTime.UtcNow

    let getSampleMeasurement () : MeasurementData = 
        //{ Header = 
        //    { ObservationTime = 
        //        { Date = roundedObservationTime.Date
        //          Hour = byte(roundedObservationTime.Hour) }
        //      StationNumber = stationNumber 
        //      RequestTime = roundToSeconds currentTime }
        //  Temperature = -1.3m }
        failwith "Not implemented"

    let sortMeasurements (observations : MeasurementData list) : MeasurementData list = 
        observations |> List.sort
    
    let saveMeasurement deviceInfo measurement =
        DatabaseUtils.writeDataContext 
            DbService.insertMeasurement connectionString (deviceInfo, measurement) 
        |> ResultUtils.get

    let saveMeasurements deviceInfo measurements =
        DatabaseUtils.writeDataContextForList 
            DbService.insertMeasurement connectionString measurements
        |> ResultUtils.get
    
    let testSaveMeasurements measurements = 
        saveMeasurements measurements
        
        let result = 
            readDataContext 
                DbService.getMeasurements connectionString
        
        let expectedResult = measurements |> sortMeasurements |> Success
        expectedResult =! (result |> Result.map sortMeasurements)

    [<Fact>]
    let ``SaveObservations should save empty list of observation correctly``() = 
        testSaveMeasurements []

    [<Fact>]
    let ``SaveObservations should save single observation correctly``() = 
        testSaveMeasurements [ getSampleMeasurement() ]

    