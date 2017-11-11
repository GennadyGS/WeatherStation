MERGE INTO Stations AS Target 
USING (VALUES 
	('270F2261-3477-4872-9580-EAD9CAB3044C', '07523951F222', 'Obolon''', NULL)
) 
AS Source (VendorId, DeviceId, Name, Description) 
ON Target.VendorId = Source.VendorId AND Target.DeviceId = Source.DeviceId
WHEN MATCHED THEN 
UPDATE SET 
	Name = Source.Name,
    Description = Source.Description
WHEN NOT MATCHED BY TARGET THEN 
INSERT (VendorId, DeviceId, Name, Description) 
VALUES (Source.VendorId, Source.DeviceId, Source.Name, Source.Description)
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;