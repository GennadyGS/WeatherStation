module WeatherStationUpload.MeasureUtils

let valueToPercent value = value * 1m<``%``>
let percentToValue value = value / 1m<``%``>

let valueToCelsius value = value * 1m<C>
let celsiusToValue value = value / 1m<C>

