namespace WeatherStationUpload.IntegrationTests

open FSharp.Data.Sql.Providers

type DbTests () =
    let executeSqlCommand sql = 
        using 
            (MSSqlServer.createConnection Settings.ConnectionStrings.WeatherStation)
            (fun connection -> 
                connection.Open()
                let command = MSSqlServer.createCommand sql connection
                command.ExecuteNonQuery())
    
    do
        try
            ["Measurements"]
            |> List.map ((sprintf "TRUNCATE TABLE %s") >> executeSqlCommand)
            |> ignore
        with
          | :? System.Data.SqlClient.SqlException as e -> 
            System.Console.Error.WriteLine(sprintf "Error cleaning databases: %s" (e.ToString()))    
