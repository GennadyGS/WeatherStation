namespace WeatherStationUpload

open System

[<Measure>] 
type ``%``

[<Measure>] 
type C

type TimeZone = 
    | TimeZone of string

type TimeInterval = 
    { From: DateTime
      To: DateTime }

type Measurement = 
    { Timestamp: DateTime
      TemperatureInside: decimal<C> option
      HumidityInside: decimal<``%``> option
      TemperatureOutside: decimal<C> option
      HumidityOutside: decimal<``%``> option }

type StationId = 
    | StationId of int

type DeviceInfo = 
    { VendorId: Guid
      DeviceId: string }

type StationInfo = 
    { StationId: StationId
      DeviceInfo: DeviceInfo
      TimeZone: TimeZone }

type DbInsertOptions = 
    { Timeout: TimeSpan option
      BatchSize: int option } with 
    static member Default =
        { Timeout = None; BatchSize = None }
