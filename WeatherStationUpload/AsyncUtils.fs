module WeatherStationUpload.AsyncUtils

let map f argAsync = async {
    let! arg = argAsync 
    return f arg
}

let bind f argAsync = async {
    let! arg = argAsync 
    return! f arg
}

let runSequentially list = async {
    for item in list do
        return! item
}

let combineWithAndInore continuation prevResult = 
    async {
        let! result = prevResult
        continuation result
        return result
    }
