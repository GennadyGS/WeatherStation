module WeatherStationUpload.AsyncUtils

let retn arg = async {
    return arg
}

let map f argAsync = async {
    let! arg = argAsync 
    return f arg
}

let bind f argAsync = async {
    let! arg = argAsync 
    return! f arg
}
