module WeatherStationUpload.ResultUtils

let get = function
    | Ok ok -> ok
    | Error error -> failwith error
