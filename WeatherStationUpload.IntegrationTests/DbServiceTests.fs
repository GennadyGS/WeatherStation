﻿namespace WeatherStationUpload.IntegrationTests

open System
open WeatherStationUpload.DatabaseUtils
open WeatherStationUpload
open Xunit

type DbServiceTests() =
    inherit DbTests()
    
    let connectionString = Settings.ConnectionStrings.WeatherStation

    let getSampleMeasurement () : Measurement = 
        { Device = 
            { VendorId = Settings.VendorId 
              DeviceId = Settings.DeviceId } 
          Data = 
            { TemperatureInside = Some (19.2m<C>)
              TemperatureOutside = Some (-12.3m<C>)
              HumidityInside = Some (56.2m<``%``>)
              HumidityOutside = Some (79.5m<``%``>) 
              Timestamp = DateTime(2017, 11, 18, 11, 02, 03) }}

    let sortMeasurements (measurements : Measurement list) : Measurement list = 
        measurements |> List.sort
    
    let saveMeasurement measurement =
        DatabaseUtils.writeDataContext 
            DbService.insertMeasurement connectionString measurement
        |> ResultUtils.get

    let saveMeasurements measurements =
        DatabaseUtils.writeDataContextForList 
            DbService.insertMeasurement connectionString measurements
        |> ResultUtils.get
    
    let testSaveMeasurements measurements = 
        saveMeasurements measurements
        
        let result = 
            readDataContext 
                DbService.getMeasurements connectionString
        
        let expectedResult = measurements |> sortMeasurements |> Ok
        Assert.Equal(expectedResult, result |> Result.map sortMeasurements)

    [<Fact>]
    let ``SaveObservations should save empty list of observation correctly``() = 
        testSaveMeasurements []

    [<Fact>]
    let ``SaveObservations should save single observation correctly``() = 
        testSaveMeasurements [ getSampleMeasurement() ]

    