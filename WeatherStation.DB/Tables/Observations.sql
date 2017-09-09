CREATE TABLE [dbo].[Observations]
(
	[VendorId] UNIQUEIDENTIFIER NOT NULL , 
    [DeviceId] VARCHAR(16) NOT NULL, 
    [Timestamp] DATETIME NOT NULL, 
    [TemperatureInside] DECIMAL(4, 2) NULL, 
    [TemperatureOutside] DECIMAL(4, 2) NULL, 
    [HumidityInside] DECIMAL(4, 2) NULL, 
    [HumidityOutside] DECIMAL(4, 2) NULL, 
    CONSTRAINT [FK_Observations_Stations] FOREIGN KEY ([VendorId], [DeviceId]) REFERENCES [Stations]([VendorId], [DeviceId])
)
