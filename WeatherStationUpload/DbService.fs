module WeatherStationUpload.DbService

open FSharp.Data.Sql
open MeasureUtils
open System
open FSharp.Data

[<Literal>]
let devConnectionString =
    @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=WeatherStation;Integrated Security=True"

type MeasurementRecord = SqlCommandProvider<"
        SELECT * FROM Measurements ORDER BY StationId", devConnectionString>.Record

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

let insertMeasurement (dataContext : DataContext) (StationId stationId, measurement : Measurement) : unit =
    let row = dataContext.InnerDataContext.Dbo.Measurements.Create()
    row.StationId <- stationId
    row.HumidityInside <- measurement.HumidityInside |> Option.map percentToValue
    row.HumidityOutside <- measurement.HumidityOutside |> Option.map percentToValue
    row.TemperatureInside <- measurement.TemperatureInside |> Option.map celsiusToValue
    row.TemperatureOutside <- measurement.TemperatureOutside |> Option.map celsiusToValue
    row.Timestamp <- measurement.Timestamp

let getMeasurements (connectionString: string) : list<StationId * Measurement> = 
    use cmd = new SqlCommandProvider<"
        SELECT * FROM Measurements ORDER BY StationId", devConnectionString>(connectionString);

    cmd.Execute() 
    |> Seq.map (
        fun record -> 
            StationId record.StationId,
            { Timestamp = record.Timestamp 
              TemperatureInside = record.TemperatureInside |> Option.map valueToCelsius
              HumidityInside = record.HumidityInside |> Option.map valueToPercent
              TemperatureOutside = record.TemperatureOutside |> Option.map valueToCelsius
              HumidityOutside = record.HumidityOutside |> Option.map valueToPercent })
    |> Seq.toList

let getStationsLastMeasurements (connectionString: string): list<StationId * DeviceInfo * DateTime option> =
    use cmd = new SqlCommandProvider<"
            SELECT s.Id stationId, s.DeviceId, s.VendorId, MAX(m.Timestamp) Timestamp FROM dbo.Stations s
            LEFT OUTER JOIN dbo.Measurements m ON s.Id = m.StationId
            GROUP BY s.Id, s.DeviceId, s.VendorId
            ", devConnectionString>(connectionString)
    cmd.Execute()
    |> Seq.map(fun record ->
            StationId record.stationId,
            {
                DeviceId = record.DeviceId
                VendorId = record.VendorId
            },
            record.Timestamp)
    |> Seq.toList
