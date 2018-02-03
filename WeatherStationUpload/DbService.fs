module WeatherStationUpload.DbService

open System
open System.Data.SqlClient
open FSharp.Interop.Dynamic
open Serilog
open Dapper
open MeasureUtils
open WeatherStationUpload

let InsertSqlStatement = "
    INSERT INTO dbo.Measurements(StationId, Timestamp, TemperatureInside, TemperatureOutside, HumidityInside, HumidityOutside)
    VALUES(@stationId, @timestamp, @temperatureInside, @temperatureOutside, @humidityInside, @humidityOutside)"

type MeasurementDbEntity = 
    { StationId: int
      Timestamp: DateTime
      TemperatureInside: Nullable<decimal>
      HumidityInside: Nullable<decimal>
      TemperatureOutside: Nullable<decimal>
      HumidityOutside: Nullable<decimal> }

let toOption (item: obj) : 'a option = 
    if (isNull (box item)) then None else Some(item :?> 'a)

let optionToNullable = function
    | Some value -> Nullable(value)
    | None -> Nullable()

let measurementToEntity (StationId stationId) (measurement: Measurement) : MeasurementDbEntity = 
    { StationId = stationId
      Timestamp = measurement.Timestamp
      TemperatureInside = measurement.TemperatureInside |> Option.map celsiusToValue |> optionToNullable
      TemperatureOutside = measurement.TemperatureOutside |> Option.map celsiusToValue |> optionToNullable
      HumidityInside = measurement.HumidityInside |> Option.map percentToValue |> optionToNullable
      HumidityOutside = measurement.HumidityOutside |> Option.map percentToValue |> optionToNullable }
      
let insertMeasurementAsync
        (logger: ILogger) 
        (connectionString: string) 
        (StationId stationId, measurement: Measurement) 
        : Async<unit> =
    async { 
        use connection = new SqlConnection(connectionString)
        return! connection.ExecuteAsync(
            InsertSqlStatement, 
            measurement |> measurementToEntity (StationId stationId)) |> Async.AwaitTask |> Async.Ignore
    }

let insertMeasurementsAsync
        (logger: ILogger) 
        (connectionString: string) 
        (options: DbInsertOptions)
        (measurements: list<StationId * Measurement>)
        : Async<unit> =
    async { 
        use connection = new SqlConnection(connectionString)
        for (stationId, measurement) in measurements do
            do! connection.ExecuteAsync(
                    InsertSqlStatement, 
                    measurement |> measurementToEntity stationId) |> Async.AwaitTask |> Async.Ignore
    }

let getMeasurementsAsync
        (logger: ILogger) 
        (connectionString: string) 
        : Async<list<StationId * Measurement>> = 
    async {
        use connection = new SqlConnection(connectionString)
        let! records = connection.QueryAsync("SELECT * FROM dbo.Measurements ORDER BY StationId") |> Async.AwaitTask 
        return records
        |> Seq.map
            (fun record -> 
                StationId record ? StationId,
                { Timestamp = record ? Timestamp 
                  TemperatureInside = record ? TemperatureInside |> toOption |> Option.map valueToCelsius
                  HumidityInside = record ? HumidityInside |> toOption |> Option.map valueToPercent
                  TemperatureOutside = record ? TemperatureOutside |> toOption |> Option.map valueToCelsius
                  HumidityOutside = record ? HumidityOutside |> toOption |> Option.map valueToPercent })
        |> List.ofSeq
    }

let getStationsLastMeasurementsAsync 
        (logger: ILogger)
        (connectionString: string)
        : Async<list<StationId * DeviceInfo * DateTime option * TimeZone>> =
    async {
        use connection = new SqlConnection(connectionString)
        let! records = connection.QueryAsync("
            SELECT s.Id stationId, s.DeviceId, s.VendorId, MAX(m.Timestamp) Timestamp FROM dbo.Stations s
            LEFT OUTER JOIN dbo.Measurements m ON s.Id = m.StationId
            GROUP BY s.Id, s.DeviceId, s.VendorId") |> Async.AwaitTask
        return
            records
            |> Seq.toList
            |> List.map
                (fun record ->
                    StationId record ? stationId,
                    {
                        DeviceId = record ? DeviceId
                        VendorId = record ? VendorId
                    },
                    record ? Timestamp |> toOption,
                    TimeZone "FLE Standard Time")
    }
