module WeatherStationUpload.AsyncUtils

let map f argAsync = async {
    let! arg = argAsync 
    return f arg
}

let bind f argAsync = async {
    let! arg = argAsync 
    return! f arg
}
