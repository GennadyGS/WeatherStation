module WeatherStationUpload.MeasureUtils

let wrapPercent value = value * 1m<``%``>
let unwrapPercent value = value / 1m<``%``>

let wrapCelsius value = value * 1m<C>
let unwrapCelsius value = value / 1m<C>

