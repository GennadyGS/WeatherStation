namespace WeatherStationUpload.IntegrationTests

open System
open WeatherStationUpload.DatabaseUtils
open WeatherStationUpload
open Xunit
open Utils
open WeatherStationUpload.ResultUtils

type DbServiceTests() =
    inherit DbTests()
    
    let connectionString = Settings.ConnectionStrings.WeatherStation
    let testDeviceInfo = getTestDeviceInfo()

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
        (testDeviceInfo, measurement)
        |> DatabaseUtils.writeDataContext DbService.insertMeasurement connectionString 
        |> ResultUtils.get

    let saveMeasurements measurements =
        measurements
        |> List.map (fun measurement -> (testDeviceInfo, measurement))
        |> DatabaseUtils.writeDataContextForList 
            DbService.insertMeasurement connectionString 
        |> ResultUtils.get
    
    let testSaveMeasurements measurements = 
        saveMeasurements measurements
        
        let result = 
            readDataContext DbService.getMeasurements connectionString
            |> Result.map (List.map snd)
        
        let expectedResult = measurements |> sortMeasurements |> Ok
        Assert.Equal(expectedResult, result |> Result.map sortMeasurements)

    [<Fact>]
    let ``SaveObservations should save empty list of observation correctly``() = 
        testSaveMeasurements []

    [<Fact>]
    let ``SaveObservations should save observations correctly``() = 
        testSaveMeasurements (getSampleMeasurements())
