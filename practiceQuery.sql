USE tilearchive;
INSERT IGNORE INTO glazetype (Name, Description) VALUES 
('Glossy', 'Shiny and Smooth'),
('Matte', 'Not Shiny, Dull');
INSERT IGNORE INTO surfacecondition (NAME) VALUES
('Rough'),
('Smooth');

INSERT IGNORE INTO tileboard (ID, Image, TileCount, DESCRIPTION, CreatedBy)
VALUES (1, 'R2下絵 (1)ＯＦ.JPG', 43, 'Test board for tile samples', 'Tyler');

INSERT IGNORE INTO testpiece (
  BoardID, Image, Color_L, Color_A, Color_B, GlazeTypeID,
  FiringTemperature, ChemicalComposition, SurfaceConditionID,
  EntryDate, FiringType, SoilType
)
VALUES
(1, 'Assets/tile1.png', 7.12, -55.51, 45.92, 1, 1220, 'Fe2O3', 1, '2024-07-11', 'RF', 'dosenbo'),
(1, 'Assets/tile2.png', 91.04, 11.02, 15.62, 1, 1240, 'Fe2O3', 1, '2024-07-12', 'RF^RF', 'uwaisi'),
(1, 'Assets/tile3.png', 81.82, 62.06, -45.58, 1, 1250, 'Fe2O3', 1, '2024-07-13', 'RF', 'akatuti'),
(1, 'Assets/tile4.png', 48.21, 52.5, 123.4, 1, 1220, 'Fe2O3', 1, '2024-10-18', 'OF', 'kizi');