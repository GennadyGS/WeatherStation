module WeatherStationUpload.DbService

open FSharp.Data
open MeasureUtils
open System
open System.Data.SqlClient

[<Literal>]
let devConnectionString =
    @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=WeatherStation;Integrated Security=True"

type WeatherStation = SqlProgrammabilityProvider<devConnectionString>

let insertMeasurementAsync (connectionString: string) (StationId stationId, measurement: Measurement) : Async<unit> =
    async { 
        use connection = new SqlConnection(connectionString)
        let measurementsTable = new WeatherStation.dbo.Tables.Measurements()
        measurementsTable.AddRow(
            stationId,
            measurement.Timestamp,
            TemperatureInside = (measurement.TemperatureInside |> Option.map celsiusToValue),
            TemperatureOutside = (measurement.TemperatureOutside |> Option.map celsiusToValue),
            HumidityInside = (measurement.HumidityInside |> Option.map percentToValue),
            HumidityOutside = (measurement.HumidityOutside |> Option.map percentToValue))
        return measurementsTable.Update(connection)
    }
    |> AsyncUtils.map ignore

let insertMeasurementsAsync (connectionString: string) (measurements: list<StationId * Measurement>) : Async<unit> =
    async { 
        use connection = new SqlConnection(connectionString)
        let measurementsTable = new WeatherStation.dbo.Tables.Measurements()
        measurements
            |> List.map 
                (fun (StationId stationId, measurement) ->
                    measurementsTable.AddRow(
                        stationId,
                        measurement.Timestamp,
                        TemperatureInside = (measurement.TemperatureInside |> Option.map celsiusToValue),
                        TemperatureOutside = (measurement.TemperatureOutside |> Option.map celsiusToValue),
                        HumidityInside = (measurement.HumidityInside |> Option.map percentToValue),
                        HumidityOutside = (measurement.HumidityOutside |> Option.map percentToValue)))
            |> ignore
        return measurementsTable.BulkCopy(connection)
    }
    |> AsyncUtils.map ignore

let getMeasurementsAsync (connectionString: string) : Async<list<StationId * Measurement>> = 
    use cmd = new SqlCommandProvider<"
        SELECT * FROM dbo.Measurements ORDER BY StationId", devConnectionString>(connectionString);

    cmd.AsyncExecute() 
    |> AsyncUtils.map 
        (Seq.map 
            (fun record -> 
                StationId record.StationId,
                { Timestamp = record.Timestamp 
                  TemperatureInside = record.TemperatureInside |> Option.map valueToCelsius
                  HumidityInside = record.HumidityInside |> Option.map valueToPercent
                  TemperatureOutside = record.TemperatureOutside |> Option.map valueToCelsius
                  HumidityOutside = record.HumidityOutside |> Option.map valueToPercent }))
    |> AsyncUtils.map Seq.toList

let getStationsLastMeasurementsAsync (connectionString: string): Async<list<StationId * DeviceInfo * DateTime option>> =
    use cmd = new SqlCommandProvider<"
            SELECT s.Id stationId, s.DeviceId, s.VendorId, MAX(m.Timestamp) Timestamp FROM dbo.Stations s
            LEFT OUTER JOIN dbo.Measurements m ON s.Id = m.StationId
            GROUP BY s.Id, s.DeviceId, s.VendorId
            ", devConnectionString>(connectionString)
    cmd.AsyncExecute()
    |> AsyncUtils.map (Seq.map(fun record ->
            StationId record.stationId,
            {
                DeviceId = record.DeviceId
                VendorId = record.VendorId
            },
            record.Timestamp))
    |> AsyncUtils.map Seq.toList
