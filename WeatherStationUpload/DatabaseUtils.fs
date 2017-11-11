module WeatherStationUpload.DatabaseUtils

open System.Data.SqlClient

let inline createDataContext connectionString : ^dc = 
    (^dc: (static member Create: string -> ^dc) connectionString)

let inline saveChangesToDataContext (dataContext : ^dc) = 
    (^dc: (static member SaveChanges: ^dc -> unit) dataContext)

let handleSqlException func =
    fun arg -> 
        try
            Ok (func arg)
        with
            | :? SqlException as e -> Error (e.ToString())

let runQuerySafe query = 
    query
    |> handleSqlException Seq.toList

let inline saveChangesSafe dataContext = 
    dataContext
    |> handleSqlException saveChangesToDataContext

let inline readDataContext func = 
    createDataContext >> func

let inline writeDataContext (func : 'dc -> 'a -> unit) = 
    createDataContext >>
    fun dataContext a -> 
        func dataContext a
        saveChangesSafe dataContext

let inline writeDataContextForList (func : 'dc -> 'a -> unit) = 
    writeDataContext (fun dataContext -> List.map (func dataContext) >> ignore)
