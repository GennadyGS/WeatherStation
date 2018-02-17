module WeatherStationUpload.DbService

open System
open System.Data.SqlClient
open FSharp.Interop.Dynamic
open Serilog
open Dapper
open Dapper.Bulk
open MeasureUtils
open WeatherStationUpload
open System.ComponentModel.DataAnnotations.Schema

[<Table("Measurements")>]
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

let getOrElse defaultValue = 
    function
    | Some value -> value
    | None -> defaultValue

let stationMeasurementToEntity (StationId stationId, measurement: Measurement) : MeasurementDbEntity = 
    { StationId = stationId
      Timestamp = measurement.Timestamp
      TemperatureInside = measurement.TemperatureInside |> Option.map celsiusToValue |> optionToNullable
      TemperatureOutside = measurement.TemperatureOutside |> Option.map celsiusToValue |> optionToNullable
      HumidityInside = measurement.HumidityInside |> Option.map percentToValue |> optionToNullable
      HumidityOutside = measurement.HumidityOutside |> Option.map percentToValue |> optionToNullable }
      
// It is required because optional parameters in C# helper methods work not as expected
type private BulkCopyUtils =
    static member BulkInsertAsync(connection: SqlConnection, 
                                  data: 'a seq,
                                  ?transaction: SqlTransaction, 
                                  ?batchSize: int, 
                                  ?bulkCopyTimeout: TimeSpan)
                                  : Async<unit> =
        let defalutBatchSize = 0
        let defaultCulkCopyTimeoutSec = 30
        connection.BulkInsertAsync(
            data, 
            transaction = (transaction |> getOrElse null), 
            batchSize = (batchSize |> getOrElse defalutBatchSize), 
            bulkCopyTimeout = (
                bulkCopyTimeout 
                |> Option.map (fun timeSpan -> int(timeSpan.TotalSeconds)) 
                |> getOrElse defaultCulkCopyTimeoutSec))
        |> Async.AwaitTask

let insertMeasurementsAsync
        (logger: ILogger) 
        (connectionString: string) 
        (options: DbInsertOptions)
        (stationMeasurements: list<StationId * Measurement>)
        : Async<unit> =
    async { 
        use connection = new SqlConnection(connectionString)
        do! connection.OpenAsync() |> Async.AwaitTask
        let measurementEntities = 
            stationMeasurements
            |> List.map stationMeasurementToEntity

        do! BulkCopyUtils.BulkInsertAsync(connection, measurementEntities, ?bulkCopyTimeout = options.Timeout, ?batchSize = options.BatchSize) |> Async.Ignore
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
        : Async<(StationInfo * DateTime option) list> =
    async {
        use connection = new SqlConnection(connectionString)
        let! records = connection.QueryAsync("
            SELECT s.Id stationId, s.DeviceId, s.VendorId, s.TimeZoneName, MAX(m.Timestamp) Timestamp
            FROM dbo.Stations s
            LEFT OUTER JOIN dbo.Measurements m ON s.Id = m.StationId
            GROUP BY s.Id, s.DeviceId, s.VendorId, s.TimeZoneName") |> Async.AwaitTask
        return
            records
            |> Seq.toList
            |> List.map
                (fun record ->
                    { StationId = StationId record ? stationId
                      DeviceInfo =
                        { DeviceId = record ? DeviceId
                          VendorId = record ? VendorId }
                      TimeZone = TimeZone record ? TimeZoneName },
                      record ? Timestamp |> toOption)
    }
