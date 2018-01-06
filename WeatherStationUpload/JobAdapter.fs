module WeatherStationUpload.JobAdapter

open System
open System.Threading.Tasks
open Serilog

let private nullableToOption (a : System.Nullable<'T>) = 
    if a.HasValue then
        Some a.Value
    else
        None

let executeAsync(logger: ILogger, 
                 connectionString: string, 
                 dbInsertTimeout: Nullable<TimeSpan>, 
                 dbInsertBatchSize: Nullable<int>,
                 intervalEndTime: DateTime, 
                 maxTimeInterval: TimeSpan): Task<bool> = 
    Job.executeAsync 
        logger 
        connectionString 
        { Timeout = dbInsertTimeout |> nullableToOption
          BatchSize = dbInsertBatchSize |> nullableToOption }
        intervalEndTime 
        maxTimeInterval
    |> Async.StartAsTask
