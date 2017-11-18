CREATE TABLE [dbo].[Stations]
(
    [Id] INT IDENTITY NOT NULL, 
    [VendorId] UNIQUEIDENTIFIER NOT NULL , 
    [DeviceId] VARCHAR(16) NOT NULL, 
    [Name] VARCHAR(256) NOT NULL, 
    [Description] VARCHAR(4096) NULL, 
    PRIMARY KEY ([Id])
)
