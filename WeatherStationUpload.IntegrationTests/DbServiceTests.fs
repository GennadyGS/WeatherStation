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

    let getSampleMeasurements () : MeasurementData list = 
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

    let sortMeasurements (measurements : MeasurementData list) : MeasurementData list = 
        measurements |> List.sortBy (fun measurement -> measurement.Timestamp)
    
    let saveMeasurement measurement =
        (testDeviceInfo, measurement)
        |> DatabaseUtils.writeDataContext DbService.insertMeasurementData connectionString 
        |> ResultUtils.get

    let saveMeasurements measurements =
        measurements
        |> List.map (fun measurement -> (testDeviceInfo, measurement))
        |> DatabaseUtils.writeDataContextForList 
            DbService.insertMeasurementData connectionString 
        |> ResultUtils.get
    
    let testSaveMeasurements measurements = 
        saveMeasurements measurements
        
        let result = 
            readDataContext 
                DbService.getMeasurements connectionString
            |> Result.map 
                (fun measurements -> 
                    List.map (fun measurement -> measurement.Data) measurements)
        
        let expectedResult = measurements |> sortMeasurements |> Ok
        Assert.Equal(expectedResult, result |> Result.map sortMeasurements)

    [<Fact>]
    let ``SaveObservations should save empty list of observation correctly``() = 
        testSaveMeasurements []

    [<Fact>]
    let ``SaveObservations should save observations correctly``() = 
        testSaveMeasurements (getSampleMeasurements())
