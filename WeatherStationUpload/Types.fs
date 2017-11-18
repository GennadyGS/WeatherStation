namespace WeatherStationUpload

open System

[<Measure>] 
type ``%``

[<Measure>] 
type C

type TimeInterval = 
    { From: DateTime
      To: DateTime }

type StationId = 
    | StationId of int

type Measurement = 
    { Timestamp: DateTime
      TemperatureInside: decimal<C> option
      HumidityInside: decimal<``%``> option
      TemperatureOutside: decimal<C> option
      HumidityOutside: decimal<``%``> option }

type DeviceInfo = 
    { VendorId: Guid
      DeviceId: string }
