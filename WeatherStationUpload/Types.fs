namespace WeatherStationUpload

open System

[<Measure>] 
type ``%``

[<Measure>] 
type C

type Measurement = 
    { Timestamp: DateTime
      TemperatureInside: decimal<C>
      HumidityInside: decimal<``%``>
      TemperatureOutside: decimal<C>
      HumidityOutside: decimal<``%``> }

type DeviceInfo = 
    { VendorId: Guid
      DeviceId: string }