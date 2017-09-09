module WeatherStationUpload.DbService

open FSharp.Data.Sql

type private SqlProvider = 
    SqlDataProvider<
        ConnectionStringName = "WeatherStation",
        UseOptionTypes = true>
