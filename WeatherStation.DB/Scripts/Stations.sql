MERGE INTO Stations AS Target 
USING (VALUES 
	('270F2261-3477-4872-9580-EAD9CAB3044C', '07523951F222', 'Obolon''', 'FLE Standard Time', NULL)
) 
AS Source (VendorId, DeviceId, Name, TimeZoneName, Description) 
ON Target.VendorId = Source.VendorId AND Target.DeviceId = Source.DeviceId
WHEN MATCHED THEN 
UPDATE SET 
	Name = Source.Name,
    Description = Source.Description,
    TimeZoneName = Source.TimeZoneName
WHEN NOT MATCHED BY TARGET THEN 
INSERT (VendorId, DeviceId, Name, TimeZoneName, Description) 
VALUES (Source.VendorId, Source.DeviceId, Source.Name, Source.TimeZoneName, Source.Description)
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;
