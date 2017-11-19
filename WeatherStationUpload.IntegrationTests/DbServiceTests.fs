namespace WeatherStationUpload.IntegrationTests

open System
open WeatherStationUpload.DatabaseUtils
open WeatherStationUpload
open Xunit
open Utils

type DbServiceTests() =
    inherit DbTests()
    
    let connectionString = Settings.ConnectionStrings.WeatherStation
    let testStationId = getTestStationId()

    let getSampleMeasurements () : Measurement list = 
        [ 
            { TemperatureInside = Some 19.2m<C>
              TemperatureOutside = Some -12.3m<C>
              HumidityInside = Some 56.2m<``%``>
              HumidityOutside = Some 79.5m<``%``> 
              Timestamp = DateTime(2017, 10, 16, 09, 34, 27) }
            { TemperatureInside = Some 23.5m<C>
              TemperatureOutside = Some 5.6m<C>
              HumidityInside = Some 46.6m<``%``>
              HumidityOutside = Some 79.1m<``%``> 
              Timestamp = DateTime(2017, 11, 18, 11, 02, 03) }
        ]

    let sortMeasurements (measurements : Measurement list) : Measurement list = 
        measurements |> List.sortBy (fun measurement -> measurement.Timestamp)
    
    let saveMeasurement measurement =
        (testStationId, measurement)
        |> DatabaseUtils.writeDataContext DbService.insertMeasurement connectionString 

    let saveMeasurements measurements =
        measurements
        |> List.map (fun measurement -> (testStationId, measurement))
        |> DatabaseUtils.writeDataContextForList 
            DbService.insertMeasurement connectionString 
    
    let testSaveMeasurements measurements = 
        saveMeasurements measurements
        
        let result = 
            readDataContext DbService.getMeasurements connectionString
            |> List.map snd
        
        let expectedResult = measurements |> sortMeasurements
        Assert.Equal<Measurement list>(expectedResult, result |> sortMeasurements)

    [<Fact>]
    let ``SaveMeasurements should save empty list of observation correctly``() = 
        testSaveMeasurements []

    [<Fact>]
    let ``SaveMeasurements should save measurements correctly``() = 
        testSaveMeasurements (getSampleMeasurements())
