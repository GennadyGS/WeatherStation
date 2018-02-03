module WeatherStationUpload.IntegrationTests.Utils

open WeatherStationUpload

let private getTestTimeZone() : TimeZone = 
    TimeZone Settings.TimeZoneName

let private getTestDeviceInfo () : DeviceInfo = 
    { VendorId = Settings.VendorId 
      DeviceId = Settings.DeviceId }

let getTestStationId () : StationId =
    StationId Settings.StationId

let getTestStationInfo () : StationInfo = 
    { StationId = getTestStationId()
      DeviceInfo = getTestDeviceInfo()
      TimeZone = getTestTimeZone() }