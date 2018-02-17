module WeatherStationUpload.JobAdapter

open System
open System.Threading.Tasks
open Serilog

let private nullableToOption (a : System.Nullable<'T>) = 
    if a.HasValue then
        Some a.Value
    else
        None

let private optionToNullable = function
    | Some value -> Nullable value
    | None -> Nullable ()

let executeAsync(logger: ILogger, 
                 connectionString: string, 
                 dbInsertTimeout: Nullable<TimeSpan>, 
                 dbInsertBatchSize: Nullable<int>,
                 intervalEndTimeUtc: DateTime, 
                 maxTimeInterval: TimeSpan): Task<bool> = 
    Job.executeAsync 
        logger 
        connectionString 
        { Timeout = dbInsertTimeout |> nullableToOption
          BatchSize = dbInsertBatchSize |> nullableToOption }
        intervalEndTimeUtc 
        maxTimeInterval
    |> Async.StartAsTask

let getStationsLastMeasurementsAsync(logger: ILogger, connectionString: string)
        : Task<struct (StationInfo * Nullable<DateTime>) array> =
    async {
        let! results = DbService.getStationsLastMeasurementsAsync logger connectionString 
        return results
        |> List.map
            (fun (stationInfo, lastMeasurementOption) -> struct(stationInfo, optionToNullable(lastMeasurementOption)))
        |> List.toArray
    } |> Async.StartAsTask
