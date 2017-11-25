module WeatherStationUpload.DbService

open FSharp.Data.Sql
open MeasureUtils
open WeatherStationUpload.DatabaseUtils
open System

type private SqlProvider = 
    SqlDataProvider<
        ConnectionStringName = "WeatherStation",
        UseOptionTypes = true>

type DataContext private (innerDataContext : SqlProvider.dataContext) = 
    member internal this.InnerDataContext: SqlProvider.dataContext = innerDataContext
    
    static member Create (connectionString: string) = 
        DataContext(SqlProvider.GetDataContext(connectionString))
    
    static member SaveChanges (dataContext : DataContext) = 
        let d: SqlProvider.dataContext = SqlProvider.GetDataContext("");

        dataContext.InnerDataContext.SubmitUpdates()

[<ReflectedDefinition>]
let private defaultToOption (item: 'a) = 
    if item <> Unchecked.defaultof<'a> then Some(item) else None

let insertMeasurement (dataContext : DataContext) (StationId stationId, measurement : Measurement) : unit =
    let row = dataContext.InnerDataContext.Dbo.Measurements.Create()
    row.StationId <- stationId
    row.HumidityInside <- measurement.HumidityInside |> Option.map percentToValue
    row.HumidityOutside <- measurement.HumidityOutside |> Option.map percentToValue
    row.TemperatureInside <- measurement.TemperatureInside |> Option.map celsiusToValue
    row.TemperatureOutside <- measurement.TemperatureOutside |> Option.map celsiusToValue
    row.Timestamp <- measurement.Timestamp

[<ReflectedDefinition>]
let private entityToMeasurement 
        (entity : SqlProvider.dataContext.``dbo.MeasurementsEntity``) : StationId * Measurement = 
    StationId entity.StationId,
    { Timestamp = entity.Timestamp 
      TemperatureInside = entity.TemperatureInside |> Option.map valueToCelsius
      HumidityInside = entity.HumidityInside |> Option.map valueToPercent
      TemperatureOutside = entity.TemperatureOutside |> Option.map valueToCelsius
      HumidityOutside = entity.HumidityOutside |> Option.map valueToPercent }

let getMeasurements (dataContext : DataContext) : list<StationId * Measurement> = 
    query {
        for measurement in dataContext.InnerDataContext.Dbo.Measurements do
        sortBy measurement.StationId
        thenByDescending measurement.Timestamp
        select (entityToMeasurement measurement)
    }
    |> runQuery

let getStationsLastMeasurements (dataContext : DataContext): list<StationId * DeviceInfo * DateTime option> =
    let stationMeasurementTimesQuery = 
        query {
            for station in dataContext.InnerDataContext.Dbo.Stations do
            sortBy station.Id
            for measurement in (!!)station.``dbo.Measurements by Id`` do
            select (station, defaultToOption measurement.Timestamp)
        }

    let getDeviceInfo (stationEntity: SqlProvider.dataContext.``dbo.StationsEntity``) = 
        { VendorId = stationEntity.VendorId
          DeviceId = stationEntity.DeviceId }
    
    query {
        for (station, measurement) in stationMeasurementTimesQuery do
        groupBy (station.Id, getDeviceInfo(station)) into group
        let maxMeasurementTime = query {
            for (_, measurementTime) in group do
            maxBy measurementTime
        }
        let (stationId, deviceInfo) = group.Key
        select (StationId stationId, deviceInfo, maxMeasurementTime)
    }
    |> runQuery
