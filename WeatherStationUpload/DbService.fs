module WeatherStationUpload.DbService

open FSharp.Data.Sql
open System.Linq
open MeasureUtils

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

let insertMeasurement (dataContext : DataContext) (measurement : Measurement) : unit =
    let row = dataContext.InnerDataContext.Dbo.Observations.Create()
    row.DeviceId <- measurement.Device.DeviceId
    row.VendorId <- measurement.Device.VendorId
    row.HumidityInside <- measurement.Data.HumidityInside |> Option.map unwrapPercent
    row.HumidityOutside <- measurement.Data.HumidityOutside |> Option.map unwrapPercent
    row.TemperatureInside <- measurement.Data.TemperatureInside |> Option.map unwrapCelsius
    row.TemperatureOutside <- measurement.Data.TemperatureOutside |> Option.map unwrapCelsius
    row.Timestamp <- measurement.Data.Timestamp

[<ReflectedDefinition>]
let private entityToMeasurement 
        (entity : SqlProvider.dataContext.``dbo.ObservationsEntity``) : Measurement = 
    { Device = 
        { VendorId = entity.VendorId
          DeviceId = entity.DeviceId }
      Data = 
        { Timestamp = entity.Timestamp 
          TemperatureInside = entity.TemperatureInside |> Option.map wrapCelsius
          HumidityInside = entity.HumidityInside |> Option.map wrapPercent
          TemperatureOutside = entity.TemperatureOutside |> Option.map wrapCelsius
          HumidityOutside = entity.HumidityOutside |> Option.map wrapPercent }}

let getMeasurements (dataContext : DataContext) : IQueryable<Measurement> = 
    query {
        for measurement in dataContext.InnerDataContext.Dbo.Observations do
        sortBy measurement.VendorId
        thenBy measurement.DeviceId
        thenByDescending measurement.Timestamp
        select (entityToMeasurement measurement)
    }
