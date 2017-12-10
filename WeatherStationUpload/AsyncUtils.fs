module WeatherStationUpload.AsyncUtils

let map f arg = async {
    let! x = arg 
    return f x
}
