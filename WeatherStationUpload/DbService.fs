module WeatherStationUpload.DbService

open FSharp.Data.Sql
open MeasureUtils
open System
open FSharp.Data

[<Literal>]
let devConnectionString =
    @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=WeatherStation;Integrated Security=True"

type WeatherStation = SqlProgrammabilityProvider<devConnectionString>

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

let insertMeasurement (connectionString: string) (StationId stationId, measurement: Measurement) : unit =
    let measurementsTable = new WeatherStation.dbo.Tables.Measurements()
    measurementsTable.AddRow(
        stationId,
        measurement.Timestamp,
        TemperatureInside = (measurement.TemperatureInside |> Option.map celsiusToValue),
        TemperatureOutside = (measurement.TemperatureOutside |> Option.map celsiusToValue),
        HumidityInside = (measurement.HumidityInside |> Option.map percentToValue),
        HumidityOutside = (measurement.HumidityOutside |> Option.map percentToValue))
    measurementsTable.Update() |> ignore

let insertMeasurements (connectionString: string) (measurements: list<StationId * Measurement>) : unit =
    let measurementsTable = new WeatherStation.dbo.Tables.Measurements()
    measurements
    |> List.map (
        fun (StationId stationId, measurement) ->
            measurementsTable.AddRow(
                stationId,
                measurement.Timestamp,
                TemperatureInside = (measurement.TemperatureInside |> Option.map celsiusToValue),
                TemperatureOutside = (measurement.TemperatureOutside |> Option.map celsiusToValue),
                HumidityInside = (measurement.HumidityInside |> Option.map percentToValue),
                HumidityOutside = (measurement.HumidityOutside |> Option.map percentToValue)))
    |> ignore
    measurementsTable.BulkCopy()

let getMeasurements (connectionString: string) : list<StationId * Measurement> = 
    use cmd = new SqlCommandProvider<"
        SELECT * FROM dbo.Measurements ORDER BY StationId", devConnectionString>(connectionString);

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
