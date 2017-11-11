namespace WeatherStationUpload

open System

[<Measure>] 
type ``%``

[<Measure>] 
type C

type Measurement = 
    { Timestamp: DateTime
      TemperatureInside: decimal<C>
      HimidityInside: decimal<``%``>
      TemperatureOutside: decimal<C>
      HimidityOutside: decimal<``%``> }

type DeviceInfo = 
    { VendorId: string
      DeviceId: string }