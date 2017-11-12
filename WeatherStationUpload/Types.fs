namespace WeatherStationUpload

open System

[<Measure>] 
type ``%``

[<Measure>] 
type C

type MeasurementData = 
    { Timestamp: DateTime
      TemperatureInside: decimal<C> option
      HumidityInside: decimal<``%``> option
      TemperatureOutside: decimal<C> option
      HumidityOutside: decimal<``%``> option }

type DeviceInfo = 
    { VendorId: Guid
      DeviceId: string }

type Measurement = 
    { Device: DeviceInfo
      Data: MeasurementData }
