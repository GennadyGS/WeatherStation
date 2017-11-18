module WeatherStationUpload.DbService

open FSharp.Data.Sql
open System.Linq
open MeasureUtils
open WeatherStationUpload.DatabaseUtils

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

let insertMeasurement (dataContext : DataContext) (deviceInfo : DeviceInfo, measurement : Measurement) : unit =
    let row = dataContext.InnerDataContext.Dbo.Observations.Create()
    row.DeviceId <- deviceInfo.DeviceId
    row.VendorId <- deviceInfo.VendorId
    row.HumidityInside <- measurement.HumidityInside |> Option.map percentToValue
    row.HumidityOutside <- measurement.HumidityOutside |> Option.map percentToValue
    row.TemperatureInside <- measurement.TemperatureInside |> Option.map celsiusToValue
    row.TemperatureOutside <- measurement.TemperatureOutside |> Option.map celsiusToValue
    row.Timestamp <- measurement.Timestamp

[<ReflectedDefinition>]
let private entityToMeasurement 
        (entity : SqlProvider.dataContext.``dbo.ObservationsEntity``) : DeviceInfo * Measurement = 
    { VendorId = entity.VendorId
      DeviceId = entity.DeviceId },
    { Timestamp = entity.Timestamp 
      TemperatureInside = entity.TemperatureInside |> Option.map valueToCelsius
      HumidityInside = entity.HumidityInside |> Option.map valueToPercent
      TemperatureOutside = entity.TemperatureOutside |> Option.map valueToCelsius
      HumidityOutside = entity.HumidityOutside |> Option.map valueToPercent }

let getMeasurements (dataContext : DataContext) : Result<(DeviceInfo * Measurement) list, string> = 
    query {
        for measurement in dataContext.InnerDataContext.Dbo.Observations do
        sortBy measurement.VendorId
        thenBy measurement.DeviceId
        thenByDescending measurement.Timestamp
        select (entityToMeasurement measurement)
    }
    |> runQuerySafe
