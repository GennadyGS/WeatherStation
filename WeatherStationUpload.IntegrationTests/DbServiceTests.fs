namespace WeatherStationUpload.IntegrationTests

open System
open WeatherStationUpload
open Xunit
open Utils
open FsUnit.Xunit
open Serilog

type DbServiceTests() =
    inherit DbTests()
    
    let testConnectionString = Settings.ConnectionStrings.WeatherStation
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
    
    member private this.saveMeasurement measurement =
        (testStationId, measurement)
        |> DbService.insertMeasurementAsync this.Logger testConnectionString 
        |> Async.RunSynchronously

    member private this.saveMeasurements measurements =
        measurements
        |> List.map (fun measurement -> (testStationId, measurement))
        |> DbService.insertMeasurementsAsync this.Logger testConnectionString 
        |> Async.RunSynchronously
    
    member private this.testSaveMeasurements measurements = 
        this.saveMeasurements measurements
        
        let result = 
            DbService.getMeasurementsAsync this.Logger testConnectionString
            |> Async.RunSynchronously
            |> List.map snd
        
        result |> should equal (measurements |> sortMeasurements)

    [<Fact>]
    member this.``SaveMeasurements should save empty list of observation correctly``() = 
        this.testSaveMeasurements []

    [<Fact>]
    member this.``SaveMeasurements should save measurements correctly``() = 
        this.testSaveMeasurements (getSampleMeasurements())

    [<Fact>]
    member this. ``getStationsLastMeasurements returns one record with empty time for empty database``() = 
        let results = 
            DbService.getStationsLastMeasurementsAsync this.Logger testConnectionString
            |> Async.RunSynchronously
        
        results = [testStationId, getTestDeviceInfo(), None] |> should be True

    [<Fact>]
    member this.``getStationsLastMeasurements returns correct last measurement time``() = 
        this.saveMeasurements (getSampleMeasurements())
        
        let results = 
            DbService.getStationsLastMeasurementsAsync this.Logger testConnectionString
            |> Async.RunSynchronously
        
        let maxSampleMeasurementTime = 
            getSampleMeasurements() 
            |> List.map (fun item -> item.Timestamp)
            |> List.max

        results |> should equal [testStationId, getTestDeviceInfo(), Some maxSampleMeasurementTime]
