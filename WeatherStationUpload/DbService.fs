module WeatherStationUpload.DbService

open FSharp.Data.Sql

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

let insertMeasurement 
        (dataContext : DataContext) 
        (deviceInfo: DeviceInfo) 
        (measurement: Measurement) 
        : unit =
    let row = dataContext.InnerDataContext.Dbo.Observations.Create()
    row.DeviceId <- deviceInfo.DeviceId
    row.VendorId <- deviceInfo.VendorId
    row.HumidityInside <- Some (measurement.HumidityInside / 1m<``%``>)
    row.HumidityOutside <- Some (measurement.HumidityOutside / 1m<``%``>)
    row.TemperatureInside <- Some (measurement.TemperatureInside / 1m<C>)
    row.TemperatureOutside <- Some (measurement.TemperatureOutside / 1m<C>)
    row.Timestamp <- measurement.Timestamp
