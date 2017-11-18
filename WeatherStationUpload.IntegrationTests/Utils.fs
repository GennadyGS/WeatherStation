module WeatherStationUpload.IntegrationTests.Utils

open WeatherStationUpload

let getTestDeviceInfo () : DeviceInfo = 
    { VendorId = Settings.VendorId 
      DeviceId = Settings.DeviceId }

let getTestStationId () : StationId =
    StationId Settings.StationId