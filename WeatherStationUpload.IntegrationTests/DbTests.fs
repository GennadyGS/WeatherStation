namespace WeatherStationUpload.IntegrationTests

type DbTests () =
    let executeSqlCommand sql = 
        use connection = new System.Data.SqlClient.SqlConnection(Settings.ConnectionStrings.WeatherStation)
        connection.Open()
        let command = connection.CreateCommand()
        command.CommandText <- sql
        command.ExecuteNonQuery() |> ignore
    
    do
        try
            ["Measurements"]
            |> List.map ((sprintf "TRUNCATE TABLE %s") >> executeSqlCommand)
            |> ignore
        with
          | :? System.Data.SqlClient.SqlException as e -> 
            System.Console.Error.WriteLine(sprintf "Error cleaning databases: %s" (e.ToString()))    
