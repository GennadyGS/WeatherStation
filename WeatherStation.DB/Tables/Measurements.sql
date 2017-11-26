CREATE TABLE [dbo].[Measurements]
(
    [StationId] INT NOT NULL , 
    [Timestamp] DATETIME NOT NULL, 
    [TemperatureInside] DECIMAL(4, 2) NULL, 
    [TemperatureOutside] DECIMAL(4, 2) NULL, 
    [HumidityInside] DECIMAL(4, 2) NULL, 
    [HumidityOutside] DECIMAL(4, 2) NULL, 
    CONSTRAINT [FK_Measurements_Stations] FOREIGN KEY ([StationId]) REFERENCES [Stations]([Id]), 
    CONSTRAINT [PK_Measurements] PRIMARY KEY ([StationId], [Timestamp])
)
